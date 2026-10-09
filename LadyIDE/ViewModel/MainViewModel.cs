using LadyIDE.Model;
using LadyIDE.WPF.CanvasElements;
using LadyIDE.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.IO;

namespace LadyIDE.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ApplicationModel _model;

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

        public ObservableCollection<string> InputTypes { get; } = new()
        {
            "String",
            "Int",
            "Double",
            "Bool",
            "Char",
            "Object"
        };

        public ObservableCollection<string> AvailableActions { get; } = new();

        public ObservableCollection<CanvasItemBase> CanvasItems { get; }
        public ObservableCollection<CanvasItemBase> SelectedItems { get; }

        public MainViewModel(ApplicationModel model)
        {
            CanvasItems = new ObservableCollection<CanvasItemBase>();
            SelectedItems = new ObservableCollection<CanvasItemBase>();

            _model = model;
            _model.CodeChanged += new EventHandler<string>(Model_CodeChanged);
            _model.FileLoaded += new EventHandler<string>(Model_FileLoaded);

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
            RecordButtonClicked = new DelegateCommand(param => OnRecordButtonClicked());
            EntityButtonClicked = new DelegateCommand(param => OnEntityButtonClicked());
            EnumButtonClicked = new DelegateCommand(param => OnEnumButtonClicked());
            CompositeButtonClicked = new DelegateCommand(param => OnCompositeButtonClicked());
            MediatorButtonClicked = new DelegateCommand(param => OnMediatorButtonClicked());
            ProxyButtonClicked = new DelegateCommand(param => OnProxyButtonClicked());
            AddInputToCommandCommand = new DelegateCommand(param => IsAddInputButtonEnabled(param as Command),param => OnAddInputToCommand(param as Command));
            AddParameterToInputCommand = new DelegateCommand(param => OnAddParameterToInput(param as Input));
        }

        //Commands
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
        public DelegateCommand ComponentButtonClicked { get; private set; }
        public DelegateCommand RecordButtonClicked { get; private set; }
        public DelegateCommand EntityButtonClicked { get; private set; }
        public DelegateCommand EnumButtonClicked { get; private set; }
        public DelegateCommand CompositeButtonClicked { get; private set; }
        public DelegateCommand MediatorButtonClicked { get; private set; }
        public DelegateCommand ProxyButtonClicked { get; private set; }


        //Events
        public event EventHandler? LoadFile;
        public event EventHandler? SaveAsFile;
        public event EventHandler? SaveFile;
        public event EventHandler<string>? CodeChanged;

        //Model event handlers
        private void Model_CodeChanged(Object? sender, String e)
        {
            Code = e;
        }

        private void Model_FileLoaded(Object? sender, String e)
        {
            Code = e;
            CodeChanged?.Invoke(this, e);
        }

        //Save and Load methods
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

        //UI event handlers
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

        private void OnDeleteItem(CanvasItemBase? item)
        {
            if (item == null)
                return;

            // Ha egy Applicationön belüli Commandot törlünk
            if (item.Parent != null)
            {
                CanvasItemBase parent = item.Parent;

                parent.Children.Remove(item);

                if (parent is Command)
                    AddInputToCommandCommand.RaiseCanExecuteChanged();

                item.Parent = null;
                item.IsHighlighted = false;
                SelectedItem = parent;
                parent.IsHighlighted = true;
                return;
            }
            else if (CanvasItems.Contains(item))
            {
                CanvasItems.Remove(item);
                SelectedItems.Remove(item);

                // Nincs már kijelölt elem
                SelectedItem = null;
            }
        }
        private void OnDeleteSelectedItems()
        {
            if (SelectedItems == null || SelectedItems.Count == 0)
                return;

            List<CanvasItemBase> itemsToDelete = SelectedItems.ToList();

            Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                foreach (CanvasItemBase item in itemsToDelete)
                {
                    CanvasItems.Remove(item);
                }

                SelectedItem = null;
                SelectedItems.Clear();
            }));
        
        }

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

        private void OnApplicationButtonClicked()
        {
            ConsoleApplication application = new ConsoleApplication
            {
                Name = $"Application{CanvasItems.OfType<ConsoleApplication>().Count() + 1}",
                X = 0,
                Y = 0
            };
            AddElementToCanvas(application);
        }

        private void OnCommandButtonClicked()
        {
            Command command = new Command
            {
                Name = $"Command{CanvasItems.OfType<Command>().Count() + 1}",
                X = 0,
                Y = 0
            };
            AddElementToCanvas(command);
        }

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
        }

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
        }

        private void OnAddParameterToInput(Input? input)
        {
            if (input == null || input.Parent == null)
                return;

            int parameterNumber = input.Children.OfType<Parameter>().Count() + 1;

            Parameter parameter = new Parameter
            {
                Name = $"{input.Parent.Name}-parameter{parameterNumber}",
                Type = "String"
            };

            Point position = input.FindFreeChildPosition(parameter.Width, parameter.Height);

            parameter.X = position.X;
            parameter.Y = position.Y;

            input.Children.Add(parameter);
            input.UpdateLayoutAndSize();
        }

        private bool IsAddInputButtonEnabled(Command? command)
        {
            if (command == null)
                return false;

            return !command.Children.OfType<Input>().Any();
        }

        private void OnComponentButtonClicked()
        {
            Component component = new Component
            {
                Name = $"Component{CanvasItems.OfType<Component>().Count() + 1}",
                X = 0,
                Y = 0
            };
            AddElementToCanvas(component);
        }

        private void OnRecordButtonClicked()
        {
            Record record = new Record
            {
                Name = $"Record{CanvasItems.OfType<Record>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(record);
        }

        private void OnEntityButtonClicked()
        {
            Entity entity = new Entity
            {
                Name = $"Entity{CanvasItems.OfType<Entity>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(entity);
        }

        private void OnEnumButtonClicked()
        {
            EnumElement enumElement = new EnumElement
            {
                Name = $"Enum{CanvasItems.OfType<EnumElement>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(enumElement);
        }

        private void OnCompositeButtonClicked()
        {
            Composite composite = new Composite
            {
                Name = $"Composite{CanvasItems.OfType<Composite>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(composite);
        }

        private void OnMediatorButtonClicked()
        {
            Mediator mediator = new Mediator
            {
                Name = $"Mediator{CanvasItems.OfType<Mediator>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(mediator);
        }

        private void OnProxyButtonClicked()
        {
            Proxy proxy = new Proxy
            {
                Name = $"Proxy{CanvasItems.OfType<Proxy>().Count() + 1}",
                X = 0,
                Y = 0
            };

            AddElementToCanvas(proxy);
        }

    }   
}
