namespace AuthDatabaseManager
{
    using AuthDatabaseManager.Pages;
    using System;

    public static class Program
    {
        private const string TITLE = "AUTH DATABASE MANAGER";
        private static readonly Version VERSION = new Version(1, 62);


        private static void Main()
        {
            var database = Database.Init("database.db3");

            Page page = new MainMenu(database);

            while (true)
            {
                Console.Clear();

                if (page is not Quit)
                {
                    Console.WriteLine($"{TITLE} v{VERSION}");
                    Console.WriteLine();
                    Console.WriteLine($"--- {page.Title} ---");
                    if (!string.IsNullOrWhiteSpace(page.Subtitle))
                    {
                        Console.WriteLine($"({page.Subtitle})");
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
