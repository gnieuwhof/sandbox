namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ToolsPage : ReturnBase
    {
        public override string Title => "Tools";


        public ToolsPage(Page returnPage)
        {
            this.ReturnPage = returnPage;
        }


        public override Page Show()
        {
            Row[] tools = new[]
            {
                new Row("1", "HEX 2 BASE64")
            };

            var list = new List<Row>();
            list.AddRange(tools);
            IEnumerable<Row> aligned = Helper.Align(list);

            List<Row> lines = aligned.ToList();
            Row line = Helper.GetLine(lines);
            lines.Add(line);

            Write.Lines(lines);

            string legend = "Option (empty returns):";

            while (true)
            {
                Console.Write(legend);
                string input = Console.ReadLine();

                if (input == "1")
                {
                    return new Hex2Base64Page(this);
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
