namespace AuthDatabaseManager.Models
{
    using System;

    public class Row
    {
        public ConsoleColor Color { get; }

        public string[] Columns { get; }

        public string Line => string.Join("  ", this.Columns);


        public Row(params string[] columns)
            : this(ConsoleColor.Gray, columns)
        {
        }

        public Row(ConsoleColor color, params string[] columns)
        {
            this.Color = color;

            this.Columns = columns ??
                throw new ArgumentNullException(nameof(columns));
        }


        public override string ToString() => this.Line;
    }
}
