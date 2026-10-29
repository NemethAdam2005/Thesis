using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Factory : CanvasItemBase
    {
        private Type? _type;

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

        public ObservableCollection<Record> Records { get; } = new();

        public ObservableCollection<Action> Actions { get; } = new();

        public Factory() : base()
        {
            Children.CollectionChanged += Children_CollectionChanged;
            RefreshChildren();
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshChildren();
            UpdateLayoutAndSize();
        }

        private void RefreshChildren()
        {
            Type = Children.OfType<Type>().FirstOrDefault();

            Records.Clear();
            foreach (Record record in Children.OfType<Record>())
                Records.Add(record);

            Actions.Clear();
            foreach (Action action in Children.OfType<Action>())
                Actions.Add(action);
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeInTwoColumns(
                Children.OfType<Type>()
                    .Cast<CanvasItemBase>()
                    .Concat(Records)
                    .Concat(Actions));
        }
    }
}