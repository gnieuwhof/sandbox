namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InactiveModelPage<T> : ShowModelsPage<T> where T : Model, new()
    {
        public override string Title => "Inactive " + base.Title;


        public InactiveModelPage(Page returnPage, Database database)
            : base(returnPage, database)
        {
            this.GetRecords = this.GetInactiveRecords;
        }


        private IEnumerable<T> GetInactiveRecords()
        {
            T[] records = this.database.GetInactiveRecords<T>();

            return records;
        }

        protected override Page AfterShow()
        {
            string legend = "Back B";
            if (records.Any())
            {
                legend = $"{legend}, Enable e, Delete d, Show s (or number)";
            }
            legend += ":";

            while (true)
            {
                Console.Write(legend);
                string input = Console.ReadLine();

                if (int.TryParse(input, out int num))
                {
                    --num;
                    if ((num >= 0) && (num <= records.Count()))
                    {
                        Model selected = records.ElementAt(num);
                        var details = new DetailsPage(this, this.database, selected);
                        details.ReturnPage = this;
                        return details;
                    }
                }

                switch (input)
                {
                    case "e":
                    case "d":
                    case "s":
                        {
                            T selected = Model.SelectRecord(
                                records, defaultToFirst: (input != "d"));

                            if (selected != null)
                            {
                                Page page;

                                if (input == "s")
                                {
                                    page = new DetailsPage(this, this.database, selected);
                                }
                                else
                                {
                                    string operation = (input == "e") ? "Enable" : "Delete";

                                    Action<Model> action = (input == "e")
                                        ? (model) => this.database.Enable(selected)
                                        : (model) => this.database.Delete(selected);

                                    ConsoleColor color = (input == "e")
                                        ? ConsoleColor.Green
                                        : ConsoleColor.Red;

                                    page = new OperationModel(
                                        this.ReturnPage,
                                        selected,
                                        operation,
                                        action,
                                        color,
                                        this.database,
                                        this
                                        );
                                }

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
