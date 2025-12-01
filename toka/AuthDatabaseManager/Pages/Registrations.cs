namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Input;
    using System;

    public class Registrations : ReturnBase
    {
        private readonly Database database;


        public override string Title => "Create Registration";


        public Registrations(Database database)
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
            var scope = new InputVal<string>(this.database, "Scopes:");

            bool result = Inputs.Get(
                this.Title,
                name,
                scope
                );

            if (result)
            {
                this.database.Registration(name.Value, scope.Value);

                Console.WriteLine("Registration saved.");
                Console.WriteLine("(any key to continue)");
                Console.ReadKey();
            }
        }
    }
}
