namespace AuthDatabaseManager.Pages
{
    using System;
    using System.Collections.Generic;

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

            string line = new string('-', longest + 2);

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

        public static IEnumerable<string> Align(
            IEnumerable<IEnumerable<string>> grid)
        {
            var result = new List<string>();

            var lenghts = new List<int>();

            foreach (var row in grid)
            {
                int c = 0;
                foreach (string column in row)
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
                foreach (string column in row)
                {
                    int longest = lenghts[c];
                    int length = column?.Length ?? 0;
                    string postFix = new string(' ', longest - length);
                    aligned.Add($"{column}{postFix}");
                    ++c;
                }

                result.Add(string.Join("  ", aligned));
                aligned.Clear();
            }

            return result;
        }
    }
}
