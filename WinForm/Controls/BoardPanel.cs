using System;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    // A custom panel to drive the layout based on CellSize.

    public class BoardPanel : Panel
    {
        private readonly BoardMetrics _metrics = new BoardMetrics();

        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register("CellSize", typeof(double), typeof(BoardPanel),
                new FrameworkPropertyMetadata(32d,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double CellSize
        {
            get { return (double)GetValue(CellSizeProperty); }
            set { SetValue(CellSizeProperty, Math.Abs(value)); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _metrics.CellSize = new Size(CellSize, CellSize);

            foreach (UIElement element in InternalChildren)
            {
                element.Measure(_metrics.CellSize);
            }

            return _metrics.RenderBounds.Size;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            foreach (UIElement element in InternalChildren)
            {
                var col = Grid.GetColumn(element);
                var row = Grid.GetRow(element);
                var spanAcross = Math.Max(1, Grid.GetColumnSpan(element));
                var spanDown = Math.Max(1, Grid.GetRowSpan(element));

                var origin = _metrics.CellOriginFromLocation(col, row);
                var bounds = new Rect(origin.X, origin.Y,
                    spanAcross * _metrics.CellSize.Width,
                    spanDown * _metrics.CellSize.Height);

                element.Arrange(bounds);
            }

            return _metrics.RenderBounds.Size;
        }
    }
}