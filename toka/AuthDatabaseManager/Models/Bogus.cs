namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;

    public class Bogus : Model
    {
        public override string[] Columns => throw new System.NotImplementedException();

        public override string Title => throw new System.NotImplementedException();

        public override string Subtitle => throw new NotImplementedException();

        public override IEnumerable<Row> Details(Database database)
        {
            throw new System.NotImplementedException();
        }

        public override Row Row(Database database)
        {
            throw new System.NotImplementedException();
        }

        public override void SetDefaults()
        {
            throw new System.NotImplementedException();
        }

        public override void SetValues()
        {
            throw new System.NotImplementedException();
        }

        public override InputBase[] UpdateInputs(Database database)
        {
            throw new System.NotImplementedException();
        }

        public override int Create(Database database, Guid id)
        {
            throw new System.NotImplementedException();
        }

        public override InputBase[] CreateInputs(Database database)
        {
            throw new System.NotImplementedException();
        }
    }
}
