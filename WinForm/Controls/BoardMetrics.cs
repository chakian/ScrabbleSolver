using Model;
using System.Windows;

namespace Controls
{
    internal class BoardMetrics
    {
        private Size _cellSize = new Size(0, 0);

        public const double LineThickness = 0.6;

        public Rect RenderBounds { get; private set; }

        public Size CellSize
        {
            get { return _cellSize; }
            set
            {
                _cellSize = value;
                RenderBounds = new Rect(0, 0, _cellSize.Width * Board.Columns + LineThickness, _cellSize.Height * Board.Rows + LineThickness);
            }
        }

        public double ColumnCoordinate(int column)
        {
            return RenderBounds.X + CellSize.Width * column;
        }

        public double RowCoordinate(int row)
        {
            return RenderBounds.Y + CellSize.Height * row;
        }

        public Point CellOriginFromLocation(int column, int row)
        {
            return new Point(ColumnCoordinate(column), RowCoordinate(row));
        }

        public Point CellOriginFromLocation(BoardLocation location)
        {
            return CellOriginFromLocation(location.Column, location.Row);
        }

        public Rect CellBoundsFromLocation(BoardLocation location)
        {
            return new Rect(CellOriginFromLocation(location), CellSize);
        }

        public Rect CellBoundsFromLocation(int column, int row)
        {
            return new Rect(CellOriginFromLocation(column, row), CellSize);
        }
    }
}