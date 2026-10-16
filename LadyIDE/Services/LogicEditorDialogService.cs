
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LadyIDE.WPF.View;
using LadyIDE.WPF.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace LadyIDE.WPF.Services
{
    public class LogicEditorDialogService
    {
        public void ShowDialog(LogicEditorViewModel viewModel)
        {
            LogicEditorWindow window = new()
            {
                Owner = Application.Current.MainWindow,
                DataContext = viewModel
            };

            Uri syntaxUri = new(
                "pack://application:,,,/LadyIDE.WPF;component/Styles/Syntax/LogicEditorSyntax.xshd",
                UriKind.Absolute);

            var resource = Application.GetResourceStream(syntaxUri)
                ?? throw new InvalidOperationException("LogicEditorSyntax.xshd resource was not found.");

            using (resource.Stream)
            using (XmlReader reader = XmlReader.Create(resource.Stream))
            {
                window.LogicTextEditor.SyntaxHighlighting =
                    HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }

            window.LogicTextEditor.Text = viewModel.Text;

            void OnRequestClose(bool saved)
            {
                window.DialogResult = saved;
            }

            void OnTextChanged(object? sender, EventArgs e)
            {
                viewModel.Text = window.LogicTextEditor.Text;
            }

            void OnCaretPositionChanged(object? sender, EventArgs e)
            {
                var caretPosition = window.LogicTextEditor.TextArea.Caret.Position;
                viewModel.UpdateCaretPosition(caretPosition.Line, caretPosition.Column, window.LogicTextEditor.TextArea.Caret.Offset);
            }

            void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(LogicEditorViewModel.Text) && window.LogicTextEditor.Text != viewModel.Text)
                    window.LogicTextEditor.Text = viewModel.Text;

                if (e.PropertyName == nameof(LogicEditorViewModel.CaretOffset))
                {
                    int offset = Math.Clamp(
                        viewModel.CaretOffset,
                        0,
                        window.LogicTextEditor.Document.TextLength);

                    if (window.LogicTextEditor.TextArea.Caret.Offset != offset)
                        window.LogicTextEditor.TextArea.Caret.Offset = offset;

                    window.LogicTextEditor.Focus();
                    Keyboard.Focus(window.LogicTextEditor.TextArea);
                }
            }

            window.Closing += (sender, e) =>
            {
                viewModel.RequestClose -= OnRequestClose;
                viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                window.LogicTextEditor.TextChanged -= OnTextChanged;
                window.LogicTextEditor.TextArea.Caret.PositionChanged -= OnCaretPositionChanged;
            };

            viewModel.RequestClose += OnRequestClose;
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            window.LogicTextEditor.TextChanged += OnTextChanged;
            window.LogicTextEditor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

            window.ShowDialog();
        }
    }
}
