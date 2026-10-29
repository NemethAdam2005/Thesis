
using LadyIDE.WPF.CanvasElements.Help;

namespace LadyIDE.WPF.CanvasElements
{
    public class Input : CanvasItemBase
    {
        public Input() : base(300, 110)
        {
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeVertically(Children);
        }
    }
}
