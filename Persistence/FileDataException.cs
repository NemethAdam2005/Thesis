using System;
using System.Collections.Generic;
using System.Text;

namespace LadyIDE.Persistence
{
    public class FileDataException : Exception
    {
        public FileDataException(string message) : base(message)
        {
        }
    }   
}
