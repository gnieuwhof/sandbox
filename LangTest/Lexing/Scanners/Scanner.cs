namespace Lexing.Scanners
{
    using System;
    using System.Linq;

    public static class Scanner
    {
        public static TokenType GetTokenType(Lexer lexer,
            char nextExpected, TokenType then, TokenType @else)
        {
#if DEBUG
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));
#endif

            Source src = lexer.Src;
            char next = src.Peek();

            TokenType tokenType = (next == nextExpected)
                ? then
                : @else;

            if (next == nextExpected)
            {
                src.Advance();
            }

            return tokenType;
        }

        public static void EnsureCurrent(Lexer lexer, params char[] characters)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Source src = lexer.Src;
            char current = src.Current;

            InnerEnsureCharIn(current, characters);
        }

        public static void EnsureCurrent(Lexer lexer,
            Func<char, bool> validator, string expectedMessage)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Source src = lexer.Src;
            char current = src.Current;

            if (!validator(current))
            {
                throw new InvalidOperationException(expectedMessage);
            }
        }

        public static void EnsureNext(Lexer lexer, params char[] characters)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Source src = lexer.Src;
            char next = src.Peek();

            InnerEnsureCharIn(next, characters);
        }

        private static void InnerEnsureCharIn(
            char chr, char[] characters)
        {
            if (characters == null)
                throw new ArgumentNullException(nameof(characters));

            if (characters.Length == 0)
            {
                throw new ArgumentException(
                    $"{nameof(characters)} must contain at least one element.");
            }

            if (!characters.Contains(chr))
            {
                throw new InvalidOperationException();
            }
        }
    }
}
