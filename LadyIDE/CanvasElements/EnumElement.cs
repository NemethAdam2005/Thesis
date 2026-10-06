using System;
using System.Collections.Generic;
using System.Text;

namespace LadyIDE.WPF.CanvasElements
{
    public class EnumElement : CanvasItemBase
    {
        public List<string> Fields { get; } = new List<string>();
        public EnumElement() : base()
        {
            
        }
    }
}
