namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Pages;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Model
    {
        [SQLite.PrimaryKey]
        public Guid ID { get; set; }

        [SQLite.Unique]
        [SQLite.NotNull]
        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool Disabled { get; set; }


        public abstract string[] Columns { get; }

        public abstract IEnumerable<string> Row(Database database);

        public abstract IEnumerable<IEnumerable<string>> Record();

        public abstract string CollectionName { get; }


        public static T SelectRecord<T>(IEnumerable<T> records)
            where T : Model
        {
            if (records?.Any() != true)
            {
                Console.WriteLine("There are no records to select.");

                return null;
            }

            while (true)
            {
                Console.Write($"Number (c to Cancel):");

                string str = Console.ReadLine();

                if (str == "c")
                {
                    return null;
                }

                if (int.TryParse(str, out int i))
                {
                    if (i > 0 && i <= records.Count())
                    {
                        return records.ElementAt(i - 1);
                    }
                }

                Write.Warning("No valid number entered.");
            }
        }
    }
}
