namespace Lexing.Scanners
{
    using System;

    public static class WhiteSpaceScanner
    {
        public static bool Scan(Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, ' ', '\t');

            Source src = lexer.Src;
            Position pos = src.GetPosition();
            string result = string.Empty;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if (!char.IsWhiteSpace(current))
                {
                    src.Reverse();
                    break;
                }

                result += current;

                src.Advance();
            }

            var token = new Token(pos, TokenType.WhiteSpace, result);

            lexer.Add(token);

            return true;
        }
    }
}
