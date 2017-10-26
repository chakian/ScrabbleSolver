using System;
using System.Diagnostics.Contracts;

namespace Model
{
    public sealed class TileInPlay
    {
        public TileInPlay(Tile tile, BoardLocation location)
        {
            Contract.Requires<ArgumentNullException>(tile != null);
            Contract.Requires<ArgumentNullException>(location != null);
            Contract.Requires<ArgumentOutOfRangeException>(location.IsWithinBounds);

            Tile = tile;
            Location = location;
        }

        public Tile Tile
        {
            get;
            private set;
        }

        public BoardLocation Location
        {
            get;
            private set;
        }

        public static implicit operator Tile(TileInPlay play)
        {
            return play.Tile;
        }
    }
}
