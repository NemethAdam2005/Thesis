using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LadyIDE.WPF.CanvasElements
{
    public class ConsoleApplication : CanvasItemBase
    {
        private string _name = string.Empty;

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
    }
}
