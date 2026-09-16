using LadyIDE.Model;
using LadyIDE.WPF.View;
using LadyIDE.WPF.ViewModel;
using LadyIDE.Persistence;
using Microsoft.Win32;
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
        private ApplicationModel _model = null!;
        private MainViewModel _mainViewModel = null!;
        private MainWindow _view = null!;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _model = new ApplicationModel(new FileDataAccess());

            _mainViewModel = new MainViewModel(_model);
            _mainViewModel.LoadFile += new EventHandler(ViewModel_LoadFile);
            _mainViewModel.SaveFile += new EventHandler(ViewModel_SaveFile);

            _view = new MainWindow();
            _view.DataContext = _mainViewModel;
            _view.Height = 700;
            _view.Width = 700;
            _view.Show();
        }

        private async void ViewModel_LoadFile(object? sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Load file";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadFileAsync(openFileDialog.FileName);
                }
            }
            catch (FileDataException exception)
            {
                MessageBox.Show(exception.Message, "LadyIDE", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void ViewModel_SaveFile(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save file";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveFileAsync(saveFileDialog.FileName);
                    }
                    catch (FileDataException exception)
                    {
                        MessageBox.Show(exception.Message, "LadyIDE", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (FileDataException exception)
            {
                MessageBox.Show(exception.Message, "LadyIDE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
