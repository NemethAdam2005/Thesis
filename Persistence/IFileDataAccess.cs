using System;
using System.Collections.Generic;
using System.Text;

namespace LadyIDE.Persistence
{
    public interface IFileDataAccess
    {
        Task<string> LoadAsync(String path);

        Task SaveAsync(String path);
    }
}
