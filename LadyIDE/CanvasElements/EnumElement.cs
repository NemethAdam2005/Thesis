using System.Collections.Generic;
using System.Collections.Specialized;
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
                _baseItem = value;
                OnPropertyChanged();
            }
        }

        public EnumElement() : base(250, 150)
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
            const double padding = 8;
            const double spacing = 6;

            double y = padding;

            foreach (EnumField field in Fields)
            {
                field.X = padding;
                field.Y = y;

                y += field.Height + spacing;
            }

            if (BaseItem == null) return;

            double fieldsWidth = Fields.Any()
                ? Fields.Max(field => field.Width)
                : 0;

            BaseItem.X = Fields.Any()
                ? fieldsWidth + spacing + padding
                : padding;

            BaseItem.Y = padding;
        }

        protected override double GetHeaderWidth()
        {
            const double padding = 8;
            const double spacing = 6;

            double fieldsWidth = Fields.Any()
                ? Fields.Max(field => field.Width)
                : 0;

            double requiredWidth = padding * 2;

            if (Fields.Any())
                requiredWidth += fieldsWidth;

            if (BaseItem != null)
                requiredWidth += (Fields.Any() ? spacing : 0) + BaseItem.Width;

            return Math.Max(requiredWidth, Name.Length * 8 + 100);
        }
    }
}