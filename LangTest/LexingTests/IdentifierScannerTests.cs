namespace LexingTests
{
    using Lexing;
    using Lexing.Scanners;
    using System;
    using Xunit;

    public class IdentifierScannerTests
    {
        private Token token = default;

        [Fact]
        public void EnsureFirstCorrectCharTestTest()
        {
            var valid = new[] { "_", "a", "X" };

            foreach (string val in valid)
            {
                var lexer = Lexer.CreateState(val);

                KeywordIdentifierScanner.Scan(lexer, ref this.token);
            }
        }

        [Fact]
        public void EnsureFirstIncorrectCharTestTest()
        {
            var lexer = Lexer.CreateState("$");

            Assert.Throws<InvalidOperationException>(() =>
                KeywordIdentifierScanner.Scan(lexer, ref this.token));
        }
    }
}
