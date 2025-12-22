namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;

    public class OperationModel : RecordPage
    {
        private readonly string operation;
        private readonly Action<Model> action;
        private readonly ConsoleColor textColor;
        private readonly Database database;
        private readonly Page performedReturnPage;

        public override string Title { get; }


        public OperationModel(
            Page returnPage,
            Model record,
            string operation,
            Action<Model> action,
            ConsoleColor textColor,
            Database database,
            Page performedReturnPage
            )
            : base(returnPage, record)
        {
            string modelName = Helper.GetName(record);

            this.Title = $"Selected {modelName}";


            this.operation = operation ??
                throw new ArgumentNullException(nameof(operation));

            this.action = action ??
                throw new ArgumentNullException(nameof(action));

            this.textColor = textColor;

            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            this.performedReturnPage = performedReturnPage;
        }

        public override Page Show()
        {
            var details = this.record.Record(this.database);

            var result = Helper.Align(details);
            Write.Lines(result);
            Console.WriteLine();

            string modelName = Helper.GetName(this.record);
            Write.Color(this.textColor, $"{this.operation} {modelName} (y/N)");

            string input = Console.ReadLine();

            if (input == "y")
            {
                this.action.Invoke(this.record);

                return this.performedReturnPage;
            }

            return this.ReturnPage;
        }
    }
}
