namespace LadyIDE.WPF.CanvasElements
{
    public class Command : CanvasItemBase
    {
        public Command() : base()
        {
        }

        public string ActionName { get; set; } = string.Empty;
    }
}