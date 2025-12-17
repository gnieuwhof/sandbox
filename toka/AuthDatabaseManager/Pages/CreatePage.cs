namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Input;
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CreatePage : RecordPage
    {
        protected readonly Database database;


        public override string Title { get; }


        public CreatePage(Database database, Model record)
            : base(record)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            this.Title = $"Create {this.modelName}";

            InputBase[] inputs = record.CreateInputs(database);

            var fields = inputs.Select(i => i.Description);

            this.Subtitle = $"fields: {string.Join(", ", fields)}";
        }


        public override Page Show()
        {
            InputBase[] inputs = this.record.CreateInputs(this.database);

            while (true)
            {
                bool result = Inputs.Get(this.database, inputs);

                if (result)
                {
                    var lines = new List<Row>();

                    foreach (InputBase inp in inputs)
                    {
                        string inputValue = inp.GetValue();

                        var row = new Row($"{inp.Description}:", inputValue);

                        lines.Add(row);
                    }

                    IEnumerable<Row> aligned = Helper.Align(lines);

                    Write.Lines(aligned);
                    Console.WriteLine();

                    Console.Write("Enter: Y, Cancel: c, Retry: r (otherwise):");

                    string ans = Console.ReadLine();
                    Console.WriteLine();

                    if (ans == "c")
                    {
                        return this.ReturnPage;
                    }

                    if (ans == "" || ans == "y")
                    {
                        break;
                    }

                    // Retry.
                    return this;
                }

                Console.WriteLine("(Cancelled)");
                Console.Write("Retry R, Back b:");
                string input = Console.ReadLine();
                if (input == "b")
                {
                    return this.ReturnPage;
                }

                // Retry.
                return this;
            }

            try
            {
                this.record.Create(this.database);
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

                // Retry.
                return this;
            }

            Write.Color(ConsoleColor.Green, $"{this.modelName} created.");
            Console.WriteLine("(any key to continue)");
            Console.ReadKey();

            return this.ReturnPage;
        }
    }
}
