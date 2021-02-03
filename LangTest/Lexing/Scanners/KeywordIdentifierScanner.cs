namespace Lexing.Scanners
{
    using System;

    public class KeywordIdentifierScanner
    {
        public static bool Scan( Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, chr => (chr == '_' || char.IsLetter(chr)),
                "Character must be an underscore or letter.");

            Source src = lexer.Src;
            Position pos = src.GetPosition();
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

            Token token;

            if(Keywords.TryGetTokenType(result, out TokenType tokenType))
            {
                token = new Token(pos, tokenType);
            }
            else
            {
                token = new Token(pos, TokenType.Identifier, result);
            }

            lexer.Add(token);

            return true;
        }
    }
}
