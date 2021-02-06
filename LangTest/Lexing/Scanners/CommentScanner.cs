namespace Lexing.Scanners
{
    using System;
    using System.Text;

    public static class CommentScanner
    {
        public static bool ScanSingleLine(Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '/');
            Scanner.EnsureNext(lexer, '/');
#endif

            Source src = lexer.Src;
            StringBuilder builder = lexer.Builder;
            builder.Clear();

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if(builder.Length >= "//".Length)
                {
                    char? next = src.Peek();

                    if((next == '\r') || (next == '\n'))
                    {
                        builder.Append(current);

                        break;
                    }
                }

                builder.Append(current);

                src.Advance();
            }

            token.Type = TokenType.SingleComment;
            token.Value = builder.ToString();

            return true;
        }

        public static bool ScanMultiLine(Lexer lexer, ref Token token)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '/');
            Scanner.EnsureNext(lexer, '*');
#endif

            Source src = lexer.Src;
            StringBuilder builder = lexer.Builder;
            builder.Clear();

            bool inComment = true;
            char previous = ' ';

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if((builder.Length > "/*".Length) &&
                    (previous == '*') && (current == '/'))
                {
                    inComment = false;

                    builder.Append(current);

                    break;
                }

                builder.Append(current);

                src.Advance();

                previous = current;
            }

            token.Type = TokenType.MultiComment;
            token.Value = builder.ToString();

            return !inComment;
        }
    }
}
