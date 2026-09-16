
using System.Timers;
using LadyIDE.Persistence;

namespace LadyIDE.Model
{
    public class ApplicationModel
    {
        private readonly IFileDataAccess _dataAccess;
        public ApplicationModel(IFileDataAccess dataAccess)
        { 
            _dataAccess = dataAccess;
        }

        //Load
        public async Task LoadFileAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");
        }

        //Save
        public async Task SaveFileAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");
        }
    }

}
