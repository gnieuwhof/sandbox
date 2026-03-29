namespace AuthDatabaseManager.Pages
{
    using System;
    using System.Collections.Generic;

    public abstract class MenuPage : Page
    {
        protected readonly Database database;
        private readonly string defaultInput;


        protected MenuPage(Database database,
            Page returnPage, string defaultInput = null)
            : base(returnPage)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            this.defaultInput = defaultInput;
        }


        protected abstract List<(char?, object)> Options { get; }


        public override Page Show()
        {
            PrintMenu();

            Console.WriteLine();

            while (true)
            {
                Console.Write("Option:");

                string input = Console.ReadLine();

                input = input.Trim();

                if (input == "" && (this.defaultInput != null))
                {
                    input = defaultInput;
                }

                foreach ((char?, object) kv in Options)
                {
                    char? key = kv.Item1;
                    object item = kv.Item2;

                    if (input == $"{key}")
                    {
                        if (item is Page page)
                        {
                            return page;
                        }
                    }
                }

                Write.Warning("No valid option entered.");
            }
        }

        protected void PrintMenu()
        {
            foreach ((char?, object) kv in Options)
            {
                object item = kv.Item2;

                if (item is Line)
                {
                    string line = Helper.GetLine(Options);
                    Console.WriteLine(line);
                    continue;
                }

                char? key = kv.Item1;
                string name = Helper.GetName(item);

                string defaultOption = ($"{kv.Item1}" == this.defaultInput)
                    ? " (default)"
                    : "";

                Console.WriteLine($"{key}  {name}{defaultOption}");
            }
        }
    }
}
