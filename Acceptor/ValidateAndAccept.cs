namespace EIS.XML
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using System.Text;
    using System.Collections.Generic;
    using System.Windows.Forms;    

    public enum RetVal
    {
        SUCCESS = 1,
        ERR_UNKNOWN = 0,
        ERR_CALL = -1,
        ERR_CONFIG = -2,
        ERR_VALIDATE = -3,
        ERR_REGISTRATION = -4,
        ERR_DEFAULT_NS_MISSED = -5,
        ERR_CARGO_TYPE_MISSED = -6,
        ERR_CARGO_TYPE_UNSUPPORTED = -7,
        ERR_APPLY_TARGET_MISSED = -8
    }        


    public class ProcessEISXml
    {
        public RetVal Run(String[] args)
        {
            RetVal ret = RetVal.ERR_UNKNOWN;

            IArgument   iArg    = new Classes.InterpretArgs(args);
            ILogger     iLog    = new Utils.Logger();
            IFileUtil   uFile   = new Utils.FileUtil();
            IConfig     eConfig = null;
            if (iArg.Mode == EMode.Register)
            {
                try
                {
                    eConfig = new Classes.Config();
                    eConfig.Load(iArg, uFile);
                }
                catch (Exception e)
                {
                    throw new Classes.EisException("Exception: " + e.Message, RetVal.ERR_CONFIG);
                }
            }
            try
            {
                var eisXmlDocument = new Classes.TheEisXmlDocument();
                ret = eisXmlDocument.Load(iArg, uFile);
                if (ret == RetVal.SUCCESS)
                    eisXmlDocument.Prepare(iArg,uFile);
                if (ret == RetVal.SUCCESS)
                    ret = eisXmlDocument.Validate(iArg, uFile,iLog);
                if (ret == RetVal.SUCCESS && eConfig != null)
                {
                    ret = eisXmlDocument.RegisterEisEntity(iArg, iLog, uFile, eConfig);
                    if (ret == RetVal.SUCCESS)
                    {
                        eConfig.Save(iArg,uFile);
                    }
                }
                Utils.UKConsole.WriteLine("Файл сумісний");
                Utils.UKConsole.WaitForConfirm();
                return ret;
            }
            catch (Classes.EisException e)
            {
                iLog.LogMessage("Exception: " + e.Message, MsgType.Error);
                Utils.UKConsole.WaitForConfirm();
                return e.Code;
            }
            catch (Exception e)
            {
                iLog.LogMessage("Exception: " + e.Message, MsgType.Error);
                Utils.UKConsole.WaitForConfirm();
                return RetVal.ERR_UNKNOWN;
            }
        }

        /*	

            private int Validate()
            {
                try
                {
                    // Set the validation event handler
                    eisXmlValidatingReader.ValidationEventHandler += new ValidationEventHandler (this.ValidationEventHandle);

                    // Read XML data
                    while (eisXmlValidatingReader.Read()){}
                    Validator.UKConsole.WriteLine ("Перевiрку закiнчено. Электроний файл {0}", (Success==true ? " сумiсний " : " несумiсний "));
                    return Success==true ? RetVal.SUCCESS : RetVal.ERR_VALIDATE; 
                }
                catch (XmlException e)
                {
                    TextWriter errorWriter = Validator.UKConsole.Error();
                    errorWriter.WriteLine("XmlException: " + e.Message);
                    return RetVal.ERR_UNKNOWN;
                }

                catch (Exception e)
                {
                    TextWriter errorWriter = Validator.UKConsole.Error();
                    errorWriter.WriteLine("Exception: " + e.Message);
                    Validator.UKConsole.WriteLine ("Exception: " + e.ToString());
                    return RetVal.ERR_UNKNOWN;
                }
                finally
                {
                    // Finished with XmlTextReader
                    if (eisXmlValidatingReader != null)
                        eisXmlValidatingReader.Close();
                }
            }
        */
    }
}


