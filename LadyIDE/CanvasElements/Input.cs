namespace LadyIDE.WPF.CanvasElements
{
    public class Input : CanvasItemBase
    {
        public Input() : base(300,110)
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

        protected override double GetHeaderWidth()
        {
            const double padding = 12;
            double childrenWidth = 0;

            foreach (CanvasItemBase child in Children)
                childrenWidth = Math.Max(childrenWidth, child.Width);

            return Math.Max(100, childrenWidth + padding * 2);
        }
    }
}