using System;

namespace Lexing.Scanners
{
    public static class OperatorScanner
    {
        public static void ScanExclamation(Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '!');

            Token token = Scanner.GetToken(lexer, '=',
                TokenType.NotEquals, TokenType.Exclamation);

            lexer.Add(token);
        }

        public static void ScanEquals( Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '=');

            Token token = Scanner.GetToken(lexer, '=',
                TokenType.EqualsEquals, TokenType.Equals);

            lexer.Add(token);
        }

        public static void ScanLessThan( Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '<');

            Token token = Scanner.GetToken( lexer, '=',
                TokenType.LessThanOrEquals, TokenType.LessThan);

            lexer.Add(token);
        }

        public static void ScanGreaterThan( Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '>');

            Token token = Scanner.GetToken(lexer, '=',
                TokenType.GreaterThanOrEquals, TokenType.GreaterThan);

            lexer.Add(token);
        }

        public static void ScanAmpersand( Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '&');

            Token token = Scanner.GetToken(lexer, '&',
                TokenType.AmpAmp, TokenType.Ampersand);

            lexer.Add(token);
        }

        public static void ScanPipeline( Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            Scanner.EnsureCurrent(lexer, '|');

            Token token = Scanner.GetToken(lexer, '|',
                TokenType.PipePipe, TokenType.Pipeline);

            lexer.Add(token);
        }
    }
}
