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
            _mainViewModel.SaveAsFile += new EventHandler(ViewModel_SaveAsFile);
            _mainViewModel.SaveFile += new EventHandler(ViewModel_SaveFile);
            _mainViewModel.CodeChanged += new EventHandler<string>(ViewModel_CodeChanged);

            _view = new MainWindow();
            _view.DataContext = _mainViewModel;
            _view.WindowState = WindowState.Maximized;
            _view.CodeEditor.TextChanged += CodeEditor_TextChanged;
            _view.CodeEditor.TextArea.Caret.PositionChanged += ViewModel_UpdateCaretPosition;
            _view.Show();
        }

        private void ViewModel_UpdateCaretPosition(object? sender, EventArgs e)
        {
            _mainViewModel.UpdateCaretPosition(_view.CodeEditor.TextArea.Caret.Line, _view.CodeEditor.TextArea.Caret.Column);
        }

        private void CodeEditor_TextChanged(object? sender, EventArgs e)
        {
            _model.Code = _view.CodeEditor.Text;
        }

        private void ViewModel_CodeChanged(object? sender, string e)
        {
            _view.CodeEditor.Text = e;
        }

        private async void ViewModel_LoadFile(object? sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Load file";
                openFileDialog.Filter = "LADY files (*.lady)|*.lady";
                openFileDialog.DefaultExt = ".lady";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadFileAsync(openFileDialog.FileName);
                    _mainViewModel.HasUnsavedChanges = false;
                    _mainViewModel.CurrentFilePath = openFileDialog.FileName;
                }
            }
            catch (FileDataException exception)
            {
                MessageBox.Show(exception.Message, "LadyIDE", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void ViewModel_SaveAsFile(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save file";
                saveFileDialog.Filter = "LADY files (*.lady)|*.lady";
                saveFileDialog.DefaultExt = ".lady";
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == true)
                {
                    await _model.SaveFileAsync(saveFileDialog.FileName);
                    _mainViewModel.CurrentFilePath = saveFileDialog.FileName;
                    _mainViewModel.HasUnsavedChanges = false;
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
                if(!string.IsNullOrEmpty(_mainViewModel.CurrentFilePath))
                {
                    await _model.SaveFileAsync(_mainViewModel.CurrentFilePath);
                    _mainViewModel.HasUnsavedChanges = false;
                }
            }
            catch (FileDataException exception)
            {
                MessageBox.Show(exception.Message, "LadyIDE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
