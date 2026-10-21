
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Component : CanvasItemBase
    {
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
            Children.CollectionChanged += (_, _) => RefreshChildren();
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
            CanvasLayout.ArrangeHorizontally(Children);
        }
    }
}
