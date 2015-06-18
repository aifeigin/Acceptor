using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EIS.XML
{
    public interface IFileUtil
    {
        bool IsFileReadOnly(string FileName);
        string GenPath(string inputPath, string fileName);
    }
    namespace Utils
    { 
        public class FileUtil : IFileUtil
        {
            // Returns wether a file is read-only. 
            public bool IsFileReadOnly(string FileName)
            {
                // Create a new FileInfo object.
                FileInfo fInfo = new FileInfo(FileName);

                // Return the IsReadOnly property value. 
                return fInfo.IsReadOnly;
            }

            public string GenPath(string inputPath, string fileName)
            {
                if (!inputPath.EndsWith("\\"))
                    inputPath += "\\";
                return String.Format("{0}{1}", inputPath, fileName);
            }
        }
    }
}
