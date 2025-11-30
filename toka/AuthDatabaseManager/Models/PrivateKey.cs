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

        public override IEnumerable<string> Row(Database database) =>
            new[] { Name, $"{ValidFrom:yyyy-MM-dd}", Fingerprint };

        public override string CollectionName => "Private Keys";

        public override IEnumerable<IEnumerable<string>> Record()
        {
            var result = new[]
            {
                new[]{ "Name:", this.Name },
                new[]{ "Valid From:", this.ValidFrom.ToString("yyyy-MM-dd") },
                new[]{ "Fingerprint:", this.Fingerprint },
            };

            return result;
        }
    }
}
