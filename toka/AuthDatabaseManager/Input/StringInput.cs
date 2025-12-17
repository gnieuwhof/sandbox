namespace AuthDatabaseManager.Input
{
    using System;

    public static class StringInput
    {
        public static string Get(string description, object defaultVal)
        {
            if (defaultVal != null)
            {
                description = $"{description} (default '{defaultVal}')";
            }

            Console.Write($"{description}:");

            string input = Console.ReadLine();

            if (input == "")
            {
                string val = null;

                if (defaultVal != null)
                {
                    Console.Write($"Default ({defaultVal}) Y, Clear x:");
                    input = Console.ReadLine();
                    if (input != "x")
                    {
                        val = $"{defaultVal}";
                    }
                }

                input = val;
            }

            return input;
        }
    }
}
