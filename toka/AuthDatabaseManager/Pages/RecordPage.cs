namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;

    public abstract class RecordPage : Page
    {
        protected readonly Model record;

        protected readonly string modelName;


        public RecordPage(Model record)
        {
            this.record = record ??
                throw new ArgumentNullException(nameof(record));

            this.modelName = Helper.GetName(record);
        }
    }
}
