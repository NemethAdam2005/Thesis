namespace LadyIDE.WPF.CanvasElements
{
    public class Parameter : CanvasItemBase
    {
        private string _type = "String";

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

        public Parameter() : base(250,43)
        {
        }

        protected override double GetHeaderWidth()
        {
            const double averageCharacterWidth = 7;
            const double extraWidth = 20;

            return Math.Max(DefaultWidth, extraWidth + (Name.Length + Type.Length) * averageCharacterWidth);
        }

        protected override double GetHeaderHeight()
        {
            return 43;
        }
    }
}
