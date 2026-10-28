
using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Field : CanvasItemBase
    {
        private bool _isLazy;
        private TypeOption? _selectedTypeOption;

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
                UpdateLayoutAndSize();
            }
        }

        public bool IsLazy
        {
            get => _isLazy;
            set
            {
                if (_isLazy == value) return;

                _isLazy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowLazyIndicator));
                UpdateLayoutAndSize();
            }
        }

        public bool ShowLazyIndicator => IsLazy;

        public Field() : base(160, 43)
        {
            TypeOption stringType = new("String", true);
            AvailableTypes.Add(stringType);
            SelectedTypeOption = stringType;
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> allItems)
        {
            List<TypeOption> types = TypeOptionProvider.RequiredTypes(
                allItems,
                primitive: true,
                record: true,
                entity: true,
                enumElement: true);

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

            UpdateLayoutAndSize();
        }

        protected override double GetRequiredOwnWidth()
        {
            string headerText = SelectedTypeOption == null
                ? Name
                : $"{Name} - {SelectedTypeOption.Name}";

            double indicatorWidth = ShowLazyIndicator ? 12 : 0;

            return CanvasLayout.GetRequiredTextWidth(
                headerText,
                CanvasLayout.HeaderChromeWidth + indicatorWidth);
        }
    }
}
