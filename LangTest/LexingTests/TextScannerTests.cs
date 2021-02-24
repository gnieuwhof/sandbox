namespace LexingTests
{
    using Lexing;
    using Lexing.Scanners;
    using Xunit;

    public class TextScannerTests
    {
        private Token token = default;

        [Fact]
        public void EmptyLiteralTest()
        {
            var lexer = Lexer.CreateState("''");

            bool result = TextScanner.ScanCharacter(lexer, ref this.token);

            Assert.False(result);
        }

        [Fact]
        public void TwoCharactersInLiteralTest()
        {
            var lexer = Lexer.CreateState("'XX'");

            bool result = TextScanner.ScanCharacter(lexer, ref this.token);

            Assert.False(result);
        }

        [Fact]
        public void UnescapedLiteralsTest()
        {
            var unescaped = new[] { "'\n'", "'\r'", "'\t'", "'\\'", "'\''" };

            foreach (string input in unescaped)
            {
                var lexer = Lexer.CreateState(input);

                bool result = TextScanner.ScanCharacter(lexer, ref this.token);

                Assert.False(result);
            }
        }

        [Fact]
        public void EscapedBackslashLiteralTest()
        {
            var escaped = new[] { "'\\''", "'\\n'", "'\\r'", "'\\t'", "'\\\\'" };

            foreach (string input in escaped)
            {
                var lexer = Lexer.CreateState(input);

                bool result = TextScanner.ScanCharacter(lexer, ref this.token);

                Assert.True(result);
            }
        }

        [Fact]
        public void FourBackslashesLiteralTest()
        {
            var lexer = Lexer.CreateState("'\\\\\\\\'");

            bool result = TextScanner.ScanCharacter(lexer, ref this.token);

            Assert.False(result);
        }

        [Fact]
        public void EmptyStringTest()
        {
            var lexer = Lexer.CreateState("\"\"");

            bool result = TextScanner.ScanString(lexer, ref this.token);

            Assert.True(result);
            Assert.Equal("", this.token.Value);
        }

        [Fact]
        public void EscapedDoubleQuoteInStringTest()
        {
            var lexer = Lexer.CreateState("\"\\\"\"");

            bool result = TextScanner.ScanString(lexer, ref this.token);

            Assert.True(result);
            Assert.Equal("\\\"", this.token.Value);
        }

        [Fact]
        public void NewLineInStringTest()
        {
            var lexer = Lexer.CreateState("\"\n\"");

            bool result = TextScanner.ScanString(lexer, ref this.token);

            Assert.False(result);
        }

        [Fact]
        public void EscapedNewLineInStringTest()
        {
            var lexer = Lexer.CreateState("\"\\n\"");

            bool result = TextScanner.ScanString(lexer, ref this.token);

            Assert.True(result);
            Assert.Equal("\\n", this.token.Value);
        }

        [Fact]
        public void TabsInStringTest()
        {
            var lexer = Lexer.CreateState("\"		\"");

            bool result = TextScanner.ScanString(lexer, ref this.token);

            Assert.True(result);
            Assert.Equal("\\t\\t", this.token.Value);
        }
    }
}
