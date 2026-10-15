using System;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Component : CanvasItemBase
    {
        private const double Padding = 15;
        private const double Spacing = 15;
        private const double MinimumWidth = 150;
        private const double MinimumHeight = 100;

        private Record? _record;
        private Action? _action;
        private Mediator? _mediator;

        public Record? Record
        {
            get => _record;
            private set
            {
                if (_record == value) return;

                _record = value;
                OnPropertyChanged();
            }
        }

        public Action? Action
        {
            get => _action;
            private set
            {
                if (_action == value) return;

                _action = value;
                OnPropertyChanged();
            }
        }

        public Mediator? Mediator
        {
            get => _mediator;
            private set
            {
                if (_mediator == value) return;

                _mediator = value;
                OnPropertyChanged();
            }
        }

        public Component() : base()
        {
            Children.CollectionChanged += (_, _) =>
            {
                RefreshChildren();
                UpdateLayoutAndSize();
            };

            RefreshChildren();
        }

        private void RefreshChildren()
        {
            Record = Children.OfType<Record>().FirstOrDefault();
            Action = Children.OfType<Action>().FirstOrDefault();
            Mediator = Children.OfType<Mediator>().FirstOrDefault();
        }

        protected override void ArrangeChildren()
        {
            double x = Padding;

            foreach (CanvasItemBase child in Children)
            {
                child.X = x;
                child.Y = Padding;

                x += child.Width + Spacing;
            }
        }

        protected override double GetHeaderWidth()
        {
            double childrenWidth = Padding * 2;

            for (int i = 0; i < Children.Count; i++)
            {
                childrenWidth += Children[i].Width;

                if (i < Children.Count - 1)
                    childrenWidth += Spacing;
            }

            double nameWidth = Name.Length * 8 + 100;

            return Math.Max(Math.Max(childrenWidth, nameWidth), MinimumWidth);
        }
    }
}