namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class KeyPair : Model, IKeyPairs
    {
        [SQLite.NotNull]
        public string PrivatePem { get; set; }

        [SQLite.NotNull]
        public string PublicKey { get; set; }

        [SQLite.NotNull]
        public string Fingerprint { get; set; }

        public DateTime ValidFrom { get; set; }

        //

        public override string Title => "Key Pairs";

        public override string Subtitle =>
            "Key Pairs are used to sign the tokens";

        public override string[] Columns =>
            new[] { "", "Name:", "Valid From:", "Fingerprint:", "Status:" };

        public override Dictionary<string, ConsoleColor> Legend =>
            new Dictionary<string, ConsoleColor>
            {
                { "Overridden", ConsoleColor.Yellow },
                { "Current", ConsoleColor.Green },
                { "Future", ConsoleColor.Gray },
            };



        private InputVal<string> privatePemPathInput;
        private InputVal<string> publicKeyPathInput;
        private InputVal<string> fingerprintInput;
        private InputVal<DateTime> validFromInput;

        public override InputBase[] CreateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.privatePemPathInput = new InputVal<string>(database,
                "Private PEM file path", InputBase.InputType.FileInput);
            this.publicKeyPathInput = new InputVal<string>(database,
                "Public KEY file path", InputBase.InputType.FileInput);
            this.fingerprintInput =
                new InputVal<string>(database, "Public Key Fingerprint");
            this.validFromInput = new InputVal<DateTime>(database, "Valid from")
            {
                Default = DateTime.UtcNow.Date
            };

            return new InputBase[]
            {
                this.nameInput,
                this.privatePemPathInput,
                this.publicKeyPathInput,
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
            KeyPair keyPair = database.KeyPair(
                id,
                this.nameInput.Value,
                this.privatePemPathInput.Value,
                this.publicKeyPathInput.Value,
                this.fingerprintInput.Value, this.validFromInput.Value
                );

            return (keyPair == null) ? 0 : 1;
        }


        public override Row Row(Database database)
        {
            KeyPairStatus status = GetStatus(database);

            return new Row(Name, $"{ValidFrom:yyyy-MM-dd}", Fingerprint, $"{status}");
        }

        public override IEnumerable<Row> Details(Database database)
        {
            KeyPairStatus status = GetStatus(database);

            ConsoleColor color = GetColor(status);

            var rows = new List<Row>();
            rows.Add(new Row("ID:", $"{this.ID}"));

            string pem = "Private PEM:";
            string[] pemLines = this.PrivatePem.Replace("\r\n", "\n").Split('\n');
            foreach (string line in pemLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                rows.Add(new Row(pem, line));
                pem = "";
            }

            string key = "Public KEY:";
            string[] keyLines = this.PublicKey.Replace("\r\n", "\n").Split('\n');
            foreach (string line in keyLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                rows.Add(new Row(key, line));
                key = "";
            }

            rows.Add(new Row("Name:", this.Name));
            rows.Add(new Row("Valid From:", this.ValidFrom.ToString("yyyy-MM-dd")));
            rows.Add(new Row("Fingerprint:", this.Fingerprint));
            rows.Add(new Row("Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")));
            rows.Add(new Row("Modified On:", this.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss")));
            rows.Add(new Row("Disabled:", $"{this.Disabled}"));
            rows.Add(new Row(color, "Status:", $"{status}"));

            return rows.ToArray();
        }

        //============

        public override IEnumerable<Row> GetGrid(
            Database database, IEnumerable<Model> records)
        {
            DateTime utcNow = DateTime.UtcNow;

            KeyPair current = records
                .Cast<KeyPair>()
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

                    KeyPairStatus status = GetStatus(utcNow, current, model);

                    ConsoleColor color = GetColor(status, modelRow.Color);

                    grid.Add(new Row(color, columns.ToArray()));
                }
            }

            return grid;
        }

        private ConsoleColor GetColor(KeyPairStatus status,
            ConsoleColor defaultColor = ConsoleColor.Gray)
        {
            return status switch
            {
                KeyPairStatus.Disabled => ConsoleColor.DarkGray,
                KeyPairStatus.Overridden => ConsoleColor.Yellow,
                KeyPairStatus.Current => ConsoleColor.Green,
                _ => defaultColor
            };
        }

        private KeyPairStatus GetStatus(Database database)
        {
            if (this.Disabled)
            {
                return KeyPairStatus.Disabled;
            }

            IEnumerable<KeyPair> KeyPairs =
                database.GetActiveRecords<KeyPair>();

            DateTime utcNow = DateTime.UtcNow;

            KeyPair current = KeyPairs
                .Where(pk => pk.ValidFrom < utcNow)
                .OrderBy(pk => pk.CreatedOn)
                .LastOrDefault();

            KeyPairStatus status = GetStatus(utcNow, current, this);

            return status;
        }
        private static KeyPairStatus GetStatus(
            DateTime utcNow, KeyPair current, Model model)
        {
            if (model.Disabled)
            {
                return KeyPairStatus.Disabled;
            }
            if (model.ID == current?.ID)
            {
                return KeyPairStatus.Current;
            }
            else if (model is KeyPair kp && kp.ValidFrom < utcNow)
            {
                return KeyPairStatus.Overridden;
            }

            return KeyPairStatus.Future;
        }


        public override T[] PreShow<T>(T[] models)
        {
            var casted = models.Cast<KeyPair>();

            var ordered = casted.OrderBy(c => c.ValidFrom);

            return ordered.Cast<T>().ToArray();
        }
    }
}
