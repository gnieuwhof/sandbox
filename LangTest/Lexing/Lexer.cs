namespace Lexing
{
    using Lexing.Scanners;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Lexer
    {
        private readonly string origin;
        private readonly List<Token> tokens;

        private bool scanError = false;

        private delegate bool Scanner(Lexer lexer);


        public Source Src { get; set; }

        public Token LastToken =>
            this.tokens.LastOrDefault();


        public Lexer(string origin, string text)
        {
            this.origin = origin ??
                throw new ArgumentNullException(nameof(origin));

            this.Src = new Source(text);
            this.tokens = new List<Token>();
        }


        public IEnumerable<Token> GetTokens(out LexicalError error)
        {
            error = null;

            while ((error == null) && !this.Src.ReachedEnd())
            {
                char current = this.Src.Current;
                Position pos = this.Src.GetPosition();

                if ((current == '_') || char.IsLetterOrDigit(current))
                {
                    KeywordIdentifierScanner.Scan(this);
                    continue;
                }

                switch (current)
                {
                    case '!':
                        OperatorScanner.ScanExclamation(this);
                        break;

                    case '&':
                        OperatorScanner.ScanAmpersand(this);
                        break;

                    case '(':
                    case ')':
                    case '*':
                    case '+':
                    case ',':
                    case '-':
                    case ';':
                    case '?':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                        this.Add(new Token(pos, current));
                        break;

                    case '|':
                        OperatorScanner.ScanPipeline(this);
                        break;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        this.Scan(NumberScanner.Scan, ref error);
                        break;

                    case '.':
                        char? next = this.Src.Peek();
                        if (char.IsDigit(next ?? 'X'))
                        {
                            this.Scan(NumberScanner.Scan, ref error);
                        }
                        else
                        {
                            this.Add(new Token(pos, current));
                        }
                        break;

                    case '/':
                        this.ScanSlash(ref error);
                        break;

                    case '=':
                        OperatorScanner.ScanEquals(this);
                        break;

                    case '<':
                        OperatorScanner.ScanLessThan(this);
                        break;

                    case '>':
                        OperatorScanner.ScanGreaterThan(this);
                        break;

                    case '"':
                        this.Scan(TextScanner.ScanString, ref error);
                        break;

                    case '\'':
                        this.Scan(TextScanner.ScanCharacter, ref error);
                        break;

                    case ' ':
                    case '\t':
                        WhiteSpaceScanner.Scan(this);
                        break;

                    case '\r':
                    case '\n':
                        TextScanner.ScanNewLine(this);
                        break;

                    default:
                        error = this.GetError();
                        break;
                }

                this.Src.Advance();
            }

            return this.tokens;
        }

        private void Scan(Scanner scanner, ref LexicalError error)
        {
            bool success = scanner(this);

            if (!success)
            {
                this.scanError = true;
                error = this.GetError();
            }
        }

        public void Add(Token token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            if (token.Type == TokenType.NewLine)
            {
                ++this.Src.Line;
                this.Src.Character = 0;
            }
            else if (token.Type == TokenType.MultiComment)
            {
                string comment = (token.Value.Length >= 4)
                    ? token.Value.Substring(2, token.Value.Length - "/**/".Length)
                    : string.Empty;

                GetPositionOffset(comment, out int lineCountOffset, out int position);
                if (lineCountOffset > 0)
                {
                    this.Src.Line += lineCountOffset;
                    this.Src.Character = position;
                }
            }

            this.tokens.Add(token);
        }

        private static void GetPositionOffset(string text,
            out int lineCountOffset, out int position)
        {
            lineCountOffset = 0;
            position = 1;

            var lexer = new Lexer(string.Empty, text);

            while (!lexer.Src.ReachedEnd())
            {
                char current = lexer.Src.Current;

                switch (current)
                {
                    case '\r':
                    case '\n':
                        {
                            TextScanner.ScanNewLine(lexer);
                            ++lineCountOffset;
                            position = 1;
                            break;
                        }
                }

                ++position;

                lexer.Src.Advance();
            }
        }

        private LexicalError GetError()
        {
            int character = this.Src.Character;

            if (this.Src.ReachedEnd())
            {
                --character;
            }

            string error = this.Src.ReachedEnd()
                ? "unexpected end"
                : $"unexpected character '{this.Src.Current}'";

            if (this.scanError && tokens.Any())
            {
                error += $" ({tokens.Last().Type})";
            }

            IEnumerable<string> lines = this.Src.GetLastLines(3);

            var lexicalError = new LexicalError(error, lines,
                new Location(this.origin, this.Src.Line, character));

            return lexicalError;
        }

        private void ScanSlash(ref LexicalError error)
        {
            char? next = this.Src.Peek();
            if (next == '/')
            {
                CommentScanner.ScanSingleLine(this);
            }
            else if (next == '*')
            {
                this.Scan(CommentScanner.ScanMultiLine, ref error);
            }
            else
            {
                Position pos = this.Src.GetPosition();

                Token token = new Token(pos, TokenType.Slash);

                this.Add(token);
            }
        }

        public static Lexer CreateState(string text, int position = 0)
        {
            var lexer = new Lexer(string.Empty, string.Empty)
            {
                Src = new Source(text)
            };
            lexer.Src.Advance(position);

            return lexer;
        }
    }
}
