using System;

namespace Model
{
    public sealed class BoardChangedEventArgs : EventArgs
    {
        public BoardChangedEventArgs(int column, int row) : this(new BoardLocation(column, row))
        {
        }

        public BoardChangedEventArgs(BoardLocation location)
        {
            Location = location;
        }

        public BoardLocation Location
        {
            get;
            private set;
        }
    }
}