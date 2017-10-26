using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class Rack : ListBox
    {
        #region TileSize property

        public static readonly DependencyProperty TileSizeProperty = DependencyProperty.Register("TileSize",
            typeof(double), typeof(Rack),
            new FrameworkPropertyMetadata(1.0d,
                  FrameworkPropertyMetadataOptions.AffectsArrange |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnTileSizeChanged));

        public double TileSize
        {
            get { return (double)GetValue(TileSizeProperty); }
            set { SetValue(TileSizeProperty, value); }
        }

        #endregion

        private readonly List<RackItem> _items = new List<RackItem>();

        public Rack()
        {
            SizeChanged += (sender, args) => RepairCollisions();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RackItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RackItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var rackElement = element as RackItem;
            if (rackElement == null) return;

            rackElement.DragFinished += OnDragFinished;
            rackElement.Width = TileSize;
            rackElement.Height = TileSize;

            if (FindCollisions(_items, rackElement).Any())
                rackElement.Position = FindSpaceFor(_items, rackElement);

            _items.Add(rackElement);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            var rackElement = element as RackItem;
            if (rackElement == null) return;

            rackElement.DragFinished -= OnDragFinished;
            _items.Remove(rackElement);
        }

        private void RepairCollisions()
        {
            for (var i = 1; i < _items.Count; ++i)
            {
                var item = _items[i];

                if (FindCollisions(_items.Take(i), item).Any())
                    item.Position = FindSpaceFor(_items.Take(i), item);
            }
        }

        private void OnDragFinished(object sender, DragEventArgs args)
        {
            var rackItem = sender as RackItem;
            if (rackItem == null) return;

            var layout = GenerateLayout(_items, _items.IndexOf(rackItem), rackItem.MaximumPosition + rackItem.Width);

            foreach (var change in layout)
            {
                change.Item1.AnimateTo(change.Item2);
            }
        }

        private static void OnTileSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rack = d as Rack;
            if (rack == null) return;

            foreach (var item in rack._items)
            {
                item.Width = rack.TileSize;
                item.Height = rack.TileSize;
            }

            rack.RepairCollisions();
        }

        private static double FindSpaceFor(IEnumerable<RackItem> list, FrameworkElement target)
        {
            var extent = target.Width;
            var priorEnd = 0d;

            var sortedList = list.ToList();
            sortedList.Sort((a, b) => (int)(a.Position - b.Position));

            foreach (var item in sortedList)
            {
                if (priorEnd + extent <= item.Position)
                    break;

                priorEnd = item.Position + item.Width;
            }

            return priorEnd;
        }

        private static IEnumerable<RackItem> FindCollisions(IEnumerable<RackItem> sourceList, RackItem target)
        {
            return sourceList.Where(source => Overlaps(source, target));
        }

        private static bool Overlaps(RackItem a, RackItem b)
        {
            return (a.Position + a.Width) > b.Position && (b.Position + b.Width) > a.Position;
        }

        private static IEnumerable<Tuple<RackItem, double>> GenerateLayout(List<RackItem> layout, int indexOfPin, double maximumWidth)
        {
            var deltas = new List<Tuple<RackItem, double>>();
            if (layout.Count == 0) return deltas;

            var pinnedItem = layout[indexOfPin];
            layout.Sort((a, b) => (int)(a.Position - b.Position));

            var cutOff = pinnedItem.Position;
            var totalBeforePin = layout.Count(item => item.Position < cutOff);

            var minimumStart = layout.Take(totalBeforePin).Aggregate(0d, (result, bounds) => result + bounds.Width);
            var maximumStart = maximumWidth - layout.Skip(totalBeforePin).Aggregate(0d, (result, bounds) => result + bounds.Width);

            var pinnedPosition = Math.Max(Math.Min(pinnedItem.Position, maximumStart), minimumStart);
            deltas.Add(Tuple.Create(pinnedItem, pinnedPosition));

            var lastPinnedPosition = pinnedPosition;

            for (var i = totalBeforePin; i > 0; --i)
            {
                var adjustableItem = layout[i - 1];

                var overlap = (adjustableItem.Position + adjustableItem.Width) - lastPinnedPosition;
                if (overlap <= 0) break;

                lastPinnedPosition = adjustableItem.Position - overlap;
                deltas.Add(Tuple.Create(adjustableItem, lastPinnedPosition));
            }

            lastPinnedPosition = pinnedPosition;

            for (var i = totalBeforePin + 1; i < layout.Count; ++i)
            {
                var adjustableItem = layout[i];

                var overlap = (lastPinnedPosition + pinnedItem.Width) - adjustableItem.Position;
                if (overlap <= 0) break;

                lastPinnedPosition = adjustableItem.Position + overlap;
                deltas.Add(Tuple.Create(adjustableItem, lastPinnedPosition));
            }

            return deltas;
        }
    }
}
