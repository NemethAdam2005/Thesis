using System.Collections.Generic;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Proxy : CanvasItemBase
    {
        private IEnumerable<Field> _fields = Enumerable.Empty<Field>();
        private Type? _type;

        public IEnumerable<Field> Fields
        {
            get => _fields;
            private set
            {
                _fields = value;
                OnPropertyChanged();
            }
        }

        public Type? Type
        {
            get => _type;
            private set
            {
                if (_type == value) return;

                _type = value;
                OnPropertyChanged();
            }
        }

        public Proxy() : base()
        {
            Children.CollectionChanged += (_, _) => RefreshChildren();
            RefreshChildren();
        }

        private void RefreshChildren()
        {
            Fields = Children.OfType<Field>().ToList();
            Type = Children.OfType<Type>().FirstOrDefault();
            UpdateLayoutAndSize();
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangePrimaryColumnWithSideItem(Fields, Type);
        }
    }
}