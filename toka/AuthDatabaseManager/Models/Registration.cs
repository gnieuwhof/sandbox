namespace AuthDatabaseManager.Models
{
    using System.Collections.Generic;

    public class Registration : Model
    {
        [SQLite.NotNull]
        public string Scopes { get; set; }


        public override string[] Columns =>
            new[] { "", "Client ID:", "Name:", "Scopes:" };

        public override Row Row(Database database) =>
            new Row($"{ID}", Name, Scopes);

        public override string CollectionName => "Registrations";

        public override IEnumerable<Row> Record()
        {
            var result = new[]
            {
                new Row( "Client ID:", $"{this.ID}"),
                new Row( "Name:", this.Name ),
                new Row( "Scopes:", this.Scopes ),
            };

            return result;
        }
    }
}
