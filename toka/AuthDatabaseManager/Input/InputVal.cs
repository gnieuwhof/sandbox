namespace AuthDatabaseManager.Input
{
    using AuthDatabaseManager.Models;
    using System;

    public class InputVal<T> : InputBase
    {
        private readonly Database database;


        public T Value { get; set; }


        public InputVal(Database database,
            string description, InputType? inputType = null
            )
            : base(description)
        {
            this.database = database ??
                throw new ArgumentNullException(nameof(database));

            if (!inputType.HasValue)
            {
                Type type = typeof(T);

                if (type == typeof(DateTime))
                {
                    inputType = InputType.DateInput;
                }

                if (type == typeof(string))
                {
                    inputType = InputType.StringInput;
                }

                if (type.IsSubclassOf(typeof(Model)))
                {
                    inputType = InputType.ModelInput;
                }

                if (type == typeof(int))
                {
                    inputType = InputType.IntInput;
                }
            }

            if (!inputType.HasValue)
            {
                throw new InvalidOperationException("Cannot determine input type.");
            }

            this.Type = inputType.Value;
        }


        public override string GetValue()
        {
            string result = GetString(this.Value);

            return result;
        }

        public override string GetDefault()
        {
            string result = GetString(this.Default);

            return result;
        }

        private string GetString(object obj)
        {
            if (obj is DateTime dt)
            {
                return dt.ToString("yyyy-MM-dd");
            }

            if (obj is Model model)
            {
                string result = string.Join(' ', model.Row(this.database));

                return $"({result})";
            }

            return $"{obj}";
        }

        public override void SetValue(object val)
        {
            if (val == null)
            {
                this.Value = default;
                return;
            }

            if (val is T t)
            {
                this.Value = t;
                return;
            }

            throw new ArgumentException("Invalid type");
        }

        public void SetDefault<M>(Guid? id) where M : Model, new()
        {
            M val = this.database.Record<M>(id.Value);

            this.Default = val;
        }
    }
}
