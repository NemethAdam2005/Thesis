
using System.Collections.Generic;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Entity : CanvasItemBase
    {
        private IEnumerable<Field> _fields = Enumerable.Empty<Field>();
        private BaseList? _baseList;

        public IEnumerable<Field> Fields
        {
            get => _fields;
            private set
            {
                _fields = value;
                OnPropertyChanged();
            }
        }

        public BaseList? BaseList
        {
            get => _baseList;
            private set
            {
                if (_baseList == value) return;

                _baseList = value;
                OnPropertyChanged();
            }
        }

        public Entity() : base()
        {
            Children.CollectionChanged += (_, _) => RefreshChildren();
            RefreshChildren();
        }

        private void RefreshChildren()
        {
            Fields = Children.OfType<Field>().ToList();
            BaseList = Children.OfType<BaseList>().FirstOrDefault();
            UpdateLayoutAndSize();
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangePrimaryColumnWithSideItem(Fields, BaseList);
        }
    }
}
