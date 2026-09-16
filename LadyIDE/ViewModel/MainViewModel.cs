using System;
using System.Collections.Generic;
using System.Text;
using LadyIDE.Model;

namespace LadyIDE.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LadyIDE.Model.Model _model;

        public MainViewModel(LadyIDE.Model.Model model)
        {
            _model = model;
        }
    }   
}
