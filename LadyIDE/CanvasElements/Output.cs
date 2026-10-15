
namespace LadyIDE.WPF.CanvasElements
{
    public class Output : CanvasItemBase
    {
        public Output() : base(300, 110)
        {
        }

        protected override void ArrangeChildren()
        {
            const double padding = 6;
            const double spacing = 2;

            double y = padding;

            foreach (CanvasItemBase child in Children)
            {
                child.X = padding;
                child.Y = y;
                y += child.Height + spacing;
            }
        }
    }
}
