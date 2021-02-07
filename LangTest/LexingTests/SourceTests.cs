namespace LexingTests
{
    using Lexing;
    using System;
    using Xunit;

    public class SourceTests
    {
        [Fact]
        public void AssertCurrentLexerNullTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Source(null));
        }

        [Fact]
        public void CurrentTest()
        {
            var src = new Source("abc");

            Assert.Equal('a', src.Current);
        }

        [Fact]
        public void CurrentInvalidCharTest()
        {
            var src = new Source(string.Empty);

            Assert.Equal(Source.InvalidChar, src.Current);
        }

        [Fact]
        public void ValidIndexerTest()
        {
            var src = new Source("abc");

            Assert.Equal('b', src[1]);
        }

        [Fact]
        public void InvalidIndexerTest()
        {
            var src = new Source("abc");

            Assert.Equal(Source.InvalidChar, src[10]);
        }

        [Fact]
        public void AdvanceTest()
        {
            var src = new Source("abc");

            src.Advance();

            Assert.Equal('b', src.Current);
        }

        [Fact]
        public void AdvancedTooMuchTest()
        {
            var src = new Source("abc");

            src.Advance(10);

            Assert.Equal(Source.InvalidChar, src.Current);
        }

        [Fact]
        public void reverseTest()
        {
            var src = new Source("abc");

            src.Advance();
            src.Reverse();

            Assert.Equal('a', src.Current);
        }

        [Fact]
        public void ReversedTooMuchTest()
        {
            var src = new Source("abc");

            src.Reverse(10);

            Assert.Equal(Source.InvalidChar, src.Current);
        }

        [Fact]
        public void TextTest()
        {
            string text = "abcdefghijklmnopqrstuvwxyz";
            
            var src = new Source(text);

            Assert.Equal(text, src.Text);
        }

        [Fact]
        public void LengthTest()
        {
            string text = "abcdefghijklmnopqrstuvwxyz";

            var src = new Source(text);

            Assert.Equal(text.Length, src.Length);
        }

        [Fact]
        public void LineZeroTest()
        {
            var src = new Source(string.Empty);

            Assert.Equal(1, src.Line);
        }

        [Fact]
        public void NextLineTest()
        {
            var lexer = new Lexer(string.Empty, Environment.NewLine);

            lexer.GetTokens(false, out LexicalError _);

            Assert.Equal(2, lexer.Src.Line);
        }

        [Fact]
        public void CharacterTest()
        {
            var lexer = new Lexer(string.Empty, "foo");

            lexer.GetTokens(false, out LexicalError _);

            Assert.Equal(4, lexer.Src.Character);
        }
    }
}
