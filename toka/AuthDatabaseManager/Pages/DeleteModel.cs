namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;

    public class DeleteModel : ReturnBase
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

            string modelName = Helper.GetName(record);
            this.Title = $"Selected {modelName}";
        }


        public override Page Show()
        {
            var details = this.record.Record();

            var result = Helper.Align(details);
            Write.Lines(result);
            Row line = Helper.GetLine(result);
            Console.WriteLine(line);
            string modelName = Helper.GetName(this.record);
            Write.Warning($"Delete {modelName} (y/N)");

            string input = Console.ReadLine();

            if (input == "y")
            {
                this.database.Delete(this.record);

                Console.WriteLine();
                Console.WriteLine($"{modelName} deleted.");
                Console.WriteLine("(any key to continue)");
                Console.ReadKey();
            }

            return this.ReturnPage;
        }
    }
}
