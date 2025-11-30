namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;

    public class DeleteModel : Page
    {
        private readonly Database database;
        private readonly Model record;

        public override string Title { get; }


        public DeleteModel(Database database, Model record)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            this.record = record ??
                throw new ArgumentNullException(nameof(record));

            string modelName = record.GetType().Name;
            this.Title = $"Selected {modelName}";
        }


        public override Page Show()
        {
            var details = this.record.Record();

            var result = Helper.Align(details);
            Write.Lines(result);

            Console.WriteLine();
            Write.Warning("Delete record (y/N)");

            string input = Console.ReadLine();

            if (input == "y")
            {
                this.database.Delete(this.record);
            }

            return null;
        }
    }
}
