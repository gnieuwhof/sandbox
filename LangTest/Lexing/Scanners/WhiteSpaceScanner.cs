namespace Lexing.Scanners
{
    using System;

    public static class WhiteSpaceScanner
    {
        public static void Scan(Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, ' ', '\t');
#endif

            Source src = lexer.Src;
            string value = string.Empty;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if (!char.IsWhiteSpace(current))
                {
                    src.Reverse();
                    break;
                }

                value += current;

                src.Advance();
            }

            token.Type = TokenType.WhiteSpace;
            token.Value = value;
        }
    }
}
