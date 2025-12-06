namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Helper
    {
        public static string GetLine(List<(char?, object)> options)
        {
            int longest = 0;

            foreach ((char?, object) option in options)
            {
                object item = option.Item2;

                if (item is Page.Line)
                {
                    continue;
                }

                string name = GetName(item);

                longest = Math.Max(name?.Length ?? 0, longest);
            }

            string line = new string('-', longest + 3);

            return line;
        }

        public static string GetName(object item)
        {
            string name = $"{item}";

            if (item is Page page)
            {
                name = page.Title;
            }

            return name;
        }

        public static Row GetLine(IEnumerable<Row> rows)
        {
            IEnumerable<string> lines = rows.Select(r => r.Line);

            string line = GetLine(lines);

            var result = new Row(line);

            return result;
        }

        public static string GetLine(IEnumerable<string> items)
        {
            int longest = 0;

            foreach (string item in items)
            {
                longest = Math.Max(item.Length, longest);
            }

            string line = new string('-', longest);

            return line;
        }

        public static IEnumerable<Row> Align(
            IEnumerable<Row> grid)
        {
            var result = new List<Row>();

            var lenghts = new List<int>();

            foreach (var row in grid)
            {
                int c = 0;
                foreach (string column in row.Columns)
                {
                    int longest = 0;
                    if (c < lenghts.Count)
                    {
                        longest = lenghts[c];
                    }
                    int length = column?.Length ?? 0;
                    longest = Math.Max(length, longest);
                    if (c >= lenghts.Count)
                    {
                        lenghts.Add(longest);
                    }
                    else
                    {
                        lenghts[c] = longest;
                    }
                    ++c;
                }
            }

            var aligned = new List<string>();
            foreach (var row in grid)
            {
                int c = 0;
                foreach (string column in row.Columns)
                {
                    int longest = lenghts[c];
                    int length = column?.Length ?? 0;
                    string postFix = new string(' ', longest - length);
                    aligned.Add($"{column}{postFix}");
                    ++c;
                }

                result.Add(new Row(row.Color, aligned.ToArray()));
                aligned.Clear();
            }

            return result;
        }

        public static string GetName(Model model)
        {
            _ = model ??
                throw new ArgumentNullException(nameof(model));

            string name = model.GetType().Name;

            var chars = new List<char>();

            bool prevIsLower = false;
            foreach (char c in name)
            {
                bool currIsLower = char.IsLower(c);

                if (prevIsLower && !currIsLower)
                {
                    chars.Add(' ');
                }

                chars.Add(c);

                prevIsLower = currIsLower;
            }

            return new string(chars.ToArray());
        }
    }
}
