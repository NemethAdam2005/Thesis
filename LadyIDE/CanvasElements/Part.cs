
using System.ComponentModel;

namespace LadyIDE.WPF.CanvasElements
{
    public class Part : CanvasItemBase
    {
        private Entity? _entity;

        public Entity? Entity
        {
            get => _entity;
            set
            {
                if (_entity == value) return;

                if (_entity != null) _entity.PropertyChanged -= Entity_PropertyChanged;

                _entity = value;

                if (_entity != null) _entity.PropertyChanged += Entity_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
                UpdateLayoutAndSize();
            }
        }

        public override string Name
        {
            get => Entity?.Name ?? string.Empty;
            set { }
        }

        public Part() : base(150, 43)
        {
        }

        private void Entity_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) && e.PropertyName != nameof(CanvasItemBase.Name)) return;

            OnPropertyChanged(nameof(Name));
            UpdateLayoutAndSize();
        }
    }
}
