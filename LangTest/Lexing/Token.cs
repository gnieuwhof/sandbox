namespace Lexing
{
    public struct Token
    {
        public TokenType Type;

        public string Value;

        public int Line;

        public int Character;


        public override string ToString()
        {
            string val = this.Value;

            switch (this.Type)
            {
                case TokenType.NewLine:
                case TokenType.WhiteSpace:
                case TokenType.SingleComment:
                    val = val
                        .Replace("\r", "\\r")
                        .Replace("\n", "\\n")
                        .Replace("\t", "\\t");
                    break;
            }

            switch (this.Type)
            {
                case TokenType.WhiteSpace:
                case TokenType.SingleComment:
                case TokenType.String:
                    val = $"\"{val}\"";
                    break;
            }

            return (this.Value == null)
                ? $"{this.Line},{this.Character}:{this.Type}"
                : $"{this.Line},{this.Character}:{this.Type}:{val}";
        }
    }
}
