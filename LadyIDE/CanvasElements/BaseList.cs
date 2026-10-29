using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class BaseList : CanvasItemBase
    {
        private List<CanvasItemBase> _allItems = new();

        public BaseList() : base(180, 100)
        {
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            UpdateAvailableBases();
            UpdateLayoutAndSize();
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> allItems)
        {
            _allItems = allItems.ToList();

            foreach (Base baseItem in Children.OfType<Base>())
                baseItem.RefreshAvailableTypes(_allItems);

            UpdateAvailableBases();
        }

        public void UpdateAvailableBases()
        {
            List<Base> bases = Children.OfType<Base>().ToList();

            foreach (Base baseItem in bases)
            {
                List<CanvasItemBase> usedItems = bases
                    .Where(other => !ReferenceEquals(other, baseItem))
                    .Select(other => other.SelectedTypeOption?.Item)
                    .OfType<CanvasItemBase>()
                    .ToList();

                baseItem.UpdateAvailableTypes(usedItems);
            }

            UpdateLayoutAndSize();
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeVertically(Children.OfType<Base>());
        }
    }
}