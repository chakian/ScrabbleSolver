using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Model
{
    public sealed class Board
    {
        private class TileCollection : ICollection<TileInPlay>
        {
            private readonly Board _source;

            private IEnumerable<BoardLocation> LocationsInRange
            {
                get;
                set;
            }

            private IEnumerable<BoardLocation> OccupiedBoardLocations
            {
                get { return LocationsInRange.Where(location => _source.TileExistsAt(location)); }
            }

            private IEnumerable<TileInPlay> TilesInPlay
            {
                get
                {
                    return OccupiedBoardLocations.Select(location => new TileInPlay(_source[location], location));
                }
            }

            public TileCollection(Board source) : this(source, BoardLocation.Zero, Columns, Rows)
            {
            }

            private TileCollection(Board source, BoardLocation rangeStart, int colCount, int rowCount)
            {
                _source = source;

                LocationsInRange = Enumerable
                    .Range(rangeStart.Row, rowCount)
                    .SelectMany(row => Enumerable.Range(rangeStart.Column, colCount).Select(column => new BoardLocation(column, row)));
            }

            public void Add(TileInPlay item)
            {
                if (item == null || Contains(item)) return;
                _source[item.Location] = item.Tile;
            }

            public void Clear()
            {
                foreach (var location in OccupiedBoardLocations)
                {
                    _source[location] = null;
                }
            }

            public bool Contains(TileInPlay item)
            {
                return item != null && TilesInPlay.Any(play => item.Location == play.Location && item.Tile == play.Tile);
            }

            public void CopyTo(TileInPlay[] array, int arrayIndex)
            {
                foreach (var tile in TilesInPlay)
                    array.SetValue(tile, arrayIndex++);
            }

            public int Count
            {
                get { return OccupiedBoardLocations.Count(); }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(TileInPlay item)
            {
                if (!Contains(item)) return false;

                _source[item.Location] = null;
                return true;
            }

            public IEnumerator<TileInPlay> GetEnumerator()
            {
                return TilesInPlay.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return TilesInPlay.GetEnumerator();
            }
        }

        public const int Columns = 15;
        public const int Rows = 15;

        private readonly Tile[,] _square;

        public EventHandler<BoardChangedEventArgs> BoardChanged;

        public Board()
        {
            _square = new Tile[Columns, Rows];
        }

        public ICollection<TileInPlay> Tiles
        {
            get { return new TileCollection(this); }
        }

        public bool IsEmpty
        {
            get { return Locations.All(location => this[location] == null); }
        }

        public bool TileExistsAt(BoardLocation location)
        {
            return TileAt(location) != null;
        }

        public Tile TileAt(BoardLocation location)
        {
            return this[location];
        }

        public Tile this[BoardLocation location]
        {
            get { return this[location.Column, location.Row]; }
            set { this[location.Column, location.Row] = value; }
        }

        public Tile this[int column, int row]
        {
            get
            {
                Contract.Requires<ArgumentOutOfRangeException>(BoardLocation.IsLocationWithinBounds(column, row));
                return _square[column, row];
            }

            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(BoardLocation.IsLocationWithinBounds(column, row));

                if (value == _square[column, row]) return;

                _square[column, row] = value;
                NotifyOfBoardChanged(column, row);
            }
        }

        public static BoardLocation CenterLocation
        {
            get { return new BoardLocation(Columns / 2, Rows / 2); }
        }

        public static IEnumerable<Tuple<BoardLocation, ScoringStyle>> AllSquares
        {
            get
            {
                return
                    from location in Locations
                    join square in PremiumSquares on location equals square.Item1 into outer
                    from square in outer.DefaultIfEmpty()
                    select Tuple.Create(location, (square == null) ? ScoringStyle.LetterOnly : square.Item2);
            }
        }

        public static IEnumerable<BoardLocation> Locations
        {
            get
            {
                return Enumerable.Range(0, Rows).SelectMany(row => Enumerable.Range(0, Columns).Select(column => new BoardLocation(column, row)));
            }
        }

        public static readonly Tuple<BoardLocation, ScoringStyle>[] PremiumSquares = new[]
        {
            // Triple Word
            Tuple.Create(new BoardLocation( 0,  2), ScoringStyle.TripleWord),
            Tuple.Create(new BoardLocation( 2,  0), ScoringStyle.TripleWord),

            Tuple.Create(new BoardLocation( 0, 12), ScoringStyle.TripleWord),
            Tuple.Create(new BoardLocation(14,  2), ScoringStyle.TripleWord),

            Tuple.Create(new BoardLocation(12,  0), ScoringStyle.TripleWord),
            Tuple.Create(new BoardLocation( 2, 14), ScoringStyle.TripleWord),
            
            Tuple.Create(new BoardLocation(12, 14), ScoringStyle.TripleWord),
            Tuple.Create(new BoardLocation(14, 12), ScoringStyle.TripleWord),

            // Triple Letter
            Tuple.Create(new BoardLocation( 1,  1), ScoringStyle.TripleLetter),
            Tuple.Create(new BoardLocation(13,  1), ScoringStyle.TripleLetter),

            Tuple.Create(new BoardLocation( 4,  4), ScoringStyle.TripleLetter),
            Tuple.Create(new BoardLocation(10,  4), ScoringStyle.TripleLetter),

            Tuple.Create(new BoardLocation( 4, 10), ScoringStyle.TripleLetter),
            Tuple.Create(new BoardLocation(10, 10), ScoringStyle.TripleLetter),

            Tuple.Create(new BoardLocation( 1, 13), ScoringStyle.TripleLetter),
            Tuple.Create(new BoardLocation(13, 13), ScoringStyle.TripleLetter),

            // Double Word
            Tuple.Create(new BoardLocation( 7,  7), ScoringStyle.DoubleWord),

            Tuple.Create(new BoardLocation( 7,  2), ScoringStyle.DoubleWord),
            Tuple.Create(new BoardLocation( 3,  3), ScoringStyle.DoubleWord),
            Tuple.Create(new BoardLocation(11,  3), ScoringStyle.DoubleWord),

            Tuple.Create(new BoardLocation( 2,  7), ScoringStyle.DoubleWord),
            Tuple.Create(new BoardLocation(12,  7), ScoringStyle.DoubleWord),

            Tuple.Create(new BoardLocation( 3, 11), ScoringStyle.DoubleWord),
            Tuple.Create(new BoardLocation(11, 11), ScoringStyle.DoubleWord),
            Tuple.Create(new BoardLocation( 7, 12), ScoringStyle.DoubleWord),
            
            // Double Letter
            Tuple.Create(new BoardLocation( 5,  0), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 9,  0), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 6,  1), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 8,  1), ScoringStyle.DoubleLetter),

            Tuple.Create(new BoardLocation( 5, 14), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 9, 14), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 6, 13), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 8, 13), ScoringStyle.DoubleLetter),

            Tuple.Create(new BoardLocation( 0,  5), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 0,  9), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 1,  6), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 1,  8), ScoringStyle.DoubleLetter),

            Tuple.Create(new BoardLocation(14,  5), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation(14,  9), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation(13,  6), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation(13,  8), ScoringStyle.DoubleLetter),

            Tuple.Create(new BoardLocation( 5,  5), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 9,  5), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 6,  6), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 8,  6), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 6,  8), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 8,  8), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 5,  9), ScoringStyle.DoubleLetter),
            Tuple.Create(new BoardLocation( 9,  9), ScoringStyle.DoubleLetter),
        };

        private void NotifyOfBoardChanged(int column, int row)
        {
            BoardChanged?.Invoke(this, new BoardChangedEventArgs(column, row));
        }
    }
}
