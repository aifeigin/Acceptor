using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EIS.XML
{ 
    namespace Classes
    {
        public class EisException : Exception
        {
            RetVal code;
            public EisException(string message) : base(message) 
            {
            }

            public EisException(string message, RetVal aCode)
                : base(message) 
            { 
                code = aCode; 
            }

            public EisException(string message, Exception innerEx) : base(message, innerEx) 
            {
            }
            public RetVal Code
            {
                get
                {
                    return code;
                }
            }
        }
    }
}