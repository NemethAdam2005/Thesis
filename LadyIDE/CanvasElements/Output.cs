
namespace LadyIDE.WPF.CanvasElements
{
    public class Output : CanvasItemBase
    {
        public Output() : base(300, 110)
        {
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeVertically(Children);
        }
    }
}
