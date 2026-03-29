namespace AuthDatabaseManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class RegistrationItem : Model, IChild
    {
        protected readonly Model parent;


        public Guid FkRegistration { get; set; }

        public DateTime Expires { get; set; }


        protected RegistrationItem()
        {
        }

        protected RegistrationItem(Model parent)
        {
            this.parent = parent;
        }


        public override Dictionary<string, ConsoleColor> Legend =>
            new Dictionary<string, ConsoleColor>
            {
                { "OK", ConsoleColor.Gray },
                { "Expiring", ConsoleColor.Yellow },
                { "Expired", ConsoleColor.Red },
                { "Disabled parent", ConsoleColor.DarkGray },
                { "Deleted parent", ConsoleColor.Magenta },
            };

        public override Dictionary<string, ConsoleColor> DetailsLegend =>
            new Dictionary<string, ConsoleColor>
            {
                { "OK", ConsoleColor.Gray },
                { "Expiring", ConsoleColor.Yellow },
                { "Expired", ConsoleColor.Red },
                { "Disabled", ConsoleColor.DarkGray },
                { "Deleted", ConsoleColor.Magenta },
            };


        public Model Parent(Database database) =>
            database.Record<Registration>(this.FkRegistration);

        protected CertificateOrSecretStatus GetStatus()
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

        protected ConsoleColor GetColor()
        {
            CertificateOrSecretStatus status = GetStatus();

            ConsoleColor result = GetColor(status);

            return result;
        }

        public static ConsoleColor GetColor(CertificateOrSecretStatus status)
        {
            return status switch
            {
                CertificateOrSecretStatus.Expiring => ConsoleColor.Yellow,
                CertificateOrSecretStatus.Expired => ConsoleColor.Red,
                _ => ConsoleColor.Gray,
            };
        }

        public override T[] PreShow<T>(T[] models)
        {
            var casted = models.Cast<RegistrationItem>();

            var ordered = casted.OrderBy(c => c.Expires);

            return ordered.Cast<T>().ToArray();
        }
    }
}
