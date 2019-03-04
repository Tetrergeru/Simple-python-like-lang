using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
// ReSharper disable All

namespace TestnN
{
    internal class Val
    {
        public Val(object value)
        {
            Value = value;
        }
        public object Value;
    }

    internal class Parser
    {
        private string[] Program { get; }

        private InvertPolish IPParser { get; }

        public Dictionary<string, Val> Variables { get; } = new Dictionary<string, Val>();

        private static Regex Comment { get; } = new Regex(@"^(//)|(#)|(--).*$");
        private static Regex Assignment { get; } = new Regex(@"^ *(?<lvalue>[a-zA-Z0-9_]+)(?<index>(\[[0-9]+\])*) *=(?<rvalue>.+)$");
        private static Regex Addition { get; } = new Regex(@"^ *(?<lvalue>[a-zA-Z0-9_]+)(?<index>(\[[0-9]+\])*) *\+=(?<rvalue>.+)$");
        private static Regex Condition { get; } = new Regex(@"^ *if (?<condition>.*): *$");
        private static Regex WhileCycle { get; } = new Regex(@"^ *while (?<condition>.*): *$");

        private static Regex Index { get; } = new Regex(@"^\[(?<value>[0-9]+)\](?<other>.*)$");

        private static Regex List { get; } = new Regex(@"^ *\[(?<content>.*)\] *$");
        private static Regex Integer { get; } = new Regex(@"^ *(?<value>-?[0-9]+) *$");
        private static Regex Double { get; } = new Regex(@"^ *(?<value>-?[0-9]+\.[0-9]+) *$");
        private static Regex Str { get; } = new Regex(@"^ *'(?<value>.*)' *$");
        private static Regex Bool { get; } = new Regex(@"^ *(?<value>(True)|(False)) *$");

        private static Regex Empty { get; } = new Regex(@"^ *$");

        public Parser(string fname)
        {
            IPParser = new InvertPolish(this);
            Program = File.ReadAllLines(fname);

            for (var i = 0; i < Program.Length; i++)
            {
                Parse(ref i);
            }

            Console.WriteLine("======================Program Finished=======================");
            foreach (var x in Variables)
            {
                Console.Write($"{x.Key} = ");
                Print(x.Value);
                Console.WriteLine();
            }
        }
        
        public Val EvalLeft(string lvalue, string index, Val value = null)
        {
            if (value != null)
            {
                if (index == "")
                    return value;
                var m = Index.Match(index);

                return EvalLeft(lvalue, m.Groups["other"].ToString(),
                    (value.Value as List<Val>)[int.Parse(m.Groups["value"].ToString())]);
            }

            if (index == "")
            {
                if (!Variables.ContainsKey(lvalue))
                    Variables[lvalue] = new Val(null);
                return Variables[lvalue];
            }

            var match = Index.Match(index);
            return EvalLeft(lvalue,
                match.Groups["other"].ToString(),
                (Variables[lvalue].Value as List<Val>)[int.Parse(match.Groups["value"].ToString())]);
        }

        public static object ParseTypes(string rvalue)
        {
            if (List.IsMatch(rvalue))
                return new List<Val>();

            if (Integer.IsMatch(rvalue))
                return int.Parse(Integer.Match(rvalue).Groups["value"].ToString());

            if (Double.IsMatch(rvalue))
                return double.Parse(Double.Match(rvalue).Groups["value"].ToString(), CultureInfo.InvariantCulture);

            if (Str.IsMatch(rvalue))
                return Str.Match(rvalue).Groups["value"].ToString();

            if (Bool.IsMatch(rvalue))
                return Bool.Match(rvalue).Groups["value"].ToString() == "True";

            return null;
        }

        public object EvalRight(string rvalue)
        {
            var result = ParseTypes(rvalue);
            if (result != null)
                return result;
            return IPParser.Count(rvalue);
        }

        public void Add(Val lvalue, object rvalue)
        {
            switch (lvalue.Value)
            {
                case string str:
                {
                    lvalue.Value = string.Concat(str, rvalue as string);
                    break;
                }
                case List<Val> list:
                {
                    list.Add(new Val(rvalue));
                    break;
                }
                case int x:
                {
                    lvalue.Value = x + (rvalue as int?);
                    break;
                }
                case double x:
                {
                    lvalue.Value = x + (rvalue as double?);
                    break;
                }
            }
        }

        public void Parse(ref int line)
        {
            if (line >= Program.Length) return;
            if (Comment.IsMatch(Program[line]) || Empty.IsMatch(Program[line])) return;
            
            var m = Assignment.Match(Program[line]);
            if (m.Success)
            {
                EvalLeft(m.Groups["lvalue"].ToString(), m.Groups["index"].ToString()).Value =
                    EvalRight(m.Groups["rvalue"].ToString());
                return;
            }
            
            m = Addition.Match(Program[line]);
            if (m.Success)
            {
                Add(EvalLeft(m.Groups["lvalue"].ToString(), m.Groups["index"].ToString()),
                    EvalRight(m.Groups["rvalue"].ToString()));
                return;
            }

            m = Condition.Match(Program[line]);
            if (m.Success && EvalRight(m.Groups["condition"].ToString()) is bool cond1)
            {
                var end = GetBlockEnd(line + 1);
                if (cond1)
                    for (int i = line + 1; i < end; i++)
                        Parse(ref i);

                line = end - 1;
                return;
            }

            m = WhileCycle.Match(Program[line]);
            if (m.Success)
            {
                var start = line + 1;
                var end = GetBlockEnd(line + 1);
                while (EvalRight(m.Groups["condition"].ToString()) is bool cond2 && cond2)
                    for (int i = start; i < end; i++)
                        Parse(ref i);

                line = end - 1;
                return;
            }

            EvalRight(Program[line]);
        }

        private int GetBlockEnd(int line)
        {
            var etalon = SpacesInBegin(line);
            while (line < Program.Length)
            {
                if (SpacesInBegin(line) < etalon)
                    return line;
                line++;
            }
            return line + 1;
        }

        private int SpacesInBegin(int line)
        {
            int count = 0;
            while (Program[line].Length > count && Program[line][count] == ' ')
                count++;
            return count;
        }

        public static void Print(Val o, bool stringsBrakets = true)
        {
            switch (o.Value)
            {
                case string str:
                {
                    if (stringsBrakets)
                        Console.Write($"\"{str}\" ");
                    else
                        Console.Write(str.Replace(@"\n", "\n"));
                    break;
                }
                case List<Val> list:
                {
                    Console.Write("[ ");
                    foreach (var y in list)
                        Print(y);
                    Console.Write("] ");
                    break;
                }
                default:
                {
                    Console.Write($"{o.Value} ");
                    break;
                }
            }
        }
    }
}