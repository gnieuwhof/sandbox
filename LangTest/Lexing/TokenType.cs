namespace Lexing
{
    using System;

    public enum TokenType : UInt16
    {

        Exclamation = 33,

        Ampersand = 38,

        OpenParen = 40,
        CloseParen = 41,
        Asterisk = 42,
        Plus = 43,
        Comma = 44,
        Minus = 45,
        Period = 46,
        Slash = 47,

        Colon = 58,
        Semicolon = 59,

        LessThan = 60,
        Equals = 61,
        GreaterThan = 62,
        Question = 63,

        OpenBracket = 91,

        CloseBracket = 93,
        Caret = 94,

        OpenBrace  = 123,
        Pipeline = 124,
        CloseBrace = 125,

        // The first 255 are reserved for ASCII.
        ASCII_END = 256,

        Char,
        Float,
        Int,
        String,

        MultiComment,
        SingleComment,
        NewLine,
        WhiteSpace,

        EqualsEquals,
        LessThanOrEquals,
        GreaterThanOrEquals,
        NotEquals,
        AmpAmp,
        PipePipe,

        Identifier,

        NamespaceKeyword,
        PublicKeyword,
        PrivateKeyword,
        ProtectedKeyword,
        StaticKeyword,
        ReadOnlyKeyword,
        ClassKeyword,
        EnumKeyword,
        GoToKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        TrueKeyword,
        FalseKeyword,
        BreakKeyword,
        ContinueKeyword,
        UsingKeyword,

        ContentEnd,
        Invalid = UInt16.MaxValue
    }
}
