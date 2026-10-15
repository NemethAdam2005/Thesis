
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
        private CanvasItemBase? _parent;
        private bool _isUpdatingLayout;

        protected readonly double MinimumWidth;
        protected readonly double MinimumHeight;

        public ObservableCollection<CanvasItemBase> Children { get; } = new();

        protected CanvasItemBase(int minimumWidth, int minimumHeight)
        {
            MinimumWidth = minimumWidth;
            MinimumHeight = minimumHeight;

            _width = minimumWidth;
            _height = minimumHeight;

            Children.CollectionChanged += Children_CollectionChanged;
        }

        protected CanvasItemBase() : this(260, 150)
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

        public double Width => _width;

        public double Height => _height;

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                if (_isHighlighted == value) return;

                _isHighlighted = value;
                OnPropertyChanged();
                UpdateLayoutAndSize();
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

        protected virtual double GetRequiredOwnWidth()
        {
            return CanvasLayout.GetRequiredTextWidth(Name);
        }

        protected virtual double GetHeaderHeight()
        {
            return CanvasLayout.HeaderHeight;
        }

        protected virtual double GetBorderAllowance()
        {
            return IsHighlighted ? 10 : 4;
        }

        public void UpdateLayoutAndSize()
        {
            if (_isUpdatingLayout) return;

            _isUpdatingLayout = true;

            try
            {
                ArrangeChildren();

                double borderAllowance = GetBorderAllowance();

                double requiredWidth = Math.Max(
                    MinimumWidth,
                    GetRequiredOwnWidth() + borderAllowance);

                double requiredHeight = Math.Max(
                    MinimumHeight,
                    GetHeaderHeight() + borderAllowance);

                if (Children.Count > 0)
                {
                    double maxRight = 0;
                    double maxBottom = 0;

                    foreach (CanvasItemBase child in Children)
                    {
                        maxRight = Math.Max(maxRight, child.X + child.Width);
                        maxBottom = Math.Max(maxBottom, child.Y + child.Height);
                    }

                    requiredWidth = Math.Max(
                        requiredWidth,
                        maxRight + CanvasLayout.Padding + borderAllowance);

                    requiredHeight = Math.Max(
                        requiredHeight,
                        GetHeaderHeight() + maxBottom + CanvasLayout.Padding + borderAllowance);
                }

                SetComputedSize(requiredWidth, requiredHeight);
            }
            finally
            {
                _isUpdatingLayout = false;
            }

            Parent?.UpdateLayoutAndSize();
        }

        private void SetComputedSize(double width, double height)
        {
            bool widthChanged = _width != width;
            bool heightChanged = _height != height;

            if (!widthChanged && !heightChanged) return;

            _width = width;
            _height = height;

            if (widthChanged) OnPropertyChanged(nameof(Width));
            if (heightChanged) OnPropertyChanged(nameof(Height));
        }

        public Point FindFreeChildPosition(double childWidth, double childHeight)
        {
            int row = Children.Count / 2;
            int column = Children.Count % 2;

            double firstColumnWidth = 0;

            for (int i = 0; i < Children.Count; i += 2)
                firstColumnWidth = Math.Max(firstColumnWidth, Children[i].Width);

            double x = column == 0
                ? CanvasLayout.Padding
                : CanvasLayout.Padding + firstColumnWidth + CanvasLayout.Spacing;

            double y = CanvasLayout.Padding;

            for (int i = 0; i < row * 2; i += 2)
            {
                double rowHeight = Children[i].Height;

                if (i + 1 < Children.Count)
                    rowHeight = Math.Max(rowHeight, Children[i + 1].Height);

                y += rowHeight + CanvasLayout.Spacing;
            }

            return new Point(x, y);
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (CanvasItemBase child in e.OldItems)
                {
                    child.PropertyChanged -= Child_PropertyChanged;

                    if (child.Parent == this)
                        child.Parent = null;
                }
            }

            if (e.NewItems != null)
            {
                foreach (CanvasItemBase child in e.NewItems)
                {
                    child.Parent = this;
                    child.PropertyChanged -= Child_PropertyChanged;
                    child.PropertyChanged += Child_PropertyChanged;
                }
            }

            UpdateLayoutAndSize();
        }

        private void Child_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Width) || e.PropertyName == nameof(Height))
                UpdateLayoutAndSize();
        }

        protected virtual void ArrangeChildren()
        {
            CanvasLayout.ArrangeInTwoColumns(Children);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
