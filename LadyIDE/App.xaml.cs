using LadyIDE.Model;
using LadyIDE.WPF.View;
using LadyIDE.WPF.ViewModel;
using System.Configuration;
using System.Data;
using System.Timers;
using System.Windows;

namespace LadyIDE.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private LadyIDE.Model.Model _model = null!;
        private MainViewModel _mainViewModel = null!;
        private MainWindow _view = null!;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _model = new LadyIDE.Model.Model();

            _mainViewModel = new MainViewModel(_model);

            _view = new MainWindow();
            _view.DataContext = _mainViewModel;
            _view.Height = 700;
            _view.Width = 700;
            _view.Show();
        }
    }

}
