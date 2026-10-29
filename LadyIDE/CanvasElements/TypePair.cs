
using LadyIDE.WPF.CanvasElements.Help;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class TypePair : CanvasItemBase
    {
        private TypeOption? _selectedType1Option;
        private TypeOption? _selectedType2Option;

        public ObservableCollection<TypeOption> AvailableTypes { get; } = new();

        public TypeOption? SelectedType1Option
        {
            get => _selectedType1Option;
            set
            {
                if (ReferenceEquals(_selectedType1Option, value)) return;

                if (_selectedType1Option != null)
                    _selectedType1Option.PropertyChanged -= SelectedTypeOption_PropertyChanged;

                _selectedType1Option = value;

                if (_selectedType1Option != null)
                    _selectedType1Option.PropertyChanged += SelectedTypeOption_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
                UpdateLayoutAndSize();
            }
        }

        public TypeOption? SelectedType2Option
        {
            get => _selectedType2Option;
            set
            {
                if (ReferenceEquals(_selectedType2Option, value)) return;

                if (_selectedType2Option != null)
                    _selectedType2Option.PropertyChanged -= SelectedTypeOption_PropertyChanged;

                _selectedType2Option = value;

                if (_selectedType2Option != null)
                    _selectedType2Option.PropertyChanged += SelectedTypeOption_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
                UpdateLayoutAndSize();
            }
        }

        public TypePair() : base(230, 43)
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

                bool isSelectedType1 = ReferenceEquals(SelectedType1Option, type);
                bool isSelectedType2 = ReferenceEquals(SelectedType2Option, type);

                AvailableTypes.Remove(type);

                if (isSelectedType1)
                    SelectedType1Option = null;

                if (isSelectedType2)
                    SelectedType2Option = null;
            }
        }

        private static bool ContainsTypeOption(
            TypeOption type,
            IEnumerable<TypeOption> container)
        {
            return container.Any(availableType =>
                ReferenceEquals(availableType.Item, type.Item));
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

        public override string Name
        {
            get => $"{SelectedType1Option?.Name ?? "Not selected"}, {SelectedType2Option?.Name ?? "Not selected"}";
            set { }
        }

        protected override double GetRequiredOwnWidth()
        {
            double type1Width = (SelectedType1Option?.Name.Length ?? 0) * CanvasLayout.CharacterWidth;
            double type2Width = (SelectedType2Option?.Name.Length ?? 0) * CanvasLayout.CharacterWidth;

            return Math.Max(MinimumWidth - 10, type1Width + type2Width + 55);
        }
    }
}
