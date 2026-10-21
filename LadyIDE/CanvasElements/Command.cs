
using System.Collections.Specialized;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Command : CanvasItemBase
    {
        private Action? _selectedAction;
        private Input? _input;
        private ActionReference? _actionReference;

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
            CanvasLayout.ArrangeVertically(Children);
        }
    }
}
