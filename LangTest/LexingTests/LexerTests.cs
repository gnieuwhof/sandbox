﻿namespace LexingTests
{
    using Lexing;
    using System;
    using Xunit;

    public class LexerTests
    {
        [Fact]
        public void OriginNullTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Lexer(null, string.Empty));
        }

        [Fact]
        public void TextNullTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Lexer(string.Empty, null));
        }

        [Fact]
        public void AddNullTokenTest()
        {
            var lexer = new Lexer(string.Empty, string.Empty);

            Assert.Throws<ArgumentNullException>(() =>
                lexer.Add(null));
        }
    }
}
