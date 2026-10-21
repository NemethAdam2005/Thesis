using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Mediator : CanvasItemBase
    {
        private TypePairs? _typePairs;

        public TypePairs? TypePairs
        {
            get => _typePairs;
            private set
            {
                if (_typePairs == value) return;

                _typePairs = value;
                OnPropertyChanged();
            }
        }

        public Mediator() : base()
        {
            Children.CollectionChanged += (_, _) => RefreshChildren();
            RefreshChildren();
        }

        private void RefreshChildren()
        {
            TypePairs = Children.OfType<TypePairs>().FirstOrDefault();
            UpdateLayoutAndSize();
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeVertically(Children);
        }
    }
}