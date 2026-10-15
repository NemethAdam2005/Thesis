
using System.Collections.Generic;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class EnumElement : CanvasItemBase
    {
        private IEnumerable<EnumField> _fields = Enumerable.Empty<EnumField>();
        private Base? _baseItem;

        public IEnumerable<EnumField> Fields
        {
            get => _fields;
            private set
            {
                _fields = value;
                OnPropertyChanged();
            }
        }

        public Base? BaseItem
        {
            get => _baseItem;
            private set
            {
                if (_baseItem == value) return;

                _baseItem = value;
                OnPropertyChanged();
            }
        }

        public EnumElement() : base()
        {
            Children.CollectionChanged += (_, _) => RefreshChildren();
            RefreshChildren();
        }

        private void RefreshChildren()
        {
            Fields = Children.OfType<EnumField>().ToList();
            BaseItem = Children.OfType<Base>().FirstOrDefault();
            UpdateLayoutAndSize();
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangePrimaryColumnWithSideItem(Fields, BaseItem);
        }
    }
}
