using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EIS.XML
{
    public enum EMode { Register , Validate }    

    public interface IArgument
    {
        EMode Mode { get; }
        string FilePath { get; }
        string SystemPath { get; }
        T arg2enum<T>(Dictionary<T, string> dict, string args);
    }
    namespace Classes
    {       
        public class InterpretArgs : IArgument
        {
            readonly string[] args;

            readonly Dictionary<EMode, string> NamedMode = new Dictionary<EMode, string>()
            {
                {  EMode.Register, "-r" },
                {  EMode.Validate, "-v" }
            };            
                        
            public InterpretArgs(string[] xArgs)
            {
                args = xArgs;
            }

            string getNArg(int n)
            {
                var arg=args.Length > n ? args[n] : null;
                if(arg==null)
                  throw new Exception("Невірний формат визову програми. Відсутній параметр вхідної строки.");
                return arg; 
            }

            public T arg2enum<T>( Dictionary<T, string> dict, string args)
            {
                var recordSet = from rec in dict where rec.Value == args select rec.Key;

                if (recordSet == null || recordSet.Count() == 0)
                    throw new Exception("Невірний формат визову програми. Неочікуваний перший параметр вхідної строки.");
                if (recordSet.Count() > 1)
                    throw new Exception("Cистемна помилка - не можлива однозначна ідентифікація режиму.");

                return recordSet.First();
            }

            public EMode Mode 
            {
                get
                {
                    var argMode=getNArg(0);
                    return arg2enum<EMode>(NamedMode, argMode);
                }
            }
            public string FilePath 
            {
                get
                {
                    return getNArg(1); 
                }
            }
            public string SystemPath 
            {
                get
                {
                    return getNArg(2); 
                }
            }        
        }
    }
}
