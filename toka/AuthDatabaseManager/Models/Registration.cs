namespace AuthDatabaseManager.Models
{
    using System.Collections.Generic;

    public class Registration : Model
    {
        [SQLite.NotNull]
        public string Scopes { get; set; }


        public override string[] Columns =>
            new[] { "", "Name:", "Scopes:" };

        public override IEnumerable<string> Row(Database database) =>
            new[] { Name, Scopes };

        public override string CollectionName => "Registrations";

        public override IEnumerable<IEnumerable<string>> Record()
        {
            var result = new[]
            {
                new[]{ "Name:", this.Name },
                new[]{ "Scopes:", this.Scopes },
            };

            return result;
        }
    }
}
