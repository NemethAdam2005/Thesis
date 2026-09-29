using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace LadyIDE.WPF.CanvasElements
{
    public class CanvasItemBase : INotifyPropertyChanged
    {
        // RichCanvas maps these directly without needing extra ItemContainerStyles

        private double _x;
        public double X
        {
            get => _x;
            set => _x = value;
        }

        private double _y;
        public double Y
        {
            get => _y;
            set => _y = value;
        }

        private double _width;
        public double Width
        {
            get => _width;
            set => _width = value;
        }

        private double _height;
        public double Height
        {
            get => _height;
            set => _height = value;
        }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Tulajdonság változása ellenőrzéssel.
        /// </summary>
        /// <param name="propertyName">Tulajdonság neve.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
