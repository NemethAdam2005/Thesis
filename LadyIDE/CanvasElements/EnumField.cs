namespace LadyIDE.WPF.CanvasElements
{
    public class EnumField : CanvasItemBase
    {
        public EnumField() : base(150, 43)
        {
        }

        protected override double GetHeaderWidth()
        {
            const double padding = 30;

            return Math.Max(150, Name.Length * 8 + padding);
        }
    }
}