using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Forms;
using Sure.SharedObjects;

namespace EIS.XML
{
    public enum EInputFileType { ft2TPAir_Quarter = 4, ft2TPAir_Year = 5, ftPOD2 = 6, ftInventory = 7, ft1TPAir = 8, ftBases1stGroup = 1, ftBases2dGroup = 2, ftBases3dGroup = 3 }

    namespace Classes
    {
        public abstract class AEisXmlDocument : XmlDocument
        {
            string baseNs = String.Empty;
            XmlNamespaceManager nsmgr = null;

            public AEisXmlDocument()
            {
            }

            protected string DefaultNs
            {
                get
                {
                    if (String.IsNullOrEmpty(baseNs))
                    {
                        XmlNamespaceManager nsmgr = Sure.SharedObjects.SureTools.NameSpaceManager(this, out baseNs);

                        if (baseNs == String.Empty)
                        {
                            throw new Classes.EisException("Default namespace відсутній", RetVal.ERR_DEFAULT_NS_MISSED);
                        }
                    }
                    return baseNs;
                }
            }
            protected XmlNamespaceManager NsManager
            {
                get
                {
                    if (nsmgr == null)
                    {
                        nsmgr = Sure.SharedObjects.SureTools.NameSpaceManager(this, out baseNs);
                    }
                    return nsmgr;
                }
            }
        }

        public class TheEisXmlDocument : AEisXmlDocument
        {
            readonly Dictionary<EInputFileType, string> NamedInputFileType = new Dictionary<EInputFileType, string>()
        {
             { EInputFileType.ft2TPAir_Quarter,"2ТП(повітря) - квартальна" }, 
             { EInputFileType.ft2TPAir_Year,"2ТП(повітря) - річна" }, 
             { EInputFileType.ftPOD2,"ПОД-2" }, 
             { EInputFileType.ftInventory,"Інвентаризація" }, 
             { EInputFileType.ft1TPAir,"1ТП(повітря)" }, 
             { EInputFileType.ftBases1stGroup,"Обґрунтування обсягів викидів(об'єкт 1-ї групи)" }, 
             { EInputFileType.ftBases2dGroup,"Обґрунтування обсягів викидів(об'єкт 2-ї групи)" }, 
             { EInputFileType.ftBases3dGroup,"Обґрунтування обсягів викидів(об'єкт 3-ї групи)" }
        };
            public event DoPreprocessing Preprocessing;
            public delegate void DoPreprocessing(IArgument iArg, IFileUtil uFile);

            public TheEisXmlDocument()
            {
                Preprocessing += new DoPreprocessing(UpgradeXsiSchemaLocation);
                Preprocessing += new DoPreprocessing(PrepareCodes2Validate);
            }

            public EInputFileType InputFileType(IArgument iArg)
            {
                string cargoType = String.Empty;
                XmlNode xnode = SelectSingleNode(String.Format("//{0}:CargoType", DefaultNs), NsManager);

                if (xnode != null)
                {
                    cargoType = xnode.InnerText;
                }
                else
                {
                    throw new Classes.EisException("Невірна струтура документу (тег CargoType не знайдено)", RetVal.ERR_CARGO_TYPE_MISSED);
                }

                return iArg.arg2enum<EInputFileType>(NamedInputFileType, cargoType); ;
            }

            public void Prepare(IArgument iArg, IFileUtil uFile)
            {
                if (Preprocessing != null)
                    Preprocessing(iArg, uFile);
            }

