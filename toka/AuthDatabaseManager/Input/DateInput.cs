namespace AuthDatabaseManager.Input
{
    using AuthDatabaseManager.Pages;
    using System;
    using System.Globalization;

    public static class DateInput
    {
        public static DateTime? Get(string description, DateTime? defaultDate)
        {
            string format = "yyyy-M-d";
            string defaultValue = "";
            string defaultVal = "";
            if (defaultDate.HasValue)
            {
                defaultVal = defaultDate.Value.ToString(format);
                defaultValue = $", default: {defaultVal}";
            }

            while (true)
            {
                Console.Write($"{description} (format: {format}{defaultValue}):");

                string input = Console.ReadLine();

                if (input == "")
                {
                    if (defaultVal != null)
                    {
                        Console.Write($"Default ({defaultVal}) Y, Clear x:");
                        input = Console.ReadLine();
                        if (input != "x")
                        {
                            input = defaultVal;
                        }
                    }
                }

                if (input == "c")
                {
                    return null;
                }

                CultureInfo provider = CultureInfo.InvariantCulture;
                bool parsed = DateTime.TryParseExact(input, format,
                    provider, DateTimeStyles.None, out DateTime result);
                if (parsed)
                {
                    return result;
                }

                Write.Warning("The date cannot be parsed (c to Cancel).");
            }
        }
    }
}
