namespace Lexing.Scanners
{
    using System;

    public static class CommentScanner
    {
        public static bool ScanSingleLine(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '/');
            Scanner.EnsureNext(lexer, '/');

            Source src = lexer.Src;
            string result = string.Empty;

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if(result.Length >= "//".Length)
                {
                    char? next = src.Peek();

                    if((next == '\r') || (next == '\n'))
                    {
                        result += current;

                        break;
                    }
                }

                result += current;

                src.Advance();
            }

            token.Type = TokenType.SingleComment;
            token.Value = result;

            return true;
        }

        public static bool ScanMultiLine(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '/');
            Scanner.EnsureNext(lexer, '*');

            Source src = lexer.Src;
            string result = string.Empty;
            bool inComment = true;
            char previous = ' ';

            while (!src.ReachedEnd())
            {
                char current = src.Current;

                if((result.Length > "/*".Length) &&
                    (previous == '*') && (current == '/'))
                {
                    inComment = false;

                    result += current;

                    break;
                }

                result += current;

                src.Advance();

                previous = current;
            }

            token.Type = TokenType.MultiComment;
            token.Value = result;

            return !inComment;
        }
    }
}
