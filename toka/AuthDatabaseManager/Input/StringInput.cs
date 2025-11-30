namespace AuthDatabaseManager.Input
{
    using System;

    public static class StringInput
    {
        public static string Get(string description)
        {
            Console.Write(description);

            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                input = null;
            }

            return input;
        }
    }
}
