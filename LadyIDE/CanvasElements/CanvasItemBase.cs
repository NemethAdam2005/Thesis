using System;
using System.Collections.Generic;
using System.Text;

namespace LadyIDE.WPF.CanvasElements
{
     public class CanvasItemBase
    {
        public string Name { get; set; } = string.Empty;

        public double Top { get; set; }

        public double Left { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
