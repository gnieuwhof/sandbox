namespace AuthDatabaseManager
{
    using AuthDatabaseManager.Models;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;

    public class Database
    {
        private const SQLiteOpenFlags OPEN_FLAGS =
            // The connection will automatically create the database file if it doesn't exist.
            SQLiteOpenFlags.Create |
            // The connection can read and write data.
            SQLiteOpenFlags.ReadWrite |
            // The connection is opened in serialized threading mode.
            SQLiteOpenFlags.FullMutex;

        private readonly SQLiteConnection connection;


        private Database(SQLiteConnection connection)
        {
            this.connection = connection;
        }


        public static Database Init(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath);

            var connection = new SQLiteConnection(filePath,
                OPEN_FLAGS, storeDateTimeAsTicks: false);

            Type modelType = typeof(Model);

            var entityTypes = modelType.Assembly.GetTypes()
                .Where(type => type.IsSubclassOf(modelType))
                .Where(type => !type.IsAbstract);

            foreach (Type entityType in entityTypes)
            {
                connection.CreateTable(entityType);
            }

            var database = new Database(connection);

            return database;
        }

        public T Record<T>(Guid? id) where T : Model, new()
        {
            T result = this.connection
                .Table<T>()
                .FirstOrDefault(r => r.ID == id);

            return result;
        }

        public TableQuery<T> GetTable<T>() where T : Model, new()
        {
            TableQuery<T> result = this.connection.Table<T>();

            return result;
        }

        public T[] GetActiveRecords<T>() where T : Model, new()
        {
            T[] result = this.connection
                .Table<T>()
                .Where(r => r.Disabled == false)
                .ToArray();

            return result;
        }

        public T[] GetInactiveRecords<T>() where T : Model, new()
        {
            T[] result = this.connection
                .Table<T>()
                .Where(r => r.Disabled == true)
                .ToArray();

            return result;
        }

        public IEnumerable<Model> GetTableFromType(Type modelType)
        {
            MethodInfo method = typeof(Database)
                .GetMethod(nameof(Database.GetActiveRecords));

            MethodInfo generic = method.MakeGenericMethod(modelType);

            var invokeResult = generic.Invoke(this, null);

            var result = invokeResult as IEnumerable<Model>;

            return result;
        }

        public int Delete(Model model)
        {
            return this.connection.Delete(model);
        }

        public int Enable(Model model)
        {
            model.Disabled = false;

            return this.Update(model);
        }

        public int Disable(Model model)
        {
            model.Disabled = true;

            return this.Update(model);
        }

        public int Update(Model model)
        {
            model.ModifiedOn = DateTime.UtcNow;

            return this.connection.Update(model);
        }

        public KeyPair KeyPair(
            Guid id,
            string name,
            string pemFilePath,
            string keyFilePath,
            string fingerprint,
            DateTime? validFrom = null
            )
        {
            string privatePem = File.ReadAllText(pemFilePath);
            string publicKey = File.ReadAllText(keyFilePath);

            DateTime utcNow = DateTime.UtcNow;

            if (!validFrom.HasValue)
            {
                validFrom = utcNow.Date;
            }

            var keyPair = new KeyPair
            {
                ID = id,
                CreatedOn = utcNow,
                ModifiedOn = utcNow,
                Name = name,
                PrivatePem = privatePem,
                PublicKey = publicKey,
                ValidFrom = validFrom.Value,
                Fingerprint = fingerprint
            };

            this.connection.Insert(keyPair);

            return keyPair;
        }

        public Administration Administration(Guid id, string name)
        {
            DateTime utcNow = DateTime.UtcNow;

            var administration = new Administration
            {
                ID = id,
                CreatedOn = utcNow,
                ModifiedOn = utcNow,
                Name = name,
            };

            this.connection.Insert(administration);

            return administration;
        }

        public Registration Registration(Guid id, string name,
            Guid administrationId, string audience, string scope, int validFor)
        {
            DateTime utcNow = DateTime.UtcNow;

            var registration = new Registration
            {
                ID = id,
                CreatedOn = utcNow,
                ModifiedOn = utcNow,
                Name = name,
                FkAdministration = administrationId,
                Audience = audience,
                Scopes = scope,
                ValidityPeriod = validFor
            };

            this.connection.Insert(registration);

            return registration;
        }

        public Secret Secret(
            Guid id,
            string name,
            Guid registrationId,
            string secretValue,
            DateTime? expires = null
            )
        {
            _ = secretValue ??
                throw new ArgumentNullException(nameof(secretValue));

            DateTime utcNow = DateTime.UtcNow;

            if (!expires.HasValue)
            {
                expires = utcNow.Date.AddDays(365);
            }

            string base64Pbkdf2 = Pbkdf2(secretValue);

            int min = Math.Min(3, secretValue.Length);
            string hint = secretValue[..min];

            var secret = new Secret
            {
                ID = id,
                CreatedOn = utcNow,
                ModifiedOn = utcNow,
                Name = name,
                FkRegistration = registrationId,
                Expires = expires.Value,
                DerivedHash = base64Pbkdf2,
                Hint = hint
            };

            this.connection.Insert(secret);

            return secret;
        }

        public Certificate Certificate(
            Guid id,
            string name,
            Guid registrationId,
            string publicPem,
            string x5t,
            DateTime expires
            )
        {
            DateTime utcNow = DateTime.UtcNow;

            var certificate = new Certificate
            {
                ID = id,
                CreatedOn = utcNow,
                ModifiedOn = utcNow,
                FkRegistration = registrationId,
                Name = name,
                PublicPem = publicPem,
                X5t = x5t,
                Expires = expires
            };

            this.connection.Insert(certificate);

            return certificate;
        }

        public ParentsStatus GetParentsStatus(IChild child)
        {
            _ = child ?? throw new ArgumentNullException(nameof(child));

            Model parent = child.Parent(this);

            if (parent == null)
            {
                return ParentsStatus.Deleted;
            }

            if (parent.Disabled)
            {
                return ParentsStatus.Disabled;
            }

            if (parent is IChild pc)
            {
                // Recurse!
                return this.GetParentsStatus(pc);
            }

            return ParentsStatus.Enabled;
        }

        public void Vacuum()
        {
            this.connection.Execute("VACUUM");
        }

        private static string Pbkdf2(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            byte[] derived = Rfc2898DeriveBytes.Pbkdf2(
                value, Array.Empty<byte>(), 100000, HashAlgorithmName.SHA512, 64);

            string result = Convert.ToBase64String(derived);

            return result;
        }
    }
}
