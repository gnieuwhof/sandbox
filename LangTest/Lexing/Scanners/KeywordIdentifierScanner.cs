namespace Lexing.Scanners
{
    using System;
    using System.Text;

    public class KeywordIdentifierScanner
    {
        public static bool Scan( Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, chr => (chr == '_' || char.IsLetter(chr)),
                "Character must be an underscore or letter.");
#endif

            Source src = lexer.Src;
            StringBuilder builder = lexer.Builder;
            builder.Clear();

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if((current != '_') && !char.IsLetterOrDigit(current))
                {
                    break;
                }

                builder.Append(current);

                src.Advance();
            }

            string result = builder.ToString();

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
