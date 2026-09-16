using System.IO;

namespace LadyIDE.Persistence
{
    public class FileDataAccess : IFileDataAccess
    {
        public async Task<string> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch
            {
                throw new FileDataException("Failed to load file.");
            }
        }

        public async Task SaveAsync(string path, string code)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteAsync(code);
                }
            }
            catch
            {
                throw new FileDataException("Failed to save file.");
            }
        }
    }
}
