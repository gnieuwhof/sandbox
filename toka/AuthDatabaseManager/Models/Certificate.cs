namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    public class Certificate : Model
    {
        private readonly Model parent;


        public Guid FkRegistration { get; set; }

        [SQLite.NotNull]
        public string PublicPem { get; set; }

        [SQLite.NotNull]
        public string X5t { get; set; }

        public DateTime Expires { get; set; }


        public override string[] Columns =>
            new[] { "", "Name:", "Expires:", "x5t:", "Registration:", "Status:" };

        public override string CollectionName => "Certificates";


        public Certificate()
        {
        }
        public Certificate(Model parent)
        {
            this.parent = parent;
        }


        public override int Create(Database database, Guid id)
        {
            Guid registrationId =
                this.parent?.ID ?? this.registrationInput.Value.ID;

            Certificate certificate = database.Certificate(
                id,
                this.Name,
                registrationId,
                this.PublicPem,
                this.X5t,
                this.Expires
                );

            return (certificate == null) ? 0 : 1;
        }

        private InputVal<Registration> registrationInput;

        private InputVal<string> pemPathInput;

        public override InputBase[] CreateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.registrationInput = new InputVal<Registration>(
                database, "Registration");
            this.pemPathInput = new InputVal<string>(database,
                "PEM file path", InputBase.InputType.FileInput);

            var inputs = new List<InputBase>();
            inputs.Add(this.nameInput);
            if (this.parent is null)
            {
                inputs.Add(this.registrationInput);
            }
            inputs.Add(this.pemPathInput);

            return inputs.ToArray();
        }

        public override IEnumerable<Row> Details(Database database)
        {
            ConsoleColor color = GetColor();
            Registration registration = database.Registration(FkRegistration);

            string[] pemLines = this.PublicPem.Replace("\r\n", "\n").Split('\n');

            var list = new List<Row>();
            list.Add(new Row("ID:", $"{this.ID}"));

            list.Add(new Row("PEM:", ""));
            int i = 0;
            foreach (string line in pemLines)
            {
                ++i;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                list.Add(new Row($"{i}:", line));
            }

            list.Add(new Row("Name:", this.Name));
            list.Add(new Row("x5t:", this.X5t));

            byte[] x5tBytes = FromBase64Url(this.X5t);
            string kidFromX5t = Convert.ToHexString(x5tBytes);
            list.Add(new Row("kid:", kidFromX5t));

            list.Add(new Row(color, "Expires:", $"{this.Expires:yyyy-MM-dd}"));
            list.Add(new Row("Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")));
            list.Add(new Row("Modified On:", this.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss")));
            list.Add(new Row("Disabled:", $"{this.Disabled}"));

            return list.ToArray();
        }
        private static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url
                .Replace('-', '+')
                .Replace('_', '/');

            // fix padding
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }

            byte[] bytes = Convert.FromBase64String(padded);

            return bytes;
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

        public override Row Row(Database database)
        {
            Registration registration = database.Registration(FkRegistration);

            CertificateOrSecretStatus status = GetStatus();

            ConsoleColor color = GetColor(status);

            if (registration == null)
            {
                color = ConsoleColor.DarkMagenta;
            }

            return new Row(color, this.Name, $"{Expires:yyyy-MM-dd}", this.X5t, registration?.Name, $"{status}");
        }

        public override void PreCreate(IList<Row> lines)
        {
            base.PreCreate(lines);

            var pem = lines.FirstOrDefault(l => l.Line.Contains("PEM"));

            string path = pem.Columns[1];

            try
            {
                this.PublicPem = File.ReadAllText(path);

            }
            catch (Exception e)
            {
                throw new Exception($"Cannot read path '{path}'", e);
            }
            try
            {
                var certificate = X509Certificate2.CreateFromPem(this.PublicPem);

                this.Expires = certificate.NotAfter;
                var expires = new Row("Expires", $"{this.Expires:yyyy-MM-dd}");
                lines.Add(expires);

                var kidBytes = Convert.FromHexString(certificate.Thumbprint);
                this.X5t = Base64UrlEncode(kidBytes);

                var x5tRow = new Row("x5t", this.X5t);
                lines.Add(x5tRow);
            }
            catch (Exception e)
            {
                throw new Exception($"Could not process PEM file '{path}'", e);
            }
        }
        private static string Base64UrlEncode(byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes);
            base64 = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return base64;
        }
        public override void SetDefaults()
        {
            this.nameInput.Default = this.Name;
        }

        public override void SetValues()
        {
            this.Name = this.nameInput.Value;
        }

        public override InputBase[] UpdateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");

            return new InputBase[]
            {
                this.nameInput,
            };
        }
    }
}
