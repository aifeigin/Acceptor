using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EIS.XML
{
    /// <summary>
    /// Output to console
    /// </summary>
    /// 
    namespace Utils
    { 
        public class UKConsole
        {
            static public void WriteLine()
            {
                Console.WriteLine();
            }
            static public void ReadLine()
            {
                Console.ReadLine();
            }
            static public string Prepare(string str)
            {
                str = str.Replace('і', 'i');
                str = str.Replace('ґ', 'г');
                return str;
            }
            static public void WriteLine(string str)
            {
                str = Prepare(str);
                Console.WriteLine(str);
            }
            static public void WriteLine(string str, string str2)
            {
                str = Prepare(str);
                str2 = Prepare(str2);
                Console.WriteLine(str, str2);
            }
            static public TextWriter Error()
            {
                return Console.Error;
            }
            public static void WaitForConfirm()
            {
                Console.WriteLine("Натиснiть клавiшу Enter для закриття вiкна ...");
                Console.ReadLine();
            }
        }
    }
}
