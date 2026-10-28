namespace LadyIDE.WPF.CanvasElements
{
    public class Logic : CanvasItemBase
    {
        private string _text = string.Empty;

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;

                _text = value;
                OnPropertyChanged();
            }
        }

        protected override double GetHeaderHeight()
        {
            return 100;
        }

        public Logic() : base(300, 130)
        {
        }
    }
}