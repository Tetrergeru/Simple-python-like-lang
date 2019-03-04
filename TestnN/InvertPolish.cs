using System;
using System.Collections.Generic;

// ReSharper disable All

namespace TestnN
{
     
    internal class InvertPolish
    {
        private Parser owner;

        private Dictionary<string, int> Priorities { get; } = new Dictionary<string, int>()
        {
            {"+", 1},
            {"-", 1},
            {"*", 2},
            {"/", 2},
            {"%", 2},
            {"<", 3},
            {"(", 0},
            {"or", 1},
            {"==", 0},
        };

        private Dictionary<string, Func<object, object, object>> Operations { get; } =
            new Dictionary<string, Func<object, object, object>>()
            {
                {"+", Functions.Add},
                {"*", Functions.Multiply},
                {"/", Functions.Divide},
                {"%", Functions.Mod},
                {"or", Functions.Or},
                {"==", Functions.Equal},
                {"<", Functions.Lesser},
            };

        private Dictionary<string, int> FuncsSignatures { get; } = new Dictionary<string, int>()
        {
            {"print", 1},
            {"len", 1},
            {"[", 2},
            {"str", 1 },
            {"is_simple", 1 },
        };

        private Dictionary<string, Func<List<object>, object>> Funcs =
            new Dictionary<string, Func<List<object>, object>>()
            {
                {
                    "print", o =>
                    {
                        Parser.Print(new Val(o[0]), false);
                        Console.WriteLine();
                        return o[0];
                    }
                },
                {"len", o => Functions.Len(o[0])},
                {"[", o => Functions.Index(o[0], o[1])},
                {"str", o => o[0].ToString()},
                {"is_simple", o => Functions.IsSimple(o[0])},
            };

        private static HashSet<char> OpSymbols { get; } = new HashSet<char>()
            {'(', ')', '[', ']', '+', '*', '-', '/', ':', '\\', '?', ' ', ','};

        private Stack<string> OperationStack { get; } = new Stack<string>();

        private Stack<object> OperandStack { get; } = new Stack<object>();

        public InvertPolish(Parser o)
        {
            owner = o;
        }

        public object Count(string a)
        {
            var start = 0;
            var finish = 0;
            SkipSpaces(a, 0, ref start);
            while (true)
            {
                SkipExpression(a, start, ref finish);

                if (finish > a.Length)
                    break;
                
                if (a[start] == '\'')
                    finish = a.IndexOf('\'', start + 1) + 1;

                var token = a.Substring(start, finish - start);
                

                Work(token);

                SkipSpaces(a, finish, ref start);
            }

            while (OperationStack.Count != 0)
                Eval();

            var result =  OperandStack.Pop();

            OperandStack.Clear();
            OperandStack.Clear();

            return result;
        }

        private void Eval()
        {
            var op = OperationStack.Pop();
            if (Operations.ContainsKey(op))
            {
                object b = OperandStack.Pop();
                object a = OperandStack.Pop();
                OperandStack.Push(Operations[op](a, b));
            }

            if (FuncsSignatures.ContainsKey(op))
            {
                var list = new List<object>();
                for (int i = 0; i < FuncsSignatures[op]; i++)
                    list.Add(OperandStack.Pop());
                list.Reverse();
                OperandStack.Push(Funcs[op](list));
            }
        }

        private int GetPriority(string operation)
        {
            if (Priorities.ContainsKey(operation))
                return Priorities[operation];
            if (Funcs.ContainsKey(operation))
                return int.MaxValue;
            throw new Exception("Операция не найдена");
        }

        private void Work(string token)
        {
            if (FuncsSignatures.ContainsKey(token))
                OperationStack.Push(token);
            else if (token == "(" || token == "[")
                OperationStack.Push(token);
            else if (token == ",")
            { }
            else if (token == ")" || token == "]")
            {
                while (OperationStack.Peek() != "[" && OperationStack.Peek() != "(")
                    Eval();
                Eval();
            }
            else if (Priorities.ContainsKey(token))
            {
                while (OperationStack.Count > 0 && GetPriority(OperationStack.Peek()) >= GetPriority(token))
                    Eval();
                OperationStack.Push(token);
            }
            else if (owner.Variables.ContainsKey(token))
            {
                OperandStack.Push(owner.Variables[token].Value);
            }
            else
            {
                OperandStack.Push(Parser.ParseTypes(token));
            }
        }

        static void SkipSpaces(string str, int i, ref int start)
        {
            while (i < str.Length && str[i] == ' ') i++;
            start = i;
        }

        static void SkipExpression(string str, int i, ref int start)
        {
            int j = i;
            while (i < str.Length && !OpSymbols.Contains(str[i])) i++;
            start = i + (i == j ? 1 : 0);
        }
    }
}