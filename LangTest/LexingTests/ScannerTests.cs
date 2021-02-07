namespace LexingTests
{
    using Lexing;
    using Lexing.Scanners;
    using System;
    using Xunit;

    public class ScannerTests
    {
        [Fact]
        public void AssertCurrentLexerNullTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Scanner.EnsureCurrent(null));
        }

        [Fact]
        public void AssertCurrentCharactersNullTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Scanner.EnsureCurrent(new Lexer(string.Empty, string.Empty), null));
        }

        [Fact]
        public void AssertCurrentNoCharactersTest()
        {
            Assert.Throws<ArgumentException>(() =>
                Scanner.EnsureCurrent(new Lexer(string.Empty, string.Empty), new char[0]));
        }

        [Fact]
        public void AssertCurrentOneTest()
        {
            var lexer = new Lexer(string.Empty, " ");

            Assert.Throws<InvalidOperationException>(() =>
                Scanner.EnsureCurrent(lexer, new[] { '=' }));
        }

        [Fact]
        public void AssertCurrentTwoTest()
        {
            var lexer = new Lexer(string.Empty, " ");

            Assert.Throws<InvalidOperationException>(() =>
                Scanner.EnsureCurrent(lexer, new[] { '=', '!' }));
        }

        [Fact]
        public void AssertCurrentThreeTest()
        {
            var lexer = new Lexer(string.Empty, " ");

            Assert.Throws<InvalidOperationException>(() =>
                Scanner.EnsureCurrent(lexer, new[] { '=', '<', '>' }));
        }

        [Fact]
        public void AssertCurrentCorrectTest()
        {
            var lexer = new Lexer(string.Empty, "=");

            Scanner.EnsureCurrent(lexer, '=');
        }
    }
}
