namespace Lexing
{
    using Lexing.Scanners;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Lexer
    {
        private readonly string origin;
        private Token[] tokens;
        private int index;

        private bool scanError = false;

        private delegate bool Scanner(Lexer lexer, ref Token token);

        public StringBuilder Builder { get; } = new StringBuilder();


        public Source Src { get; set; }


        public Lexer(string origin, string text)
        {
            this.origin = origin ??
                throw new ArgumentNullException(nameof(origin));

            this.Src = new Source(text);
        }


        public IEnumerable<Token> GetTokens(out LexicalError error)
        {
            error = null;

            int size = Math.Max(256, this.Src.Text.Length / 4);
            this.tokens = new Token[size];

            this.index = 0;
            while (error == null)
            {
                if (this.tokens.Length == this.index)
                {
                    // Double the array
                    var temp = new Token[this.tokens.Length * 2];
                    Array.Copy(this.tokens, temp, this.index);
                    this.tokens = temp;
                }

                ref Token token = ref this.tokens[this.index];

                this.Lex(ref token, ref error);

                this.Update(ref token);

                this.Src.Advance();

                if (token.Type == TokenType.ContentEnd)
                {
                    break;
                }

                ++this.index;
            }

            return this.tokens;
        }

        private void Lex(ref Token token, ref LexicalError error)
        {
            token.Line = this.Src.Line;
            token.Character = this.Src.Character;

            if (this.Src.ReachedEnd())
            {
                token.Type = TokenType.ContentEnd;
                return;
            }

            char current = this.Src.Current;

            if ((current == '_') || char.IsLetter(current))
            {
                KeywordIdentifierScanner.Scan(this, ref token);
                return;
            }

            switch (current)
            {
                case '!':
                    OperatorScanner.ScanExclamation(this, ref token);
                    break;

                case '&':
                    OperatorScanner.ScanAmpersand(this, ref token);
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
                case '^':
                case ':':
                    token.Type = GetTokenType(current);
                    break;

                case '|':
                    OperatorScanner.ScanPipeline(this, ref token);
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
                    this.Scan(NumberScanner.Scan, ref token, ref error);
                    break;

                case '.':
                    char next = this.Src.Peek();
                    if (char.IsDigit(next))
                    {
                        this.Scan(NumberScanner.Scan, ref token, ref error);
                    }
                    else
                    {
                        token.Type = GetTokenType(current);
                    }
                    break;

                case '/':
                    this.ScanSlash(ref token, ref error);
                    break;

                case '=':
                    OperatorScanner.ScanEquals(this, ref token);
                    break;

                case '<':
                    OperatorScanner.ScanLessThan(this, ref token);
                    break;

                case '>':
                    OperatorScanner.ScanGreaterThan(this, ref token);
                    break;

                case '"':
                    this.Scan(TextScanner.ScanString, ref token, ref error);
                    break;

                case '\'':
                    this.Scan(TextScanner.ScanCharacter, ref token, ref error);
                    break;

                case ' ':
                case '\t':
                    WhiteSpaceScanner.Scan(this, ref token);
                    break;

                case '\r':
                case '\n':
                    TextScanner.ScanNewLine(this, ref token);
                    break;

                default:
                    error = this.GetError();
                    break;
            }
        }

        private static TokenType GetTokenType(char c)
        {
            UInt16 intVal = Convert.ToUInt16(c);

            if (Enum.TryParse<TokenType>($"{intVal}", out TokenType tokenType))
            {
                return tokenType;
            }

            return TokenType.Invalid;
        }

        private void Scan(Scanner scanner,
            ref Token token, ref LexicalError error)
        {
            bool success = scanner(this, ref token);

            if (!success)
            {
                this.scanError = true;
                error = this.GetError();
            }
        }

        public void Update(ref Token token)
        {
            if (token.Type == TokenType.NewLine)
            {
                ++this.Src.Line;
                this.Src.Character = 0;
            }
            else if (token.Type == TokenType.MultiComment)
            {
                int minLength = "/**/".Length;
                string comment = (token.Value.Length > minLength)
                    ? token.Value.Substring(2, token.Value.Length - minLength)
                    : string.Empty;

                GetPositionOffset(comment, out int lineCountOffset, out int position);
                if (lineCountOffset > 0)
                {
                    this.Src.Line += lineCountOffset;
                    this.Src.Character = position;
                }
            }
        }

        private static void GetPositionOffset(string text,
            out int lineCountOffset, out int position)
        {
            lineCountOffset = 0;
            position = 1;

            var lexer = new Lexer(string.Empty, text);
            Token token = default;

            while (!lexer.Src.ReachedEnd())
            {
                char current = lexer.Src.Current;

                switch (current)
                {
                    case '\r':
                    case '\n':
                        {
                            TextScanner.ScanNewLine(lexer, ref token);
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

            if (this.scanError && (this.index < this.tokens.Length))
            {
                error += $" ({this.tokens[index].Type})";
            }

            IEnumerable<string> lines = this.Src.GetLastLines(3);

            var lexicalError = new LexicalError(error, lines,
                new Location(this.origin, this.Src.Line, character));

            return lexicalError;
        }

        private void ScanSlash(ref Token token, ref LexicalError error)
        {
            char next = this.Src.Peek();
            if (next == '/')
            {
                CommentScanner.ScanSingleLine(this, ref token);
            }
            else if (next == '*')
            {
                this.Scan(CommentScanner.ScanMultiLine, ref token, ref error);
            }
            else
            {
                token.Type = TokenType.Slash;
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
