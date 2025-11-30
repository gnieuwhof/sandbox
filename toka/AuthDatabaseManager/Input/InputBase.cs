namespace AuthDatabaseManager.Input
{
    using System;

    public abstract class InputBase
    {
        public enum InputType
        {
            StringInput,
            FileInput,
            DateInput,
            ModelInput
        }


        public InputType Type { get; protected set; }

        public string Description { get; }

        public object Default { get; set; }


        public InputBase(string description)
        {
            this.Description = description ??
                throw new ArgumentNullException(nameof(description));
        }


        public abstract string GetValue();

        public abstract void SetValue(object val);


        public Type GetGenericType()
        {
            Type type = this.GetType();

            Type genericType = type.GetGenericArguments()[0];

            return genericType;
        }
    }
}
