using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Component : CanvasItemBase
    {
        public ObservableCollection<Record> Records { get; } = new();
        public ObservableCollection<Action> Actions { get; } = new();

        public Component() : base()
        {
            Children.CollectionChanged += Children_CollectionChanged;
            RefreshChildren();
        }

        private void Children_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            RefreshChildren();
            UpdateLayoutAndSize();
        }

        private void RefreshChildren()
        {
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
                Records.Cast<CanvasItemBase>()
                    .Concat(Actions));
        }
    }
}