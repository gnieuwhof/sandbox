namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PrivateKey : Model, IPrivateKey
    {
        [SQLite.NotNull]
        public string Content { get; set; }

        [SQLite.NotNull]
        public string Fingerprint { get; set; }

        public DateTime ValidFrom { get; set; }


        public override string[] Columns =>
            new[] { "", "Name:", "Valid From:", "Fingerprint:", "Status:" };

        public override Row Row(Database database)
        {
            PrivateKeyStatus status = GetStatus(database);

            return new Row(Name, $"{ValidFrom:yyyy-MM-dd}", Fingerprint, $"{status}");
        }

        public override string CollectionName => "Private Keys";

        public override IEnumerable<Row> Record(Database database)
        {
            IEnumerable<Row> details = this.Details(database);

            IEnumerable<Row> record = details
                .Where(d => !d.Columns[0].StartsWith("ID"))
                .Where(d => !d.Columns[0].StartsWith("Created"))
                .Where(d => !d.Columns[0].StartsWith("Modified"))
                .Where(d => !d.Columns[0].StartsWith("Disabled"));

            return record;
        }

        public override IEnumerable<Row> Details(Database database)
        {
            var result = new[]
            {
                new Row( "ID:", $"{this.ID}" ),
                new Row( "Name:", this.Name ),
                new Row( "Valid From:", this.ValidFrom.ToString("yyyy-MM-dd") ),
                new Row( "Fingerprint:", this.Fingerprint ),
                new Row( "Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Modified On:", this.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Disabled:", $"{this.Disabled}" ),
            };

            return result;
        }

        //============

        public override IEnumerable<Row> GetGrid(Database database, IEnumerable<Model> records)
        {
            DateTime utcNow = DateTime.UtcNow;

            PrivateKey current = records
                .Cast<PrivateKey>()
                .Where(pk => pk.ValidFrom < utcNow)
                .OrderBy(pk => pk.CreatedOn)
                .LastOrDefault();

            var grid = new List<Row>();

            int index = 0;
            foreach (object record in records)
            {
                ++index;
                if (record is Model model)
                {
                    var columns = new List<string>();

                    columns.Add($"{index}");

                    var modelRow = model.Row(database);

                    columns.AddRange(modelRow.Columns);

                    ConsoleColor color = modelRow.Color;

                    if (model.Disabled)
                    {
                        color = ConsoleColor.DarkGray;
                    }
                    else if (record == current)
                    {
                        color = ConsoleColor.Green;
                    }
                    else if (record is PrivateKey pk && pk.ValidFrom < utcNow)
                    {
                        color = ConsoleColor.Yellow;
                    }

                    grid.Add(new Row(color, columns.ToArray()));
                }
            }

            return grid;
        }

        private PrivateKeyStatus GetStatus(Database database)
        {
            if (this.Disabled)
            {
                return PrivateKeyStatus.Disabled;
            }

            IEnumerable<PrivateKey> privateKeys =
                database.GetActiveRecords<PrivateKey>();

            DateTime utcNow = DateTime.UtcNow;

            PrivateKey current = privateKeys
                .Where(pk => pk.ValidFrom < utcNow)
                .OrderBy(pk => pk.CreatedOn)
                .LastOrDefault();


            if (this.ID == current.ID)
            {
                return PrivateKeyStatus.Current;
            }
            else if (this.ValidFrom < utcNow)
            {
                return PrivateKeyStatus.Overridden;
            }

            return PrivateKeyStatus.Future;
        }

        private InputVal<string> pemPathInput;

        private InputVal<string> nameInput;
        private InputVal<string> fingerprintInput;
        private InputVal<DateTime> validFromInput;

        public override InputBase[] CreateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.pemPathInput = new InputVal<string>(database,
                "PEM file path", InputBase.InputType.FileInput);
            this.fingerprintInput =
                new InputVal<string>(database, "Public Key Fingerprint");
            this.validFromInput = new InputVal<DateTime>(database, "Valid from")
            {
                Default = DateTime.UtcNow.Date
            };

            return new InputBase[]
            {
                this.nameInput,
                this.pemPathInput,
                this.fingerprintInput,
                this.validFromInput
            };
        }


        public override InputBase[] UpdateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.fingerprintInput =
                new InputVal<string>(database, "Public Key Fingerprint");
            this.validFromInput = new InputVal<DateTime>(database, "Valid from");

            return new InputBase[]
            {
                this.nameInput,
                this.fingerprintInput,
                this.validFromInput
            };
        }

        public override void SetDefaults()
        {
            this.nameInput.Default = this.Name;
            this.fingerprintInput.Default = this.Fingerprint;
            this.validFromInput.Default = this.ValidFrom;
        }

        public override void SetValues()
        {
            this.Name = this.nameInput.Value;
            this.Fingerprint = this.fingerprintInput.Value;
            this.ValidFrom = this.validFromInput.Value;
        }

        public override int Create(Database database, Guid id)
        {
            PrivateKey privateKey = database.PrivateKey(
                id,
                this.nameInput.Value,
                this.pemPathInput.Value,
                this.fingerprintInput.Value, this.validFromInput.Value
                );

            return (privateKey == null) ? 0 : 1;
        }

        public override T[] PreShow<T>(T[] models)
        {
            var casted = models.Cast<PrivateKey>();

            var ordered = casted.OrderBy(c => c.ValidFrom);

            return ordered.Cast<T>().ToArray();
        }
    }
}
