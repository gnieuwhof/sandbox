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


        public Registration Registration(Guid id)
        {
            Registration result = this.connection.Get<Registration>(id);

            return result;
        }

        public T[] GetTable<T>() where T : Model, new()
        {
            T[] result = this.connection.Table<T>().ToArray();

            return result;
        }
        public IEnumerable<Model> GetTableFromType(Type modelType)
        {
            MethodInfo method = typeof(Database)
                .GetMethod(nameof(Database.GetTable));

            MethodInfo generic = method.MakeGenericMethod(modelType);

            var invokeResult = generic.Invoke(this, null);

            var result = invokeResult as IEnumerable<Model>;

            return result;
        }

        public int Delete(Model model)
        {
            return this.connection.Delete(model);
        }

        public PrivateKey PrivateKey(
            string name,
            string keyFilePath,
            string fingerprint,
            DateTime? validFrom = null
            )
        {
            string content = File.ReadAllText(keyFilePath);

            DateTime utcNow = DateTime.UtcNow;

            if (!validFrom.HasValue)
            {
                validFrom = utcNow.Date;
            }

            var privateKey = new PrivateKey
            {
                ID = Guid.NewGuid(),
                CreatedOn = utcNow,
                Name = name,
                Content = content,
                ValidFrom = validFrom.Value,
                Fingerprint = fingerprint
            };

            this.connection.Insert(privateKey);

            return privateKey;
        }

        public Registration Registration(string name, string scope)
        {
            DateTime utcNow = DateTime.UtcNow;

            var registration = new Registration
            {
                ID = Guid.NewGuid(),
                CreatedOn = utcNow,
                Name = name,
                Scopes = scope
            };

            this.connection.Insert(registration);

            return registration;
        }

        public Secret Secret(
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
            string hint = secretValue.Substring(0, min);

            var secret = new Secret
            {
                ID = Guid.NewGuid(),
                CreatedOn = utcNow,
                Name = name,
                FkRegistration = registrationId,
                Expires = expires.Value,
                DerivedHash = base64Pbkdf2,
                Hint = hint
            };

            this.connection.Insert(secret);

            return secret;
        }

        private static string Pbkdf2(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            byte[] derived = Rfc2898DeriveBytes.Pbkdf2(
                value, new byte[0], 100000, HashAlgorithmName.SHA512, 64);

            string result = Convert.ToBase64String(derived);

            return result;
        }

        public IEnumerable<Row> GetGrid(IEnumerable<Model> records)
        {
            var grid = new List<Row>();

            int index = 0;
            foreach (object record in records)
            {
                ++index;
                if (record is Model model)
                {
                    var columns = new List<string>();

                    columns.Add($"{index}");

                    var modelRow = model.Row(this);

                    columns.AddRange(modelRow.Columns);

                    grid.Add(new Row(modelRow.Color, columns.ToArray()));
                }
            }

            return grid;
        }
    }
}
