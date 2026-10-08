namespace LadyIDE.WPF.CanvasElements
{
    public class Input : CanvasItemBase
    {
        private string _type = string.Empty;

        public string Type
        {
            get => _type;
            set
            {
                if (_type == value)
                    return;

                _type = value;
                OnPropertyChanged();
            }
        }

        public Input() : base(50,80)
        {
        }
    }
}