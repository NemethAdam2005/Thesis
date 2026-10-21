
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

        public Parameter() : base(200, 43)
        {
        }

        protected override double GetRequiredOwnWidth()
        {
            return base.GetRequiredOwnWidth() + Type.Length * 7;
        }
    }
}
