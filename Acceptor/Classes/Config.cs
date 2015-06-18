using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;


namespace EIS.XML
{
    public interface IConfig
    {
        string  RegisteredHost { get; }
        string  OutPath { get; }
        String  SequenceNumber { get; }
        String  NextSequenceNumber { get; }
        string  EisFileName(string seqNum);
        void    Load(IArgument iArg, IFileUtil iFile);
        void    Save(IArgument iArg, IFileUtil uFile);
    }
    namespace Classes
    { 
        public class Config : AEisXmlDocument, IConfig
        {
            const string CONF_FILE_NAME = "eisconf.xml";
            public string EisFileName(string seqNum)
            {
                return "eis" + seqNum + ".xml";
            }
           
            public void Save(IArgument iArg,IFileUtil uFile)
            {
                XmlTextWriter xml = new XmlTextWriter(uFile.GenPath(iArg.SystemPath, CONF_FILE_NAME), Encoding.GetEncoding("windows-1251"));
                try
                {
                    xml.Namespaces = true;
                    xml.Formatting = Formatting.Indented;
                    xml.Indentation = 4;
                    Save(xml);
                }
                finally
                {
                    xml.Close();
                }
            }

            private string GetConfigValue(string XPath)
            {
                XmlNode rNode = SelectSingleNode(XPath, NsManager);
                if (rNode == null)
                    throw new EisException("Config error", RetVal.ERR_CONFIG);
                return rNode.InnerText;
            }

            public string RegisteredHost
            {
                get
                {
                    return GetConfigValue("//econf:eisSettings/econf:RegisteredHost");
                }
            }

            public string OutPath
            {
                get
                {
                    string outPath = GetConfigValue("//econf:eisSettings/econf:OutputDir");
                    if (!Directory.Exists(outPath))
                        Directory.CreateDirectory(outPath);
                    return outPath;
                }
            }


            public String NextSequenceNumber
            {
                get
                {
                    XmlNode rNode = SelectSingleNode("//econf:eisSettings/econf:SequenceNumber", NsManager);
                    if (rNode == null)
                        throw new EisException("Config error", RetVal.ERR_CONFIG);
                    Int32 ret = Int32.Parse(rNode.InnerText);
                    ret++;
                    string seqNum = ret.ToString();
                    rNode.InnerText = seqNum;

                    return seqNum;
                }
            }

            public String SequenceNumber
            {
                get
                {
                    return GetConfigValue("//econf:eisSettings/econf:SequenceNumber");
                }
            }

            public void Load(IArgument iArg, IFileUtil iFile)
            {
                XmlTextReader reader = new XmlTextReader(iFile.GenPath(iArg.SystemPath, CONF_FILE_NAME));
                try
                {
                    reader.MoveToContent();		// Moves the reader to the root node.							
                    Load(reader);
                }
                finally
                {
                    reader.Close();
                }
            }
        }
    }
}
