using LadyIDE.Model;
using LadyIDE.WPF.CanvasElements;
using LadyIDE.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;

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
            ApplicationButtonClicked = new DelegateCommand(param => OnApplicationButtonClicked());
            CommandButtonClicked = new DelegateCommand(param => OnCommandButtonClicked());
            SelectItemCommand = new DelegateCommand(param => OnSelectItem(param as CanvasItemBase));
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }
        
        //Commands
        public DelegateCommand LoadFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand ColorThemeButtonClicked { get; private set; }
        public DelegateCommand ShowLadyElementsButtonClicked { get; private set; }
        public DelegateCommand ApplicationButtonClicked { get; private set; }
        public DelegateCommand CommandButtonClicked { get; private set; }
        public DelegateCommand SelectItemCommand { get; private set; }

        //Events
        public event EventHandler? LoadFile;
        public event EventHandler? SaveFile;
        public event EventHandler<string>? CodeChanged;

        public void AddApplication(double x, double y)
        {
            const double appWidth = 300;
            const double appHeight = 180;
            const double spacing = 20;

            double newX = Math.Max(0, x);
            double newY = Math.Max(0, y);

            // Ütközés esetén új pozíció keresése.
            while (CanvasItems.Any(item =>
                newX < item.X + item.Width + spacing &&
                newX + appWidth + spacing > item.X &&
                newY < item.Y + item.Height + spacing &&
                newY + appHeight + spacing > item.Y))
            {
                newX += appWidth + spacing;

                if (newX + appWidth > 1200)
                {
                    newX = 20;
                    newY += appHeight + spacing;
                }
            }

            int applicationNumber = CanvasItems
                .OfType<ConsoleApplication>()
                .Count() + 1;

            ConsoleApplication application = new ConsoleApplication
            {
                Name = $"Application{applicationNumber}",
                X = newX,
                Y = newY,
                Width = appWidth,
                Height = appHeight
            };

            CanvasItems.Add(application);

            Command command1 = new Command
            {
                Name = "Command1",
                Width = 180,
                Height = 120
            };

            Point position = application.FindFreeChildPosition(
                command1.Width,
                command1.Height);

            command1.X = position.X;
            command1.Y = position.Y;

            command1.Parent = application;
            application.Children.Add(command1);

            Command command2 = new Command
            {
                Name = "Command2",
                Width = 180,
                Height = 120
            };

            Point position2 = application.FindFreeChildPosition(
                command2.Width,
                command2.Height);

            command2.X = position2.X;
            command2.Y = position2.Y;

            command2.Parent = application;
            application.Children.Add(command2);

            Command command3 = new Command
            {
                Name = "Command3",
                Width = 180,
                Height = 120
            };

            Point position3 = application.FindFreeChildPosition(
                command3.Width,
                command3.Height);

            command3.X = position3.X;
            command3.Y = position3.Y;

            command3.Parent = application;
            application.Children.Add(command3);
            Command command4 = new Command
            {
                Name = "Command4",
                Width = 180,
                Height = 120
            };

            Point position4 = application.FindFreeChildPosition(
                command4.Width,
                command4.Height);

            command4.X = position4.X;
            command4.Y = position4.Y;

            command4.Parent = application;
            application.Children.Add(command4);

            application.UpdateSizeFromChildren();
        }

        public void AddCommand(double x, double y)
        {
            //const double commandWidth = 200;
            //const double commandHeight = 100;
            //const double spacing = 20;

            //double newX = Math.Max(0, x);
            //double newY = Math.Max(0, y);

            //// Ütközés esetén új pozíció keresése.
            //while (CanvasItems.Any(item =>
            //    newX < item.X + item.Width + spacing &&
            //    newX + commandWidth + spacing > item.X &&
            //    newY < item.Y + item.Height + spacing &&
            //    newY + commandHeight + spacing > item.Y))
            //{
            //    newX += commandWidth + spacing;

            //    if (newX + commandWidth > 1200)
            //    {
            //        newX = 20;
            //        newY += commandHeight + spacing;
            //    }
            //}

            //int commandNumber = CanvasItems
            //    .OfType<Command>()
            //    .Count() + 1;

            //Command command = new Command
            //{
            //    Name = $"Command{commandNumber}",
            //    X = newX,
            //    Y = newY,
            //    Width = commandWidth,
            //    Height = commandHeight
            //};

            //CanvasItems.Add(command);

            ConsoleApplication? app = CanvasItems.OfType<ConsoleApplication>().FirstOrDefault();
            Command command2 = new Command
            {
                Name = "Command2",
                Width = 180,
                Height = 120
            };

            if (app == null)
            {
                return;
            }

            Point position2 = app.FindFreeChildPosition(
                command2.Width,
                command2.Height);

            command2.X = position2.X;
            command2.Y = position2.Y;

            command2.Parent = app;
            app.Children.Add(command2);
            app.UpdateSizeFromChildren();

        }

        private void Model_CodeChanged(Object? sender, String e)
        {
            CodeChanged?.Invoke(this, e);
        }

        private void OnSelectItem(CanvasItemBase? item)
        {
            // Minden korábbi highlight törlése
            foreach (CanvasItemBase rootItem in CanvasItems)
            {
                rootItem.IsHighlighted = false;

                foreach (CanvasItemBase child in rootItem.Children)
                {
                    child.IsHighlighted = false;
                }
            }

            // Üres canvasra kattintottunk
            if (item == null)
            {
                SelectedItem = null;
                return;
            }

            // Az utoljára kattintott konkrét elem
            SelectedItem = item;

            // Gyermek elemre kattintottunk
            if (item.Parent != null)
            {
                item.IsHighlighted = true;
                item.Parent.IsHighlighted = true;
                return;
            }

            // Root elemre kattintottunk
            item.IsHighlighted = true;
        }

        private void SelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // A RichCanvas által kijelölt összes root elem kiemelése
            foreach (CanvasItemBase item in SelectedItems)
            {
                item.IsHighlighted = true;
            }
        }

        private void OnLoadFile()
        {
            LoadFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveFile()
        {
            SaveFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnApplicationButtonClicked()
        {
            AddApplication(0.0, 0.0);
        }

        private void OnCommandButtonClicked()
        {
            AddCommand(0.0,0.0);
        }

        private void OnColorThemeButtonClicked()
        {
            IsLightMode = !IsLightMode;

            ThemeManager.SetTheme(IsLightMode);
        }

        private void OnShowLadyElementsButtonClicked()
        {
            IsLadyElementsVisible = !IsLadyElementsVisible;
        }

        public void UpdateCaretPosition(int line, int column)
        {
            CaretPositionText = $"  Ln: {line}, Col: {column}";
        }
    }   
}
