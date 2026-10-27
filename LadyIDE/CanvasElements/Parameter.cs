
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Parameter : CanvasItemBase
    {
        public ObservableCollection<TypeOption> AvailableTypes { get; } = new();

        private TypeOption? _selectedTypeOption;

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
                UpdateLayoutAndSize();
            }
        }

        public Parameter() : base(200, 43)
        {
            TypeOption stringType = new("String", true);
            AvailableTypes.Add(stringType);
            SelectedTypeOption = stringType;
        }

        private void SelectedTypeOption_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(TypeOption.Name)) return;

            UpdateLayoutAndSize();
        }

        public void RefreshTypeOptions(IEnumerable<CanvasItemBase> allItems)
        {
            List<TypeOption> types = TypeOptionProvider.RequiredTypes(
                allItems,
                primitive: true,
                record: true,
                entity: true,
                enumElement: true,
                proxy: true);

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
        
        private static bool ContainsTypeOption(TypeOption type,IEnumerable<TypeOption> container)
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

        protected override double GetRequiredOwnWidth()
        {
            return base.GetRequiredOwnWidth() +
                   (SelectedTypeOption?.Name.Length ?? 0) * 7;
        }
    }
}
