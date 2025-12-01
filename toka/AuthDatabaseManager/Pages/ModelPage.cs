namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ModelPage<T> : Page where T : Model, new()
    {
        private readonly Database database;
        private readonly ReturnBase createPage;

        public override string Title { get; }


        public ModelPage(Database database, ReturnBase createPage)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            this.createPage = createPage ??
                throw new ArgumentNullException(nameof(createPage));


            this.createPage.ReturnPage = this;

            T instance = Activator.CreateInstance<T>();

            Title = instance.CollectionName;
        }


        public override Page Show()
        {
            T[] records = this.database.GetTable<T>();

            var grid = this.database.GetGrid(records);

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

            Row line = Helper.GetLine(lines);
            lines.Add(line);

            Write.Lines(lines);

            string legend = "Back B, Create: c";
            if (records.Any())
            {
                legend = $"{legend}, Delete d";
            }
            Console.WriteLine(legend);
            string input = Console.ReadLine();

            if (input == "c")
            {
                return this.createPage;
            }
            else if (input == "d")
            {
                T selected = Model.SelectRecord(records);

                if (selected != null)
                {
                    var deletePage = new DeleteModel(this.database, selected);

                    deletePage.ReturnPage = this;

                    return deletePage;
                }
            }

            return new MainMenu(this.database);
        }
    }
}
