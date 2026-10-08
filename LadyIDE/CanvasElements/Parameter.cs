using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LadyIDE.WPF.CanvasElements
{
    public class Parameter : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _type = string.Empty;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (_type == value)
                    return;

                _type = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}