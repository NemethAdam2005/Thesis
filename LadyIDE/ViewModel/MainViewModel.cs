using LadyIDE.Model;
using LadyIDE.WPF.CanvasElements;
using LadyIDE.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace LadyIDE.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ApplicationModel _model;
        private string _code = string.Empty;

        public string Code
        {
            get => _code;
            set
            {
                if (_code == value)
                    return;

                _code = value;
                _model.Code = value;

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

        public ObservableCollection<CanvasItemBase> CanvasItems { get; }
        public ObservableCollection<CanvasItemBase> SelectedItems { get; }

        public MainViewModel(ApplicationModel model)
        {
            CanvasItems = new ObservableCollection<CanvasItemBase>();
            SelectedItems = new ObservableCollection<CanvasItemBase>();

            _model = model;
            _model.CodeChanged += new EventHandler<string>(Model_CodeChanged);

            LoadFileCommand = new DelegateCommand(param => OnLoadFile());
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
            ComponentButtonClicked = new DelegateCommand(param => OnComponentButtonClicked());
            RecordButtonClicked = new DelegateCommand(param => OnRecordButtonClicked());
            EntityButtonClicked = new DelegateCommand(param => OnEntityButtonClicked());
            EnumButtonClicked = new DelegateCommand(param => OnEnumButtonClicked());
            CompositeButtonClicked = new DelegateCommand(param => OnCompositeButtonClicked());
            MediatorButtonClicked = new DelegateCommand(param => OnMediatorButtonClicked());
            ProxyButtonClicked = new DelegateCommand(param => OnProxyButtonClicked());
        }

        //Commands
        public DelegateCommand LoadFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand ColorThemeButtonClicked { get; private set; }
        public DelegateCommand ShowLadyElementsButtonClicked { get; private set; }
        public DelegateCommand ShowPropertiesButtonClicked { get; private set; }
        public DelegateCommand SelectItemCommand { get; private set; }
        public DelegateCommand AddCommandToApplicationCommand { get; private set; }
        public DelegateCommand DeleteItemCommand { get; private set; }
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
        public event EventHandler? SaveFile;
        public event EventHandler<string>? CodeChanged;

        //Model event handlers
        private void Model_CodeChanged(Object? sender, String e)
        {
            CodeChanged?.Invoke(this, e);
        }

        //Save and Load methods
        private void OnLoadFile()
        {
            LoadFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveFile()
        {
            SaveFile?.Invoke(this, EventArgs.Empty);
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

                item.Parent = null;
                item.IsHighlighted = false;

                // A szülő Application lesz újra kijelölve
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

        private void OnSelectItem(CanvasItemBase? item)
        {
            if (item == null)
            {
                SelectedItem = null;
                // Minden korábbi highlight törlése
                foreach (CanvasItemBase rootItem in CanvasItems)
                {
                    rootItem.IsHighlighted = false;

                    foreach (CanvasItemBase child in rootItem.Children)
                    {
                        child.IsHighlighted = false;
                    }
                }
                return;
            }

            // Ha az elem egy Application/önálló elem,
            // akkor a korábbi kijelölések maradjanak meg.
            if (item.Parent == null)
            {
                item.IsHighlighted = true;
                foreach (CanvasItemBase child in item.Children)
                {
                    child.IsHighlighted = false;
                }
            }
            else
            {
                // Ha child, akkor ugyanazon parent többi childját
                // levesszük a kijelölésről.
                foreach (CanvasItemBase sibling in item.Parent.Children)
                {
                    sibling.IsHighlighted = sibling == item;
                }

                // A parent is legyen kiemelve, mert a child benne van.
                item.Parent.IsHighlighted = true;
            }

            if (SelectedItem != null && SelectedItem.Parent != null)
            {
                foreach (CanvasItemBase child in SelectedItem.Parent.Children)
                {
                    child.IsHighlighted = child == item;
                }
            }

            // Ez csak a Properties panelhez tartozó kijelölés.
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

            application.UpdateSizeFromChildren();
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
