namespace LadyIDE.WPF.CanvasElements
{
    public class TypePairs : CanvasItemBase
    {
        public TypePairs() : base(340, 100)
        {
            Name = "TypePairs";
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeVertically(Children);
        }
    }
}
