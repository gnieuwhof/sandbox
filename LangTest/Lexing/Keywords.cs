namespace Lexing
{
    using System.Collections.Generic;

    public static class Keywords
    {
        private static readonly Dictionary<string, TokenType> dict =
            new Dictionary<string, TokenType>
            {
                { "break", TokenType.BreakKeyword },
                { "class", TokenType.ClassKeyword },
                { "continue", TokenType.ContinueKeyword },
                { "else", TokenType.ElseKeyword },
                { "enum", TokenType.EnumKeyword },
                { "false", TokenType.FalseKeyword },
                { "goto", TokenType.GoToKeyword },
                { "if", TokenType.IfKeyword },
                { "namespace", TokenType.NamespaceKeyword },
                { "private", TokenType.PrivateKeyword },
                { "protected", TokenType.ProtectedKeyword },
                { "public", TokenType.PublicKeyword },
                { "readonly", TokenType.ReadOnlyKeyword },
                { "static", TokenType.StaticKeyword },
                { "true", TokenType.TrueKeyword },
                { "using", TokenType.UsingKeyword },
                { "while", TokenType.WhileKeyword },
            };

        public static bool TryGetTokenType(
            string identifierOrKeyword, out TokenType tokenType)
        {
            return dict.TryGetValue(identifierOrKeyword, out tokenType);
        }
    }
}
