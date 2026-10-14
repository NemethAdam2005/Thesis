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

        protected override double GetHeaderWidth()
        {
            if (IsLazy)
            {
                return base.GetHeaderWidth() + 10;
            }
            return base.GetHeaderWidth();
        }
    }
}