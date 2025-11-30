namespace AuthDatabaseManager.Input
{
    using AuthDatabaseManager.Pages;
    using System;
    using System.IO;

    public static class PathInput
    {
        public static string GetFile(string description)
        {
            while (true)
            {
                Console.WriteLine(description);
                string input = Console.ReadLine();

                if (File.Exists(input))
                {
                    return input;
                }

                Write.Warning("The file cannot be found.");
                Console.WriteLine("Try again? (Y/n)");

                input = Console.ReadLine();

                if (input == "n")
                {
                    return null;
                }
            }
        }
    }
}
