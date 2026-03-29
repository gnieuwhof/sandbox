namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System.Collections.Generic;

    public class MainMenu : MenuPage
    {
        public MainMenu(Database database)
            : base(database, returnPage: null)
        {
        }


        public override string Title => "Main Menu";

        protected override List<(char?, object)> Options
        {
            get
            {
                Database db = this.database;

                return new()
                {
                    ('1', new MainModelPage<KeyPair>(null, db, new CreatePage(null, db, new KeyPair()))),
                    ('2', new MainModelPage<Administration>(null, db, new CreatePage(null, db, new Administration()))),
                    ('3', new MainModelPage<Registration>(null, db, new CreatePage(null, db, new Registration()))),
                    ('4', new MainModelPage<Secret>(null, db, new CreatePage(null, db, new Secret()))),
                    ('5', new MainModelPage<Certificate>(null, db, new CreatePage(null, db, new Certificate()))),
                    (null, LINE),
                    ('i', new InfoPage(this)),
                    ('t', new ToolsPage(this, db)),
                    ('d', new DiagnosisPage(this, db)),
                    (null, LINE),
                    ('q', new Quit()),
                };
            }
        }
    }
}
