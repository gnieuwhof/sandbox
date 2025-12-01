namespace AuthDatabaseManager.Models
{
    using System;
    using System.Collections.Generic;

    public class PrivateKey : Model
    {
        [SQLite.NotNull]
        public string Content { get; set; }

        [SQLite.NotNull]
        public string Fingerprint { get; set; }

        public DateTime ValidFrom { get; set; }


        public override string[] Columns =>
            new[] { "", "Name:", "Valid From:", "Fingerprint:" };

        public override Row Row(Database database) =>
            new Row(Name, $"{ValidFrom:yyyy-MM-dd}", Fingerprint);

        public override string CollectionName => "Private Keys";

        public override IEnumerable<Row> Record()
        {
            var result = new[]
            {
                new Row( "Name:", this.Name ),
                new Row( "Valid From:", this.ValidFrom.ToString("yyyy-MM-dd") ),
                new Row( "Fingerprint:", this.Fingerprint ),
            };

            return result;
        }
    }
}
