using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Base : CanvasItemBase
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

                Name = _selectedTypeOption?.Name ?? string.Empty;

                OnPropertyChanged();
                UpdateLayoutAndSize();

                if (!_isUpdatingAvailableTypes)
                    (Parent as BaseList)?.UpdateAvailableBases();
            }
        }

        public Base() : base(150, 43)
        {
        }

        public override string Name
        {
            get => SelectedTypeOption?.Name ?? "Not selected";
            set { }
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> allItems)
        {
            List<CanvasItemBase> items = allItems.ToList();

            CanvasItemBase? owner = Parent switch
            {
                BaseList baseList => baseList.Parent,
                EnumElement enumElement => enumElement,
                _ => null
            };

            _isUpdatingAvailableTypes = true;

            try
            {
                if (owner is Record)
                    _allAvailableTypes = TypeOptionProvider.RequiredTypes(
                        items,
                        record: true,
                        exclude: owner);
                else if (owner is Entity)
                    _allAvailableTypes = TypeOptionProvider.RequiredTypes(
                        items,
                        entity: true,
                        exclude: owner);
                else if (owner is EnumElement)
                    _allAvailableTypes = TypeOptionProvider.RequiredTypes(
                        items,
                        enumElement: true,
                        exclude: owner);
                else
                    _allAvailableTypes.Clear();

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

            Name = SelectedTypeOption?.Name ?? string.Empty;
            UpdateLayoutAndSize();
        }

        protected override double GetRequiredOwnWidth()
        {
            return CanvasLayout.GetRequiredTextWidth(Name, 0);
        }
    }
}