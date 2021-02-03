namespace Lexing
{
    using System;

    public class Token
    {
        public readonly TokenType Type;

        public readonly string Value;

        public readonly int Line;

        public readonly int Character;


        public Token(Position pos, TokenType type, string val)
        {
            this.Line = pos.Line;
            this.Character = pos.Character;
            this.Type = type;
            this.Value = val;
        }

        public Token(Position pos, TokenType type)
            : this(pos, type, null)
        {
        }

        public Token(Position pos, char c)
        {
            this.Line = pos.Line;
            this.Character = pos.Character;

            UInt16 intVal = Convert.ToUInt16(c);

            TokenType tokenType = (TokenType)Enum.Parse(typeof(TokenType), $"{intVal}");

            this.Type = tokenType;
        }


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
