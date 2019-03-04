using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace TestnN
{
    public static class Functions
    {
        public static object Add(object a, object b)
        {
            switch (a)
            {
                case int x:
                {
                    if (b is int y)
                        return x + y;
                    if (b is double z)
                        return x + z;
                    throw new Exception("Типы не совпадают");
                }
                case double x:
                {
                    if (b is int y)
                        return x + y;
                    if (b is double z)
                        return x + z;
                    throw new Exception("Типы не совпадают");
                }
                case string s1:
                {
                    if (b is string s2)
                        return string.Concat(s1, s2);
                    throw new Exception("Типы не совпадают");
                }
                case List<Val> list:
                {
                    if (b is List<Val> l2)
                    {
                        var newList = new List<Val>();
                        newList.AddRange(list);
                        newList.AddRange(l2);
                        return newList;
                    }
                    if (b is object obj)
                    {
                        var newList = new List<Val>();
                        newList.AddRange(list);
                        newList.Add(new Val(obj));
                        return newList;
                    }
                    throw new Exception("Типы не совпадают");
                }
            }
            throw new Exception("Типы не совпадают");
        }

        public static object Multiply(object a, object b)
        {
            switch (a)
            {
                case int x:
                {
                    if (b is int y)
                        return x * y;
                    if (b is double z)
                        return x * z;
                    throw new Exception("Типы не совпадают");
                }
                case double x:
                {
                    if (b is int y)
                        return x * y;
                    if (b is double z)
                        return x * z;
                    throw new Exception("Типы не совпадают");
                }
            }
            throw new Exception("Типы не совпадают");
        }

        public static object Divide(object a, object b)
        {
            switch (a)
            {
                case int x:
                {
                    if (b is int y)
                        return x / y;
                    if (b is double z)
                        return x / z;
                    throw new Exception("Типы не совпадают");
                }
                case double x:
                {
                    if (b is int y)
                        return x / y;
                    if (b is double z)
                        return x / z;
                    throw new Exception("Типы не совпадают");
                }
            }
            throw new Exception("Типы не совпадают");
        }

        public static object Mod(object a, object b)
        {
            if (a is int x && b is int y)
                return x % y;
            throw new Exception("Типы не совпадают");
        }

        public static object Or(object a, object b)
        {
            if (a is bool x && b is bool y)
                return x || y;
            throw new Exception("Типы не совпадают");
        }

        public static object Equal(object a, object b)
        {
            switch (a)
            {
                case int x:
                {
                    if (b is int y)
                        return x == y;
                    if (b is double z)
                        return x == z;
                    throw new Exception("Типы не совпадают");
                }
                case double x:
                {
                    if (b is double z)
                        return x == z;
                    throw new Exception("Типы не совпадают");
                }
                case string s1:
                {
                    if (b is string s2)
                        return s1 == s2;
                    throw new Exception("Типы не совпадают");
                }
                case bool b1:
                {
                    if (b is bool b2)
                        return b1 == b2;
                    throw new Exception("Типы не совпадают");
                }
            }
            throw new Exception("Типы не совпадают");
        }

        public static object Lesser(object a, object b)
        {
            switch (a)
            {
                case int x:
                {
                    if (b is int y)
                        return x < y;
                    if (b is double z)
                        return x < z;
                    throw new Exception("Типы не совпадают");
                }
                case double x:
                {
                    if (b is double z)
                        return x < z;
                    throw new Exception("Типы не совпадают");
                }
            }
            throw new Exception("Типы не совпадают");
        }

        public static object Len(object list)
        {
            switch (list)
            {
                case string str:
                {
                    return str.Length;
                }
                case List<Val> lst:
                {
                    return lst.Count;
                }
            }
            throw new Exception("Типы не совпадают");
        }

        public static object Index(object list, object idx)
        {
            if (!(idx is int i)) throw new Exception("Типы не совпадают");

            switch (list)
            {
                case string str:
                {
                    return str[i];
                }
                case List<Val> lst:
                {
                    return lst[i];
                }
            }
            throw new Exception("Типы не совпадают");
        }

        public static object IsSimple(object number)
        {
            if (number is int x)
            {
                int end = (int)Math.Round(Math.Sqrt(x));
                for (int i = 2; i < end; i++)
                    if (x % i == 0)
                        return false;
                return true;
            }
            throw new Exception("Типы не совпадают");
        }
    }
}