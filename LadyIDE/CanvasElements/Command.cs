using System.Collections.Generic;

namespace LadyIDE.WPF.CanvasElements
{
    public class Command : CanvasItemBase
    {
        public Command() : base()
        {
        }

        public List<Parameter> Input { get; } = new();

        public string ActionName { get; set; } = string.Empty;
    }
}