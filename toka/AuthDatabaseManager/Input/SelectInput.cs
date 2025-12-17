namespace AuthDatabaseManager.Input
{
    using AuthDatabaseManager.Models;
    using AuthDatabaseManager.Pages;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SelectInput
    {
        public static object Get(InputBase input, Database database)
        {
            _ = database ?? throw new ArgumentNullException(nameof(database));

            Type modelType = input.GetGenericType();

            IEnumerable<Model> records = database.GetTableFromType(modelType);

            if (!records.Any())
            {
                Write.Warning("There are no records to select.");
                Console.WriteLine("any key to continue");
                Console.ReadKey();

                return null;
            }

            var instance = new Bogus();

            var grid = instance.GetGrid(database, records);

            IEnumerable<Row> aligned = Helper.Align(grid);

            List<Row> lines = aligned.ToList();

            Row line = Helper.GetLine(aligned);
            lines.Add(line);

            Console.WriteLine($"{input.Description}");
            Write.Lines(lines);
            Console.WriteLine();

            Model selected = Model.SelectRecord(records);

            return selected;
        }
    }
}
