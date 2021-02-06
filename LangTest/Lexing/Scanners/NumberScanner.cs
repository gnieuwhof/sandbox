namespace Lexing.Scanners
{
    using System;

    public static class NumberScanner
    {
        public static bool Scan( Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer,
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.');
#endif

            Source src = lexer.Src;
            string result = string.Empty;
            bool dotEncountered = false;
            bool success = true;

            while (!src.ReachedEnd())
            {
                char current = src.Current;
                char? next = src.Peek();

                if (dotEncountered && (next == '.'))
                {
                    src.Advance();
                    success = false;
                    break;
                }

                if (current == '.')
                {
                    dotEncountered = true;

                    if (!char.IsDigit(next ?? 'X'))
                    {
                        success = false;
                        break;
                    }
                }
                else if (!char.IsDigit(current))
                {
                    break;
                }

                result += current;

                src.Advance();
            }

            if(success)
            {
                src.Reverse();
            }

            TokenType tokenType = dotEncountered
                ? TokenType.Float
                : TokenType.Int;

            token.Type = tokenType;
            token.Value = result;

            return success;
        }
    }
}
