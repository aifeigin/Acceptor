using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EIS.Xml
{
    public static class Interpreter
    {
        static int arg2enum(Type T, Dictionary<int, string> dict, string args)
        {
            var recordSet = from rec in dict where rec.Value == args select rec.Key;

            if (recordSet == null || recordSet.Count() == 0)
                throw new Exception("Невірний формат визову програми. Неочікуваний перший параметр вхідної строки.");
            if (recordSet.Count() > 1)
                throw new Exception("Cистемна помилка - не можлива однозначна ідентифікація режиму.");

            return recordSet.First();
        }
    }
}
