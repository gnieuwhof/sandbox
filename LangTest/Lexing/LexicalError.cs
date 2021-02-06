namespace Lexing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LexicalError
    {
        public LexicalError(string error, IEnumerable<string> lines, Location location)
        {
            this.Error = error ??
                throw new ArgumentNullException(nameof(error));

            this.Lines = lines ??
                throw new ArgumentNullException(nameof(lines));

            this.Location = location ??
                throw new ArgumentNullException(nameof(location));
        }


        public string Error { get; }

        public IEnumerable<string> Lines { get; }

        public Location Location { get; }


        public override string ToString()
        {
            string source = this.Location.Source;
            int lineNumber = this.Location.Line;
            int character = this.Location.Character;

            var lines = new List<string>
            {
                $"Lexical error: {this.Error}",
                $"Source {source}, Line {lineNumber}, Character {character}",
                string.Empty
            };
            lines.AddRange(this.GetLinesAndErrorIndicator());

            string result = string.Join(Environment.NewLine, lines);

            return result;
        }

        private IEnumerable<string> GetLinesAndErrorIndicator()
        {
            var lines = new List<string>();

            int maxPrefixLength = $"{this.Location.Line}".Length + "0".Length;
            string prefixLineSeparator = ":";

            int lineCount = this.Lines.Count();
            for (int i = 0, index = lineCount - 1; i < lineCount; ++i, --index)
            {
                string line = this.Lines.ElementAt(i);

                string prefix = $"{this.Location.Line - index}";

                line = line.Replace("\\", "\\\\");
                line = line.Replace("\t", "\\t");

                prefix = PrefixIfShorterThan(prefix, "0", maxPrefixLength);

                line = $"{prefix}{prefixLineSeparator}{line}";

                lines.Add(line);
            }

            string errorIndicatorPrefix =
                new string(' ', maxPrefixLength + prefixLineSeparator.Length);

            string characterIndicator = this.GetCharacterIndicator();

            lines.Add(errorIndicatorPrefix + characterIndicator);

            return lines;
        }

        private static string PrefixIfShorterThan(
            string line, string prefix, int length)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            while(line.Length < length)
            {
                line = prefix + line;
            }

            return line;
        }

        private string GetCharacterIndicator()
        {
            string characterIndicator = string.Empty;

            string errorLine = this.Lines.Last();

            if (errorLine.Length > 0)
            {
                for (int i = 0; i < this.Location.Character - 1; ++i)
                {
                    char chr = errorLine[i];
                    characterIndicator += (new[] { '\t', '\\' }.Contains(chr))
                        ? ". "
                        : ".";
                }
            }

            characterIndicator += '^';

            return characterIndicator;
        }
    }
}
