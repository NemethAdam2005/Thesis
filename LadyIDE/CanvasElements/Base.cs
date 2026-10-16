
namespace LadyIDE.WPF.CanvasElements
{
    public class Base : CanvasItemBase
    {
        public Base() : base(150, 43)
        {
        }

        protected override double GetRequiredOwnWidth()
        {
            return CanvasLayout.GetRequiredTextWidth(Name, 0);
        }
    }
}
