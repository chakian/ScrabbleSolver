using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Model
{
    public sealed class BoardLocation
    {
        public BoardLocation(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public int Column { get; set; }
        public int Row { get; set; }

        [Pure]
        public bool IsWithinBounds
        {
            get { return IsLocationWithinBounds(Column, Row); }
        }

        [Pure]
        public IEnumerable<BoardLocation> PrecedingLocations
        {
            get
            {
                for (var i = Column; --i >= 0;)
                    yield return new BoardLocation(i, Row);
            }
        }

        [Pure]
        public IEnumerable<BoardLocation> FollowingLocationsAcross
        {
            get
            {
                for (var i = Column; ++i < Board.Columns;)
                    yield return new BoardLocation(i, Row);
            }
        }

        [Pure]
        public IEnumerable<BoardLocation> PrecedingLocationsUp
        {
            get
            {
                for (var i = Row; --i >= 0;)
                    yield return new BoardLocation(Column, i);
            }
        }

        [Pure]
        public IEnumerable<BoardLocation> FollowingLocationsDown
        {
            get
            {
                for (var i = Row; ++i < Board.Rows;)
                    yield return new BoardLocation(Column, i);
            }
        }

        [Pure]
        public IEnumerable<BoardLocation> NeighbouringLocationsAcross
        {
            get
            {
                return new[]
                {
                    new BoardLocation(Column - 1, Row),
                    new BoardLocation(Column + 1, Row)
                }.Where(location => location.IsWithinBounds);
            }
        }

        [Pure]
        public IEnumerable<BoardLocation> NeighbouringLocationsDown
        {
            get
            {
                return new[]
                {
                    new BoardLocation(Column, Row - 1),
                    new BoardLocation(Column, Row + 1)
                }.Where(location => location.IsWithinBounds);
            }
        }

        [Pure]
        public IEnumerable<BoardLocation> NeighbouringLocations
        {
            get
            {
                return NeighbouringLocationsAcross.Concat(NeighbouringLocationsDown);
            }
        }

        [Pure]
        public bool Equals(BoardLocation other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoardLocation);
        }

        public override int GetHashCode()
        {
            unchecked { return (Column * 397) ^ Row; }
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", Column, Row);
        }

        public static BoardLocation Zero
        {
            get { return new BoardLocation(0, 0); }
        }

        [Pure]
        public static bool IsLocationWithinBounds(int column, int row)
        {
            return column >= 0 && column < Board.Columns && row >= 0 && row < Board.Rows;
        }

        public static bool operator ==(BoardLocation a, BoardLocation b)
        {
            return ReferenceEquals(a, b) || ((object)a) != null && ((object)b) != null && (a.Column == b.Column && a.Row == b.Row);
        }

        public static bool operator !=(BoardLocation a, BoardLocation b)
        {
            return !(a == b);
        }
    }
}
