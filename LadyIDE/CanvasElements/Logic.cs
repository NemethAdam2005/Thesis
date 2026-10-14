
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
                UpdateLayoutAndSize();
            }
        }

        protected override double GetHeaderHeight()
        {
            const double lineHeight = 20;
            const double padding = 26;
            const int maxCharactersPerLine = 30;

            int lineCount = 0;

            foreach (string line in Text.Split('\n'))
            {
                lineCount += Math.Max(1,(int)Math.Ceiling(line.Length / (double)maxCharactersPerLine));
            }

            return Math.Max(100, lineCount * lineHeight + padding);
        }

        public Logic() : base(300, 110)
        {
        }
    }
}
