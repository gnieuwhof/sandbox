namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;

    public class MainMenu : Page
    {
        private readonly Database database;

        private List<(char?, object)> Options
        {
            get
            {
                Database db = this.database;

                return new()
                {
                    ('1', new MainModelPage<PrivateKey>(null, db, new CreatePage(null, db, new PrivateKey()))),
                    ('2', new MainModelPage<Registration>(null, db, new CreatePage(null, db, new Registration()))),
                    ('3', new MainModelPage<Secret>(null, db, new CreatePage(null, db, new Secret()))),
                    ('4', new MainModelPage<Certificate>(null, db, new CreatePage(null, db, new Certificate()))),
                    (null, LINE),
                    ('i', new InfoPage(this)),
                    ('t', new ToolsPage(this, db)),
                    (null, LINE),
                    ('q', new Quit()),
                };
            }
        }


        public override string Title => "Main Menu";


        public MainMenu(Database database) : base(returnPage: null)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));
        }


        public override Page Show()
        {
            PrintMenu();

            Console.WriteLine();

            while (true)
            {
                Console.Write("Option:");

                string input = Console.ReadLine();

                input = input.Trim();

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

        private void PrintMenu()
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

                Console.WriteLine($"{key}  {name}");
            }
        }
    }
}
