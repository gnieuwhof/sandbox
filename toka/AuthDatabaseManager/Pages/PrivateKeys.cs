namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Input;
    using System;

    public class PrivateKeys : ReturnBase
    {
        private readonly Database database;


        public override string Title => "Create Private Key";


        public PrivateKeys(Database database)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));
        }


        public override Page Show()
        {
            Page result = ExceptionRetry(this.ReturnPage, CreateRecord);

            return result;
        }

        private void CreateRecord()
        {
            var name = new InputVal<string>(this.database, "Name:");
            var pemPath = new InputVal<string>(this.database,
                "PEM file path:", InputBase.InputType.FileInput);
            var fingerprint = new InputVal<string>(
                this.database, "Public key fingerprint:");
            var validFrom = new InputVal<DateTime>(this.database, "Valid from:")
            {
                Default = DateTime.UtcNow.Date
            };

            bool result = Inputs.Get(
                this.Title,
                name,
                pemPath,
                fingerprint,
                validFrom
                );

            if (result)
            {
                this.database.PrivateKey(name.Value, pemPath.Value,
                    fingerprint.Value, validFrom.Value);

                Console.WriteLine("Private Key saved.");
                Console.WriteLine("(any key to continue)");
                Console.ReadKey();
            }
        }
    }
}
