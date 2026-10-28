using LadyIDE.Model;
using LadyIDE.WPF.CanvasElements;
using LadyIDE.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Common;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;

namespace LadyIDE.WPF.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Fields and Properties
        private readonly ApplicationModel _model;
        private readonly LogicEditorDialogService _logicEditorDialogService = new();
        private readonly LogicEditorViewModel _logicEditorViewModel;

        private string _code = string.Empty;
        public string Code 
        { 
            get => _code; 
            private set 
            { 
                if (_code == value) 
                    return; 
                _code = value; 
                OnPropertyChanged();
                HasUnsavedChanges = true;
            } 
        }

        private string _consoleText = string.Empty;
        public string ConsoleText
        {
            get => _consoleText;
            private set
            {
                if (_consoleText == value)
                    return;
                _consoleText = value;
                OnPropertyChanged();
            }
        }


        private bool _isLightMode = false;

        public bool IsLightMode
        {
            get => _isLightMode;

            set
            {
                if (_isLightMode == value)
                    return;

                _isLightMode = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ColorThemeText));
            }
        }

        public string ColorThemeText =>
            IsLightMode ? "Dark mode" : "Light mode";


        private bool _isLadyElementsVisible = true;

        public bool IsLadyElementsVisible
        {
            get => _isLadyElementsVisible;

            set
            {
                if (_isLadyElementsVisible == value)
                    return;

                _isLadyElementsVisible = value;

                LadyElementsColumnWidth = value
                    ? new GridLength(205)
                    : new GridLength(0);

                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowLadyElementsText));
            }
        }

        public string ShowLadyElementsText =>
            IsLadyElementsVisible
                ? "Hide Lady elements"
                : "Show Lady elements";


        private GridLength _ladyElementsColumnWidth =
            new GridLength(205);

        public GridLength LadyElementsColumnWidth
        {
            get => _ladyElementsColumnWidth;

            private set
            {
                if (_ladyElementsColumnWidth == value)
                    return;

                _ladyElementsColumnWidth = value;

                OnPropertyChanged();
            }
        }

        private bool _isPropertiesVisible = true;

        public bool IsPropertiesVisible
        {
            get => _isPropertiesVisible;
            set
            {
                if (_isPropertiesVisible == value)
                    return;

                _isPropertiesVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PropertiesColumnWidth));
                OnPropertyChanged(nameof(ShowPropertiesText));
            }
        }

        public GridLength PropertiesColumnWidth =>
            IsPropertiesVisible
                ? new GridLength(245)
                : new GridLength(0);

        public string ShowPropertiesText =>
            IsPropertiesVisible
                ? "Hide Properties section"
                : "Show Properties section";

        private string _caretPositionText = "  Ln: 1, Col: 1";

        public string CaretPositionText
        {
            get => _caretPositionText;
            set
            {
                if (_caretPositionText == value)
                    return;

                _caretPositionText = value;
                OnPropertyChanged();
            }
        }

        private string? _currentFilePath;
        public string? CurrentFilePath
        {
            get => _currentFilePath;
            set
            {
                if (_currentFilePath == value)
                    return;
                _currentFilePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CodeTabHeader));
            }
        }

        private bool _hasUnsavedChanges = false;

        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set
            {
                if (_hasUnsavedChanges == value)
                    return;
                _hasUnsavedChanges = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CodeTabHeader));
            }
        }

        public string CodeTabHeader
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentFilePath))
                {
                    return "Code";
                }

                return $"{Path.GetFileName(CurrentFilePath)}{(HasUnsavedChanges ? " *" : "")}";
            }
        }

        private CanvasItemBase? _selectedItem;
        public CanvasItemBase? SelectedItem
        {
            get => _selectedItem;
            private set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> CompositeTypes { get; } = new()
        {
            "1-N",
            "N-N"
        };

        public ObservableCollection<CanvasElements.Action> AvailableActions { get; } = new();

        public ObservableCollection<CanvasItemBase> CanvasItems { get; }
        public ObservableCollection<CanvasItemBase> SelectedItems { get; }
        #endregion

        #region Constructor
        public MainViewModel(ApplicationModel model)
        {
            CanvasItems = new ObservableCollection<CanvasItemBase>();
            SelectedItems = new ObservableCollection<CanvasItemBase>();

            _logicEditorViewModel = new LogicEditorViewModel();

            _model = model;
            _model.CodeChanged += new EventHandler<string>(Model_CodeChanged);
            _model.FileLoaded += new EventHandler<(string,string)>(Model_FileLoaded);
            _model.FileSaved += new EventHandler<string>(Model_FileSaved);

            EditLogicCommand = new DelegateCommand(param => OnEditLogic(param as Logic));
            LoadFileCommand = new DelegateCommand(param => OnLoadFile());
            SaveAsFileCommand = new DelegateCommand(param => OnSaveAsFile());
            SaveFileCommand = new DelegateCommand(param => OnSaveFile());
            ColorThemeButtonClicked = new DelegateCommand(param => OnColorThemeButtonClicked());
            ShowLadyElementsButtonClicked = new DelegateCommand(param => OnShowLadyElementsButtonClicked());
            ShowPropertiesButtonClicked = new DelegateCommand(param => OnShowPropertiesButtonClicked());
            ApplicationButtonClicked = new DelegateCommand(param => OnApplicationButtonClicked());
            CommandButtonClicked = new DelegateCommand(param => OnCommandButtonClicked());
            SelectItemCommand = new DelegateCommand(param => OnSelectItem(param as CanvasItemBase));
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            AddCommandToApplicationCommand = new DelegateCommand(param => OnAddCommandToApplication(param as ConsoleApplication));
            DeleteItemCommand = new DelegateCommand(param => OnDeleteItem(param as CanvasItemBase));
            DeleteSelectedItemsCommand = new DelegateCommand(param => OnDeleteSelectedItems());
            ComponentButtonClicked = new DelegateCommand(param => OnComponentButtonClicked());
            AddRecordToComponentCommand = new DelegateCommand(param => OnAddRecordToComponent(param as Component));
            AddActionToComponentCommand = new DelegateCommand(param => OnAddActionToComponent(param as Component));
            AddTypePairsToMediatorCommand = new DelegateCommand(param => IsAddTypePairsButtonEnabled(param as Mediator),param => OnAddTypePairsToMediator(param as Mediator));
            AddTypePairToTypePairsCommand = new DelegateCommand(param => OnAddTypePairToTypePairs(param as TypePairs));
            RecordButtonClicked = new DelegateCommand(param => OnRecordButtonClicked());
            EntityButtonClicked = new DelegateCommand(param => OnEntityButtonClicked());
            EnumButtonClicked = new DelegateCommand(param => OnEnumButtonClicked());
            CompositeButtonClicked = new DelegateCommand(param => OnCompositeButtonClicked());
            MediatorButtonClicked = new DelegateCommand(param => OnMediatorButtonClicked());
            ProxyButtonClicked = new DelegateCommand(param => OnProxyButtonClicked());
            AddFieldToProxyCommand = new DelegateCommand(param => OnAddFieldToProxy(param as Proxy));
            AddTypeToProxyCommand = new DelegateCommand(param => IsAddTypeToProxyButtonEnabled(param as Proxy),param => OnAddTypeToProxy(param as Proxy));
            AddInputToCommandCommand = new DelegateCommand(param => IsAddInputButtonEnabled(param as Command),param => OnAddInputToCommand(param as Command));
            AddParameterToInputCommand = new DelegateCommand(param => OnAddParameterToInput(param as Input));
            AddFieldToRecordCommand = new DelegateCommand(param => OnAddFieldToRecord(param as Record));
            AddBaseListToRecordCommand = new DelegateCommand(param => IsAddBaseListButtonEnabled(param as Record),param => OnAddBaseListToRecord(param as Record));
            AddBaseToBaseListCommand = new DelegateCommand(param => OnAddBaseToBaseList(param as BaseList));
            AddFieldToEntityCommand = new DelegateCommand(param => OnAddFieldToEntity(param as Entity));
            AddBaseListToEntityCommand = new DelegateCommand(param => IsAddBaseListButtonEnabled(param as Entity), param => OnAddBaseListToEntity(param as Entity));
            AddLogicToFieldCommand = new DelegateCommand(param => IsAddLogicButtonEnabled(param as Field), param => OnAddLogicToField(param as Field));
            AddBaseToEnumElementCommand = new DelegateCommand(param => IsAddBaseToEnumElementButtonEnabled(param as EnumElement),param => OnAddBaseToEnumElement(param as EnumElement));
            AddEnumFieldToEnumElementCommand = new DelegateCommand(param => OnAddEnumFieldToEnumElement(param as EnumElement));
            AddPartsToCompositeCommand = new DelegateCommand(param => IsAddPartsButtonEnabled(param as Composite),param => OnAddPartsToComposite(param as Composite));
            AddPartToPartsCommand = new DelegateCommand(param => OnAddPartToParts(param as Parts));
            AddInputToActionCommand = new DelegateCommand(param => IsAddInputToActionButtonEnabled(param as LadyIDE.WPF.CanvasElements.Action),param => OnAddInputToAction(param as LadyIDE.WPF.CanvasElements.Action));
            AddOutputToActionCommand = new DelegateCommand(param => IsAddOutputToActionButtonEnabled(param as LadyIDE.WPF.CanvasElements.Action),param => OnAddOutputToAction(param as LadyIDE.WPF.CanvasElements.Action));
            AddParameterToOutputCommand = new DelegateCommand(param => OnAddParameterToOutput(param as Output));
            AddLogicToActionCommand = new DelegateCommand(param => IsAddLogicToActionButtonEnabled(param as LadyIDE.WPF.CanvasElements.Action),param => OnAddLogicToAction(param as LadyIDE.WPF.CanvasElements.Action));
            AddActionReferenceToCommandCommand = new DelegateCommand(param => IsAddActionReferenceToCommandButtonEnabled(param as Command),param => OnAddActionReferenceToCommand(param as Command));
            FactoryButtonClicked = new DelegateCommand(param => OnFactoryButtonClicked());
            AddRecordToFactoryCommand = new DelegateCommand(param => OnAddRecordToFactory(param as Factory));
            AddActionToFactoryCommand = new DelegateCommand(param => OnAddActionToFactory(param as Factory));
            AddTypeToFactoryCommand = new DelegateCommand(param => IsAddTypeToFactoryButtonEnabled(param as Factory),param => OnAddTypeToFactory(param as Factory));
        }
        #endregion

        #region Commands
        public DelegateCommand EditLogicCommand { get; private set; }
        public DelegateCommand LoadFileCommand { get; private set; }
        public DelegateCommand SaveAsFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand ColorThemeButtonClicked { get; private set; }
        public DelegateCommand ShowLadyElementsButtonClicked { get; private set; }
        public DelegateCommand ShowPropertiesButtonClicked { get; private set; }
        public DelegateCommand SelectItemCommand { get; private set; }
        public DelegateCommand AddCommandToApplicationCommand { get; private set; }
        public DelegateCommand AddInputToCommandCommand { get; private set; }
        public DelegateCommand AddParameterToInputCommand { get; private set; }
        public DelegateCommand DeleteItemCommand { get; private set; }
        public DelegateCommand DeleteSelectedItemsCommand { get; private set; }
        public DelegateCommand ApplicationButtonClicked { get; private set; }
        public DelegateCommand CommandButtonClicked { get; private set; }
        public DelegateCommand AddActionReferenceToCommandCommand { get; private set; }
        public DelegateCommand ComponentButtonClicked { get; private set; }

        public DelegateCommand AddRecordToComponentCommand { get; }
        public DelegateCommand AddActionToComponentCommand { get; }
        public DelegateCommand AddTypePairsToMediatorCommand { get; private set; }
        public DelegateCommand AddTypePairToTypePairsCommand { get; private set; }

        public DelegateCommand RecordButtonClicked { get; private set; }
        public DelegateCommand AddFieldToRecordCommand { get; private set; }

        public DelegateCommand AddBaseListToRecordCommand { get; private set; }
        public DelegateCommand AddBaseToBaseListCommand { get; private set; }

        public DelegateCommand EntityButtonClicked { get; private set; }
        public DelegateCommand AddFieldToEntityCommand { get; private set; }
        public DelegateCommand AddBaseListToEntityCommand { get; private set; }
        public DelegateCommand AddLogicToFieldCommand { get; private set; }
        public DelegateCommand EnumButtonClicked { get; private set; }
        public DelegateCommand AddBaseToEnumElementCommand { get; private set; }
        public DelegateCommand AddEnumFieldToEnumElementCommand { get; private set; }
        public DelegateCommand CompositeButtonClicked { get; private set; }
        public DelegateCommand AddPartsToCompositeCommand { get; private set; }
        public DelegateCommand AddPartToPartsCommand { get; private set; }
        public DelegateCommand MediatorButtonClicked { get; private set; }
        public DelegateCommand ProxyButtonClicked { get; private set; }
        public DelegateCommand AddFieldToProxyCommand { get; private set; }
        public DelegateCommand AddTypeToProxyCommand { get; private set; }

        public DelegateCommand AddInputToActionCommand { get; private set; }
        public DelegateCommand AddOutputToActionCommand { get; private set; }
        public DelegateCommand AddParameterToOutputCommand { get; private set; }
        public DelegateCommand AddLogicToActionCommand { get; private set; }
        public DelegateCommand FactoryButtonClicked { get; private set; }

        public DelegateCommand AddRecordToFactoryCommand { get; private set; }

        public DelegateCommand AddActionToFactoryCommand { get; private set; }

        public DelegateCommand AddTypeToFactoryCommand { get; private set; }
        #endregion

        #region Events
        public event EventHandler? LoadFile;
        public event EventHandler? SaveAsFile;
        public event EventHandler? SaveFile;
        public event EventHandler<string>? CodeChanged;
        #endregion

        #region Model evenet handlers
        private void Model_CodeChanged(Object? sender, String e)
        {
            Code = e;
        }

        private void Model_FileLoaded(Object? sender, (String e, String path) args)
        {
            Code = args.e;
            CodeChanged?.Invoke(this, args.e);
            ConsoleText += $"[{DateTime.Now:HH:mm:ss}] File loaded successfully from {args.path}.\n";
        }

        private void Model_FileSaved(Object? sender, String e)
        { 
            ConsoleText += $"[{DateTime.Now:HH:mm:ss}] File saved successfully to {e}.\n";
        }
        #endregion

        #region Logic editor

        private void OnEditLogic(Logic? logic)
        {
            if (logic is not Logic) return;

            _logicEditorViewModel.LoadLogic(logic);
            _logicEditorDialogService.ShowDialog(_logicEditorViewModel);
        }

        #endregion

        #region Save and Load methods
        private void OnLoadFile()
        {
            LoadFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveFile()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
            {
                OnSaveAsFile();
            }
            else
            {
                SaveFile?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnSaveAsFile()
        {
            SaveAsFile?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Log methods
        private void LogAction(string message)
        {
            ConsoleText += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
        }

        private void LogDeletion(CanvasItemBase item, CanvasItemBase? parent)
        {
            string itemDescription = item.Name == String.Empty 
                ? $"{item.GetType().Name}"
                : $"{item.GetType().Name} '{item.Name}'";
            string location = parent == null
                ? "canvas"
                : parent.Name == String.Empty ? $"{parent.GetType().Name}" : $"{parent.GetType().Name} '{parent.Name}'";

            LogAction($"{itemDescription} deleted from {location}.");
        }

        private void LogAddition(CanvasItemBase item, CanvasItemBase? parent)
        {
            string itemDescription = item.Name == String.Empty
                ? $"{item.GetType().Name}"
                : $"{item.GetType().Name} '{item.Name}'";
            string location = parent == null
                ? "canvas"
                : parent.Name == String.Empty ? $"{parent.GetType().Name}" : $"{parent.GetType().Name} '{parent.Name}'";

            LogAction($"{itemDescription} added to {location}.");
        }
        #endregion

        #region UI event handlers

        #region Menu button click handlers
        private void OnColorThemeButtonClicked()
        {
            IsLightMode = !IsLightMode;

            ThemeManager.SetTheme(IsLightMode);
        }

        private void OnShowLadyElementsButtonClicked()
        {
            IsLadyElementsVisible = !IsLadyElementsVisible;
        }

        private void OnShowPropertiesButtonClicked()
        {
            IsPropertiesVisible = !IsPropertiesVisible;
        }

        public void UpdateCaretPosition(int line, int column)
        {
            CaretPositionText = $"  Ln: {line}, Col: {column}";
        }
        #endregion

        #region Delete methods
        private void OnDeleteItem(CanvasItemBase? item)
        {
            if (item == null)
                return;

            // Ha egy gyermekelemet törlünk
            if (item.Parent != null)
            {
                CanvasItemBase parent = item.Parent;

                parent.Children.Remove(item);
                LogDeletion(item, parent);

                if (parent is Command)
                    AddInputToCommandCommand.RaiseCanExecuteChanged();
                    AddActionReferenceToCommandCommand.RaiseCanExecuteChanged();

                if (parent is Record)
                    AddBaseListToRecordCommand.RaiseCanExecuteChanged();

                if (parent is Entity)
                    AddBaseListToEntityCommand.RaiseCanExecuteChanged();

                if (parent is Field)
                    AddLogicToFieldCommand.RaiseCanExecuteChanged();

                if (parent is EnumElement)
                    AddBaseToEnumElementCommand.RaiseCanExecuteChanged();

                if (parent is Composite)
                    AddPartsToCompositeCommand.RaiseCanExecuteChanged();

                if (parent is CanvasElements.Action)
                    AddInputToActionCommand.RaiseCanExecuteChanged();
                    AddOutputToActionCommand.RaiseCanExecuteChanged();
                    AddLogicToActionCommand.RaiseCanExecuteChanged();

                if (parent is Mediator)
                    AddTypePairsToMediatorCommand.RaiseCanExecuteChanged();

                if (parent is Proxy)
                    AddTypeToProxyCommand.RaiseCanExecuteChanged();

                if(parent is Factory)
                    AddTypeToFactoryCommand.RaiseCanExecuteChanged();

                item.Parent = null;
                item.IsHighlighted = false;
                SelectedItem = parent;
                parent.IsHighlighted = true;

                RefreshAllTypeOptions();
                return;
            }
            else if (CanvasItems.Contains(item))
            {

                CanvasItems.Remove(item);
                SelectedItems.Remove(item);
                LogDeletion(item, null);

                // Nincs már kijelölt elem
                SelectedItem = null;
                RefreshAllTypeOptions();
            }
        }

        private void OnDeleteSelectedItems()
        {
            if (SelectedItems == null || SelectedItems.Count == 0) return;

            List<CanvasItemBase> itemsToDelete = SelectedItems.ToList();

            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                foreach (CanvasItemBase item in itemsToDelete)
                {
                    if (item.Parent == null && CanvasItems.Remove(item))
                    {
                        LogDeletion(item,null);
                    }
                }

                SelectedItem = null;
                SelectedItems.Clear();

                RefreshAllTypeOptions();
            }));
        }

        private void RefreshAllTypeOptions()
        {
            List<CanvasItemBase> allItems = GetAllCanvasItems(CanvasItems).ToList();

            foreach (CanvasElements.Parameter parameter in allItems.OfType<CanvasElements.Parameter>())
                parameter.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            foreach (TypePair typePair in allItems.OfType<TypePair>())
                typePair.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            foreach (CanvasElements.Type type in allItems.OfType<CanvasElements.Type>())
                type.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            foreach (Field field in allItems.OfType<Field>())
                field.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            foreach (BaseList baseList in allItems.OfType<BaseList>())
                baseList.RefreshAvailableTypes(allItems);

            foreach (Parts parts in allItems.OfType<Parts>())
                parts.RefreshAvailableTypes(allItems);

            foreach (ActionReference actionReference in allItems.OfType<ActionReference>())
                actionReference.RefreshAvailableTypes(allItems);

            foreach (EnumElement enumElement in allItems.OfType<EnumElement>())
                enumElement.RefreshAvailableTypes(allItems);
        }

        private IEnumerable<CanvasItemBase> GetAllCanvasItems(IEnumerable<CanvasItemBase> items)
        {
            List<CanvasItemBase> allItems = new List<CanvasItemBase>(); 
            foreach (CanvasItemBase item in items)
            {
                allItems.Add(item);

                foreach (CanvasItemBase child in GetAllCanvasItems(item.Children))
                    allItems.Add(child);
            }
            return allItems;
        }

        #endregion

        #region Selection methods
        private void ClearHighlights(IEnumerable<CanvasItemBase> items)
        {
            foreach (CanvasItemBase item in items)
            {
                item.IsHighlighted = false;
                ClearHighlights(item.Children);
            }
        }

        private void OnSelectItem(CanvasItemBase? item)
        {
            if (item == null)
            {
                SelectedItem = null;
                ClearHighlights(CanvasItems);
                SelectedItems.Clear();
                return;
            }

            CanvasItemBase? root = item;

            while (root.Parent != null)
                root = root.Parent;

            foreach (CanvasItemBase rootItem in CanvasItems)
            {
                if (rootItem != root)
                    ClearHighlights(rootItem.Children);
            }

            ClearHighlights(root.Children);

            CanvasItemBase? current = item;

            while (current != null)
            {
                current.IsHighlighted = true;
                current = current.Parent;
            }

            SelectedItem = item;
        }

        private void SelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // A RichCanvas által kijelölt összes root elem kiemelése
            foreach (CanvasItemBase item in SelectedItems)
            {
                item.IsHighlighted = true;
            }
        }
        #endregion

        #region Element addition methods
        //Basic addition function
        private void AddElementToCanvas(CanvasItemBase item)
        {
            const double spacing = 20;
            double newX = item.X;
            double newY = item.Y;
            while (CanvasItems.Any(existingItem =>
                newX < existingItem.X + existingItem.Width + spacing &&
                newX + item.Width + spacing > existingItem.X &&
                newY < existingItem.Y + existingItem.Height + spacing &&
                newY + item.Height + spacing > existingItem.Y))
            {
                newX += item.Width + spacing;
                if (newX + item.Width > 1200)
                {
                    newX = 20;
                    newY += item.Height + spacing;
                }
            }
            item.X = newX;
            item.Y = newY;
            CanvasItems.Add(item);
        }

        //Application
        private void OnApplicationButtonClicked()
        {
            ConsoleApplication application = new ConsoleApplication
            {
                Name = $"Application{CanvasItems.OfType<ConsoleApplication>().Count() + 1}",
                X = 0,
                Y = 0
            };
            AddElementToCanvas(application);
            LogAddition(application, null);
        }

        //Command
        private void OnCommandButtonClicked()
        {
            Command command = new Command
            {
                Name = $"Command{CanvasItems.OfType<Command>().Count() + 1}",
                X = 0,
                Y = 0
            };
            AddElementToCanvas(command);
            LogAddition(command, null);
        }

        //ActionReference to Command
        private bool IsAddActionReferenceToCommandButtonEnabled(Command? command)
        {
            return command != null && !command.Children.OfType<ActionReference>().Any();
        }

        private void OnAddActionReferenceToCommand(Command? command)
        {
            if (command == null || command.Children.OfType<ActionReference>().Any()) return;

            ActionReference actionReference = new()
            {
                Parent = command
            };

            Point position = command.FindFreeChildPosition(
                actionReference.Width,
                actionReference.Height);

            actionReference.X = position.X;
            actionReference.Y = position.Y;

            command.Children.Add(actionReference);

            actionReference.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            command.UpdateLayoutAndSize();
            AddActionReferenceToCommandCommand.RaiseCanExecuteChanged();

            OnSelectItem(actionReference);

            LogAddition(actionReference, command);
        }

        //Command to Application
        private void OnAddCommandToApplication(ConsoleApplication? application)
        {
            if (application == null)
                return;

            int commandNumber = application.Children
                .OfType<Command>()
                .Count() + 1;

            Command command = new Command
            {
                Name = $"{application.Name}-Command{commandNumber}"
            };

            Point position = application.FindFreeChildPosition(
                command.Width,
                command.Height);

            command.X = position.X;
            command.Y = position.Y;

            command.Parent = application;
            application.Children.Add(command);

            application.UpdateLayoutAndSize();
            LogAddition(command, application);
        }

        //Input to Command
        private void OnAddInputToCommand(Command? command)
        {
            if (command == null)
                return;

            int inputNumber = command.Children.OfType<Input>().Count() + 1;

            Input input = new Input();

            Point position = command.FindFreeChildPosition(input.Width, input.Height);

            input.X = position.X;
            input.Y = position.Y;

            input.Parent = command;
            command.Children.Add(input);
            command.UpdateLayoutAndSize();
            AddInputToCommandCommand.RaiseCanExecuteChanged();
            LogAddition(input, command);
        }

        //Parameter to Input

        private void OnAddParameterToInput(Input? input)
        {
            if (input == null || input.Parent == null) return;

            int parameterNumber = input.Children.OfType<CanvasElements.Parameter>().Count() + 1;

            CanvasElements.Parameter parameter = new()
            {
                Name = $"{input.Parent.Name}-parameter{parameterNumber}",
            };

            parameter.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            Point position = input.FindFreeChildPosition(parameter.Width, parameter.Height);
            parameter.X = position.X;
            parameter.Y = position.Y;

            input.Children.Add(parameter);
            input.UpdateLayoutAndSize();
            LogAddition(parameter, input);
        }


        private bool IsAddInputButtonEnabled(Command? command)
        {
            if (command == null)
                return false;

            return !command.Children.OfType<Input>().Any();
        }

        //Component
        private void OnComponentButtonClicked()
        {
            Component component = new Component
            {
                Name = $"Component{CanvasItems.OfType<Component>().Count() + 1}",
                X = 0,
                Y = 0
            };
            AddElementToCanvas(component);
            LogAddition(component, null);
        }

        //Record to Component

        private void OnAddRecordToComponent(Component? component)
        {
            if (component == null) return;

            int recordCount = GetAllCanvasItems(CanvasItems)
                .OfType<Record>()
                .Count();

            Record record = new Record
            {
                Name = $"Record{recordCount + 1}",
                Parent = component
            };

            component.Children.Add(record);
            component.UpdateLayoutAndSize();

            AddRecordToComponentCommand.RaiseCanExecuteChanged();
            RefreshAllTypeOptions();

            LogAddition(record, component);
        }

        //Action to Component
        private void OnAddActionToComponent(Component? component)
        {
            if (component == null) return;

            int actionCount = GetAllCanvasItems(CanvasItems)
                .OfType<CanvasElements.Action>()
                .Count();

            CanvasElements.Action action = new CanvasElements.Action
            {
                Name = $"Action{actionCount + 1}",
                Parent = component
            };

            component.Children.Add(action);
            component.UpdateLayoutAndSize();

            AddActionToComponentCommand.RaiseCanExecuteChanged();
            RefreshAllTypeOptions();

            LogAddition(action, component);
        }

        // TypePairs to Mediator
        private bool IsAddTypePairsButtonEnabled(Mediator? mediator)
        {
            if (mediator == null) return false;

            return !mediator.Children.OfType<TypePairs>().Any();
        }

        private void OnAddTypePairsToMediator(Mediator? mediator)
        {
            if (mediator == null || mediator.Children.OfType<TypePairs>().Any()) return;

            TypePairs typePairs = new TypePairs
            {
                Parent = mediator
            };

            Point position = mediator.FindFreeChildPosition(typePairs.Width, typePairs.Height);

            typePairs.X = position.X;
            typePairs.Y = position.Y;

            mediator.Children.Add(typePairs);
            mediator.UpdateLayoutAndSize();

            AddTypePairsToMediatorCommand.RaiseCanExecuteChanged();

            LogAddition(typePairs, mediator);
        }

        // TypePair to TypePairs
        private void OnAddTypePairToTypePairs(TypePairs? typePairs)
        {
            if (typePairs == null) return;

            TypePair typePair = new TypePair
            {
                Parent = typePairs
            };

            typePair.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            Point position = typePairs.FindFreeChildPosition(typePair.Width, typePair.Height);

            typePair.X = position.X;
            typePair.Y = position.Y;

            typePairs.Children.Add(typePair);
            typePairs.UpdateLayoutAndSize();

            LogAddition(typePair, typePairs);
        }

        //Record
        private void OnRecordButtonClicked()
        {
            Record record = new Record
            {
                Name = $"Record{CanvasItems.OfType<Record>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(record);
            LogAddition(record, null);
            RefreshAllTypeOptions();
        }

        //Field to Record
        private void OnAddFieldToRecord(Record? record)
        {
            if (record == null) return;

            int fieldNumber = record.Children
                .OfType<Field>()
                .Count() + 1;

            Field field = new Field
            {
                Name = $"Field{fieldNumber}"
            };

            field.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            Point position = record.FindFreeChildPosition(
                field.Width,
                field.Height);

            field.X = position.X;
            field.Y = position.Y;

            field.Parent = record;
            record.Children.Add(field);

            record.UpdateLayoutAndSize();
            LogAddition(field, record);
        }

        //BaseList to Record
        private bool IsAddBaseListButtonEnabled(Record? record)
        {
            if (record == null) return false;

            return !record.Children.OfType<BaseList>().Any();
        }

        private void OnAddBaseListToRecord(Record? record)
        {
            if (record == null || record.Children.OfType<BaseList>().Any()) return;

            BaseList baseList = new BaseList
            {
                Name = "BaseList",
                Parent = record
            };

            Point position = record.FindFreeChildPosition(baseList.Width, baseList.Height);
            baseList.X = position.X;
            baseList.Y = position.Y;

            record.Children.Add(baseList);
            record.UpdateLayoutAndSize();

            AddBaseListToRecordCommand.RaiseCanExecuteChanged();
            LogAddition(baseList, record);
        }

        //Base to BaseList
        private void OnAddBaseToBaseList(BaseList? baseList)
        {
            if (baseList == null) return;

            int baseNumber = baseList.Children.OfType<Base>().Count() + 1;

            Base baseItem = new Base
            {
                Parent = baseList
            };

            baseItem.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            Point position = baseList.FindFreeChildPosition(baseItem.Width, baseItem.Height);
            baseItem.X = position.X;
            baseItem.Y = position.Y;

            baseList.Children.Add(baseItem);
            baseList.UpdateLayoutAndSize();
            LogAddition(baseItem, baseList);
        }

        //Entity
        private void OnEntityButtonClicked()
        {
            Entity entity = new Entity
            {
                Name = $"Entity{CanvasItems.OfType<Entity>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(entity);
            RefreshAllTypeOptions();
            LogAddition(entity, null);
        }

        //Field to Entity
        private void OnAddFieldToEntity(Entity? entity)
        {
            if (entity == null) return;

            int fieldNumber = entity.Children
                .OfType<Field>()
                .Count() + 1;

            Field field = new Field
            {
                Name = $"Field{fieldNumber}"
            };

            field.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            field.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Field.IsLazy))
                    AddLogicToFieldCommand.RaiseCanExecuteChanged();
            };

            Point position = entity.FindFreeChildPosition(field.Width,field.Height);
            field.X = position.X;
            field.Y = position.Y;

            field.Parent = entity;
            entity.Children.Add(field);

            entity.UpdateLayoutAndSize();
            LogAddition(field, entity);
        }

        //BaseList to Entity
        private bool IsAddBaseListButtonEnabled(Entity? entity)
        {
            if (entity == null) return false;

            return !entity.Children.OfType<BaseList>().Any();
        }

        private void OnAddBaseListToEntity(Entity? entity)
        {
            if (entity == null || entity.Children.OfType<BaseList>().Any()) return;

            BaseList baseList = new BaseList
            {
                Name = "BaseList"
            };

            Point position = entity.FindFreeChildPosition(baseList.Width, baseList.Height);
            baseList.X = position.X;
            baseList.Y = position.Y;

            baseList.Parent = entity;
            entity.Children.Add(baseList);

            entity.UpdateLayoutAndSize();
            AddBaseListToEntityCommand.RaiseCanExecuteChanged();
            LogAddition(baseList, entity);
        }

        //Logic to Field
        private bool IsAddLogicButtonEnabled(Field? field)
        {
            if (field == null) return false;

            return !field.Children.OfType<Logic>().Any();
        }

        private void OnAddLogicToField(Field? field)
        {
            if (field == null || !IsAddLogicButtonEnabled(field)) return;

            Logic logic = new Logic
            {
                Text = "text", 
            };



            Point position = field.FindFreeChildPosition(logic.Width, logic.Height);

            logic.X = position.X;
            logic.Y = position.Y;

            logic.Parent = field;
            field.Children.Add(logic);
            field.UpdateLayoutAndSize();

            AddLogicToFieldCommand.RaiseCanExecuteChanged();
            LogAddition(logic, field);
        }

        //Enum
        private void OnEnumButtonClicked()
        {
            EnumElement enumElement = new EnumElement
            {
                Name = $"Enum{CanvasItems.OfType<EnumElement>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(enumElement);
            RefreshAllTypeOptions();
            LogAddition(enumElement, null);
        }

        //Base to Enum
        private bool IsAddBaseToEnumElementButtonEnabled(EnumElement? enumElement)
        {
            if (enumElement == null) return false;

            return !enumElement.Children.OfType<Base>().Any();
        }

        private void OnAddBaseToEnumElement(EnumElement? enumElement)
        {
            if (enumElement == null || enumElement.Children.OfType<Base>().Any()) return;

            Base baseItem = new Base
            {
                Name = "Base",
                Parent = enumElement
            };

            Point position = enumElement.FindFreeChildPosition(baseItem.Width, baseItem.Height);
            baseItem.X = position.X;
            baseItem.Y = position.Y;

            enumElement.Children.Add(baseItem);
            enumElement.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));
            enumElement.UpdateLayoutAndSize();

            AddBaseToEnumElementCommand.RaiseCanExecuteChanged();
            LogAddition(baseItem, enumElement);
        }

        //EnumField to Enum
        private void OnAddEnumFieldToEnumElement(EnumElement? enumElement)
        {
            if (enumElement == null) return;

            int fieldNumber = enumElement.Children.OfType<EnumField>().Count() + 1;

            EnumField field = new EnumField
            {
                Name = $"Field{fieldNumber}",
                Parent = enumElement
            };

            Point position = enumElement.FindFreeChildPosition(field.Width, field.Height);
            field.X = position.X;
            field.Y = position.Y;

            enumElement.Children.Add(field);
            enumElement.UpdateLayoutAndSize();
            LogAddition(field, enumElement);
        }

        //Composite
        private void OnCompositeButtonClicked()
        {
            Composite composite = new Composite
            {
                Name = $"Composite{CanvasItems.OfType<Composite>().Count() + 1}",
                X = 0,
                Y = 0
            };

            CompositeTypeElement type = new CompositeTypeElement
            {
                Name = composite.CompositeType,
                Parent = composite
            };

            Point typePosition = composite.FindFreeChildPosition(type.Width, type.Height);
            type.X = typePosition.X;
            type.Y = typePosition.Y;
            composite.Children.Add(type);

            composite.UpdateLayoutAndSize();

            OnAddPartsToComposite(composite);
            OnAddPartToParts(composite.Children.OfType<Parts>().FirstOrDefault());
            OnAddPartToParts(composite.Children.OfType<Parts>().FirstOrDefault());

            AddElementToCanvas(composite);
            LogAddition(composite, null);
        }

        //Parts to Composite
        private bool IsAddPartsButtonEnabled(Composite? composite)
        {
            if (composite == null) return false;

            return !composite.Children.OfType<Parts>().Any();
        }


        private void OnAddPartsToComposite(Composite? composite)
        {
            if (composite == null || composite.Children.OfType<Parts>().Any()) return;

            Parts parts = new Parts
            {
            };

            Point position = composite.FindFreeChildPosition(parts.Width, parts.Height);

            parts.X = position.X;
            parts.Y = position.Y;
            parts.Parent = composite;

            composite.Children.Add(parts);
            composite.UpdateLayoutAndSize();

            AddPartsToCompositeCommand.RaiseCanExecuteChanged();

            LogAddition(parts, composite);
        }

        //Part to Parts
        private void OnAddPartToParts(Parts? parts)
        {
            if (parts == null) return;

            Part part = new Part
            {
                Parent = parts
            };

            Point position = parts.FindFreeChildPosition(part.Width, part.Height);

            part.X = position.X;
            part.Y = position.Y;

            parts.Children.Add(part);

            parts.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));
            parts.UpdateLayoutAndSize();

            LogAddition(part, parts);
        }

        //Mediator
        private void OnMediatorButtonClicked()
        {
            Mediator mediator = new Mediator
            {
                Name = $"Mediator{CanvasItems.OfType<Mediator>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(mediator);
            LogAddition(mediator, null);
        }

        //Proxy
        private void OnProxyButtonClicked()
        {
            Proxy proxy = new Proxy
            {
                Name = $"Proxy{CanvasItems.OfType<Proxy>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(proxy);
            LogAddition(proxy, null);
        }

        // Field to Proxy
        private void OnAddFieldToProxy(Proxy? proxy)
        {
            if (proxy == null) return;

            int fieldNumber = proxy.Children.OfType<Field>().Count() + 1;

            Field field = new Field
            {
                Name = $"Field{fieldNumber}",
                Parent = proxy
            };

            field.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            field.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Field.IsLazy))
                    AddLogicToFieldCommand.RaiseCanExecuteChanged();
            };

            Point position = proxy.FindFreeChildPosition(field.Width, field.Height);

            field.X = position.X;
            field.Y = position.Y;

            proxy.Children.Add(field);
            proxy.UpdateLayoutAndSize();

            LogAddition(field, proxy);
        }

        // Type to Proxy
        private bool IsAddTypeToProxyButtonEnabled(Proxy? proxy)
        {
            if (proxy == null) return false;

            return !proxy.Children.OfType<CanvasElements.Type>().Any();
        }

        private void OnAddTypeToProxy(Proxy? proxy)
        {
            if (proxy == null || proxy.Children.OfType<CanvasElements.Type>().Any()) return;

            CanvasElements.Type type = new CanvasElements.Type
            {
                Parent = proxy
            };

            type.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            Point position = proxy.FindFreeChildPosition(type.Width, type.Height);

            type.X = position.X;
            type.Y = position.Y;

            proxy.Children.Add(type);
            proxy.UpdateLayoutAndSize();

            AddTypeToProxyCommand.RaiseCanExecuteChanged();

            LogAddition(type, proxy);
        }

        //Input to Action
        private bool IsAddInputToActionButtonEnabled(CanvasElements.Action? action)
        {
            if (action == null) return false;

            return action.InputElement == null;
        }

        private void OnAddInputToAction(CanvasElements.Action? action)
        {
            if (action == null || !IsAddInputToActionButtonEnabled(action)) return;

            Input input = new Input
            {
                Parent = action
            };

            Point position = action.FindFreeChildPosition(input.Width, input.Height);
            input.X = position.X;
            input.Y = position.Y;

            action.Children.Add(input);
            action.UpdateLayoutAndSize();

            AddInputToActionCommand.RaiseCanExecuteChanged();
            LogAddition(input, action);
        }

        //Output to Action
        private bool IsAddOutputToActionButtonEnabled(CanvasElements.Action? action)
        {
            if (action == null) return false;

            return action.OutputElement == null;
        }

        private void OnAddOutputToAction(CanvasElements.Action? action)
        {
            if (action == null || !IsAddOutputToActionButtonEnabled(action)) return;

            Output output = new Output
            {
                Parent = action
            };

            Point position = action.FindFreeChildPosition(output.Width, output.Height);
            output.X = position.X;
            output.Y = position.Y;

            action.Children.Add(output);
            action.UpdateLayoutAndSize();

            AddOutputToActionCommand.RaiseCanExecuteChanged();
            LogAddition(output, action);

            CanvasElements.Parameter result = new CanvasElements.Parameter
            {
                Name = $"result"
            };
            Point resultPosition = output.FindFreeChildPosition(result.Width, result.Height);

            result.X = resultPosition.X;
            result.Y = resultPosition.Y;

            output.Children.Add(result);
            LogAddition(result, output);
        }

        //Parameter to Output
        private void OnAddParameterToOutput(Output? output)
        {
            if (output == null || output.Parent == null)
                return;

            int parameterNumber = output.Children.OfType<CanvasElements.Parameter>().Count() + 1;

            CanvasElements.Parameter parameter = new CanvasElements.Parameter
            {
                Name = $"{output.Parent.Name}-parameter{parameterNumber}"
            };

            Point position = output.FindFreeChildPosition(parameter.Width, parameter.Height);

            parameter.X = position.X;
            parameter.Y = position.Y;

            output.Children.Add(parameter);
            LogAddition(parameter, output);
        }

        //Logic to Action
        private bool IsAddLogicToActionButtonEnabled(CanvasElements.Action? action)
        {
            if (action == null) return false;

            return action.LogicElement == null;
        }

        private void OnAddLogicToAction(CanvasElements.Action? action)
        {
            if (action == null || !IsAddLogicToActionButtonEnabled(action)) return;

            Logic logic = new Logic
            {
                Text = "text",
            };

            Point position = action.FindFreeChildPosition(logic.Width, logic.Height);
            logic.X = position.X;
            logic.Y = position.Y;

            action.Children.Add(logic);
            action.UpdateLayoutAndSize();

            AddLogicToActionCommand.RaiseCanExecuteChanged();
            LogAddition(logic, action);
        }

        //Factroy to canvas
        private void OnFactoryButtonClicked()
        {
            Factory factory = new Factory
            {
                Name = $"Factory{CanvasItems.OfType<Factory>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(factory);
            LogAddition(factory, null);
        }

        // Record to Factory
        private void OnAddRecordToFactory(Factory? factory)
        {
            if (factory == null) return;

            int recordNumber = GetAllCanvasItems(CanvasItems)
                .OfType<Record>()
                .Count() + 1;

            Record record = new Record
            {
                Name = $"Record{recordNumber}",
                Parent = factory
            };

            factory.Children.Add(record);
            factory.UpdateLayoutAndSize();

            RefreshAllTypeOptions();

            LogAddition(record, factory);
        }

        // Action to Factory
        private void OnAddActionToFactory(Factory? factory)
        {
            if (factory == null) return;

            int actionNumber = GetAllCanvasItems(CanvasItems)
                .OfType<CanvasElements.Action>()
                .Count() + 1;

            CanvasElements.Action action = new CanvasElements.Action
            {
                Name = $"Action{actionNumber}",
                Parent = factory
            };

            factory.Children.Add(action);
            factory.UpdateLayoutAndSize();

            RefreshAllTypeOptions();

            LogAddition(action, factory);
        }

        // Type to Factory
        private bool IsAddTypeToFactoryButtonEnabled(Factory? factory)
        {
            return factory != null && factory.Type == null;
        }

        private void OnAddTypeToFactory(Factory? factory)
        {
            if (factory == null || factory.Type != null) return;

            CanvasElements.Type type = new CanvasElements.Type
            {
                Parent = factory
            };

            type.RefreshAvailableTypes(GetAllCanvasItems(CanvasItems));

            factory.Children.Add(type);
            factory.UpdateLayoutAndSize();

            AddTypeToFactoryCommand.RaiseCanExecuteChanged();

            LogAddition(type, factory);
        }
        #endregion

        #endregion

    }
}
