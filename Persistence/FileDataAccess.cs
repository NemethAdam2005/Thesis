using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

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
                    String line = await reader.ReadLineAsync() ?? String.Empty;
                    
                    return line;
                }
            }
            catch
            {
                throw new FileDataException("Failed to load file.");
            }
        }

        public async Task SaveAsync(string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync("Saved!");
                }
            }
            catch
            {
                throw new FileDataException("Failed to save file.");
            }
        }
    }
}
