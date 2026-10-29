using LadyIDE.WPF.CanvasElements.Help;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements
{
    public class ActionReference : CanvasItemBase
    {
        private TypeOption? _selectedTypeOption;
        private Action? _observedAction;
        private Input? _observedInput;

        public ObservableCollection<TypeOption> AvailableTypes { get; } = new();

        public ObservableCollection<ActionReferenceArgument> Arguments { get; } = new();

        public TypeOption? SelectedTypeOption
        {
            get => _selectedTypeOption;
            set
            {
                if (ReferenceEquals(_selectedTypeOption, value)) return;

                if (_selectedTypeOption != null)
                    _selectedTypeOption.PropertyChanged -= SelectedTypeOption_PropertyChanged;

                if (_observedAction != null)
                    _observedAction.PropertyChanged -= Action_PropertyChanged;

                UnsubscribeFromInput();

                _selectedTypeOption = value;

                if (_selectedTypeOption != null)
                    _selectedTypeOption.PropertyChanged += SelectedTypeOption_PropertyChanged;

                _observedAction = _selectedTypeOption?.Item as Action;

                if (_observedAction != null)
                    _observedAction.PropertyChanged += Action_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ReferencedAction));
                OnPropertyChanged(nameof(Name));

                SubscribeToInput();
                RefreshArguments();

                UpdateLayoutAndSize();
                Parent?.UpdateLayoutAndSize();
            }
        }

        public Action? ReferencedAction => SelectedTypeOption?.Item as Action;

        public override string Name
        {
            get => SelectedTypeOption?.Name ?? "Not selected";
            set { }
        }

        public ActionReference() : base(300, 43)
        {
        }

        public void RefreshAvailableTypes(IEnumerable<CanvasItemBase> allItems)
        {
            List<Action> actions = allItems
                .OfType<Action>()
                .ToList();

            foreach (Action action in actions)
            {
                if (AvailableTypes.Any(type => ReferenceEquals(type.Item, action))) continue;

                AvailableTypes.Add(new TypeOption(action));
            }

            foreach (TypeOption type in AvailableTypes.ToList())
            {
                if (type.Item is Action action &&
                    actions.Any(current => ReferenceEquals(current, action))) continue;

                if (ReferenceEquals(SelectedTypeOption, type))
                    SelectedTypeOption = null;

                AvailableTypes.Remove(type);
            }
        }

        private void SelectedTypeOption_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) &&
                e.PropertyName != nameof(TypeOption.Name)) return;

            OnPropertyChanged(nameof(Name));
            UpdateLayoutAndSize();
            Parent?.UpdateLayoutAndSize();
        }

        private void Action_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) ||
                e.PropertyName == nameof(CanvasItemBase.Name))
            {
                OnPropertyChanged(nameof(Name));
                UpdateLayoutAndSize();
                Parent?.UpdateLayoutAndSize();
            }

            if (string.IsNullOrEmpty(e.PropertyName) ||
                e.PropertyName == nameof(Action.InputElement))
            {
                SubscribeToInput();
                RefreshArguments();
            }
        }

        private void SubscribeToInput()
        {
            UnsubscribeFromInput();

            _observedInput = ReferencedAction?.InputElement;

            if (_observedInput != null)
                _observedInput.Children.CollectionChanged += Input_Children_CollectionChanged;
        }

        private void UnsubscribeFromInput()
        {
            if (_observedInput == null) return;

            _observedInput.Children.CollectionChanged -= Input_Children_CollectionChanged;
            _observedInput = null;
        }

        private void Input_Children_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            RefreshArguments();
        }

        private void RefreshArguments()
        {
            List<Parameter> parameters = ReferencedAction?.InputElement?.Children
                .OfType<Parameter>()
                .ToList() ?? new List<Parameter>();

            foreach (ActionReferenceArgument argument in Arguments.ToList())
            {
                if (!parameters.Contains(argument.Parameter))
                    Arguments.Remove(argument);
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                Parameter parameter = parameters[i];

                int currentIndex = Arguments
                    .ToList()
                    .FindIndex(argument => ReferenceEquals(argument.Parameter, parameter));

                if (currentIndex == -1)
                {
                    Arguments.Insert(i, new ActionReferenceArgument(parameter));
                }
                else if (currentIndex != i)
                {
                    Arguments.Move(currentIndex, i);
                }
            }
        }
    }
}