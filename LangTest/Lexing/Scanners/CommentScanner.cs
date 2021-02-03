namespace Lexing.Scanners
{
    using System;

    public static class CommentScanner
    {
        public static bool ScanSingleLine(Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '/');
            Scanner.EnsureNext(lexer, '/');

            Source src = lexer.Src;
            Position pos = src.GetPosition();
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

            var token = new Token(pos/*src.Line, src.Character*/, TokenType.SingleComment, result);

            lexer.Add(token);

            return true;
        }

        public static bool ScanMultiLine(Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '/');
            Scanner.EnsureNext(lexer, '*');

            Source src = lexer.Src;
            Position pos = src.GetPosition();
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

            var token = new Token(pos, TokenType.MultiComment, result);

            lexer.Add(token);

            return !inComment;
        }
    }
}
