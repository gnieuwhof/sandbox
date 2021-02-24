namespace Lexing.Scanners
{
    using System;
    using System.Linq;
    using System.Text;

    public static class TextScanner
    {
        public static bool ScanString(Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '"');
#endif

            bool inString = false;
            Source src = lexer.Src;
            StringBuilder builder = lexer.Builder;
            bool escaped = false;
            var illegalCharacters = new[] { '\r', '\n' };
            var escapeCharacters = new[] { 'r', 'n', 't', '"', '\\' };

            builder.Clear();

            token.Type = TokenType.String;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if ((current == '"') && !escaped)
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
                else if ((current == '\\') && !escaped)
                {
                    escaped = true;

                    builder.Append(current);
                }
                else if (illegalCharacters.Contains(current))
                {
                    return false;
                }
                else
                {
                    if (escaped && escapeCharacters.Contains(current))
                    {
                        escaped = false;
                    }

                    if (current != '\t')
                    {
                        builder.Append(current);
                    }
                    else
                    {
                        builder.Append('\\');
                        builder.Append('t');
                    }
                }

                src.Advance();
            }

            token.Type = TokenType.String;
            token.Value = builder.ToString();

            return !inString;
        }

        public static bool ScanCharacter(Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '\'');
#endif

            Source src = lexer.Src;
            string result = string.Empty;
            bool inCharacter = false;
            bool escaped = false;
            var illegalCharacters = new[] { '\r', '\n', '\t' };
            var escapeCharacters = new[] { 'r', 'n', 't', '\'', '\\' };

            token.Type = TokenType.Char;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if ((current == '\'') && !escaped)
                {
                    if (!inCharacter)
                    {
                        inCharacter = true;
                    }
                    else if (result == string.Empty)
                    {
                        return false;
                    }
                    else
                    {
                        inCharacter = false;
                        break;
                    }
                }
                else if (escaped)
                {
                    if (!escapeCharacters.Contains(current))
                    {
                        return false;
                    }

                    result += current;

                    escaped = false;
                }
                else if (illegalCharacters.Contains(current))
                {
                    return false;
                }
                else if (result == string.Empty)
                {
                    if (current == '\\')
                    {
                        escaped = true;
                    }

                    result += current;
                }
                else
                {
                    return false;
                }

                src.Advance();
            }

            token.Value = result;

            return (!inCharacter && !escaped);
        }

        public static bool ScanNewLine(Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '\r', '\n');
#endif

            Source src = lexer.Src;
            string value = string.Empty;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if ((value != string.Empty) && (current != '\n'))
                {
                    break;
                }

                value += current;

                src.Advance();
            }

            src.Reverse();

            token.Type = TokenType.NewLine;
            token.Value = value;

            return true;
        }
    }
}
