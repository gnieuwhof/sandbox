namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
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

        public DateTime ModifiedOn { get; set; }

        public bool Disabled { get; set; }


        public abstract string[] Columns { get; }

        public abstract Row Row(Database database);

        public abstract IEnumerable<Row> Record(Database database);

        public abstract IEnumerable<Row> Details(Database database);

        public abstract string CollectionName { get; }

        public abstract InputBase[] CreateInputs(Database database);

        public abstract int Create(Database database, Guid id);

        public abstract InputBase[] UpdateInputs(Database database);

        public abstract void SetDefaults();

        public abstract void SetValues();


        public static T SelectRecord<T>(IEnumerable<T> records, bool defaultToFirst)
            where T : Model
        {
            if (records?.Any() != true)
            {
                Console.WriteLine("There are no records to select.");

                return null;
            }

            while (true)
            {
                if (defaultToFirst && (records.Count() == 1))
                {
                    return records.First();
                }

                string options = defaultToFirst
                    ? "Number (default: 1, c to Cancel)"
                    : "Number (c to Cancel)";

                Console.Write($"{options}:");

                string str = Console.ReadLine();

                if (defaultToFirst && (str == ""))
                {
                    return records.First();
                }

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

        public virtual IEnumerable<Row> GetGrid(
            Database database, IEnumerable<Model> records)
        {
            var grid = new List<Row>();

            int index = 0;
            foreach (Model record in records)
            {
                ++index;

                var columns = new List<string>();

                columns.Add($"{index}");

                var modelRow = record.Row(database);

                columns.AddRange(modelRow.Columns);

                ConsoleColor color = record.Disabled
                    ? ConsoleColor.DarkGray
                    : modelRow.Color;

                grid.Add(new Row(color, columns.ToArray()));
            }

            return grid;
        }

        public virtual T[] PreShow<T>(T[] models) where T : Model
        {
            return models;
        }
    }
}
