using System;
using System.Collections.Generic;
using System.Text;
using LadyIDE.Model;

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

        public MainViewModel(ApplicationModel model)
        {
            _model = model;
            _model.CodeChanged += new EventHandler<string>(Model_CodeChanged);

            LoadFileCommand = new DelegateCommand(param => OnLoadFile());
            SaveFileCommand = new DelegateCommand(param => OnSaveFile());
        }
        
        //Commands
        public DelegateCommand LoadFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }

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
    }   
}
