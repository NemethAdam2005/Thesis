
using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Type : CanvasItemBase
    {
        private TypeOption? _selectedTypeOption;

        public ObservableCollection<TypeOption> AvailableTypes { get; } = new();

        public TypeOption? SelectedTypeOption
        {
            get => _selectedTypeOption;
            set
            {
                if (ReferenceEquals(_selectedTypeOption, value)) return;
                if (value != null && value.Item is not Record) return;

                if (_selectedTypeOption != null)
                    _selectedTypeOption.PropertyChanged -= SelectedTypeOption_PropertyChanged;

                _selectedTypeOption = value;

                if (_selectedTypeOption != null)
                    _selectedTypeOption.PropertyChanged += SelectedTypeOption_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
                UpdateLayoutAndSize();
            }
        }

        public override string Name
        {
            get => SelectedTypeOption?.Name ?? "Not selected";
            set { }
        }

        public Type() : base(180, 43)
        {
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> allItems)
        {
            List<TypeOption> types = TypeOptionProvider.RequiredTypes(
                allItems,
                record: true);

            foreach (TypeOption type in types)
            {
                if (!ContainsTypeOption(type, AvailableTypes))
                    AvailableTypes.Add(type);
            }

            foreach (TypeOption type in AvailableTypes.ToList())
            {
                if (ContainsTypeOption(type, types)) continue;

                if (ReferenceEquals(SelectedTypeOption, type))
                    SelectedTypeOption = null;

                AvailableTypes.Remove(type);
            }
        }

        private static bool ContainsTypeOption(
            TypeOption type,
            IEnumerable<TypeOption> container)
        {
            foreach (TypeOption availableType in container)
            {
                if (type.Item != null || availableType.Item != null)
                {
                    if (type.Item != null &&
                        ReferenceEquals(type.Item, availableType.Item))
                        return true;

                    continue;
                }

                if (type.IsPrimitive == availableType.IsPrimitive &&
                    type.Name == availableType.Name)
                    return true;
            }

            return false;
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
            return CanvasLayout.GetRequiredTextWidth(Name, 100);
        }
    }
}
