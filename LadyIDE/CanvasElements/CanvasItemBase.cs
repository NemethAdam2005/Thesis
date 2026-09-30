using System.Collections.ObjectModel;
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

        public CanvasItemBase? _parent;
        public ObservableCollection<CanvasItemBase> Children { get; } = new();

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
                if (_width == value)
                    return;

                _width = value;
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

        public void UpdateSizeFromChildren()
        {
            if (Children.Count == 0)
                return;

            const double padding = 20;
            const double headerHeight = 48;

            double requiredWidth = 0;
            double requiredHeight = 0;

            foreach (CanvasItemBase child in Children)
            {
                double right = child.X + child.Width;
                double bottom = child.Y + child.Height;

                if (right > requiredWidth)
                    requiredWidth = right;

                if (bottom > requiredHeight)
                    requiredHeight = bottom;
            }

            Width = requiredWidth + padding;
            Height = headerHeight + requiredHeight + padding;
        }

        public Point FindFreeChildPosition(double childWidth, double childHeight)
        {
            const double padding = 20;
            const double spacing = 20;

            // Első oszlop
            double firstColumnX = padding;

            // Második oszlop
            double secondColumnX = padding;

            double firstColumnWidth = 0;

            // Megkeressük az első oszlop maximális szélességét
            foreach (CanvasItemBase child in Children)
            {
                if (child.X == firstColumnX)
                {
                    firstColumnWidth = Math.Max(
                        firstColumnWidth,
                        child.Width);
                }
            }

            secondColumnX += firstColumnWidth + spacing;

            // Megkeressük, melyik sorba kerül az új elem.
            int row = Children.Count / 2;

            double y = padding;

            for (int i = 0; i < row; i++)
            {
                CanvasItemBase leftChild = Children[i * 2];

                double rowHeight = leftChild.Height;

                if (i * 2 + 1 < Children.Count)
                {
                    CanvasItemBase rightChild =
                        Children[i * 2 + 1];

                    rowHeight = Math.Max(
                        rowHeight,
                        rightChild.Height);
                }

                y += rowHeight + spacing;
            }

            // Páratlan index → bal oszlop
            if (Children.Count % 2 == 0)
            {
                return new Point(
                    firstColumnX,
                    y);
            }

            // Páros index → jobb oszlop
            return new Point(
                secondColumnX,
                y);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}