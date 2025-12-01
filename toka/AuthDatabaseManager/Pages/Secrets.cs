namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Input;
    using AuthDatabaseManager.Models;
    using System;

    public class Secrets : ReturnBase
    {
        private readonly Database database;


        public override string Title => "Create Secret";


        public Secrets(Database database)
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
            var registration = new InputVal<Registration>(
                this.database, "Registration:");
            var value = new InputVal<string>(this.database, "Secret:");
            var expires = new InputVal<DateTime>(this.database, "Expires:")
            {
                Default = DateTime.UtcNow.Date.AddYears(1)
            };

            bool result = DatabaseInput.Get(
                this.Title,
                this.database,
                name,
                registration,
                value,
                expires
                );

            if (result)
            {
                this.database.Secret(name.Value,
                    registration.Value.ID, value.Value, expires.Value);

                Console.WriteLine("Secret saved.");
                Console.WriteLine("(any key to continue)");
                Console.ReadKey();
            }
        }
    }
}
