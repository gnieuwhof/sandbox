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

            this.Legend = record.DetailsLegend;
        }


        public override Page Show()
        {
            Type type = this.record.GetType();
            bool isRegistration = (type == typeof(Registration));
            bool isAdministration = (type == typeof(Administration));

            IEnumerable<Row> details = this.record.Details(this.database);

            if (this.record.Disabled)
            {
                foreach (Row row in details)
                {
                    row.Color = ConsoleColor.DarkGray;
                }
            }

            IEnumerable<Row> aligned = Helper.Align(details);

            Write.Lines(aligned);
            Console.WriteLine();

            string dOption = this.record.Disabled
                ? "Delete"
                : "Disable";

            string actions = $"Back B, Modify m, {dOption} d";
            if (isRegistration)
            {
                actions += ", Secrets s, Certificates c";
            }
            if (isAdministration)
            {
                actions += ", Registrations r";
            }
            Console.Write($"{actions}:");

            string input = Console.ReadLine();

            if (isRegistration)
            {
                if (input == "s")
                {
                    Page ret = new ChildRecordsPage<Secret>(
                        this,
                        this.database,
                        this.record,
                        () => this.GetChildren<Secret>(
                            s => s.FkRegistration == this.record.ID)
                        );

                    return ret;
                }
                if (input == "c")
                {
                    Page ret = new ChildRecordsPage<Certificate>(
                        this,
                        this.database,
                        this.record,
                        () => this.GetChildren<Certificate>(
                            c => c.FkRegistration == this.record.ID)
                        );

                    return ret;
                }
            }
            if (isAdministration && (input == "r"))
            {
                Page ret = new ChildRecordsPage<Registration>(
                    this,
                    this.database,
                    this.record,
                    () => this.GetChildren<Registration>(
                        r => r.FkAdministration == this.record.ID)
                    );

                return ret;
            }

            if (input == "m")
            {
                Page page = new ModifyPage(this, this.database, this.record);

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

        private IEnumerable<T> GetChildren<T>(Func<T, bool> predicate)
            where T : Model, new()
        {
            var records = this.database.GetActiveRecords<T>();

            records = records
                .Where(predicate)
                .Cast<T>()
                .ToArray();

            return records;
        }
    }
}
