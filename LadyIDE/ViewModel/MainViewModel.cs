using LadyIDE.Model;
using LadyIDE.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using LadyIDE.WPF.CanvasElements;

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

        public ObservableCollection<CanvasItemBase> CanvasItems { get; }

        public MainViewModel(ApplicationModel model)
        {
            CanvasItems = new ObservableCollection<CanvasItemBase>();
            CanvasItems.Add(new CanvasItemBase());
            _model = model;
            _model.CodeChanged += new EventHandler<string>(Model_CodeChanged);

            LoadFileCommand = new DelegateCommand(param => OnLoadFile());
            SaveFileCommand = new DelegateCommand(param => OnSaveFile());
            ColorThemeButtonClicked = new DelegateCommand(param => OnColorThemeButtonClicked());
            ShowLadyElementsButtonClicked = new DelegateCommand(param => OnShowLadyElementsButtonClicked());
        }
        
        //Commands
        public DelegateCommand LoadFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand ColorThemeButtonClicked { get; private set; }
        public DelegateCommand ShowLadyElementsButtonClicked { get; private set; }

        //Events
        public event EventHandler? LoadFile;
        public event EventHandler? SaveFile;

        private void Model_CodeChanged(Object? sender, String e)
        {
            Code = e;
        }

        private void OnLoadFile()
        {
            LoadFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveFile()
        {
            SaveFile?.Invoke(this, EventArgs.Empty);
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
    }   
}
