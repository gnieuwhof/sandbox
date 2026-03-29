namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Registration : Model, IRegistration, IChild
    {
        private readonly Model parent;


        public Guid FkAdministration { get; set; }

        [SQLite.NotNull]
        public string Audience { get; set; }

        [SQLite.NotNull]
        public string Scopes { get; set; }

        public int ValidityPeriod { get; set; }


        public override string[] Columns =>
            new[] { "", "Client ID:", "Name:", "Scopes:", "Valid for (min):" };

        public override string Subtitle =>
            "Registrations are what the token gives access to";

        public override Dictionary<string, ConsoleColor> Legend =>
            new Dictionary<string, ConsoleColor>
            {
                { "OK", ConsoleColor.Gray },
                { "Disabled parent", ConsoleColor.DarkGray },
                { "Deleted parent", ConsoleColor.Magenta },
            };


        public override Row Row(Database database)
        {
            ConsoleColor rowColor = ConsoleColor.Gray;

            ParentsStatus parentsStatus = database.GetParentsStatus(this);
            if (parentsStatus == ParentsStatus.Deleted)
            {
                rowColor = ConsoleColor.Magenta;
            }
            else if (parentsStatus == ParentsStatus.Disabled)
            {
                rowColor = ConsoleColor.DarkGray;
            }

            return new Row(rowColor, $"{ID}", Name, Scopes, $"{ValidityPeriod}");
        }

        public override string Title => "Registrations";


        public Registration()
        {
        }

        public Registration(Model parent)
        {
            this.parent = parent;
        }


        public Model Parent(Database database) =>
            database.Record<Administration>(this.FkAdministration);

        public override IEnumerable<Row> Record(Database database)
        {
            IEnumerable<Row> details = this.Details(database);

            IEnumerable<Row> record = details
                .Where(d => !d.Columns[0].StartsWith("ID"))
                .Where(d => !d.Columns[0].StartsWith("Audience"))
                .Where(d => !d.Columns[0].StartsWith("Created"))
                .Where(d => !d.Columns[0].StartsWith("Modified"))
                .Where(d => !d.Columns[0].StartsWith("Disabled"));

            return record;
        }

        public override IEnumerable<Row> Details(Database database)
        {
            Administration administration =
                database.Record<Administration>(FkAdministration);

            ConsoleColor adminColor = ConsoleColor.Gray;
            if (administration == null)
            {
                adminColor = ConsoleColor.Magenta;
            }
            else if (administration.Disabled)
            {
                adminColor = ConsoleColor.DarkGray;
            }

            var result = new[]
            {
                new Row( "ID:", $"{this.ID}" ),
                new Row( "Name:", this.Name ),
                new Row( adminColor, "Administration:", administration?.Name ),
                new Row( "Audience:", this.Audience ),
                new Row( "Scopes:", this.Scopes ),
                new Row( "Validity (min):", $"{this.ValidityPeriod}" ),
                new Row( "Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Modified On:", this.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Disabled:", $"{this.Disabled}" ),
            };

            return result;
        }

        private InputVal<Administration> administrationInput;
        private InputVal<string> audienceInput;
        private InputVal<string> scopesInput;
        private InputVal<int> validityInput;


        public override InputBase[] CreateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.administrationInput = new InputVal<Administration>(
                database, "Administration");
            this.audienceInput = new InputVal<string>(database, "Audience");
            this.scopesInput = new InputVal<string>(database, "Scopes");
            this.validityInput = new InputVal<int>(database, "Validity (min)");
            this.validityInput.Default = 60;
            this.validityInput.Validator = Validate;

            var inputs = new List<InputBase>();
            inputs.Add(this.nameInput);
            if (this.parent is null)
            {
                inputs.Add(this.administrationInput);
            }
            inputs.Add(this.audienceInput);
            inputs.Add(this.scopesInput);
            inputs.Add(this.validityInput);

            return inputs.ToArray();
        }

        public override InputBase[] UpdateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.administrationInput = new InputVal<Administration>(
                database, "Administration");
            this.audienceInput = new InputVal<string>(database, "Audience");
            this.scopesInput = new InputVal<string>(database, "Scopes");
            this.validityInput = new InputVal<int>(database, "Validity (min)");
            this.validityInput.Validator = Validate;

            return new InputBase[]
            {
                this.nameInput,
                this.administrationInput,
                this.audienceInput,
                this.scopesInput,
                this.validityInput
            };
        }

        public override void SetDefaults()
        {
            this.nameInput.Default = this.Name;

            Guid? administrationId = (this.FkAdministration != Guid.Empty)
                ? this.FkAdministration
                : this.parent?.ID;
            this.administrationInput
                .SetDefault<Administration>(administrationId);

            this.audienceInput.Default = this.Audience;
            this.scopesInput.Default = this.Scopes;
            this.validityInput.Default = this.ValidityPeriod;
        }

        public override void SetValues()
        {
            this.Name = this.nameInput.Value;

            if (this.administrationInput.Value == null)
            {
                this.administrationInput.SetValue(this.parent);
            }

            this.FkAdministration =
                this.administrationInput.Value.ID;

            this.Audience = this.audienceInput.Value;
            this.Scopes = this.scopesInput.Value;
            this.ValidityPeriod = this.validityInput.Value;
        }

        public override int Create(Database database, Guid id)
        {
            Registration registration = database.Registration(
                id,
                this.nameInput.Value,
                this.administrationInput.Value.ID,
                this.audienceInput.Value,
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
