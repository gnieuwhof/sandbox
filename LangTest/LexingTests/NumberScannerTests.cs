namespace LexingTests
{
    using Lexing;
    using Lexing.Scanners;
    using System;
    using Xunit;

    public class NumberScannerTests
    {
        private Token token = default;

        [Fact]
        public void ScanNullTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NumberScanner.Scan(null, ref this.token));
        }

        [Fact]
        public void IntegerTest()
        {
            var lexer = Lexer.CreateState("123");

            bool result = NumberScanner.Scan(lexer, ref this.token);

            Assert.True(result);
        }

        [Fact]
        public void IntegerTypeTest()
        {
            var lexer = Lexer.CreateState("123");

            _ = NumberScanner.Scan(lexer, ref this.token);

            Assert.Equal(TokenType.Int, this.token.Type);
        }

        [Fact]
        public void FloatTest()
        {
            var lexer = Lexer.CreateState("1.23");

            bool result = NumberScanner.Scan(lexer, ref this.token);

            Assert.True(result);
        }

        [Fact]
        public void FloatTypeTest()
        {
            var lexer = Lexer.CreateState("1.23");

            _ = NumberScanner.Scan(lexer, ref this.token);

            Assert.Equal(TokenType.Float, this.token.Type);
        }

        [Fact]
        public void StartsWithPeriodTest()
        {
            var lexer = Lexer.CreateState(".1");

            bool result = NumberScanner.Scan(lexer, ref this.token);

            Assert.True(result);
        }

        [Fact]
        public void EndsWithPeriodTest()
        {
            var lexer = Lexer.CreateState("1.");

            bool result = NumberScanner.Scan(lexer, ref this.token);

            Assert.False(result);
        }

        [Fact]
        public void DoublePeriodTest()
        {
            var lexer = Lexer.CreateState("1.2.3");

            bool result = NumberScanner.Scan(lexer, ref this.token);

            Assert.False(result);
        }

        [Fact]
        public void SuccessPositionTest()
        {
            var lexer = Lexer.CreateState("123");

            _ = NumberScanner.Scan(lexer, ref this.token);

            Assert.Equal(2, lexer.Src.Index);
        }

        [Fact]
        public void MultiplePeriodsPositionTest()
        {
            var lexer = Lexer.CreateState("1.2.3");

            _ = NumberScanner.Scan(lexer, ref this.token);

            Assert.Equal(3, lexer.Src.Index);
        }

        [Fact]
        public void EndsWithPeriodPositionTest()
        {
            var lexer = Lexer.CreateState("1.");

            _ = NumberScanner.Scan(lexer, ref this.token);

            Assert.Equal(1, lexer.Src.Index);
        }

        [Fact]
        public void EndsWithPeriodSpacePositionTest()
        {
            var lexer = Lexer.CreateState("1. ");

            _ = NumberScanner.Scan(lexer, ref this.token);

            Assert.Equal(1, lexer.Src.Index);
        }
    }
}
