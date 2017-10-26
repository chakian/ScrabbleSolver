using System.Diagnostics.Contracts;

namespace Model
{
    public sealed class Tile
    {
        public const char BlankChar = '?';

        private char _letter;

        private Tile()
        {
            Letter = BlankChar;
            Points = 0;
        }

        public Tile(char letter, int points)
        {
            Contract.Requires(char.IsLetter(letter));
            Contract.Requires(points > 0);

            Letter = letter;
            Points = points;
        }

        public char Letter
        {
            get { return _letter; }
            private set { _letter = char.ToUpper(value); }
        }

        public int Points
        {
            get; private set;
        }

        public bool InitiallyBlank
        {
            get { return Points == 0; }
        }

        public void UseBlankAs(char letter)
        {
            Contract.Requires(InitiallyBlank && Letter == BlankChar);
            Letter = letter;
        }

        public void RestoreBlank()
        {
            if (InitiallyBlank) _letter = BlankChar;
        }

        public static Tile Blank
        {
            get { return new Tile(); }
        }
    }
}
