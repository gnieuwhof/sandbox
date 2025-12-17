namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;

    public class OperationModel : RecordPage
    {
        private readonly string operation;
        private readonly string executed;
        private readonly Action<Model> action;
        private readonly ConsoleColor textColor;
        private readonly Database database;

        public override string Title { get; }


        public OperationModel(
            Model record,
            string operation,
            string executed,
            Action<Model> action,
            ConsoleColor textColor,
            Database database
            )
            : base(record)
        {
            string modelName = Helper.GetName(record);

            this.Title = $"Selected {modelName}";


            this.operation = operation ??
                throw new ArgumentNullException(nameof(operation));

            this.executed = executed ??
                throw new ArgumentNullException(nameof(executed));

            this.action = action ??
                throw new ArgumentNullException(nameof(action));

            this.textColor = textColor;

            this.database = database ??
                throw new ArgumentNullException(nameof(database));
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

                Console.WriteLine();
                string executed = this.executed.ToLower();
                Write.Color(this.textColor, $"{modelName} {executed}.");
                Console.WriteLine("(any key to continue)");
                Console.ReadKey();
            }

            return this.ReturnPage;
        }
    }
}
