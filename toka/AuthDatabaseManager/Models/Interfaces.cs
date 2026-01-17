namespace AuthDatabaseManager.Models
{
    using System;

    public interface IModel
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public bool Disabled { get; set; }
    }

    public interface IPrivateKey : IModel
    {
        public string Content { get; set; }

        public string Fingerprint { get; set; }

        public DateTime ValidFrom { get; set; }
    }

    public interface IRegistration : IModel
    {
        public string Scopes { get; set; }

        public int ValidityPeriod { get; set; }
    }

    public interface ISecret : IModel
    {
        public Guid FkRegistration { get; set; }

        public string DerivedHash { get; set; }

        public DateTime Expires { get; set; }

        public string Hint { get; set; }
    }
}
