using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Controls
{
    public class RackItem : ListBoxItem
    {
        private static int _zIndex;

        private readonly Storyboard _storyboard = new Storyboard();
        private readonly DoubleAnimation _positionAnimation;
        private bool _animating;

        private bool? _wasDragInitiated;
        private Point _dragStartPoint;

        public event DragEventHandler DragStarted;
        public event DragEventHandler DragMoved;
        public event DragEventHandler DragFinished;

        public RackItem()
        {
            PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseMove += OnMouseMove;
            MouseLeftButtonUp += OnMouseLeftButtonUp;

            _positionAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 0.3,
                EasingFunction = new BackEase { Amplitude = 0.6, EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(_positionAnimation, this);
            Storyboard.SetTargetProperty(_positionAnimation, new PropertyPath("(Canvas.Left)"));

            _storyboard.Children.Add(_positionAnimation);
            _storyboard.Completed += (sender, args) =>
            {
                BeginAnimation(Canvas.LeftProperty, null);
                Position = _positionAnimation.To.Value;
                _animating = false;
            };
        }

        public double Position
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, Math.Min(Math.Max(0, value), MaximumPosition)); }
        }

        public double MaximumPosition
        {
            get
            {
                var parent = VisualParent as FrameworkElement;
                return (parent == null ? double.MaxValue : parent.ActualWidth) - ActualWidth;
            }
        }

        public void AnimateTo(double position)
        {
            if (_animating)
            {
                _storyboard.Pause();
            }

            if (VisualParent != null)
            {
                _animating = true;

                _positionAnimation.To = position;
                _storyboard.Begin();
            }
        }

        #region Dragging events

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            args.Handled = true;
            ((FrameworkElement)sender).CaptureMouse();

            _wasDragInitiated = false;
            _dragStartPoint = args.GetPosition(sender as UIElement);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            ((FrameworkElement)sender).ReleaseMouseCapture();

            if (_wasDragInitiated == false)
            {
                IsSelected = !IsSelected;
            }
            else if (DragFinished != null)
            {
                var offset = args.GetPosition(sender as UIElement) - _dragStartPoint;
                DragFinished(this, new DragEventArgs(offset.X, offset.Y, args));
            }

            _wasDragInitiated = null;
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (!_wasDragInitiated.HasValue) return;

            var position = args.GetPosition(sender as UIElement);

            if (_wasDragInitiated == false)
            {
                if (BoundsOfDragThresholdAt(_dragStartPoint).Contains(position)) return;

                _wasDragInitiated = true;
                Panel.SetZIndex(this, ++_zIndex);

                if (DragStarted != null)
                    DragStarted(this, new DragEventArgs(0, 0, args));
            }

            var offset = position - _dragStartPoint;

            Position += offset.X;

            if (DragMoved != null)
                DragMoved(this, new DragEventArgs(offset.X, offset.Y, args));
        }

        #endregion

        private static Rect BoundsOfDragThresholdAt(Point point)
        {
            var distance = new Size(
                SystemParameters.MinimumHorizontalDragDistance,
                SystemParameters.MinimumVerticalDragDistance);

            return new Rect(point.X - distance.Width / 2, point.Y - distance.Height / 2, distance.Width, distance.Height);
        }
    }
}