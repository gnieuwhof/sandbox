namespace Lexing
{
    using System;
    using System.Collections.Generic;
    using System.Text;

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
                if (!this.ReachedEnd() && this.Index >= 0)
                {
                    return this.Text[this.Index];
                }

                return InvalidChar;
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
            var builder = new StringBuilder();

            string postError = string.Empty;
            int index = (this.Index + 1);

            if ((this.Current != '\n') && (this.Current != '\r'))
            {
                while (index < this.Length)
                {
                    char chr = this[index];

                    if ((chr != '\n') && (chr != '\r'))
                    {
                        builder.Append(chr);
                    }
                    else
                    {
                        break;
                    }

                    ++index;
                }

                postError = builder.ToString();

                builder.Clear();
            }

            while((this.Current == '\n') || (this.Current == '\r'))
            {
                this.Reverse();
            }

            index = this.Index;
            bool postAdded = false;
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


                    string line = builder.ToString();

                    line = Reverse(line);

                    if(!postAdded)
                    {
                        line += postError;

                        postAdded = true;
                    }

                    lines.Add(line);

                    builder.Clear();

                    --amount;

                    continue;
                }

                if (chr != InvalidChar)
                {
                    builder.Append(chr);
                }

                --index;
            }

            if ((index == -1) && (amount > 0))
            {
                string line = builder.ToString();

                line = Reverse(line);

                if(!postAdded)
                {
                    line += postError;
                }

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
