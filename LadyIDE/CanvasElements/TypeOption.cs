using System.ComponentModel;

namespace LadyIDE.WPF.CanvasElements
{
    public class TypeOption : INotifyPropertyChanged
    {
        private readonly string? _primitiveName;

        public CanvasItemBase? Item { get; }
        public bool IsPrimitive { get; }

        public string Name => Item switch
        {
            Entity entity => entity.Name,
            Record record => record.Name,
            EnumElement enumElement => enumElement.Name,
            Proxy proxy => proxy.Name,
            _ => Item?.Name ?? _primitiveName ?? string.Empty
        };

        public event PropertyChangedEventHandler? PropertyChanged;

        public TypeOption(string name, bool isPrimitive = false)
        {
            _primitiveName = name;
            IsPrimitive = isPrimitive;
        }

        public TypeOption(CanvasItemBase item)
        {
            Item = item;

            if (item is INotifyPropertyChanged observableItem)
                observableItem.PropertyChanged += Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) ||
                e.PropertyName == nameof(CanvasItemBase.Name))
            {
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public override string ToString() => Name;
    }
}