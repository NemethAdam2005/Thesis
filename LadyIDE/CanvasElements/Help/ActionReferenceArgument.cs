using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LadyIDE.WPF.CanvasElements.Help
{
    public class ActionReferenceArgument : INotifyPropertyChanged
    {
        private string _value = string.Empty;

        public Parameter Parameter { get; }

        public string Value
        {
            get => _value;
            set
            {
                if (_value == value) return;

                _value = value;
                OnPropertyChanged();
            }
        }

        public ActionReferenceArgument(Parameter parameter)
        {
            Parameter = parameter;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}