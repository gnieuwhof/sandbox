namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MainModelPage<T> : ModelPage<T> where T : Model, new()
    {
        private readonly Page createPage;


        public MainModelPage(Database database, Page createPage)
            : base(database)
        {
            this.createPage = createPage ??
                throw new ArgumentNullException(nameof(createPage));

            this.createPage.ReturnPage = this;
        }


        public override Page Show()
        {
            T[] records = this.database.GetActiveRecords<T>();

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

            if (!records.Any())
            {
                lines.Add(new Row("(there are no records to show)"));
            }

            Write.Lines(lines);
            Console.WriteLine();

            string legend = "Back B, Create: c";
            if (records.Any())
            {
                legend = $"{legend}, Disable d, Show s";
            }
            legend += ", Inactive Records i:";

            while (true)
            {
                Console.Write(legend);
                string input = Console.ReadLine();

                if (input == "c")
                {
                    return this.createPage;
                }
                else if (input == "d" || input == "s")
                {
                    T selected = Model.SelectRecord(records);

                    if (selected != null)
                    {
                        Page page;

                        if (input == "d")
                        {
                            page = new OperationModel(
                                selected,
                                "Disable",
                                "Disabled",
                                (model) => this.database.Disable(model),
                                ConsoleColor.Yellow,
                                this.database
                                );
                        }
                        else
                        {
                            page = new DetailsPage(this.database, selected);
                        }

                        page.ReturnPage = this;

                        return page;
                    }
                }
                else if (input == "i")
                {
                    Page inactivePage = new InactiveModelPage<T>(this.database);

                    inactivePage.ReturnPage = this;

                    return inactivePage;
                }
                else
                {
                    return base.Show();
                }
            }
        }
    }
}
