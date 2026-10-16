
using System;
using LadyIDE.WPF.CanvasElements;

namespace LadyIDE.WPF.ViewModel
{
    public class LogicEditorViewModel : ViewModelBase
    {
        private Logic? _logic;
        private string _text = string.Empty;
        private string _originalText = string.Empty;
        private int _caretOffset;
        private string _caretPositionText = "Ln: 1, Col: 1";

        public Logic? Logic
        {
            get => _logic;
            private set
            {
                if (_logic == value) return;

                _logic = value;
                OnPropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                value ??= string.Empty;

                if (_text == value) return;

                _text = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDirty));
            }
        }

        public int CaretOffset
        {
            get => _caretOffset;
            private set
            {
                int newOffset = Math.Clamp(value, 0, Text.Length);

                if (_caretOffset == newOffset) return;

                _caretOffset = newOffset;
                OnPropertyChanged();
            }
        }

        public string CaretPositionText
        {
            get => _caretPositionText;
            private set
            {
                if (_caretPositionText == value) return;

                _caretPositionText = value;
                OnPropertyChanged();
            }
        }

        public bool IsDirty => Text != _originalText;

        public DelegateCommand InsertTokenCommand { get; }

        public DelegateCommand InsertCodeCommand { get; }

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand CancelCommand { get; }

        public event Action<bool>? RequestClose;

        public LogicEditorViewModel()
        {
            InsertTokenCommand = new DelegateCommand(InsertToken);
            InsertCodeCommand = new DelegateCommand(InsertCode);
            SaveCommand = new DelegateCommand(_ => Logic is not null, Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        public void LoadLogic(Logic logic)
        {
            Logic = logic;
            _originalText = logic.Text ?? string.Empty;
            Text = _originalText;
            CaretOffset = 0;
            CaretPositionText = "Ln: 1, Col: 1";

            OnPropertyChanged(nameof(IsDirty));
        }

        public void UpdateCaretPosition(int line, int column, int offset)
        {
            CaretOffset = offset;
            CaretPositionText = $"  Ln: {line}, Col: {column}";
        }

        private void InsertToken(object? parameter)
        {
            if (parameter is not string token || token.Length == 0) return;

            int offset = Math.Clamp(CaretOffset, 0, Text.Length);
            int newCaretOffset = offset + token.Length;

            if (token == "()") newCaretOffset = offset + 1;

            Text = Text.Insert(offset, token);
            CaretOffset = newCaretOffset;
        }

        private void InsertCode(object? parameter)
        {
            const string codeBlock = "<code></code>";

            int offset = Math.Clamp(CaretOffset, 0, Text.Length);

            Text = Text.Insert(offset, codeBlock);
            CaretOffset = offset + "<code>".Length;
        }

        private void Save(object? parameter)
        {
            if (Logic is null) return;

            Logic.Text = Text;
            _originalText = Text;

            OnPropertyChanged(nameof(IsDirty));

            RequestClose?.Invoke(true);
        }

        private void Cancel(object? parameter)
        {
            RequestClose?.Invoke(false);
        }
    }
}
