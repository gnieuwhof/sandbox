namespace LexingTests
{
    using Lexing;
    using Lexing.Scanners;
    using Xunit;

    public class TextScannerTests
    {
        private Token token = default;

        [Fact]
        public void NoCharacterInLiteralTest()
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
        public void TabInLiteralTest()
        {
            var lexer = Lexer.CreateState("'\t'");

            bool result = TextScanner.ScanCharacter(lexer, ref this.token);

            Assert.False(result);
        }

        [Fact]
        public void UnescapedLiteralTest()
        {
            var unescaped = new[] { "'\n'", "'\r'", "'\t'", "'\\'" };

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
            var lexer = Lexer.CreateState("'\\\\'");

            bool result = TextScanner.ScanCharacter(lexer, ref this.token);

            Assert.True(result);
        }

        [Fact]
        public void FourBackslashesLiteralTest()
        {
            var lexer = Lexer.CreateState("'\\\\\\\\'");

            bool result = TextScanner.ScanCharacter(lexer, ref this.token);

            Assert.False(result);
        }
    }
}
