namespace Lexing
{
    public struct Position
    {
        public readonly int Line;
        public readonly int Character;


        public Position(int line, int character)
        {
            this.Line = line;
            this.Character = character;
        }
    }
}
