using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text;

namespace LadyIDE.WPF.CanvasElements
{
    public class Command : CanvasItemBase
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

        public List<Parameter> Input { get; } = new();

        public string Action { get; set; } = string.Empty;
    }
}
