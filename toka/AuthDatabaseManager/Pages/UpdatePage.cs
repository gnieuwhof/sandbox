namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Input;
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UpdatePage : RecordPage
    {
        protected readonly Database database;


        public override string Title { get; }


        public UpdatePage(Page returnPage, Database database, Model record)
            : base(returnPage, record)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            this.Title = $"Modify {this.modelName}";

            InputBase[] inputs = record.UpdateInputs(database);

            var fields = inputs.Select(i => i.Description);

            this.Subtitle = $"fields: {string.Join(", ", fields)}";
        }


        public override Page Show()
        {
            InputBase[] inputs = this.record.UpdateInputs(this.database);

            this.record.SetDefaults();

            var defaults = inputs.Select(i => i.GetDefault()).ToList();

            bool changed = false;

            while (true)
            {
                bool result = Inputs.Get(this.database, inputs);

                if (result)
                {
                    var lines = new List<Row>();
                    int index = 0;
                    foreach (InputBase inp in inputs)
                    {
                        string previousVal = defaults[index];

                        string inputValue = inp.GetValue();

                        var row = new Row($"{inp.Description}:", inputValue);

                        if (inputValue != previousVal)
                        {
                            var before = new Row(
                                ConsoleColor.Red, $"{inp.Description}:", previousVal);

                            lines.Add(before);

                            row.Color = ConsoleColor.Green;

                            changed = true;
                        }

                        lines.Add(row);

                        ++index;
                    }

                    IEnumerable<Row> aligned = Helper.Align(lines);

                    Write.Lines(aligned);
                    Console.WriteLine();

                    if (!changed)
                    {
                        Console.WriteLine("(No changes)");
                    }

                    string actions = changed
                        ? "Enter: Y, Cancel: c, Retry: r (otherwise)"
                        : "Retry: R, Cancel: c";
                    Console.Write($"{actions}:");

                    string ans = Console.ReadLine();
                    Console.WriteLine();

                    if (ans == "c")
                    {
                        return this.ReturnPage;
                    }

                    if (changed)
                    {
                        if (ans == "" || ans == "y")
                        {
                            break;
                        }
                    }

                    return this.Retry();
                }

                Console.WriteLine("(Cancelled)");
                Console.Write("Retry R, Back b:");
                string input = Console.ReadLine();
                if (input == "b")
                {
                    return this.ReturnPage;
                }

                return this.Retry();
            }

            this.record.SetValues();

            try
            {
                this.database.Update(this.record);
            }
            catch (Exception ex)
            {
                Write.Error("(Error)");
                Write.Error(ex.Message);
                Console.WriteLine();
                Console.Write("Retry R, Back b:");
                string input = Console.ReadLine();
                if (input == "b")
                {
                    return this.ReturnPage;
                }

                return this.Retry();
            }

            return this.ReturnPage;
        }

        private Page Retry()
        {
            Model model = this.database
                .GetTableFromType(this.record.GetType())
                .FirstOrDefault(m => m.ID == this.record.ID);

            var page = new UpdatePage(this.ReturnPage, this.database, model);

            return page;
        }
    }
}
