namespace AuthDatabaseManager.Input
{
    public class InputString : InputBase
    {
        public InputString(string description) : base(description)
        {
        }

        public override string GetValue()
        {
            string str = StringInput.Get(this.Description);

            return str;
        }

        public override void SetValue(object val)
        {
            this.SetValue(val);
        }
    }
}
