namespace AuthDatabaseManager.Models
{
    using AuthDatabaseManager.Input;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    public class Certificate : RegistrationItem, ICertificate
    {
        [SQLite.NotNull]
        public string PublicPem { get; set; }

        [SQLite.NotNull]
        public string X5t { get; set; }

        //

        public override string[] Columns =>
            new[] { "", "Name:", "Expires:", "x5t:", "Registration:", "Status:" };

        public override string Title => "Certificates";

        public override string Subtitle =>
            "Certificates are used to get a token";


        public Certificate()
        {
        }

        public Certificate(Model parent) : base(parent)
        {
        }


        private InputVal<Registration> registrationInput;
        private InputVal<string> publicPemPathInput;

        public override InputBase[] CreateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.registrationInput = new InputVal<Registration>(
                database, "Registration");
            this.publicPemPathInput = new InputVal<string>(database,
                "Public PEM (CRT) file path", InputBase.InputType.FileInput);

            var inputs = new List<InputBase>();
            inputs.Add(this.nameInput);
            if (this.parent is null)
            {
                inputs.Add(this.registrationInput);
            }
            inputs.Add(this.publicPemPathInput);

            return inputs.ToArray();
        }

        public override InputBase[] UpdateInputs(Database database)
        {
            this.nameInput = new InputVal<string>(database, "Name");
            this.registrationInput = new InputVal<Registration>(
                database, "Registration");

            return new InputBase[]
            {
                this.nameInput,
                this.registrationInput,
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
        }

        public override int Create(Database database, Guid id)
        {
            Certificate certificate = database.Certificate(
                id,
                this.Name,
                this.registrationInput.Value.ID,
                this.PublicPem,
                this.X5t,
                this.Expires
                );

            return (certificate == null) ? 0 : 1;
        }

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

            return new Row(rowColor, this.Name, $"{Expires:yyyy-MM-dd}", this.X5t, registration?.Name, $"{status}");
        }

        public override IEnumerable<Row> Details(Database database)
        {
            ConsoleColor color = GetColor();

            ParentsStatus parentsStatus = database.GetParentsStatus(this);

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

            var rows = new List<Row>();
            rows.Add(new Row("ID:", $"{this.ID}"));

            string pem = "Public PEM (CRT):";
            string[] pemLines = this.PublicPem.Replace("\r\n", "\n").Split('\n');
            foreach (string line in pemLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                rows.Add(new Row(pem, line));
                pem = "";
            }

            rows.Add(new Row("Name:", this.Name));
            rows.Add(new Row(regColor, "Registration:", registration?.Name));
            if (registration != null)
            {
                Administration administration =
                    database.Record<Administration>(registration.FkAdministration);
                ConsoleColor adminColor = ConsoleColor.Gray;
                if(administration == null)
                {
                    adminColor = ConsoleColor.Magenta;
                }
                else if(administration.Disabled)
                {
                    adminColor = ConsoleColor.DarkGray;
                }
                rows.Add(new Row(adminColor, "Administration:", administration?.Name));
            }
            rows.Add(new Row("x5t:", this.X5t));

            byte[] x5tBytes = FromBase64Url(this.X5t);
            string kidFromX5t = Convert.ToHexString(x5tBytes);
            rows.Add(new Row("kid:", kidFromX5t));

            rows.Add(new Row(color, "Expires:", $"{this.Expires:yyyy-MM-dd}"));
            rows.Add(new Row("Created On:", this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")));
            rows.Add(new Row("Modified On:", this.ModifiedOn.ToString("yyyy-MM-dd HH:mm:ss")));
            rows.Add(new Row("Disabled:", $"{this.Disabled}"));

            return rows.ToArray();
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
                throw new Exception($"Could not process public PEM (CRT) file '{path}'", e);
            }
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes);
            base64 = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return base64;
        }
    }
}
