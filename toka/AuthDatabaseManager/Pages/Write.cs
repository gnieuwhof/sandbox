namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Write
    {
        public static void Lines(IEnumerable<string> lines)
        {
            Color(Console.ForegroundColor, lines.ToArray());
        }
        public static void Lines(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                Color(row.Color, row.Line);
            }
        }

        public static void Warning(string warning)
        {
            Color(ConsoleColor.Yellow, warning);
        }

        public static void Error(string error)
        {
            Color(ConsoleColor.Red, error);
        }

        public static void Color(ConsoleColor color, params string[] messages)
        {
            var foregroundColor = Console.ForegroundColor;

            Console.ForegroundColor = color;

            foreach (string message in messages)
            {
                Console.WriteLine(message);
            }

            Console.ForegroundColor = foregroundColor;
        }

        public static void Cyan(string message)
        {
            Color(ConsoleColor.Cyan, message);
        }

        public static void Green(string message)
        {
            Color(ConsoleColor.Green, message);
        }

        public static void Dark(string message)
        {
            Color(ConsoleColor.DarkGray, message);
        }
    }
}