            void UpgradeXsiSchemaLocation(IArgument iArg, IFileUtil uFile)
            {
                XmlAttribute xsiLocation = null;

                for (int i = 0; i < DocumentElement.Attributes.Count; i++)
                {
                    if (DocumentElement.Attributes[i].Name.IndexOf("xsi") == 0)
                    {
                        xsiLocation = DocumentElement.Attributes[i];
                        break;
                    }
                }
                if (xsiLocation != null)
                {
                    String location = xsiLocation.Value;
                    string delimStr = " ";
                    char[] delimiter = delimStr.ToCharArray();
                    string[] split = location.Split(delimiter, 64);
                    string location1 = String.Empty;
                    foreach (string strs in split)
                    {
                        int slashIndex = strs.LastIndexOf("/");
                        if (slashIndex >= 0)
                            location1 += strs.Substring(slashIndex + 1, strs.Length - slashIndex - 1);
                        else
                            location1 += strs;
                        location1 += " ";
                    }

                    if (xsiLocation.LocalName == "SchemaLocation")
                    {
                        if (uFile.IsFileReadOnly(iArg.FilePath))
                        {
                            if (MessageBox.Show("Файл має бути змінений для подальшої обробки. Для цього потрібно зкопіювати вхідний XML файл на жорсткий диск та повторити імпорт для копії. Бажаєте так зробити ?",
                                "Попередження",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning) == DialogResult.Yes)
                                throw new Exception("Імпорт перерваний для прекопіювання файлу");
                        }
                        else
                        {
                            DocumentElement.Attributes.Remove(xsiLocation);

                            XmlAttribute schemaLoc = CreateAttribute("xsi:schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
                            schemaLoc.Value = location1;
                            DocumentElement.SetAttributeNode(schemaLoc);
                        }
                    }
                }
            }

            private int Code2Int(XmlDocument xDoc, XmlNamespaceManager ns, String srcPath, String srcNodePath)
            {
                int changeNum = 0;
                XmlNodeList emissionList = xDoc.SelectNodes(srcPath, ns);

                foreach (XmlNode emissions in emissionList)
                {
                    XmlNode fSrcCode = emissions.SelectSingleNode(srcNodePath, ns);
                    string NumSeparator = ".";

                    if (fSrcCode != null && fSrcCode.InnerText.IndexOf(NumSeparator) >= 0)
                    {
                        String[] tokens = fSrcCode.InnerText.Split(NumSeparator[0]);
                        if (tokens.Length > 1)
                        {
                            string warning = String.Format("Номер джерела утворення {0} приведений до цілого значення {1} для забезпечення подальшої обробки.", fSrcCode.InnerText, tokens[0]);
                            fSrcCode.InnerText = tokens[0];
                            Sure.SharedObjects.SureTools.PutComment(fSrcCode, warning, true);
                            changeNum++;
                        }
                    }
                }
                return changeNum;
            }

            public void PrepareCodes2Validate(IArgument iArg, IFileUtil uFile)
            {
                /* 	To support decimal source codes   */
                int changeNum = 0;

                if (NsManager.HasNamespace("disp"))
                {
                    changeNum = Code2Int(this, NsManager, "//disp:srcEmission", "disp:srcCode") +
                                Code2Int(this, NsManager, "//disp:gasCleanEquipProps", "disp:srcCode");
                }
                if (NsManager.HasNamespace("emisrang"))
                {
                    changeNum = Code2Int(this, NsManager, "//emisrang:basicSrcEmissionRange", "emisrang:srcCode") +
                                Code2Int(this, NsManager, "//emisrang:srcEmissionRange", "emisrang:srcCode");
                }
                bool isChanged = (changeNum > 0);
                if (isChanged)
                {
                    if (uFile.IsFileReadOnly(iArg.FilePath))
                    {
                        if (MessageBox.Show("Файл містить коди джерел викидів у дійсних числах, що можуть бути приведені до цілих значень для подальшої обробки. Для цього потрібно зкопіювати вхідний XML файл на жорсткий диск та повторити імпорт для копії. Бажаєте так зробити ?",
                                      "Попередження",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Warning) == DialogResult.Yes)
                            throw new Exception("Імпорт перерваний для прекопіювання файлу");
                    }
                    else
                    {
                        if (MessageBox.Show("Файл містить коди джерел викидів у дійсних числах, що можуть бути приведені до цілих значень для подальшої обробки. Зробити це ?",
                                      "Попередження",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Warning) == DialogResult.Yes)
                            Save(iArg.FilePath);
                    }
                }
            }

            public XmlSchemaSet Schemas2Use(IArgument iArg, IFileUtil uFile, out RetVal retVal)
            {
                var myXmlSchemaCollection = new XmlSchemaSet();

                String schemeName = String.Empty;
                EInputFileType cargoType = InputFileType(iArg);
                switch (cargoType)
                {
                    case EInputFileType.ft2TPAir_Year:
                        {
                            schemeName = uFile.GenPath(iArg.SystemPath, "twoTpAirYear.xsd");
                            myXmlSchemaCollection.Add("TTPA_Year", new XmlTextReader(@schemeName));
                            myXmlSchemaCollection.Add("TTPAir", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "twoTpAir.xsd")));
                            myXmlSchemaCollection.Add("root", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "root.xsd")));
                            myXmlSchemaCollection.Add("poll", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "pollutant.xsd")));
                            myXmlSchemaCollection.Add("measure", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "measure.xsd")));
                            myXmlSchemaCollection.Add("equip", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "equipment.xsd")));
                            myXmlSchemaCollection.Add("geo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "geo.xsd")));
                            myXmlSchemaCollection.Add("cargo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "cargo.xsd")));
                            retVal = EIS.XML.RetVal.SUCCESS;
                            break;
                        }
                    case EInputFileType.ft2TPAir_Quarter:
                        {
                            schemeName = uFile.GenPath(iArg.SystemPath, "twoTpAirQuater.xsd");
                            myXmlSchemaCollection.Add("TTPA_Quater", new XmlTextReader(@schemeName));
                            myXmlSchemaCollection.Add("TTPAir", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "twoTpAir.xsd")));
                            myXmlSchemaCollection.Add("root", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "root.xsd")));
                            myXmlSchemaCollection.Add("poll", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "pollutant.xsd")));
                            myXmlSchemaCollection.Add("measure", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "measure.xsd")));
                            myXmlSchemaCollection.Add("equip", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "equipment.xsd")));
                            myXmlSchemaCollection.Add("geo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "geo.xsd")));
                            myXmlSchemaCollection.Add("cargo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "cargo.xsd")));
                            retVal = EIS.XML.RetVal.SUCCESS;
                            break;
                        }
                    case EInputFileType.ftPOD2:
                        {
                            schemeName = uFile.GenPath(iArg.SystemPath, "pod2.xsd");
                            myXmlSchemaCollection.Add("POD2", new XmlTextReader(@schemeName));
                            myXmlSchemaCollection.Add("TTPAir", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "twoTpAir.xsd")));
                            myXmlSchemaCollection.Add("root", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "root.xsd")));
                            myXmlSchemaCollection.Add("poll", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "pollutant.xsd")));
                            myXmlSchemaCollection.Add("measure", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "measure.xsd")));
                            myXmlSchemaCollection.Add("equip", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "equipment.xsd")));
                            myXmlSchemaCollection.Add("geo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "geo.xsd")));
                            myXmlSchemaCollection.Add("cargo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "cargo.xsd")));
                            retVal = EIS.XML.RetVal.SUCCESS;
                            break;
                        }
                    case EInputFileType.ftInventory:
                        {
                            schemeName = uFile.GenPath(iArg.SystemPath, "invent.xsd");
                            myXmlSchemaCollection.Add("invent", new XmlTextReader(@schemeName));
                            myXmlSchemaCollection.Add("TTPAir", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "twoTpAir.xsd")));
                            myXmlSchemaCollection.Add("root", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "root.xsd")));
                            myXmlSchemaCollection.Add("poll", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "pollutant.xsd")));
                            myXmlSchemaCollection.Add("measure", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "measure.xsd")));
                            myXmlSchemaCollection.Add("equip", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "equipment.xsd")));
                            myXmlSchemaCollection.Add("geo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "geo.xsd")));
                            myXmlSchemaCollection.Add("cargo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "cargo.xsd")));
                            myXmlSchemaCollection.Add("graphics", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "graphics.xsd")));
                            myXmlSchemaCollection.Add("subst", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "substance.xsd")));
                            myXmlSchemaCollection.Add("infores", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "infores.xsd")));
                            myXmlSchemaCollection.Add("method", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "method.xsd")));
                            retVal = EIS.XML.RetVal.SUCCESS;
                            break;
                        }
                    case EInputFileType.ft1TPAir:
                        {
                            schemeName = uFile.GenPath(iArg.SystemPath, "oneTpAir.xsd");
                            myXmlSchemaCollection.Add("OTPAir", new XmlTextReader(@schemeName));
                            myXmlSchemaCollection.Add("TTPAir", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "twoTpAir.xsd")));
                            myXmlSchemaCollection.Add("root", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "root.xsd")));
                            myXmlSchemaCollection.Add("poll", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "pollutant.xsd")));
                            myXmlSchemaCollection.Add("measure", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "measure.xsd")));
                            myXmlSchemaCollection.Add("equip", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "equipment.xsd")));
                            myXmlSchemaCollection.Add("geo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "geo.xsd")));
                            myXmlSchemaCollection.Add("cargo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "cargo.xsd")));
                            myXmlSchemaCollection.Add("largeSrc", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "largesrc.xsd")));
                            retVal = EIS.XML.RetVal.SUCCESS;
                            break;
                        }
                    case EInputFileType.ftBases1stGroup:
                    case EInputFileType.ftBases2dGroup:
                    case EInputFileType.ftBases3dGroup:
                        {
                            schemeName = uFile.GenPath(iArg.SystemPath, String.Format("bases{0}.xsd", (int)cargoType));
                            myXmlSchemaCollection.Add("bases", new XmlTextReader(@schemeName));
                            /*
                            myXmlSchemaCollection.Add("sdzone", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "sdzone.xsd")));
                            myXmlSchemaCollection.Add("poll", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "pollutant.xsd")));
                            myXmlSchemaCollection.Add("assets", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "assets.xsd")));
                            myXmlSchemaCollection.Add("infores", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "infores.xsd")));
                            myXmlSchemaCollection.Add("mater", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "material.xsd")));
                            myXmlSchemaCollection.Add("fuel", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "fuel.xsd")));
                            myXmlSchemaCollection.Add("root", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "root.xsd")));
                            myXmlSchemaCollection.Add("equip", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "equipment.xsd")));
                            myXmlSchemaCollection.Add("cargo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "cargo.xsd")));
                            myXmlSchemaCollection.Add("omeasure", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "omeasure.xsd")));
                            myXmlSchemaCollection.Add("geo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "geo.xsd")));
                            myXmlSchemaCollection.Add("mime", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "mime.xsd")));
                            myXmlSchemaCollection.Add("invcargo", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "invbase.xsd")));
                            myXmlSchemaCollection.Add("invent", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "invtype.xsd")));

                            myXmlSchemaCollection.Add("emisposs", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "emisposs.xsd")));
                            myXmlSchemaCollection.Add("measure", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "basesmeasure.xsd")));
                            myXmlSchemaCollection.Add("emisrang", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "emisrang.xsd")));
                            myXmlSchemaCollection.Add("req", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "requir.xsd")));
                            myXmlSchemaCollection.Add("besttech", new XmlTextReader(uFile.GenPath(iArg.SystemPath, "besttech.xsd")));
                            */
                            retVal = EIS.XML.RetVal.SUCCESS;
                            break;
                        }
                    default:
                        {
                            throw new Classes.EisException(String.Format("Тип документу {0} не підтримується", cargoType), RetVal.ERR_CARGO_TYPE_UNSUPPORTED);
                        }
                }
                return myXmlSchemaCollection;
            }

            public RetVal Load(IArgument iArg, IFileUtil uFile)
            {
                RetVal retVal = RetVal.SUCCESS;

                XmlReaderSettings settings = new XmlReaderSettings();

                XmlReader reader = XmlReader.Create(iArg.FilePath, settings);
                try
                {
                    Load(reader);
                }
                finally
                {
                    reader.Close();
                }
                //settings.ValidationType = ValidationType.Schema;
                //settings.Schemas = Schemas2Use(iArg, uFile, out retVal);
                return retVal;
            }

            public RetVal Validate(IArgument iArg, IFileUtil uFile, ILogger iLog)
            {
                RetVal retVal = RetVal.SUCCESS;
                Schemas = Schemas2Use(iArg, uFile, out retVal);
                Validate(
                    delegate(object sender, ValidationEventArgs args)
                    {
                        iLog.LogMessage("\tПомилка перевiркi сумiсностi : " + args.Message, MsgType.Error);

                        if (args.Severity == XmlSeverityType.Warning)
                        {
                            iLog.LogMessage("Схему для первiрки сумiсностi не знайдено", MsgType.Warning);
                        }
                        else
                            if (args.Severity == XmlSeverityType.Error)
                                iLog.LogMessage("Помилка при перевiрцi сумiсностi ", MsgType.Error);

                        if (args.Exception != null) // XSD schema validation error
                        {
                            iLog.LogMessage("Помилка при  при перевiрцi сумiсностi : " + args.Exception.SourceUri + "," + args.Exception.LinePosition + "," + args.Exception.LineNumber, MsgType.Error);
                        }
                        retVal = RetVal.ERR_VALIDATE;
                    }
                    );
                return retVal;
            }
            public RetVal RegisterEisEntity(IArgument iArg, ILogger iLog, IFileUtil uFile, IConfig eConfig)
            {
                try
                {
                    SetRegistration(DocumentElement, eConfig);

                    // Write out data as XML			
                    Save(uFile.GenPath(eConfig.OutPath, eConfig.EisFileName(eConfig.SequenceNumber)));

                    return RetVal.SUCCESS;
                }
                catch (XmlException e)
                {
                    iLog.LogMessage("XmlException: " + e.Message, MsgType.Error);
                    return RetVal.ERR_REGISTRATION;
                }
                catch (Exception e)
                {
                    iLog.LogMessage("Exception: " + e.Message, MsgType.Error);
                    return RetVal.ERR_REGISTRATION;
                }
            }

            private void SupressNullAttr(XmlNode node)
            {
                XmlAttributeCollection attrColl = node.Attributes;
                if (attrColl["xsi:nil"] != null)
                    attrColl.Remove(attrColl["xsi:nil"]);
            }

            private void SetRegistration(XmlNode node, IConfig eConfig)
            {
                if (node.Name == "root:RegistrationDate")
                {
                    SupressNullAttr(node);
                    node.InnerText = DateTime.Today.ToString("yyyy-MM-dd");
                }

                if (node.Name == "root:RegisteredHosts")
                {
                    SupressNullAttr(node);

                    node.InnerText = eConfig.RegisteredHost;
                }

                if (node.Name == "root:SequenceNumber")
                {
                    SupressNullAttr(node);

                    node.InnerText = eConfig.NextSequenceNumber;
                }

                node = node.FirstChild;
                while (node != null)
                {
                    SetRegistration(node, eConfig);
                    node = node.NextSibling;
                }
            }
        }
    }
}
