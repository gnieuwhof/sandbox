namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;

    public abstract class ModelPage<T> : Page where T : Model, new()
    {
        protected readonly Database database;

        public override string Title { get; }


        public ModelPage(Database database)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            T instance = Activator.CreateInstance<T>();

            Title = instance.CollectionName;

            if (typeof(T) == typeof(PrivateKey))
            {
                this.Subtitle = "Private Keys are used to sign the tokens";
            }
            else if (typeof(T) == typeof(Registration))
            {
                this.Subtitle = "Registrations a what the token gives access to";
            }
            else if (typeof(T) == typeof(Secret))
            {
                this.Subtitle = "Secrets are used to get a token";
            }
        }


        public override Page Show()
        {
            return new MainMenu(this.database);
        }
    }
}
