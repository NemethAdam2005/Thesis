
using System.ComponentModel;

namespace LadyIDE.WPF.CanvasElements
{
    public class ActionReference : CanvasItemBase
    {
        private Action? _action;

        public Action? Action
        {
            get => _action;
            set
            {
                if (_action == value) return;

                if (_action != null)
                    _action.PropertyChanged -= Action_PropertyChanged;

                _action = value;

                if (_action != null)
                    _action.PropertyChanged += Action_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
                UpdateLayoutAndSize();
            }
        }

        public override string Name
        {
            get => Action?.Name ?? string.Empty;
            set { }
        }

        public ActionReference() : base(300, 43)
        {
        }

        private void Action_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(CanvasItemBase.Name))
            {
                OnPropertyChanged(nameof(Name));
                UpdateLayoutAndSize();
            }
        }
    }
}
