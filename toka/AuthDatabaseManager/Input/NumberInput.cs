namespace AuthDatabaseManager.Input
{
    using AuthDatabaseManager.Pages;
    using System;

    public static class NumberInput
    {
        public static int? Get(string description,
            int? defaultNum, Func<object, (bool, string)> validator)
        {
            if (defaultNum.HasValue)
            {
                description += $" (Cancel c, default: {defaultNum})";
            }
            else
            {
                description += " (Cancel c)";
            }

            while (true)
            {
                Console.Write($"{description}:");

                string input = Console.ReadLine();

                if (input == "")
                {
                    if (defaultNum != null)
                    {
                        Console.Write($"Default ({defaultNum}) Y, Clear x:");
                        input = Console.ReadLine();
                        if (input != "x")
                        {
                            input = $"{defaultNum}";
                        }
                    }
                }

                if (input == "c")
                {
                    return null;
                }

                bool parsed = int.TryParse(input, out int result);
                if (parsed)
                {
                    if (validator != null)
                    {
                        (bool isValid, string msg) = validator.Invoke(result);

                        if (!isValid)
                        {
                            Write.Warning(msg);

                            continue;
                        }
                    }

                    return result;
                }

                Write.Warning("The number cannot be parsed (c to Cancel).");
            }
        }
    }
}
