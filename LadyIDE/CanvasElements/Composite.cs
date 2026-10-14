
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Composite : CanvasItemBase
    {
        private string _compositeType = "1-N";

        public string CompositeType
        {
            get => _compositeType;
            set
            {
                if (_compositeType == value) return;

                _compositeType = value;
                OnPropertyChanged();

                CompositeTypeElement? typeElement = Children
                    .OfType<CompositeTypeElement>()
                    .FirstOrDefault();

                if (typeElement != null) typeElement.Name = value;
            }
        }

        private Parts? _parts;

        public Parts? Parts
        {
            get => _parts;
            private set
            {
                _parts = value;
                OnPropertyChanged();
            }
        }

        public Composite() : base()
        {
            Children.CollectionChanged += (_, _) => RefreshChildren();
            RefreshChildren();
        }

        private void RefreshChildren()
        {
            Parts = Children.OfType<Parts>().FirstOrDefault();
        }

    }
}
