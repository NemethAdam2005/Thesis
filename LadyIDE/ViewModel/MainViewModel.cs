using System;
using System.Collections.Generic;
using System.Text;
using LadyIDE.Model;

namespace LadyIDE.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ApplicationModel _model;

        public MainViewModel(ApplicationModel model)
        {
            _model = model;
            LoadCommand = new DelegateCommand(param => OnLoadFile());
            SaveCommand = new DelegateCommand(param => OnSaveFile());
        }

        //Commands
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        //Events
        public event EventHandler? LoadFile;
        public event EventHandler? SaveFile;

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
