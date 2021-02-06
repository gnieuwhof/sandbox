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
            var valid = new[] { "_", "a", "X", "abc", "_a" };

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

        [Fact]
        public void InvalidCharInIdentifierTest()
        {
            var lexer = Lexer.CreateState("abc$def");

            KeywordIdentifierScanner.Scan(lexer, ref this.token);

            Assert.Equal(2, lexer.Src.Index);
        }

        [Fact]
        public void ValidEndTest()
        {
            var valid = new[]
            {
                "Foo()",
                "foo)",
                "foo[0]",
                "foo]",
                "foo+bar",
                "foo-bar",
                "foo*bar",
                "foo/bar",
                "foo^bar",
                "foo%bar",
                "foo, int bar",
                "foo;",
                "foo=bar",
                "foo&&bar",
                "foo||bar",
                "foo!=bar",
                "foo<bar",
                "foo>bar",
                "foo?bar:qux",
                "bar:qux",
                "foo = bar",
                "foo\t= bar",
                "foo\r",
                "foo\n",
            };

            foreach (string val in valid)
            {
                var lexer = Lexer.CreateState(val);

                KeywordIdentifierScanner.Scan(lexer, ref this.token);

                Assert.Equal(2, lexer.Src.Index);
            }
        }
    }
}
