namespace Lexing.Scanners
{
    using System;

    public static class WhiteSpaceScanner
    {
        public static bool Scan(Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, ' ', '\t');
#endif

            Source src = lexer.Src;
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

            token.Type = TokenType.WhiteSpace;
            token.Value = result;

            return true;
        }
    }
}
