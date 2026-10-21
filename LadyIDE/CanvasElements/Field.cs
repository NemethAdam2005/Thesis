
using System;

namespace LadyIDE.WPF.CanvasElements
{
    public class Field : CanvasItemBase
    {
        private string _type = "String";
        private bool _isLazy;

        public bool IsEntityField => Parent is Entity;

        public bool ShowLazyIndicator => IsLazy && IsEntityField;

        public string Type
        {
            get => _type;
            set
            {
                if (_type == value) return;

                _type = value;
                OnPropertyChanged();
                UpdateLayoutAndSize();
            }
        }

        public bool IsLazy
        {
            get => _isLazy;
            set
            {
                if (_isLazy == value) return;

                _isLazy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowLazyIndicator));
                UpdateLayoutAndSize();
            }
        }

        public Field() : base(160, 43)
        {
        }

        protected override double GetRequiredOwnWidth()
        {
            string headerText = $"{Name} - {Type}";
            double indicatorWidth = ShowLazyIndicator ? 12 : 0;

            return CanvasLayout.GetRequiredTextWidth(
                headerText,
                CanvasLayout.HeaderChromeWidth + indicatorWidth);
        }
    }
}
