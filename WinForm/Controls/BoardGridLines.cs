using Model;
using System;
using System.Windows;
using System.Windows.Media;

namespace Controls
{
    public class BoardGridLines : FrameworkElement
    {
        private readonly BoardMetrics _metrics = new BoardMetrics();
        
        #region Dependency properties

        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register("CellSize", typeof(double), typeof(BoardGridLines),
                new FrameworkPropertyMetadata(32d,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty GridLinesBrushProperty =
            DependencyProperty.Register("GridLinesBrush", typeof(Brush), typeof(BoardGridLines),
                new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static readonly DependencyProperty TripleWordSquareBrushProperty = 
        //    DependencyProperty.Register("TripleWordSquareBrush", typeof(Brush), typeof(BoardGridLines),
        //        new FrameworkPropertyMetadata(Brushes.Firebrick, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static readonly DependencyProperty TripleLetterSquareBrushProperty = 
        //    DependencyProperty.Register("TripleLetterSquareBrush", typeof(Brush), typeof(BoardGridLines),
        //        new FrameworkPropertyMetadata(Brushes.DodgerBlue, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static readonly DependencyProperty DoubleWordSquareBrushProperty = 
        //    DependencyProperty.Register("DoubleWordSquareBrush", typeof(Brush), typeof(BoardGridLines),
        //        new FrameworkPropertyMetadata(Brushes.Pink, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static readonly DependencyProperty DoubleLetterSquareBrushProperty = 
        //    DependencyProperty.Register("DoubleLetterSquareBrush", typeof(Brush), typeof(BoardGridLines),
        //        new FrameworkPropertyMetadata(Brushes.LightBlue, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        public double CellSize
        {
            get { return (double)GetValue(CellSizeProperty); }
            set { SetValue(CellSizeProperty, Math.Abs(value)); }
        }

        //public Brush TripleWordSquareBrush
        //{
        //    get { return (Brush)GetValue(TripleWordSquareBrushProperty); }
        //    set { SetValue(TripleWordSquareBrushProperty, value); }
        //}

        //public Brush TripleLetterSquareBrush
        //{
        //    get { return (Brush)GetValue(TripleLetterSquareBrushProperty); }
        //    set { SetValue(TripleLetterSquareBrushProperty, value); }
        //}

        //public Brush DoubleWordSquareBrush
        //{
        //    get { return (Brush)GetValue(DoubleWordSquareBrushProperty); }
        //    set { SetValue(DoubleWordSquareBrushProperty, value); }
        //}

        //public Brush DoubleLetterSquareBrush
        //{
        //    get { return (Brush)GetValue(DoubleLetterSquareBrushProperty); }
        //    set { SetValue(DoubleLetterSquareBrushProperty, value); }
        //}

        public Brush GridLinesBrush
        {
            get { return (Brush)GetValue(GridLinesBrushProperty); }
            set { SetValue(GridLinesBrushProperty, value); }
        }

        public BoardLocation HitTest(Point pt)
        {
            return !_metrics.RenderBounds.Contains(pt)
                ? null
                : new BoardLocation(
                    (int)((pt.X - _metrics.RenderBounds.X) / _metrics.CellSize.Width),
                    (int)((pt.Y - _metrics.RenderBounds.Y) / _metrics.CellSize.Height));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _metrics.CellSize = new Size(CellSize, CellSize);
            return _metrics.RenderBounds.Size;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return _metrics.RenderBounds.Size;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.PushGuidelineSet(CreateGuidelines());

            //DrawSquares(drawingContext);
            DrawGridLines(drawingContext);

            drawingContext.Pop();
        }

        private GuidelineSet CreateGuidelines()
        {
            var guidelineSet = new GuidelineSet();
            const double penHalfThickness = BoardMetrics.LineThickness / 2d;

            for (var i = 0; i <= Board.Columns; ++i)
                guidelineSet.GuidelinesX.Add(_metrics.ColumnCoordinate(i) + penHalfThickness);

            for (var i = 0; i <= Board.Rows; ++i)
                guidelineSet.GuidelinesY.Add(_metrics.RowCoordinate(i) + penHalfThickness);

            return guidelineSet;
        }

        //private void DrawSquares(DrawingContext drawingContext)
        //{
        //    drawingContext.DrawRectangle(Background, null, _metrics.RenderBounds);

        //    foreach (var square in Board.PremiumSquares)
        //    {
        //        Brush brush;

        //        switch (square.Item2)
        //        {
        //            case ScoringStyle.TripleWord:
        //                brush = TripleWordSquareBrush;
        //                break;

        //            case ScoringStyle.TripleLetter:
        //                brush = TripleLetterSquareBrush;
        //                break;

        //            case ScoringStyle.DoubleWord:
        //                brush = DoubleWordSquareBrush;
        //                break;

        //            case ScoringStyle.DoubleLetter:
        //                brush = DoubleLetterSquareBrush;
        //                break;

        //            default:
        //                continue;
        //        }

        //        if (brush == null) continue;

        //        drawingContext.DrawRectangle(brush, null, _metrics.CellBoundsFromLocation(square.Item1));
        //    }
        //}

        private void DrawGridLines(DrawingContext drawingContext)
        {
            if (GridLinesBrush == null) return;

            var gridLinePen = new Pen(GridLinesBrush, BoardMetrics.LineThickness);

            // Vertical
            {
                var p1 = new Point(0, _metrics.RenderBounds.Top);
                var p2 = new Point(0, _metrics.RenderBounds.Bottom);

                for (var i = 0; i <= Board.Columns; ++i)
                {
                    p1.X = p2.X = _metrics.ColumnCoordinate(i);
                    drawingContext.DrawLine(gridLinePen, p1, p2);
                }
            }

            // Horizontal
            {
                var p1 = new Point(_metrics.RenderBounds.Left, 0);
                var p2 = new Point(_metrics.RenderBounds.Right, 0);

                for (var i = 0; i <= Board.Rows; ++i)
                {
                    p1.Y = p2.Y = _metrics.RowCoordinate(i);
                    drawingContext.DrawLine(gridLinePen, p1, p2);
                }
            }
        }

        //private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        //{
        //    var location = HitTest(args.GetPosition(this));
        //    if (location == null || !location.IsWithinBounds) return;

        //    RaiseEvent(new SquareClickedEventArgs(SquareClickedEvent, location));
        //}
    }
}
