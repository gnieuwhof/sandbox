namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MainModelPage<T> : ShowModelsPage<T> where T : Model, new()
    {
        private readonly Page createPage;


        public MainModelPage(Page returnPage, Database database, Page createPage)
            : base(returnPage, database)
        {
            this.createPage = createPage ??
                throw new ArgumentNullException(nameof(createPage));

            this.createPage.ReturnPage = this;

            this.GetRecords = this.GetActiveRecords;
        }


        private IEnumerable<T> GetActiveRecords()
        {
            T[] records = this.database.GetActiveRecords<T>();

            return records;
        }

        protected override Page AfterShow()
        {
            string legend = "Back B, Create: c";
            if (records.Any())
            {
                legend = $"{legend}, Disable d, Show s (or number)";
            }
            legend += ", Inactive Records i:";

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

                if (input == "c")
                {
                    return this.createPage;
                }
                else if (input == "d" || input == "s")
                {
                    T selected = Model.SelectRecord(
                        records, defaultToFirst: (input != "d"));

                    if (selected != null)
                    {
                        Page page;

                        if (input == "d")
                        {
                            page = new OperationModel(
                                this,
                                selected,
                                "Disable",
                                (model) => this.database.Disable(model),
                                ConsoleColor.Yellow,
                                this.database,
                                this
                                );
                        }
                        else
                        {
                            var details = new DetailsPage(this, this.database, selected);
                            details.ReturnPage = this;
                            page = details;
                        }

                        return page;
                    }
                }
                else if (input == "i")
                {
                    Page inactivePage = new InactiveModelPage<T>(this, this.database);

                    return inactivePage;
                }
                else
                {
                    return new MainMenu(this.database);
                }
            }
        }
    }
}
