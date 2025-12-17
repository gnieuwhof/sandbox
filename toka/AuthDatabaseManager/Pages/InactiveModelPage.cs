namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InactiveModelPage<T> : ModelPage<T> where T : Model, new()
    {
        public override string Title => "Inactive " + base.Title;


        public InactiveModelPage(Database database) : base(database)
        {
        }


        public override Page Show()
        {
            T[] records = this.database.GetInactiveRecords<T>();

            T instance = Activator.CreateInstance<T>();
            var grid = instance.GetGrid(this.database, records);

            var list = new List<Row>();

            T record = records.FirstOrDefault();

            if (record != null)
            {
                list.Add(new Row(record.Columns));
            }

            list.AddRange(grid);

            IEnumerable<Row> aligned = Helper.Align(list);

            List<Row> lines = aligned.ToList();

            foreach (Row row in lines.Skip(1))
            {
                row.Color = ConsoleColor.DarkGray;
            }

            if (!records.Any())
            {
                lines.Add(new Row("(there are no records to show)"));
            }

            Write.Lines(lines);
            Console.WriteLine();

            string legend = "Back B";
            if (records.Any())
            {
                legend = $"{legend}, Enable e, Delete d, Show s";
            }
            legend += ":";

            while (true)
            {
                Console.Write(legend);
                string input = Console.ReadLine();

                switch(input)
                {
                    case "e":
                    case "d":
                    case "s":
                        {
                            T selected = Model.SelectRecord(records);

                            if (selected != null)
                            {
                                Page page;

                                if (input == "s")
                                {
                                    page = new DetailsPage(this.database, selected);
                                }
                                else
                                {
                                    string operation = (input == "e") ? "Enable" : "Delete";

                                    string executed = (input == "e") ? "Enabled" : "Deleted";

                                    Action<Model> action = (input == "e")
                                        ? (model) => this.database.Enable(selected)
                                        : (model) => this.database.Delete(selected);

                                    ConsoleColor color = (input == "e")
                                        ? ConsoleColor.Green
                                        : ConsoleColor.Red;

                                    page = new OperationModel(
                                        selected,
                                        operation,
                                        executed,
                                        action,
                                        color,
                                        this.database
                                        );
                                }

                                page.ReturnPage = this;

                                return page;
                            }
                            break;
                        }
                    default:
                        return this.ReturnPage;
                }
            }
        }
    }
}
