using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Type : CanvasItemBase
    {
        private CanvasItemBase? _selectedType;

        public ObservableCollection<CanvasItemBase> AvailableTypes { get; } = new();

        public CanvasItemBase? SelectedType
        {
            get => _selectedType;
            set
            {
                if (value != null && !IsSupportedType(value) || ReferenceEquals(value, Parent)) return;
                if (_selectedType == value) return;

                if (_selectedType != null)
                    _selectedType.PropertyChanged -= SelectedType_PropertyChanged;

                _selectedType = value;

                if (_selectedType != null)
                    _selectedType.PropertyChanged += SelectedType_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
                UpdateLayoutAndSize();
            }
        }

        public override string Name
        {
            get => SelectedType switch
            {
                Entity entity => $"Entity: {entity.Name}",
                Record record => $"Record: {record.Name}",
                Proxy proxy => $"Proxy: {proxy.Name}",
                _ => "Not selected"
            };
            set { }
        }

        public Type() : base(180, 43)
        {
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> availableTypes)
        {
            CanvasItemBase[] supportedTypes = availableTypes
                .Where(item => IsSupportedType(item) && !ReferenceEquals(item, Parent))
                .Distinct()
                .ToArray();

            foreach (CanvasItemBase item in AvailableTypes.ToList())
                if (!supportedTypes.Contains(item))
                    AvailableTypes.Remove(item);

            foreach (CanvasItemBase item in supportedTypes)
                if (!AvailableTypes.Contains(item))
                    AvailableTypes.Add(item);

            if (SelectedType != null && !supportedTypes.Contains(SelectedType))
                SelectedType = null;
        }

        private static bool IsSupportedType(CanvasItemBase item)
        {
            return item is Entity or Record or Proxy;
        }

        private void SelectedType_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) &&
                e.PropertyName != nameof(CanvasItemBase.Name)) return;

            OnPropertyChanged(nameof(Name));
            UpdateLayoutAndSize();
        }

        protected override double GetRequiredOwnWidth()
        {
            return CanvasLayout.GetRequiredTextWidth(Name, 100);
        }
    }
}