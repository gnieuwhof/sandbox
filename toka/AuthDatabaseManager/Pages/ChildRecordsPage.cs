namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ChildRecordsPage<T> : ShowModelsPage<T> where T : Model, new()
    {
        private readonly Model parent;


        public ChildRecordsPage(
            Page returnPage,
            Database database,
            Model parent,
            Func<IEnumerable<T>> getRecords
            )
            : base(returnPage, database, getRecords)
        {
            this.parent = parent;
        }


        public override string Title
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.parent?.Name))
                {
                    return $"{this.parent.Name}: {base.Title}";
                }

                return base.Title;
            }
        }


        protected override Page AfterShow()
        {
            string legend = "Back B, Create: c";
            if (this.records.Any())
            {
                legend = $"{legend}, Disable d, Show s (or number)";
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

                Page page;

                if (input == "c")
                {
                    T instance = (T)Activator.CreateInstance(typeof(T), this.parent);
                    page = new CreatePage(this, this.database, instance);

                    return page;
                }
                else if (input == "d" || input == "s")
                {
                    var selected = (T)Model.SelectRecord(
                        records, defaultToFirst: (input != "d"));

                    if (selected != null)
                    {
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
                            page = new DetailsPage(this, this.database, selected);
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
                    return this.ReturnPage;
                }
            }
        }
    }
}
