
using LadyIDE.WPF.CanvasElements.Help;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class Action : CanvasItemBase
    {
        private bool _pipeAndFilterAction;
        private Input? _inputElement;
        private Output? _outputElement;
        private Logic? _logicElement;

        public bool PipeAndFilterAction
        {
            get => _pipeAndFilterAction;
            set
            {
                if (_pipeAndFilterAction == value) return;

                _pipeAndFilterAction = value;
                OnPropertyChanged();
                UpdateLayoutAndSize();
            }
        }

        public Input? InputElement
        {
            get => _inputElement;
            private set
            {
                if (_inputElement == value) return;

                _inputElement = value;
                OnPropertyChanged();
            }
        }

        public Output? OutputElement
        {
            get => _outputElement;
            private set
            {
                if (_outputElement == value) return;

                _outputElement = value;
                OnPropertyChanged();
            }
        }

        public Logic? LogicElement
        {
            get => _logicElement;
            private set
            {
                if (_logicElement == value) return;

                _logicElement = value;
                OnPropertyChanged();
            }
        }

        public Action() : base()
        {
            Children.CollectionChanged += (_, _) => RefreshChildren();
            RefreshChildren();
        }

        private void RefreshChildren()
        {
            InputElement = Children.OfType<Input>().FirstOrDefault();
            OutputElement = Children.OfType<Output>().FirstOrDefault();
            LogicElement = Children.OfType<Logic>().FirstOrDefault();
        }

        protected override double GetRequiredOwnWidth()
        {
            double requiredWidth = base.GetRequiredOwnWidth();

            if (PipeAndFilterAction) requiredWidth += 45;

            return requiredWidth;
        }

        protected override void ArrangeChildren()
        {
            CanvasLayout.ArrangeVertically(Children);
        }
    }
}
