namespace LadyIDE.WPF.CanvasElements
{
    public class TypePairs : CanvasItemBase
    {
        public TypePairs() : base()
        {
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeVertically(Children);
        }
    }
}