using System.Timers;
using LadyIDE.Persistence;

namespace LadyIDE.Model
{
    public class ApplicationModel
    {
        private readonly IFileDataAccess _dataAccess;
        private string _code = string.Empty; 
        public string Code 
        {
            get => _code; 
            set 
            { 
                if (_code == value) 
                    return; _code = value; 
                CodeChanged?.Invoke(this, _code); 
            } 
        }

        //Events
        public event EventHandler<string>? CodeChanged;
        public event EventHandler<(string,string)>? FileLoaded;
        public event EventHandler<string>? FileSaved;

        public ApplicationModel(IFileDataAccess dataAccess)
        { 
            _dataAccess = dataAccess;
        }

        //Load
        public async Task LoadFileAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            Code = await _dataAccess.LoadAsync(path);
            FileLoaded?.Invoke(this, (Code, path));
        }

        //Save
        public async Task SaveFileAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, Code);
            FileSaved?.Invoke(this, path);
        }
    }

}
