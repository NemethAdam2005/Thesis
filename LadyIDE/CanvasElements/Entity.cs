using System;
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
                _baseList = value;
                OnPropertyChanged();
            }
        }

        public Entity() : base(260, 150)
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
            const double padding = 8;
            const double spacing = 6;

            double y = padding;

            foreach (Field field in Fields)
            {
                field.X = padding;
                field.Y = y;
                y += field.Height + spacing;
            }

            if (BaseList == null) return;

            double fieldsWidth = Fields.Any()
                ? Fields.Max(field => field.Width)
                : 0;

            BaseList.X = Fields.Any()
                ? padding + fieldsWidth + spacing
                : padding;

            BaseList.Y = padding;
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

            if (BaseList != null)
                requiredWidth += (Fields.Any() ? spacing : 0) + BaseList.Width;

            return Math.Max(requiredWidth, Name.Length * 8 + 100);
        }
    }
}