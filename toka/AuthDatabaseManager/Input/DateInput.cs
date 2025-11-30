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
                defaultValue = $", or d for default: {defaultVal}";
            }

            while (true)
            {
                Console.Write($"{description} (format: {format}{defaultValue}):");

                string input = Console.ReadLine();

                if (input == "d")
                {
                    input = defaultVal;
                }
                else if (input == "c")
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
