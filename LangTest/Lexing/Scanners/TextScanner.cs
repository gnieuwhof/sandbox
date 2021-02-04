namespace Lexing.Scanners
{
    using System;
    using System.Linq;

    public static class TextScanner
    {
        public static bool ScanString( Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '"');

            string result = string.Empty;
            bool inString = false;
            Source src = lexer.Src;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if (current == '"')
                {
                    if (!inString)
                    {
                        inString = true;
                    }
                    else
                    {
                        inString = false;
                        break;
                    }
                }
                else
                {
                    result += current;
                }

                src.Advance();
            }

            token.Type = TokenType.String;
            token.Value = result;

            return !inString;
        }

        public static bool ScanCharacter( Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '\'');

            Source src = lexer.Src;
            string result = string.Empty;
            bool inCharacter = false;
            bool escape = false;
            var escapeCharacters = new[] { 'n', 'r', 't', '\\' };

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if (current == '\'')
                {
                    if (!inCharacter)
                    {
                        inCharacter = true;
                    }
                    else
                    {
                        inCharacter = false;
                        break;
                    }
                }
                else if(escape)
                {
                    if(!escapeCharacters.Contains(current))
                    {
                        return false;
                    }

                    result += current;

                    escape = false;
                }
                else if (current == '\\')
                {
                    escape = true;

                    result += current;
                }
                else if(result == string.Empty)
                {
                    result += current;
                }
                else
                {
                    return false;
                }

                src.Advance();
            }

            token.Type = TokenType.Char;
            token.Value = result;

            return !inCharacter;
        }

        public static bool ScanNewLine(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '\r', '\n');

            Source src = lexer.Src;
            string result = string.Empty;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if((result != string.Empty) && (current != '\n'))
                {
                    break;
                }
                
                result += current;

                src.Advance();
            }

            src.Reverse();

            token.Type = TokenType.NewLine;
            token.Value = result;

            return true;
        }
    }
}
