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

        public override string Name
        {
            get => $"({GetTypeName(Type1)}, {GetTypeName(Type2)})";
            set { }
        }

        public TypePair() : base(360, 48)
        {
        }

        public void RefreshAvailableTypes(System.Collections.Generic.IEnumerable<CanvasItemBase> availableTypes)
        {
            CanvasItemBase[] supportedTypes = availableTypes
                .Where(IsSupportedType)
                .Distinct()
                .ToArray();

            foreach (CanvasItemBase availableType in AvailableTypes.ToList())
                if (!supportedTypes.Contains(availableType))
                    AvailableTypes.Remove(availableType);

            foreach (CanvasItemBase availableType in supportedTypes)
                if (!AvailableTypes.Contains(availableType))
                    AvailableTypes.Add(availableType);

            if (Type1 != null && !supportedTypes.Contains(Type1)) Type1 = null;
            if (Type2 != null && !supportedTypes.Contains(Type2)) Type2 = null;
        }

        private void SetType(ref CanvasItemBase? field, CanvasItemBase? value, string propertyName)
        {
            if (value != null && !IsSupportedType(value)) return;
            if (field == value) return;

            if (field != null) field.PropertyChanged -= Type_PropertyChanged;

            field = value;

            if (field != null) field.PropertyChanged += Type_PropertyChanged;

            OnPropertyChanged(propertyName);
            OnPropertyChanged(nameof(Name));
            UpdateLayoutAndSize();
        }

        private static bool IsSupportedType(CanvasItemBase item)
        {
            return item is Entity or Record or Proxy;
        }

        private static string GetTypeName(CanvasItemBase? item)
        {
            return item switch
            {
                Entity entity => $"Entity: {entity.Name}",
                Record record => $"Record: {record.Name}",
                Proxy proxy => $"Proxy: {proxy.Name}",
                _ => "Not selected"
            };
        }

        private void Type_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) && e.PropertyName != nameof(Name)) return;

            OnPropertyChanged(nameof(Name));
            UpdateLayoutAndSize();
        }
    }
}
