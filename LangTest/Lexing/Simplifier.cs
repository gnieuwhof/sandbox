namespace Lexing
{
    using System.Collections.Generic;

    public static class Simplifier
    {
        public static IEnumerable<Token> Filter(IEnumerable<Token> tokens)
        {
            bool inSeparator = false;
            TokenType previousType = TokenType.BOGUS;

            int line = 0;
            int character = 0;
            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.WhiteSpace:
                    case TokenType.SingleComment:
                    case TokenType.MultiComment:
                    case TokenType.NewLine:
                        if (!inSeparator)
                        {
                            inSeparator = true;
                            line = token.Line;
                            character = token.Character;
                        }
                        continue;
                }

                if (inSeparator)
                {
                    inSeparator = false;

                    bool yieldResult = true;

                    switch (previousType)
                    {
                        case TokenType.OpenBrace:
                        case TokenType.OpenBracket:
                        case TokenType.OpenParen:

                        case TokenType.Comma:
                        case TokenType.Semicolon:
                        case TokenType.Equals:
                        case TokenType.Plus:
                        case TokenType.Minus:
                        case TokenType.Asterisk:
                        case TokenType.Slash:

                        case TokenType.CloseBrace:
                        case TokenType.CloseBracket:
                        case TokenType.CloseParen:
                            yieldResult = false;
                            break;
                    }

                    if (yieldResult)
                    {
                        Position pos = new Position(line, character);

                        yield return new Token(pos, TokenType.Separator);
                    }
                }

                previousType = token.Type;

                yield return token;
            }
        }
    }
}
