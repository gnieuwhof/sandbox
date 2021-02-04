namespace Lexing.Scanners
{
    using System;

    public static class OperatorScanner
    {
        public static void ScanExclamation(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '!');

            token.Type = Scanner.GetTokenType(lexer, '=',
                TokenType.NotEquals, TokenType.Exclamation);
        }

        public static void ScanAmpersand(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '&');

            token.Type = Scanner.GetTokenType(lexer, '&',
                TokenType.AmpAmp, TokenType.Ampersand);
        }

        public static void ScanPipeline(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '|');

            token.Type = Scanner.GetTokenType(lexer, '|',
                TokenType.PipePipe, TokenType.Pipeline);
        }

        public static void ScanEquals(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '=');

            token.Type = Scanner.GetTokenType(lexer, '=',
                TokenType.EqualsEquals, TokenType.Equals);
        }

        public static void ScanLessThan(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '<');

            token.Type = Scanner.GetTokenType(lexer, '=',
                TokenType.LessThanOrEquals, TokenType.LessThan);
        }

        public static void ScanGreaterThan(Lexer lexer, ref Token token)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '>');

            token.Type = Scanner.GetTokenType(lexer, '=',
                TokenType.GreaterThanOrEquals, TokenType.GreaterThan);
        }
    }
}
