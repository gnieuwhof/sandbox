namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;

    public class DetailsPage : Page
    {
        private readonly Database database;
        private readonly Model record;


        public override string Title { get; }


        public DetailsPage(Database database, Model record)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            this.record = record ??
                throw new ArgumentNullException(nameof(record));

            string modelName = Helper.GetName(record);

            this.Title = $"{modelName} Details";
        }


        public override Page Show()
        {
            IEnumerable<Row> details = this.record.Details(this.database);

            IEnumerable<Row> aligned = Helper.Align(details);

            Write.Lines(aligned);

            Console.WriteLine();
            Console.Write("Back B, Modify m:");
            string input = Console.ReadLine();

            if (input == "m")
            {
                Page page = new UpdatePage(this.database, this.record);

                page.ReturnPage = this;

                return page;
            }

            return this.ReturnPage;
        }
    }
}
