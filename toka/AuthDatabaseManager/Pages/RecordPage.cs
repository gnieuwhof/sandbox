namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;

    public abstract class RecordPage : Page
    {
        protected readonly Model record;

        protected readonly string modelName;


        public RecordPage(Page returnPage, Model record) : base(returnPage)
        {
            this.record = record ??
                throw new ArgumentNullException(nameof(record));

            this.modelName = Helper.GetName(record);
        }
    }
}
