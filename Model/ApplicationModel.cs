using System.Timers;
using LadyIDE.Persistence;

namespace LadyIDE.Model
{
    public class ApplicationModel
    {
        private readonly IFileDataAccess _dataAccess;
        public string Code { get; set; } = string.Empty;

        //Events
        public event EventHandler<string>? CodeChanged;

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
            CodeChanged?.Invoke(this, Code);
        }

        //Save
        public async Task SaveFileAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, Code);
        }
    }

}
