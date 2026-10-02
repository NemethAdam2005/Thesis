using System.Collections.Generic;

namespace LadyIDE.WPF.CanvasElements
{
    public class Command : CanvasItemBase
    {
        public Command() : base(200, 120, 120)
        {
        }

        public List<Parameter> Input { get; } = new();

        public string Action { get; set; } = string.Empty;
    }
}