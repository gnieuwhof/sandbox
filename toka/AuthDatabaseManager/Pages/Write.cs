namespace AuthDatabaseManager.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Write
    {
        public static void Lines(IEnumerable<string> lines)
        {
            Color(Console.ForegroundColor, lines.ToArray());
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
    }
}
