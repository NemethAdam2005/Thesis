using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class TypePair : CanvasItemBase
    {
        private CanvasItemBase? _type1;
        private CanvasItemBase? _type2;

        public ObservableCollection<CanvasItemBase> AvailableTypes { get; } = new();

        public CanvasItemBase? Type1
        {
            get => _type1;
            set => SetType(ref _type1, value, nameof(Type1));
        }

        public CanvasItemBase? Type2
        {
            get => _type2;
            set => SetType(ref _type2, value, nameof(Type2));
        }

        public string Type1DisplayName => GetTypeDisplayName(Type1);

        public string Type2DisplayName => GetTypeDisplayName(Type2);

        public override string Name
        {
            get => $"({Type1DisplayName}, {Type2DisplayName})";
            set { }
        }

        public TypePair() : base(230, 43)
        {
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> availableTypes)
        {
            CanvasItemBase[] supportedTypes = availableTypes
                .Where(IsSupportedType)
                .Distinct()
                .ToArray();

            foreach (CanvasItemBase item in AvailableTypes.ToList())
                if (!supportedTypes.Contains(item))
                    AvailableTypes.Remove(item);

            foreach (CanvasItemBase item in supportedTypes)
                if (!AvailableTypes.Contains(item))
                    AvailableTypes.Add(item);

            if (Type1 != null && !supportedTypes.Contains(Type1)) Type1 = null;
            if (Type2 != null && !supportedTypes.Contains(Type2)) Type2 = null;
        }

        private void SetType(ref CanvasItemBase? field, CanvasItemBase? value, string propertyName)
        {
            if (value != null && !IsSupportedType(value)) return;
            if (field == value) return;

            if (field != null) field.PropertyChanged -= ReferencedType_PropertyChanged;

            field = value;

            if (field != null) field.PropertyChanged += ReferencedType_PropertyChanged;

            OnPropertyChanged(propertyName);
            OnPropertyChanged(nameof(Type1DisplayName));
            OnPropertyChanged(nameof(Type2DisplayName));
            OnPropertyChanged(nameof(Name));

            UpdateLayoutAndSize();
        }

        private static bool IsSupportedType(CanvasItemBase item)
        {
            return item is Entity or Record or Proxy;
        }

        private static string GetTypeDisplayName(CanvasItemBase? item)
        {
            return item switch
            {
                Entity entity => $"Entity: {entity.Name}",
                Record record => $"Record: {record.Name}",
                Proxy proxy => $"Proxy: {proxy.Name}",
                _ => "Not selected"
            };
        }

        private void ReferencedType_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) &&
                e.PropertyName != nameof(CanvasItemBase.Name)) return;

            OnPropertyChanged(nameof(Type1DisplayName));
            OnPropertyChanged(nameof(Type2DisplayName));
            OnPropertyChanged(nameof(Name));

            UpdateLayoutAndSize();
        }

        protected override double GetRequiredOwnWidth()
        {
            double type1Width = Type1DisplayName.Length * CanvasLayout.CharacterWidth;
            double type2Width = Type2DisplayName.Length * CanvasLayout.CharacterWidth;

            return Math.Max(
                MinimumWidth - 10,
                type1Width + type2Width + 55);
        }

    }
}