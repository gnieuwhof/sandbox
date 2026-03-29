namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;

    public class Secret : RegistrationItem, ISecret
    {
        [SQLite.NotNull]
        public string DerivedHash { get; set; }

        [SQLite.NotNull]
        public string Hint { get; set; }

        //

        public override string[] Columns =>
            new[] { "", "Name:", "Expires:", "Hint:", "Registration:", "Status:" };

        public override string Subtitle =>
            "Secrets are used to get a token";

        public override string Title => "Secrets";


        public Secret()
        {
        }

        public Secret(Model parent) : base(parent)
        {
        }


        private InputVal<Registration> registrationInput;
        private InputVal<string> secretInput;
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
            this.registrationInput = new InputVal<Registration>(
                database, "Registration");
            this.expiresInput = new InputVal<DateTime>(database, "Expires");

            return new InputBase[]
            {
                this.nameInput,
                this.registrationInput,
                this.expiresInput
            };
        }

        public override void SetDefaults()
        {
            this.nameInput.Default = this.Name;

            Guid? registrationId = (this.FkRegistration != Guid.Empty)
                ? this.FkRegistration
                : this.parent?.ID;
            this.registrationInput
                .SetDefault<Registration>(registrationId);

            this.expiresInput.Default = this.Expires;
        }

        public override void SetValues()
        {
            this.Name = this.nameInput.Value;

            if (this.registrationInput.Value == null)
            {
                this.registrationInput.SetValue(this.parent);
            }

            this.FkRegistration =
                this.registrationInput.Value.ID;

            this.Expires = this.expiresInput.Value;
        }

        public override int Create(Database database, Guid id)
        {
            Secret secret = database.Secret(
                id,
                this.nameInput.Value,
                this.registrationInput.Value.ID,
                this.secretInput.Value,
                this.expiresInput.Value
                );

            return (secret == null) ? 0 : 1;
        }

        //

        public override Row Row(Database database)
        {
            Registration registration =
                database.Record<Registration>(FkRegistration);

            CertificateOrSecretStatus status = GetStatus();

            ConsoleColor rowColor = GetColor(status);

            ParentsStatus parentsStatus = database.GetParentsStatus(this);
            if (parentsStatus == ParentsStatus.Deleted)
            {
                rowColor = ConsoleColor.Magenta;
            }
            else if (parentsStatus == ParentsStatus.Disabled)
            {
                rowColor = ConsoleColor.DarkGray;
            }

            return new Row(rowColor, Name, $"{Expires:yyyy-MM-dd}", Hint, registration?.Name, $"{status}");
        }

        public override IEnumerable<Row> Details(Database database)
        {
            CertificateOrSecretStatus status = GetStatus();

            ConsoleColor color = GetColor(status);

            Registration registration =
                database.Record<Registration>(FkRegistration);

            ConsoleColor regColor = ConsoleColor.Gray;
            if (registration == null)
            {
                regColor = ConsoleColor.Magenta;
            }
            else if (registration.Disabled)
            {
                regColor = ConsoleColor.DarkGray;
            }

            IEnumerable<string> parts = Split(this.DerivedHash, 38);

            var rows = new List<Row>();
            rows.Add(new Row("ID:", $"{this.ID}"));
            rows.Add(new Row("Name:", this.Name));
            rows.Add(new Row(regColor, "Registration:", registration?.Name));
            if (registration != null)
            {
                Administration administration =
                    database.Record<Administration>(registration.FkAdministration);
                ConsoleColor adminColor = ConsoleColor.Gray;
                if (administration == null)
                {
                    adminColor = ConsoleColor.Magenta;
                }
                else if (administration.Disabled)
                {
                    adminColor = ConsoleColor.DarkGray;
                }
                rows.Add(new Row(adminColor, "Administration:", administration?.Name));
            }
            rows.Add(new Row("Hint:", this.Hint));
            string hash = "Hash:";
            foreach (string part in parts)
            {
                rows.Add(new Row(hash, part));
                hash = "";
            }
            string expires = status switch
            {
                CertificateOrSecretStatus.Expired => "Expired",
                _ => "Expires"
            };
            rows.Add(new Row(color, $"{expires}:", $"{this.Expires:yyyy-MM-dd}"));
            rows.Add(new Row("Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")));
            rows.Add(new Row("Modified On:", this.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss")));
            rows.Add(new Row("Disabled:", $"{this.Disabled}"));

            return rows;
        }
        private static IEnumerable<string> Split(string toSplit, int sizes)
        {
            var result = new List<string>();

            string remainder = toSplit;

            while (remainder.Length > 0)
            {
                int min = Math.Min(remainder.Length, sizes);

                string part = remainder.Substring(0, min);

                result.Add(part);

                remainder = remainder.Substring(min);
            }

            return result;
        }
    }
}
