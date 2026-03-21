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


        public CreatePage(Page returnPage, Database database, Model record)
            : base(returnPage, record)
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
            Guid id = Guid.NewGuid();

            Console.WriteLine($"ID: {id}");
            Console.WriteLine();

            InputBase[] inputs = this.record.CreateInputs(this.database);

            while (true)
            {
                try
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

                        base.record.PreCreate(lines);

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
                    return RetryOrGoBack();
                }
                catch (Exception e)
                {
                    Write.Error("Could not process input");
                    Write.Error(e.Message);
                    if (e.InnerException != null)
                    {
                        Write.Error("InnerException:");
                        Write.Error(e.InnerException.Message);
                    }
                    return RetryOrGoBack();
                }
            }

            try
            {
                this.record.SetValues();
                this.record.Create(this.database, id);
            }
            catch (Exception ex)
            {
                Write.Error("(Error)");
                Write.Error(ex.Message);
                Console.WriteLine();
                return RetryOrGoBack();
            }

            return this.ReturnPage;
        }

        private Page RetryOrGoBack()
        {
            Console.Write("Retry R, Back b:");
            string input = Console.ReadLine();
            if (input == "b")
            {
                return this.ReturnPage;
            }

            // Retry.
            return this;
        }
    }
}
