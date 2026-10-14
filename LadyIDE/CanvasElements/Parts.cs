using System.Collections.ObjectModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Parts : CanvasItemBase
    {
        private Entity? _selectedEntity;

        public ObservableCollection<Entity> AvailableEntities { get; } = new();

        public Entity? SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                if (_selectedEntity == value) return;

                _selectedEntity = value;
                OnPropertyChanged();
            }
        }

        public Parts() : base(220, 100)
        {
        }

        public void RefreshAvailableEntities(IEnumerable<Entity> allEntities)
        {
            Entity[] usedEntities = Children
                .OfType<Part>()
                .Where(part => part.Entity != null)
                .Select(part => part.Entity!)
                .ToArray();

            Entity[] availableEntities = allEntities
                .Where(entity => !usedEntities.Contains(entity))
                .ToArray();

            AvailableEntities.Clear();

            foreach (Entity entity in availableEntities)
                AvailableEntities.Add(entity);

            if (SelectedEntity != null && !AvailableEntities.Contains(SelectedEntity))
                SelectedEntity = null;
        }

        protected override void ArrangeChildren()
        {
            const double padding = 8;
            const double spacing = 6;

            double y = padding;

            foreach (Part part in Children)
            {
                part.X = padding;
                part.Y = y;
                y += part.Height + spacing;
            }
        }
    }
}