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

        public override IEnumerable<string> Row(Database database)
        {
            Registration registration = database.Registration(FkRegistration);

            return new[] { Name, $"{Expires:yyyy-MM-dd}", Hint, registration.Name };
        }

        public override string CollectionName => "Secrets";

        public override IEnumerable<IEnumerable<string>> Record()
        {
            var result = new[]
            {
                new[]{ "Name:", this.Name },
                new[]{ "Hint:", this.Hint },
                new[]{ "Expires:", $"{this.Expires:yyyy-MM-dd}" },
            };

            return result;
        }
    }
}
