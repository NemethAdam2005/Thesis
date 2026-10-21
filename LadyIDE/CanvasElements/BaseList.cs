
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
            CanvasLayout.ArrangeVertically(Children.OfType<Base>());
        }
    }
}
