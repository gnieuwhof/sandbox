namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;

    public class Administration : Model, IAdministration
    {
        public override string[] Columns => new[] { "", "ID:", "Name:" };

        public override string Title => "Administrations";

        public override string Subtitle => 
            "Administrations are collections of Registrations";


        public override InputBase[] CreateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");

            return new InputBase[]
            {
                this.nameInput,
            };
        }

        public override InputBase[] UpdateInputs(Database database) =>
            CreateInputs(database);

        public override void SetDefaults()
        {
            this.nameInput.Default = this.Name;
        }

        public override void SetValues()
        {
            this.Name = this.nameInput.Value;
        }

        public override int Create(Database database, Guid id)
        {
            Administration administration =
                database.Administration(id, this.nameInput.Value);

            return (administration == null) ? 0 : 1;
        }


        public override Row Row(Database database) => new Row($"{ID}", Name);

        public override IEnumerable<Row> Details(Database database)
        {
            var result = new[]
            {
                new Row( "ID:", $"{this.ID}" ),
                new Row( "Name:", this.Name ),
                new Row( "Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Modified On:", this.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss") ),
                new Row( "Disabled:", $"{this.Disabled}" ),
            };

            return result;
        }
    }
}
