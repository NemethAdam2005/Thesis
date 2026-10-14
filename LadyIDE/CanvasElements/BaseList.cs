using System;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class BaseList : CanvasItemBase
    {
        public BaseList() : base(180, 100)
        {
        }

        protected override void ArrangeChildren()
        {
            const double padding = 8;
            const double spacing = 6;

            double y = padding;

            foreach (Base item in Children.OfType<Base>())
            {
                item.X = padding;
                item.Y = y;
                y += item.Height + spacing;
            }
        }

        protected override double GetHeaderWidth()
        {
            const double padding = 6;

            double requiredWidth = 0;

            foreach (CanvasItemBase child in Children)
                requiredWidth = Math.Max(requiredWidth, child.Width);

            return Math.Max(DefaultWidth, requiredWidth + padding * 2);
        }
    }
}