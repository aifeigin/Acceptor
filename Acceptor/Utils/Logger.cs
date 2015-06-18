using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace EIS.XML
{
    public enum MsgType { Information, Warning, Error }
    public interface ILogger
    {
        void LogMessage(string message, MsgType msgType);
    }
    namespace Utils
    {        
        public class Logger : ILogger
        {
            public void LogMessage(string message, MsgType msgType)
            {
                TextWriter errorWriter = UKConsole.Error();
                errorWriter.WriteLine(UKConsole.Prepare(message));
            }
        }
    }
}
