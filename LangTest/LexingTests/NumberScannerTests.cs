namespace LexingTests
{
    using Lexing;
    using Lexing.Scanners;
    using System;
    using Xunit;

    public class NumberScannerTests
    {
        [Fact]
        public void ScanNullTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NumberScanner.Scan(null));
        }

        [Fact]
        public void IntegerTest()
        {
            var lexer = Lexer.CreateState("123");

            bool result = NumberScanner.Scan(lexer);

            Assert.True(result);
        }

        [Fact]
        public void IntegerTypeTest()
        {
            var lexer = Lexer.CreateState("123");

            _ = NumberScanner.Scan(lexer);

            Assert.Equal(TokenType.Int, lexer.LastToken.Type);
        }

        [Fact]
        public void FloatTest()
        {
            var lexer = Lexer.CreateState("1.23");

            bool result = NumberScanner.Scan(lexer);

            Assert.True(result);
        }

        [Fact]
        public void FloatTypeTest()
        {
            var lexer = Lexer.CreateState("1.23");

            _ = NumberScanner.Scan(lexer);

            Assert.Equal(TokenType.Float, lexer.LastToken.Type);
        }

        [Fact]
        public void StartsWithPeriodTest()
        {
            var lexer = Lexer.CreateState(".1");

            bool result = NumberScanner.Scan(lexer);

            Assert.True(result);
        }

        [Fact]
        public void EndsWithPeriodTest()
        {
            var lexer = Lexer.CreateState("1.");

            bool result = NumberScanner.Scan(lexer);

            Assert.False(result);
        }

        [Fact]
        public void DoublePeriodTest()
        {
            var lexer = Lexer.CreateState("1.2.3");

            bool result = NumberScanner.Scan(lexer);

            Assert.False(result);
        }

        [Fact]
        public void SuccessPositionTest()
        {
            var lexer = Lexer.CreateState("123");

            _ = NumberScanner.Scan(lexer);

            Assert.Equal(2, lexer.Src.Index);
        }

        [Fact]
        public void MultiplePeriodsPositionTest()
        {
            var lexer = Lexer.CreateState("1.2.3");

            _ = NumberScanner.Scan(lexer);

            Assert.Equal(3, lexer.Src.Index);
        }

        [Fact]
        public void EndsWithPeriodPositionTest()
        {
            var lexer = Lexer.CreateState("1.");

            _ = NumberScanner.Scan(lexer);

            Assert.Equal(1, lexer.Src.Index);
        }

        [Fact]
        public void EndsWithPeriodSpacePositionTest()
        {
            var lexer = Lexer.CreateState("1. ");

            _ = NumberScanner.Scan(lexer);

            Assert.Equal(1, lexer.Src.Index);
        }
    }
}
