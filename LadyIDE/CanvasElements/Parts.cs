using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Parts : CanvasItemBase
    {
        private List<CanvasItemBase> _allItems = new();

        public Parts() : base(220, 100)
        {
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            foreach (Part part in Children.OfType<Part>())
                part.RefreshAvailableTypes(_allItems);

            UpdateAvailableParts();
            UpdateLayoutAndSize();
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> allItems)
        {
            _allItems = allItems.ToList();

            foreach (Part part in Children.OfType<Part>())
                part.RefreshAvailableTypes(_allItems);

            UpdateAvailableParts();
        }

        public void UpdateAvailableParts()
        {
            List<Part> parts = Children.OfType<Part>().ToList();

            foreach (Part part in parts)
            {
                List<CanvasItemBase> usedItems = parts
                    .Where(other => !ReferenceEquals(other, part))
                    .Select(other => other.SelectedTypeOption?.Item)
                    .OfType<CanvasItemBase>()
                    .ToList();

                part.UpdateAvailableTypes(usedItems);
            }

            UpdateLayoutAndSize();
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeVertically(Children.OfType<Part>());
        }
    }
}