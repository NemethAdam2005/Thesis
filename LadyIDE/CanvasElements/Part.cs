using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Part : CanvasItemBase
    {
        private TypeOption? _selectedTypeOption;
        private List<TypeOption> _allAvailableTypes = new();
        private bool _isUpdatingAvailableTypes;

        public ObservableCollection<TypeOption> AvailableTypes { get; } = new();

        public TypeOption? SelectedTypeOption
        {
            get => _selectedTypeOption;
            set
            {
                if (ReferenceEquals(_selectedTypeOption, value)) return;

                if (_selectedTypeOption != null)
                    _selectedTypeOption.PropertyChanged -= SelectedTypeOption_PropertyChanged;

                _selectedTypeOption = value;

                if (_selectedTypeOption != null)
                    _selectedTypeOption.PropertyChanged += SelectedTypeOption_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));

                UpdateLayoutAndSize();

                if (!_isUpdatingAvailableTypes)
                    (Parent as Parts)?.UpdateAvailableParts();
            }
        }

        public override string Name
        {
            get => SelectedTypeOption?.Name ?? "Not selected";
            set { }
        }

        public Part() : base(200, 43)
        {
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> allItems)
        {
            _isUpdatingAvailableTypes = true;

            try
            {
                _allAvailableTypes = TypeOptionProvider.RequiredTypes(
                    allItems,
                    entity: true);

                if (SelectedTypeOption?.Item is CanvasItemBase selectedItem &&
                    !_allAvailableTypes.Any(type => ReferenceEquals(type.Item, selectedItem)))
                    SelectedTypeOption = null;
            }
            finally
            {
                _isUpdatingAvailableTypes = false;
            }
        }

        internal void UpdateAvailableTypes(IEnumerable<CanvasItemBase> usedItems)
        {
            List<CanvasItemBase> used = usedItems.ToList();

            List<TypeOption> types = _allAvailableTypes
                .Where(type => type.Item != null &&
                    !used.Any(item => ReferenceEquals(item, type.Item)))
                .ToList();

            _isUpdatingAvailableTypes = true;

            try
            {
                if (SelectedTypeOption?.Item is CanvasItemBase selectedItem &&
                    used.Any(item => ReferenceEquals(item, selectedItem)))
                    SelectedTypeOption = null;

                foreach (TypeOption type in types)
                {
                    if (!ContainsTypeOption(type, AvailableTypes))
                        AvailableTypes.Add(type);
                }

                foreach (TypeOption type in AvailableTypes.ToList())
                {
                    if (ContainsTypeOption(type, types)) continue;

                    AvailableTypes.Remove(type);
                }
            }
            finally
            {
                _isUpdatingAvailableTypes = false;
            }
        }

        private static bool ContainsTypeOption(
            TypeOption type,
            IEnumerable<TypeOption> container)
        {
            return type.Item != null &&
                container.Any(availableType =>
                    ReferenceEquals(type.Item, availableType.Item));
        }

        private void SelectedTypeOption_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) &&
                e.PropertyName != nameof(TypeOption.Name)) return;

            OnPropertyChanged(nameof(Name));
            UpdateLayoutAndSize();
        }

        protected override double GetRequiredOwnWidth()
        {
            return CanvasLayout.GetRequiredTextWidth(Name, 0);
        }
    }
}