namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DetailsPage : Page
    {
        private readonly Database database;
        private readonly Model record;


        public override string Title { get; }


        public DetailsPage(Page returnPage, Database database, Model record)
            : base(returnPage)
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
            Type type = this.record.GetType();
            bool isRegistration = (type == typeof(Registration));

            IEnumerable<Row> details = this.record.Details(this.database);

            IEnumerable<Row> aligned = Helper.Align(details);

            Write.Lines(aligned);
            Console.WriteLine();

            string dOption = this.record.Disabled
                ? "Delete"
                : "Disable";

            string actions = $"Back B, Modify m, {dOption} d";
            if (isRegistration)
            {
                actions += ", Secrets s";
            }
            Console.Write($"{actions}:");

            string input = Console.ReadLine();

            if (isRegistration && (input == "s"))
            {

                Page ret = new PrivateKeySecretsPage(this,
                    this.database, this.record, this.GetChildSecrets);

                return ret;
            }

            if (input == "m")
            {
                Page page = new UpdatePage(this, this.database, this.record);

                return page;
            }
            else if (input == "d")
            {
                string toPerform;
                Action<Model> action;
                ConsoleColor color;

                if (this.record.Disabled)
                {
                    toPerform = "Delete";
                    action = (model) => this.database.Delete(model);
                    color = ConsoleColor.Red;
                }
                else
                {
                    toPerform = "Disable";
                    action = (model) => this.database.Disable(model);
                    color = ConsoleColor.Yellow;
                }

                Page page = new OperationModel(
                    this,
                    this.record,
                    toPerform,
                    action,
                    color,
                    this.database,
                    this.ReturnPage
                    );

                return page;
            }

            return this.ReturnPage;
        }

        private IEnumerable<Secret> GetChildSecrets()
        {
            var records = this.database.GetActiveRecords<Secret>();

            records = records
                .Where(r => r.FkRegistration == this.record.ID)
                .Cast<Secret>()
                .ToArray();

            return records;
        }
    }
}
