namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;

    public abstract class ModelPage<T> : Page where T : Model, new()
    {
        protected readonly Database database;

        public override string Title { get; }


        public ModelPage(Page returnPage, Database database) : base(returnPage)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            T instance = Activator.CreateInstance<T>();

            this.Title = instance.Title;

            this.Subtitle = instance.Subtitle;

            this.Legend = instance.Legend;
        }
    }
}
