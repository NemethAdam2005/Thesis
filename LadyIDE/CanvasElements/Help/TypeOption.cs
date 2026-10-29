using System.ComponentModel;

namespace LadyIDE.WPF.CanvasElements.Help
{
    public class TypeOption : INotifyPropertyChanged
    {
        private readonly string? _primitiveName;

        public CanvasItemBase? Item { get; }
        public bool IsPrimitive { get; }

        public string Name => Item switch
        {
            Entity entity => entity.Name ?? "Not selected",
            Record record => record.Name ?? "Not selected",
            EnumElement enumElement => enumElement.Name ?? "Not selected",
            _ => Item?.Name ?? _primitiveName ?? "Not selected"
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
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(CanvasItemBase.Name))
            {
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(Name)));
            }
        }
    }
}