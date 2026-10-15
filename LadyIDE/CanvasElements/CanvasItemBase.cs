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
        private const double DefaultHeaderHeight = 48;
        private const double DefaultHeaderPadding = 120;
        private const double AverageCharacterWidth = 8;
        private const double ContentPadding = 20;

        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private bool _isHighlighted;
        private string? _name;
        private CanvasItemBase? _parent;
        private bool _isUpdatingSize;

        protected readonly double DefaultWidth;
        protected readonly double DefaultHeight;

        public ObservableCollection<CanvasItemBase> Children { get; } = new();

        protected CanvasItemBase(int width, int height)
        {
            DefaultWidth = width;
            DefaultHeight = height;

            _width = width;
            _height = height;

            Children.CollectionChanged += Children_CollectionChanged;
        }

        protected CanvasItemBase() : this(250, 150)
        {
        }

        public virtual string Name
        {
            get => _name ?? string.Empty;
            set
            {
                if (_name == value) return;

                _name = value;
                OnPropertyChanged();
                UpdateLayoutAndSize();
            }
        }

        public double X
        {
            get => _x;
            set
            {
                if (_x == value) return;

                _x = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y == value) return;

                _y = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => _width;
            private set
            {
                if (_width == value) return;

                _width = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => _height;
            private set
            {
                if (_height == value) return;

                _height = value;
                OnPropertyChanged();
            }
        }

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                if (_isHighlighted == value) return;

                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        public CanvasItemBase? Parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;

                _parent = value;
                OnPropertyChanged();
            }
        }

        protected virtual double GetHeaderWidth()
        {
            return DefaultHeaderPadding + Name.Length * AverageCharacterWidth;
        }

        protected virtual double GetHeaderHeight()
        {
            return DefaultHeaderHeight;
        }

        public void UpdateLayoutAndSize()
        {
            if (_isUpdatingSize) return;

            _isUpdatingSize = true;

            double previousWidth = Width;
            double previousHeight = Height;

            try
            {
                ArrangeChildren();

                double requiredWidth = Math.Max(DefaultWidth, GetHeaderWidth());
                double requiredHeight = Math.Max(DefaultHeight, GetHeaderHeight());

                if (Children.Count > 0)
                {
                    double maxRight = 0;
                    double maxBottom = 0;

                    foreach (CanvasItemBase child in Children)
                    {
                        maxRight = Math.Max(maxRight, child.X + child.Width);
                        maxBottom = Math.Max(maxBottom, child.Y + child.Height);
                    }

                    requiredWidth = Math.Max(requiredWidth, maxRight + ContentPadding);
                    requiredHeight = Math.Max(
                        requiredHeight,
                        GetHeaderHeight() + maxBottom + ContentPadding);
                }

                Width = requiredWidth;
                Height = requiredHeight;
            }
            finally
            {
                _isUpdatingSize = false;
            }

            if (Width != previousWidth || Height != previousHeight)
                Parent?.UpdateLayoutAndSize();
        }

        public Point FindFreeChildPosition(double childWidth, double childHeight)
        {
            const double padding = 20;
            const double spacing = 20;

            int row = Children.Count / 2;
            int column = Children.Count % 2;

            double firstColumnWidth = 0;

            for (int i = 0; i < Children.Count; i += 2)
                firstColumnWidth = Math.Max(firstColumnWidth, Children[i].Width);

            double x = column == 0 ? padding : padding + firstColumnWidth + spacing;
            double y = padding;

            for (int i = 0; i < row * 2; i += 2)
            {
                double rowHeight = Children[i].Height;

                if (i + 1 < Children.Count)
                    rowHeight = Math.Max(rowHeight, Children[i + 1].Height);

                y += rowHeight + spacing;
            }

            return new Point(x, y);
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (CanvasItemBase child in e.OldItems)
                {
                    if (child.Parent == this)
                        child.Parent = null;
                }
            }

            if (e.NewItems != null)
            {
                foreach (CanvasItemBase child in e.NewItems)
                    child.Parent = this;
            }

            UpdateLayoutAndSize();
        }

        protected virtual void ArrangeChildren()
        {
            const double padding = 8;
            const double spacing = 6;

            double firstColumnWidth = 0;

            for (int i = 0; i < Children.Count; i += 2)
                firstColumnWidth = Math.Max(firstColumnWidth, Children[i].Width);

            double y = padding;

            for (int i = 0; i < Children.Count; i += 2)
            {
                CanvasItemBase leftChild = Children[i];

                leftChild.X = padding;
                leftChild.Y = y;

                double rowHeight = leftChild.Height;

                if (i + 1 < Children.Count)
                {
                    CanvasItemBase rightChild = Children[i + 1];

                    rightChild.X = padding + firstColumnWidth + spacing;
                    rightChild.Y = y;

                    rowHeight = Math.Max(rowHeight, rightChild.Height);
                }

                y += rowHeight + spacing;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}