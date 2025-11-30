namespace AuthDatabaseManager.Input
{
    using System;

    public static class Inputs
    {
        public static bool Get(string title, params InputBase[] inputs)
        {
            bool result = DatabaseInput.Get(
                title, database: null, inputs);

            return result;
        }

        public static bool Get(InputBase input, Database database = null)
        {
            switch (input.Type)
            {
                case InputBase.InputType.FileInput:
                    {
                        string filePath = PathInput.GetFile(input.Description);
                        input.SetValue(filePath);
                        return (filePath != null);
                    }
                case InputBase.InputType.DateInput:
                    {
                        DateTime? defaultDate = null;
                        if (input.Default is DateTime dt)
                        {
                            defaultDate = dt;
                        }
                        DateTime? date = DateInput.Get(input.Description, defaultDate);
                        input.SetValue(date);
                        return (date != null);
                    }
                case InputBase.InputType.StringInput:
                    {
                        string str = StringInput.Get(input.Description);
                        input.SetValue(str);
                        return true;
                    }
                case InputBase.InputType.ModelInput:
                    {
                        object selected = SelectInput.Get(input, database);
                        input.SetValue(selected);
                        return (selected != null);
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
