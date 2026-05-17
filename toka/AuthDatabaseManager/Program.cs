namespace AuthDatabaseManager
{
    using AuthDatabaseManager.Pages;
    using System;
    using System.IO;
    using System.Linq;

    public static class Program
    {
        private const string TITLE = "AUTH DATABASE MANAGER";
        private static readonly Version VERSION = new Version(1, 91);


        private static void Main(string[] args)
        {
            string file = "database.db3";

            if (args.Length > 0)
            {
                file = args[0];
            }

            if (!File.Exists(file))
            {
                Console.WriteLine($"{TITLE} v{VERSION}");
                Console.WriteLine();
                Write.Warning($"File '{file}' does not exist, a new file will be created.");
                Console.WriteLine("Do you want to continue? (Y/n)");
                string input = Console.ReadLine();
                if (input == "n")
                {
                    return;
                }
            }

            var database = Database.Init(file);

            Page page = new MainMenu(database);

            while (true)
            {
                Console.Clear();

                if (page is not Quit)
                {
                    Console.WriteLine($"{TITLE} v{VERSION} (file: {file})");
                    Console.WriteLine();
                    Console.WriteLine($"--- {page.Title} ---");
                    if (!string.IsNullOrWhiteSpace(page.Subtitle))
                    {
                        Console.WriteLine($"({page.Subtitle})");
                    }
                    if (page.Legend?.Any() == true)
                    {
                        foreach (var kv in page.Legend)
                        {
                            Console.ForegroundColor = kv.Value;
                            Console.Write($"[ {kv.Key} ]");
                        }
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }

                page.PreShow();
                page = page.Show();

                if (page == null)
                {
                    break;
                }
            }
        }
    }
}
