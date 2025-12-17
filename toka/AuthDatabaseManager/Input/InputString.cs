namespace AuthDatabaseManager.Input
{
    public class InputString : InputBase
    {
        public InputString(string description) : base(description)
        {
        }

        public override string GetValue()
        {
            string str = StringInput.Get(this.Description, this.Default);

            return str;
        }

        public override string GetDefault()
        {
            return $"{this.Default}";
        }

        public override void SetValue(object val)
        {
            this.SetValue(val);
        }
    }
}
