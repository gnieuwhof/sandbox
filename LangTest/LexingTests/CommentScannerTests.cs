namespace LexingTests
{
    using Lexing;
    using Lexing.Scanners;
    using System;
    using Xunit;

    public class CommentScannerTests
    {
        private Token token = default;

        [Fact]
        public void ScanSingleLineNullTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
                CommentScanner.ScanSingleLine(null, ref this.token));
        }

        [Fact]
        public void ScanSingleLineFalseTest()
        {
            var lexer = Lexer.CreateState("/X");

            Assert.Throws<InvalidOperationException>(() =>
                CommentScanner.ScanSingleLine(lexer, ref this.token));
        }

        [Fact]
        public void ScanSingleLineTrueTest()
        {
            var lexer = Lexer.CreateState("//X");

            bool result = CommentScanner.ScanSingleLine(lexer, ref this.token);

            Assert.True(result);
        }

        [Fact]
        public void ScanSingleLineNewLineTest()
        {
            string comment = "// abc 123";

            var lexer = Lexer.CreateState(comment + Environment.NewLine);

            bool result = CommentScanner.ScanSingleLine(lexer, ref this.token);

            Assert.True(result);

            Assert.Equal(comment, this.token.Value);
        }

        [Fact]
        public void ScanMultiLineTrueTest()
        {
            var lexer = Lexer.CreateState("/**/");

            bool result = CommentScanner.ScanMultiLine(lexer, ref this.token);

            Assert.True(result);
        }

        [Fact]
        public void ScanMultiLineFalseTest()
        {
            foreach (string text in new[] { "/*/", "/**", "/*" })
            {
                var lexer = Lexer.CreateState(text);

                bool result = CommentScanner.ScanMultiLine(lexer, ref this.token);

                Assert.False(result);
            }
        }

        [Fact]
        public void MultiLineSuccessPositionTest()
        {
            var lexer = Lexer.CreateState("/**/");

            _ = CommentScanner.ScanMultiLine(lexer, ref this.token);

            Assert.Equal(3, lexer.Src.Index);
        }

        [Fact]
        public void MultiLineErrorPositionTest()
        {
            var lexer = Lexer.CreateState("/*/");

            _ = CommentScanner.ScanMultiLine(lexer, ref this.token);

            Assert.Equal(3, lexer.Src.Index);
        }
    }
}
