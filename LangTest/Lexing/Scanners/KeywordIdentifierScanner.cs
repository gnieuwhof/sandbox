namespace Lexing.Scanners
{
    using System;

    public class KeywordIdentifierScanner
    {
        public static bool Scan( Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, chr => (chr == '_' || char.IsLetter(chr)),
                "Character must be an underscore or letter.");

            Source src = lexer.Src;
            string result = string.Empty;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if((current != '_') && !char.IsLetterOrDigit(current))
                {
                    break;
                }

                result += current;

                src.Advance();
            }

            if (Keywords.TryGetTokenType(result, out TokenType tokenType))
            {
                token.Type = tokenType;
            }
            else
            {
                token.Type = TokenType.Identifier;
                token.Value = result;
            }

            return true;
        }
    }
}
