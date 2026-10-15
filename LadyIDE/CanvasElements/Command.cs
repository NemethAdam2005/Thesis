
using System.Collections.Specialized;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Command : CanvasItemBase
    {
        private Action? _selectedAction;

        public Action? SelectedAction
        {
            get => _selectedAction;
            set
            {
                if (_selectedAction == value) return;

                _selectedAction = value;
                OnPropertyChanged();
            }
        }

        private Input? _input;

        public Input? Input
        {
            get => _input;
            private set
            {
                if (_input == value) return;

                _input = value;
                OnPropertyChanged();
            }
        }

        private ActionReference? _actionReference;

        public ActionReference? ActionReference
        {
            get => _actionReference;
            private set
            {
                if (_actionReference == value) return;

                _actionReference = value;
                OnPropertyChanged();
            }
        }

        public Command() : base()
        {
            Children.CollectionChanged += Children_CollectionChanged;
            RefreshChildCollections();
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshChildCollections();
        }

        private void RefreshChildCollections()
        {
            Input = Children.OfType<Input>().FirstOrDefault();
            ActionReference = Children.OfType<ActionReference>().FirstOrDefault();
        }

        protected override void ArrangeChildren()
        {
            const double padding = 8;
            const double spacing = 6;

            double y = padding;

            foreach (var child in Children)
            {
                child.X = padding;
                child.Y = y;

                y += child.Height + spacing;
            }
        }
    }
}
