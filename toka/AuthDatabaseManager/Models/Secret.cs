namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Secret : Model, ISecret
    {
        public Guid FkRegistration { get; set; }

        [SQLite.NotNull]
        public string DerivedHash { get; set; }

        public DateTime Expires { get; set; }

        [SQLite.NotNull]
        public string Hint { get; set; }


        public override string[] Columns =>
            new[] { "", "Name:", "Expires:", "Hint:", "Registration:", "Status:" };

        public override string CollectionName => "Secrets";


        public override Row Row(Database database)
        {
            Registration registration = database.Registration(FkRegistration);

            SecretStatus status = GetStatus();

            ConsoleColor color = GetColor(status);

            if (registration == null)
            {
                color = ConsoleColor.DarkMagenta;
            }

            return new Row(color, Name, $"{Expires:yyyy-MM-dd}", Hint, registration?.Name, $"{status}");
        }

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
            ConsoleColor color = GetColor();
            Registration registration = database.Registration(FkRegistration);

            var result = new[]
            {
                new Row( "ID:", $"{this.ID}" ),
                new Row( "Name:", this.Name ),
                new Row( "Registration:", registration?.Name ),
                new Row( "Hint:", this.Hint ),
                new Row( "Hash:", this.DerivedHash ),
                new Row( color, "Expires:", $"{this.Expires:yyyy-MM-dd}" ),
                new Row( "Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Modified On:", this.ModifiedOn?.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Disabled:", $"{this.Disabled}" ),
            };

            return result;
        }

        private ConsoleColor GetColor()
        {
            SecretStatus status = GetStatus();

            ConsoleColor result = GetColor(status);

            return result;
        }

        private static ConsoleColor GetColor(SecretStatus status)
        {
            return status switch
            {
                SecretStatus.Expiring => ConsoleColor.Yellow,
                SecretStatus.Expired => ConsoleColor.Red,
                _ => ConsoleColor.Gray,
            };
        }

        private SecretStatus GetStatus()
        {
            if (this.Disabled)
            {
                return SecretStatus.Disabled;
            }

            DateTime nowDate = DateTime.UtcNow.Date;
            if (this.Expires.Date < nowDate)
            {
                return SecretStatus.Expired;
            }
            else if ((this.Expires - nowDate).TotalDays <= 31)
            {
                return SecretStatus.Expiring;
            }

            return SecretStatus.Valid;
        }

        private InputVal<string> secretInput;
        private InputVal<Registration> registrationInput;

        private InputVal<string> nameInput;
        private InputVal<DateTime> expiresInput;

        public override InputBase[] CreateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.registrationInput = new InputVal<Registration>(
                database, "Registration");
            this.secretInput = new InputVal<string>(database, "Secret");
            this.expiresInput = new InputVal<DateTime>(database, "Expires")
            {
                Default = DateTime.UtcNow.Date.AddMonths(12)
            };

            return new InputBase[]
            {
                this.nameInput,
                this.registrationInput,
                this.secretInput,
                this.expiresInput
            };
        }

        public override InputBase[] UpdateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.expiresInput = new InputVal<DateTime>(database, "Expires");

            return new InputBase[]
            {
                this.nameInput,
                this.expiresInput
            };
        }

        public override void SetDefaults()
        {
            this.nameInput.Default = this.Name;
            this.expiresInput.Default = this.Expires;
        }

        public override void SetValues()
        {
            this.Name = this.nameInput.Value;
            this.Expires = this.expiresInput.Value;
        }

        public override int Create(Database database)
        {
            Secret secret = database.Secret(
                this.nameInput.Value,
                this.registrationInput.Value.ID,
                this.secretInput.Value,
                this.expiresInput.Value
                );

            return (secret == null) ? 0 : 1;
        }
    }
}
