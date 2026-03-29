namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DiagnosisDetailsPage : Page
    {
        private readonly Database database;
        private readonly DiagLevel level;


        public DiagnosisDetailsPage(Page returnPage, Database database, DiagLevel level)
            : base(returnPage)
        {
            this.database = database;
            this.level = level;

            this.Title = level switch
            {
                DiagLevel.Error => $"Diagnosis | ERRORS",
                DiagLevel.Warning => $"Diagnosis | WARNINGS",
                _ => $"Diagnosis | INFO"
            };

            this.Legend = LEGEND;
        }


        public override string Title { get; }


        public static Dictionary<string, ConsoleColor> LEGEND =>
            new Dictionary<string, ConsoleColor>
            {
                { "Info", ConsoleColor.Gray },
                { "Warning", ConsoleColor.Yellow },
                { "Error", ConsoleColor.Red },
            };


        public override Page Show()
        {
            var list = new List<(DiagLevel, string)>();

            list.AddRange(this.ProcessType<KeyPair>());
            list.AddRange(this.ProcessType<Administration>());
            list.AddRange(this.ProcessType<Registration>());
            list.AddRange(this.ProcessType<Secret>());
            list.AddRange(this.ProcessType<Certificate>());

            foreach (var line in list)
            {
                if ((int)line.Item1 >= (int)this.level)
                {
                    if (line.Item1 == DiagLevel.Info)
                    {
                        Console.WriteLine($"INFO: {line.Item2}");
                    }
                    if (line.Item1 == DiagLevel.Warning)
                    {
                        Write.Warning($"WARN: {line.Item2}");
                    }
                    if (line.Item1 == DiagLevel.Error)
                    {
                        Write.Error($"ERRO: {line.Item2}");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return this.ReturnPage;
        }

        private IEnumerable<(DiagLevel, string)> ProcessType<T>()
            where T : Model, new()
        {
            var list = new List<(DiagLevel, string)>();

            IEnumerable<T> records = this.GetRecords<T>(_ => true);

            foreach (T record in records)
            {
                IEnumerable<(DiagLevel, string)> recordDiag =
                    this.ProcessRecord(record);

                list.AddRange(recordDiag);
            }

            return list;
        }

        private IEnumerable<(DiagLevel, string)> ProcessRecord<T>(T record)
            where T : Model
        {
            var list = new List<(DiagLevel, string)>();

            Type type = typeof(T);

            list.Add((DiagLevel.Info, $"{type.Name}: {record.Name}"));

            if (record.Disabled)
            {
                list.Add((DiagLevel.Warning, $"{type.Name}: {record.Name} is DISABLED"));
            }

            DiagLevel level = DiagLevel.Info;
            int childCount = -1;

            if (!record.Disabled && record is KeyPair kp)
            {
                var activeKeyPairs = this.database
                    .GetActiveRecords<KeyPair>()
                    .Where(k => k.ValidFrom < DateTime.UtcNow.Date)
                    .Where(k => k.ValidFrom > kp.ValidFrom);

                if (activeKeyPairs.Any())
                {
                    list.Add((DiagLevel.Warning,
                        $"{type.Name}: {record.Name} is OVERRIDDER"));
                }
            }

            if (record is Administration admin)
            {
                var registrations = this
                    .GetRecords<Registration>(r => r.FkAdministration == admin.ID);

                childCount = registrations.Count();

                if (childCount == 0)
                {
                    level = DiagLevel.Warning;
                }

                string s = childCount == 1 ? "" : "s";
                list.Add((level,
                    $"{type.Name}: {record.Name} has {childCount} registration{s}"));
            }

            if (record is Registration reg)
            {
                var parent = this
                    .GetRecords<Administration>(a => a.ID == reg.FkAdministration)
                    .FirstOrDefault();

                if (parent == null)
                {
                    list.Add((DiagLevel.Error,
                        $"{type.Name}: {record.Name} parent Administration is deleted"));
                }
                else if (parent.Disabled)
                {
                    list.Add((DiagLevel.Error,
                        $"{type.Name}: {record.Name} parent Administration is disabled"));
                }

                var secrets = this
                        .GetRecords<Secret>(r => r.FkRegistration == reg.ID);

                var certificates = this
                    .GetRecords<Certificate>(r => r.FkRegistration == reg.ID);

                int secretsCount = secrets.Count();

                int certificatesCount = certificates.Count();

                childCount = secretsCount + certificatesCount;

                if (childCount == 0)
                {
                    level = DiagLevel.Warning;
                }

                string s = secretsCount == 1 ? "" : "s";
                list.Add((level,
                    $"{type.Name}: {record.Name} has {secretsCount} secret{s}"));

                s = certificatesCount == 1 ? "" : "s";
                list.Add((level,
                    $"{type.Name}: {record.Name} has {certificatesCount} certificate{s}"));
            }

            if (record is Secret secret)
            {
                var parent = this
                    .GetRecords<Registration>(r => r.ID == secret.FkRegistration)
                    .FirstOrDefault();

                if (parent == null)
                {
                    list.Add((DiagLevel.Error,
                        $"{type.Name}: {record.Name} parent Registration is deleted"));
                }
                else if (parent.Disabled)
                {
                    list.Add((DiagLevel.Error,
                        $"{type.Name}: {record.Name} parent Registration is disabled"));
                }

                if (secret.Expires < DateTime.UtcNow.Date)
                {
                    list.Add((DiagLevel.Error,
                        $"{type.Name}: {record.Name} is EXPIRED"));
                }
                else if (secret.Expires < DateTime.UtcNow.AddDays(28))
                {
                    list.Add((DiagLevel.Warning,
                        $"{type.Name}: {record.Name} is EXPIRING"));
                }
            }

            if (record is Certificate certificate)
            {
                var parent = this
                    .GetRecords<Registration>(r => r.ID == certificate.FkRegistration)
                    .FirstOrDefault();

                if (parent == null)
                {
                    list.Add((DiagLevel.Error,
                        $"{type.Name}: {record.Name} parent Registration is deleted"));
                }
                else if (parent.Disabled)
                {
                    list.Add((DiagLevel.Error,
                        $"{type.Name}: {record.Name} parent Registration is disabled"));
                }

                if (certificate.Expires < DateTime.UtcNow.Date)
                {
                    list.Add((DiagLevel.Error,
                        $"{type.Name}: {record.Name} is EXPIRED"));
                }
                else if (certificate.Expires < DateTime.UtcNow.AddDays(28))
                {
                    list.Add((DiagLevel.Warning,
                        $"{type.Name}: {record.Name} is EXPIRING"));
                }
            }

            return list;
        }

        private IEnumerable<T> GetRecords<T>(Func<T, bool> predicate)
            where T : Model, new()
        {
            IEnumerable<T> records = this.database.GetTable<T>()
                .Where(predicate);

            return records;
        }
    }
}
