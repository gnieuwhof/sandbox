namespace AuthDatabaseManager.Pages
{
    using System.Collections.Generic;

    public class DiagnosisPage : MenuPage
    {
        public DiagnosisPage(Page returnPage, Database database)
            : base(database, returnPage, "q")
        {
        }

        public override string Title => "Diagosis";

        protected override List<(char?, object)> Options
        {
            get
            {
                Database db = this.database;

                return new()
                {
                    ('1', new DiagnosisDetailsPage(this, this.database, DiagLevel.Info)),
                    ('2', new DiagnosisDetailsPage(this, this.database, DiagLevel.Warning)),
                    ('3', new DiagnosisDetailsPage(this, this.database, DiagLevel.Error)),
                    (null, LINE),
                    ('q', this.ReturnPage),
                };
            }
        }
    }
}
