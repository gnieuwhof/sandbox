namespace LexingTests
{
    using Lexing;
    using Xunit;

    public class KeywordsTests
    {
        [Fact]
        public void GotoKeywordTest()
        {
            TokenType tokenType;
            bool result = Keywords.TryGetTokenType("goto", out tokenType);

            Assert.True(result);
            Assert.Equal(TokenType.GoToKeyword, tokenType);
        }

        [Fact]
        public void InvalidKeywordTest()
        {
            bool result = Keywords.TryGetTokenType("XX", out TokenType _);

            Assert.False(result);
        }
    }
}
