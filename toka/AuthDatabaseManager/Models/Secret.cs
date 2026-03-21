namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Secret : Model, ISecret
    {
        private readonly Model parent;

        public Guid FkRegistration { get; set; }

        [SQLite.NotNull]
        public string DerivedHash { get; set; }

        public DateTime Expires { get; set; }

        [SQLite.NotNull]
        public string Hint { get; set; }


        public override string[] Columns =>
            new[] { "", "Name:", "Expires:", "Hint:", "Registration:", "Status:" };

        public override string CollectionName => "Secrets";


        public Secret()
        {
        }
        public Secret(Model parent)
        {
            this.parent = parent;
        }


        public override Row Row(Database database)
        {
            Registration registration = database.Registration(FkRegistration);

            CertificateOrSecretStatus status = GetStatus();

            ConsoleColor color = GetColor(status);

            if (registration == null)
            {
                color = ConsoleColor.DarkMagenta;
            }

            return new Row(color, Name, $"{Expires:yyyy-MM-dd}", Hint, registration?.Name, $"{status}");
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
                new Row( "Modified On:", this.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Disabled:", $"{this.Disabled}" ),
            };

            return result;
        }

        private ConsoleColor GetColor()
        {
            CertificateOrSecretStatus status = GetStatus();

            ConsoleColor result = GetColor(status);

            return result;
        }

        private static ConsoleColor GetColor(CertificateOrSecretStatus status)
        {
            return status switch
            {
                CertificateOrSecretStatus.Expiring => ConsoleColor.Yellow,
                CertificateOrSecretStatus.Expired => ConsoleColor.Red,
                _ => ConsoleColor.Gray,
            };
        }

        private CertificateOrSecretStatus GetStatus()
        {
            if (this.Disabled)
            {
                return CertificateOrSecretStatus.Disabled;
            }

            DateTime nowDate = DateTime.UtcNow.Date;
            if (this.Expires.Date < nowDate)
            {
                return CertificateOrSecretStatus.Expired;
            }
            else if ((this.Expires - nowDate).TotalDays <= 31)
            {
                return CertificateOrSecretStatus.Expiring;
            }

            return CertificateOrSecretStatus.Valid;
        }

        private InputVal<string> secretInput;
        private InputVal<Registration> registrationInput;
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

            var inputs = new List<InputBase>();
            inputs.Add(this.nameInput);
            if (this.parent is null)
            {
                inputs.Add(this.registrationInput);
            }
            inputs.Add(this.secretInput);
            inputs.Add(this.expiresInput);

            return inputs.ToArray();
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

        public override int Create(Database database, Guid id)
        {
            Guid registrationId =
                this.parent?.ID ?? this.registrationInput.Value.ID;

            Secret secret = database.Secret(
                id,
                this.nameInput.Value,
                registrationId,
                this.secretInput.Value,
                this.expiresInput.Value
                );

            return (secret == null) ? 0 : 1;
        }

        public override T[] PreShow<T>(T[] models)
        {
            var casted = models.Cast<Secret>();

            var ordered = casted.OrderBy(c => c.Expires);

            return ordered.Cast<T>().ToArray();
        }
    }
}
