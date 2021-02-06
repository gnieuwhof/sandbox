namespace Lexing
{
    using System;
    using System.Collections.Generic;

    public class Source
    {
        public const char InvalidChar = char.MaxValue;


        public Source(string text)
        {
            this.Text = text ??
                throw new ArgumentNullException(nameof(text));
        }


        public char this[int i]
        {
            get
            {
                if (i < this.Text.Length)
                {
                    return this.Text[i];
                }

                return InvalidChar;
            }
        }

        public string Text { get; }

        public int Index { get; private set; }

        public int Line { get; set; } = 1;

        public int Character { get; set; } = 1;

        public int Length => this.Text.Length;

        public char Current
        {
            get
            {
                if (!this.ReachedEnd())
                {
                    return this.Text[this.Index];
                }

                throw new InvalidOperationException();
            }
        }


        public bool ReachedEnd()
        {
            return (this.Index >= this.Length);
        }

        public void Advance(int positions = 1)
        {
            this.Index += positions;

            this.Character += positions;
        }

        public void Reverse(int positions = 1) =>
            this.Advance(positions * -1);

        public IEnumerable<string> GetLastLines(int amount)
        {
            var lines = new List<string>();
            int index = this.Index;

            string line = string.Empty;

            while ((index >= 0) && (amount > 0))
            {
                char chr = this[index];

                if ((chr == '\n') || (chr == '\r'))
                {
                    --index;
                    if (chr == '\n')
                    {
                        chr = this[index];

                        if (chr == '\r')
                        {
                            --index;
                        }
                    }

                    line = Reverse(line);

                    lines.Add(line);

                    line = string.Empty;

                    --amount;

                    continue;
                }

                if (chr != InvalidChar)
                {
                    line += chr;
                }

                --index;
            }

            if ((index == -1) && (amount > 0))
            {
                line = Reverse(line);

                lines.Add(line);
            }

            lines.Reverse();

            return lines;
        }

        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public char Peek(int positions = 1)
        {
            if (this.Index < this.Length - positions)
            {
                return this[this.Index + positions];
            }

            return InvalidChar;
        }
    }
}
