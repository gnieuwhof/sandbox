namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ToolsPage : Page
    {
        private readonly Database database;


        public override string Title => "Tools";


        public ToolsPage(Page returnPage, Database database)
        {
            this.ReturnPage = returnPage;

            this.database = database;
        }


        public override Page Show()
        {
            Row[] tools = new[]
            {
                new Row("1", "HEX 2 BASE64"),
                new Row("2", "Secrets Generator"),
                new Row("3", "Passwords Generator")
            };

            var list = new List<Row>();
            list.AddRange(tools);
            IEnumerable<Row> aligned = Helper.Align(list);

            List<Row> lines = aligned.ToList();
            Write.Lines(lines);
            Console.WriteLine();

            string legend = "Option (empty returns):";

            while (true)
            {
                Console.Write(legend);
                string input = Console.ReadLine();

                if (input == "1")
                {
                    return new Hex2Base64Page(this);
                }
                if (input == "2")
                {
                    return new SecretsGeneratorPage(this, this.database);
                }
                if (input == "3")
                {
                    return new PasswordsGeneratorPage(this, this.database);
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    return this.ReturnPage;
                }

                Write.Warning("No valid option entered.");
            }
        }
    }
}
