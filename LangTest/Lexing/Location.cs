namespace Lexing
{
    using System;

    public class Location
    {
        public Location(string source, int line, int character)
        {
            this.Source = source ??
                throw new ArgumentNullException(nameof(source));

            this.Line = line;

            this.Character = character;
        }


        public string Source { get; set; }

        public int Line { get; }

        public int Character { get; }
    }
}
