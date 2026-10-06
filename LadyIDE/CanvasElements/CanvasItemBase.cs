using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace LadyIDE.WPF.CanvasElements
{
    public abstract class CanvasItemBase : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private bool _isHighlighted;
        private string? _name;

        private readonly double _defaultWidth;
        private readonly double _defaultHeight;
        private readonly double _headerExtraWidth;

        public CanvasItemBase? _parent;

        public ObservableCollection<CanvasItemBase> Children { get; } = new();

        protected CanvasItemBase()
        {
            _defaultWidth = 250;
            _defaultHeight = 150;
            _headerExtraWidth = 120;

            _width = _defaultWidth;
            _height = _defaultHeight;

            Children.CollectionChanged += Children_CollectionChanged;
        }

        public string Name
        {
            get
            {
                if (_name != null)
                {
                    return _name;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();

                Width = Math.Max(_defaultWidth, GetMinimumWidth());
            }
        }

        public double X
        {
            get => _x;
            set
            {
                if (_x == value)
                    return;

                _x = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y == value)
                    return;

                _y = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                double newWidth = Math.Max(value, GetMinimumWidth());

                if (_width == newWidth)
                    return;

                _width = newWidth;
                UpdateSizeFromChildren();
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                if (_height == value)
                    return;

                _height = value;
                OnPropertyChanged();
            }
        }

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                if (_isHighlighted == value)
                    return;

                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        public CanvasItemBase? Parent
        {
            get => _parent;
            set
            {
                if (_parent == value)
                    return;

                _parent = value;
                OnPropertyChanged();
            }
        }

        public double GetMinimumWidth()
        {
            if(_name == null)
                return _defaultWidth;

            const double averageCharacterWidth = 8;

            double nameWidth = _name.Length * averageCharacterWidth;

            return Math.Max(_defaultWidth, _headerExtraWidth + nameWidth);
        }

        public void UpdateSizeFromChildren()
        {
            const double padding = 20;
            const double headerHeight = 48;

            double requiredWidth = _defaultWidth;
            double requiredHeight = _defaultHeight;

            if (Children.Count > 0)
            {
                double maxRight = 0;
                double maxBottom = 0;

                foreach (CanvasItemBase child in Children)
                {
                    maxRight = Math.Max(maxRight, child.X + child.Width);
                    maxBottom = Math.Max(maxBottom, child.Y + child.Height);
                }

                requiredWidth = maxRight + padding;
                requiredHeight = headerHeight + maxBottom + padding;
            }

            Width = requiredWidth;
            Height = Math.Max(_defaultHeight, requiredHeight);
        }

        public Point FindFreeChildPosition(double childWidth, double childHeight)
        {
            const double padding = 20;
            const double spacing = 20;

            double firstColumnX = padding;
            double secondColumnX = padding;
            double firstColumnWidth = 0;

            foreach (CanvasItemBase child in Children)
            {
                if (child.X == firstColumnX)
                    firstColumnWidth = Math.Max(firstColumnWidth, child.Width);
            }

            secondColumnX += firstColumnWidth + spacing;

            int row = Children.Count / 2;
            double y = padding;

            for (int i = 0; i < row; i++)
            {
                CanvasItemBase leftChild = Children[i * 2];
                double rowHeight = leftChild.Height;

                if (i * 2 + 1 < Children.Count)
                {
                    CanvasItemBase rightChild = Children[i * 2 + 1];
                    rowHeight = Math.Max(rowHeight, rightChild.Height);
                }

                y += rowHeight + spacing;
            }

            return Children.Count % 2 == 0
                ? new Point(firstColumnX, y)
                : new Point(secondColumnX, y);
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (CanvasItemBase child in e.NewItems)
                {
                    child.Parent = this;

                    child.PropertyChanged -= Child_PropertyChanged;
                    child.PropertyChanged += Child_PropertyChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (CanvasItemBase child in e.OldItems)
                {
                    child.PropertyChanged -= Child_PropertyChanged;

                    if (child.Parent == this)
                        child.Parent = null;
                }

                RearrangeChildren();
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
                RearrangeChildren();

            UpdateSizeFromChildren();
        }

        private void Child_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Name) || e.PropertyName == nameof(Width) || e.PropertyName == nameof(Height))
            {
                RearrangeChildren();
                UpdateSizeFromChildren();
            }
        }

        private void RearrangeChildren()
        {
            const double padding = 20;
            const double spacing = 20;

            double firstColumnWidth = 0;

            for (int i = 0; i < Children.Count; i += 2)
                firstColumnWidth = Math.Max(firstColumnWidth, Children[i].Width);

            for (int i = 0; i < Children.Count; i++)
            {
                CanvasItemBase child = Children[i];

                int column = i % 2;
                int row = i / 2;

                child.X = column == 0
                    ? padding
                    : padding + firstColumnWidth + spacing;

                double y = padding;

                for (int previousRow = 0; previousRow < row; previousRow++)
                {
                    int firstIndex = previousRow * 2;
                    double previousRowHeight = 0;

                    if (firstIndex < Children.Count)
                        previousRowHeight = Math.Max(previousRowHeight, Children[firstIndex].Height);

                    int secondIndex = firstIndex + 1;

                    if (secondIndex < Children.Count)
                        previousRowHeight = Math.Max(previousRowHeight, Children[secondIndex].Height);

                    y += previousRowHeight + spacing;
                }

                child.Y = y;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}