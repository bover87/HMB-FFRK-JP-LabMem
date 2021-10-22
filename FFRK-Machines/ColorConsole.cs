﻿using System;

namespace FFRK_Machines
{
    public class ColorConsole
    {

        public static bool Timestamps { get; set; }
        private static bool stamped = false;
        private static object stampLock = new object();
        private static object colorLock = new object();

        public static void Write(ConsoleColor color, string format, params object[] arg)
        {
            lock (colorLock)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Write(format, arg);
                Console.ForegroundColor = current;
            }
        }

        public static void Write(ConsoleColor color, string value)
        {
            lock (colorLock)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Write(value);
                Console.ForegroundColor = current;
            }

        }

        public static void Write(string format, params object[] arg)
        {
            lock (stampLock)
            {
                DoTimestamp(false);
                Console.Write(format, arg);
            }
        }

        public static void Write(string value)
        {
            lock (stampLock)
            {
                DoTimestamp(false);
                Console.Write(value);
            }
        }

        public static void WriteLine(ConsoleColor color, string format, params object[] arg)
        {
            lock (colorLock)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = color;
                WriteLine(format, arg);
                Console.ForegroundColor = current;
            }
            
        }

        public static void WriteLine(ConsoleColor color, string value)
        {
            lock (colorLock)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = color;
                WriteLine(value);
                Console.ForegroundColor = current;
            }
        }

        public static void WriteLine(string value)
        {
            lock (stampLock)
            {
                DoTimestamp(true);
                Console.WriteLine(value);
            }
            
        }

        public static void WriteLine(string format, params object[] arg)
        {
            lock (stampLock)
            {
                DoTimestamp(true);
                Console.WriteLine(format, arg);
            }
           
        }

        private static void DoTimestamp(bool newLine)
        {
            if (Timestamps)
            {
                if (!stamped)
                {
                    lock (colorLock)
                    {
                        var current = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("{0:HH:mm:ss} ", DateTime.Now);
                        Console.ForegroundColor = current;
                    }
                }
                stamped = !newLine;
            }

        }

    }
}
