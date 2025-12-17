namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using AuthDatabaseManager.Pages;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Registration : Model, IRegistration
    {
        [SQLite.NotNull]
        public string Scopes { get; set; }

        public int ValidityPeriod { get; set; }


        public override string[] Columns =>
            new[] { "", "Client ID:", "Name:", "Scopes:", "Valid for (min):" };

        public override Row Row(Database database) =>
            new Row($"{ID}", Name, Scopes, $"{ValidityPeriod}");

        public override string CollectionName => "Registrations";

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
                new Row( "Scopes:", this.Scopes ),
                new Row( "Validity (min):", $"{this.ValidityPeriod}" ),
                new Row( "Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Modified On:", this.ModifiedOn?.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Disabled:", $"{this.Disabled}" ),
            };

            return result;
        }

        private InputVal<string> nameInput;
        private InputVal<string> scopesInput;
        private InputVal<int> validityInput;


        public override InputBase[] CreateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.scopesInput = new InputVal<string>(database, "Scopes");
            this.validityInput = new InputVal<int>(database, "Validity (min)");
            this.validityInput.Default = 60;
            this.validityInput.Validator = Validate;

            return new InputBase[]
            {
                this.nameInput,
                this.scopesInput,
                this.validityInput
            };
        }

        public override InputBase[] UpdateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.scopesInput = new InputVal<string>(database, "Scopes");
            this.validityInput = new InputVal<int>(database, "Validity (min)");
            this.validityInput.Validator = Validate;

            return new InputBase[]
            {
                this.nameInput,
                this.scopesInput,
                this.validityInput
            };
        }

        public override void SetDefaults()
        {
            this.nameInput.Default = this.Name;
            this.scopesInput.Default = this.Scopes;
            this.validityInput.Default = this.ValidityPeriod;
        }

        public override void SetValues()
        {
            this.Name = this.nameInput.Value;
            this.Scopes = this.scopesInput.Value;
            this.ValidityPeriod = this.validityInput.Value;
        }

        public override int Create(Database database)
        {
            Registration registration = database.Registration(
                this.nameInput.Value,
                this.scopesInput.Value,
                this.validityInput.Value
                );

            return (registration == null) ? 0 : 1;
        }

        private static (bool, string) Validate(object obj)
        {
            bool isValid;
            string msg = "Invalid input (input must be at least 1)";

            if (obj is int num)
            {
                isValid = (num > 0);
            }
            else
            {
                throw new InvalidOperationException();
            }

            return (isValid, msg);
        }
    }
}
