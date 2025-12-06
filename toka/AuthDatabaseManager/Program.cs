namespace AuthDatabaseManager
{
    using AuthDatabaseManager.Pages;
    using System;

    public static class Program
    {
        private const string TITLE = "AUTH DATABASE MANAGER";
        private static readonly Version VERSION = new Version(1, 23);


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
                    Console.WriteLine($"--- {page.Title} ---");
                    Console.WriteLine();
                }

                page = page.Show();

                if (page == null)
                {
                    break;
                }
            }
        }
    }
}
