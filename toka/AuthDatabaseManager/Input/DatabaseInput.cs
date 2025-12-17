namespace AuthDatabaseManager.Input
{
    using AuthDatabaseManager.Models;
    using AuthDatabaseManager.Pages;
    using System;
    using System.Collections.Generic;

    public static class DatabaseInput
    {
        public static bool Get(
            Database database, params InputBase[] inputs)
        {
            int number = 0;
            int total = inputs.Length;
            foreach (InputBase input in inputs)
            {
                ++number;

                Console.Write($"({number}/{total}) ");

                bool processed = Inputs.Get(input, database);

                Console.WriteLine();

                if (!processed)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
