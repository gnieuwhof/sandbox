namespace AuthDatabaseManager.Pages
{
    using System;

    public class VacuumPage : Page
    {
        private readonly Database database;


        public VacuumPage(Page returnPage, Database database)
            : base(returnPage)
        {
            this.database = database;
        }

        public override string Title => "Vacuum Database";

        public override Page Show()
        {
            Console.WriteLine("Are you sure you want to vacuum the database? (y/N)");
            string input = Console.ReadLine();
            if (input == "y")
            {
                Console.WriteLine("Calling VACUUM");
                this.database.Vacuum();
                Write.Green("VACUUM completed");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            return this.ReturnPage;
        }
    }
}
