
using LadyIDE.WPF.CanvasElements.Help;

namespace LadyIDE.WPF.CanvasElements
{
    public class EnumField : CanvasItemBase
    {
        public EnumField() : base(150, 43)
        {
        }

        protected override double GetRequiredOwnWidth()
        {
            return CanvasLayout.GetRequiredTextWidth(Name, 30);
        }
    }
}
