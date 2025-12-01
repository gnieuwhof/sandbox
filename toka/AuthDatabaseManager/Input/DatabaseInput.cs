namespace AuthDatabaseManager.Input
{
    using AuthDatabaseManager.Pages;
    using System;
    using System.Collections.Generic;

    public static class DatabaseInput
    {
        public static bool Get(string title,
            Database database, params InputBase[] inputs)
        {
            while (true)
            {
                int number = 0;
                int total = inputs.Length;
                foreach (InputBase input in inputs)
                {
                    ++number;

                    Console.Write($"({number}/{total}) ");

                    bool processed = Inputs.Get(input, database);

                    if (!processed)
                    {
                        return false;
                    }

                    Console.WriteLine();
                }

                ClearScreen(title);

                Console.WriteLine("Create Record?");

                var lines = new List<string>();
                foreach (InputBase input in inputs)
                {
                    string inputValue = input.GetValue();

                    lines.Add($"- {input.Description} {inputValue}");
                }

                string line = Helper.GetLine(lines);
                Write.Lines(lines);
                Console.WriteLine(line);
                Console.WriteLine("Enter: Y, Cancel: c, Retry: r (otherwise)");

                string ans = Console.ReadLine();

                ans = ans.ToLower();

                if (ans == "" || ans == "y")
                {
                    return true;
                }

                if (ans == "c")
                {
                    return false;
                }

                ClearScreen(title);
            }
        }

        private static void ClearScreen(string title)
        {
            Console.Clear();
            Console.WriteLine($"--- {title} ---");
            Console.WriteLine();
        }
    }
}
