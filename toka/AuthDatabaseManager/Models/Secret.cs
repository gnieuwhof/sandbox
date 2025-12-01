namespace AuthDatabaseManager.Models
{
    using System;
    using System.Collections.Generic;

    public class Secret : Model
    {
        public Guid FkRegistration { get; set; }

        [SQLite.NotNull]
        public string DerivedHash { get; set; }

        public DateTime Expires { get; set; }

        [SQLite.NotNull]
        public string Hint { get; set; }


        public override string[] Columns =>
            new[] { "", "Name:", "Expires:", "Hint:", "Registration:" };

        public override string CollectionName => "Secrets";


        public override Row Row(Database database)
        {
            Registration registration = database.Registration(FkRegistration);

            ConsoleColor color = GetColor(this.Expires);

            return new Row(color, Name, $"{Expires:yyyy-MM-dd}", Hint, registration.Name);
        }

        public override IEnumerable<Row> Record()
        {
            ConsoleColor color = GetColor(this.Expires);

            var result = new[]
            {
                new Row( "Name:", this.Name ),
                new Row( "Hint:", this.Hint ),
                new Row( color, "Expires:", $"{this.Expires:yyyy-MM-dd}" ),
            };

            return result;
        }

        private static ConsoleColor GetColor(DateTime expires)
        {
            ConsoleColor color = ConsoleColor.Gray;

            DateTime nowDate = DateTime.UtcNow.Date;
            if (expires.Date < nowDate)
            {
                color = ConsoleColor.Red;
            }
            else if ((expires - nowDate).TotalDays <= 31)
            {
                color = ConsoleColor.Yellow;
            }

            return color;
        }
    }
}
