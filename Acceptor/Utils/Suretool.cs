using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace Sure
{
	public enum UserRoles	{ NONE='N', ADMIN_ROLE='A', USER_ROLE='U', SUPERVISOR_ROLE='S' };
	public enum DocType		{ NONE=0x0000, SYSTEM_DOC=0x0001, USER_DOC=0x0002 } 	

	public enum DocAttrib   
	{ 
		ATTR_IN_PROCESS = 0, 
		ATTR_DONE = 128, 
		ATTR_CANCELLED =  256,
		ATTR_CONSISTENT = 4, 
		ATTR_LOADED = 8,
		ATTR_CLIENTISSUE=16
	}
	public enum ClientType
	{
		ATTR_FIRM = 0x0001,
		ATTR_ENTREPRENEUR = 0x0002,
		ATTR_PERSON = 0x0004,
		ATTR_OPERATOR = 0x0400
	}
	namespace SharedObjects
	{
		/// <summary>
		/// Summary description for SureTools 
		/// </summary>
		public class SureTools
		{
			public SureTools()
			{
				//
				// TODO: Add constructor logic here
				//
			}

			static public XmlNamespaceManager NameSpaceManager(XmlDocument xDoc)
			{							
				XmlNamespaceManager NsMgr=new XmlNamespaceManager(xDoc.NameTable);
				XmlElement xelem=xDoc.DocumentElement;
				if(xelem != null) 
				{
					for(int i=0;i<xelem.Attributes.Count;i++)
					{
						if(xelem.Attributes[i].Name.IndexOf("xmlns")==0)
						{
							int nPos=xelem.Attributes[i].Name.IndexOf(":");
							if(nPos>0)
							{
								String prefix = xelem.Attributes[i].Name.Substring(nPos+1, xelem.Attributes[i].Name.Length-nPos-1);
								NsMgr.AddNamespace(prefix,xelem.Attributes[i].Value);	
							}
						}
					}
				}
				return 	NsMgr;	
			}

			static public XmlNamespaceManager NameSpaceManager(XmlDocument xDoc,out string baseNs)
			{							
				XmlNamespaceManager NsMgr=new XmlNamespaceManager(xDoc.NameTable);
				XmlElement xelem=xDoc.DocumentElement;
				baseNs=String.Empty;
				if(xelem != null) 
				{
					for(int i=0;i<xelem.Attributes.Count;i++)
					{
						if(xelem.Attributes[i].Name.IndexOf("xmlns")==0)
						{
							int nPos=xelem.Attributes[i].Name.IndexOf(":");
							if(nPos>0)
							{
								String prefix = xelem.Attributes[i].Name.Substring(nPos+1, xelem.Attributes[i].Name.Length-nPos-1);
								NsMgr.AddNamespace(prefix,xelem.Attributes[i].Value);	
							}
							else
								baseNs=xelem.Attributes[i].Value;
						}						
					}
				}
				return 	NsMgr;	
			}

			public static byte[] GetBuffer(XmlDocument xdoc)
			{
				byte[] bDoc=null;
				System.IO.MemoryStream  m=new System.IO.MemoryStream(xdoc.InnerXml.Length);		
				try
				{
					XmlTextWriter xWrite=new XmlTextWriter(m, System.Text.Encoding.GetEncoding("windows-1251"));			
					try
					{
						xdoc.Save(xWrite);
						bDoc=m.GetBuffer();
					}
					finally
					{
						xWrite.Flush();
						xWrite.Close();						
					}
				}
				finally
				{
				}
				return bDoc;
			}

			public static XmlDocument Bytes2Doc(byte[] bytes)
			{
				XmlDocument xDoc=new XmlDocument();

				if(bytes!=null) 
				{
					System.IO.MemoryStream m=new System.IO.MemoryStream();
					m.Write(bytes,0,bytes.Length);
					try
					{
						m.Seek(0,SeekOrigin.Begin);
						xDoc.Load(m);
					}
					catch(Exception e)
					{
						throw e;
					}
				} 
				return xDoc;
			}

			public static void SetNode(XmlNode baseNode,String nodeName,String nodeValue,XmlNamespaceManager ns)
			{
				XmlNode contextNode=baseNode.SelectSingleNode(nodeName,ns);
				if(contextNode==null)
					throw new Exception(String.Format("Відсутній вузол {0}.",nodeName));
				contextNode.InnerText=nodeValue;
				AssignNillable(contextNode,contextNode.InnerText==String.Empty);
			}

			public static void MoveNode(XmlDocument srcDoc, string SrcXPathExp,
				XmlDocument tgtDoc, string TgtXPathExp)
			{
				XPathNavigator nav = srcDoc.CreateNavigator();					
				XPathExpression Expr = nav.Compile(SrcXPathExp);
				XmlNamespaceManager context = SureTools.NameSpaceManager(srcDoc);
				Expr.SetContext(context);
				XPathNodeIterator Iterator = nav.Select(Expr);
				while (Iterator.MoveNext())
				{
					String name = Iterator.Current.Name;
					XmlNode node1 = srcDoc.SelectSingleNode("//" + name, SureTools.NameSpaceManager(srcDoc));
					XmlNode node2 = tgtDoc.ImportNode(node1, true);					
					XmlNode parent_node = tgtDoc.SelectSingleNode(TgtXPathExp, SureTools.NameSpaceManager(tgtDoc));
					if(parent_node != null) 
					{
						XmlNode old_node = parent_node.SelectSingleNode("//" + name, SureTools.NameSpaceManager(tgtDoc));
						if(node2.InnerText!=String.Empty)
							parent_node.ReplaceChild(node2, old_node);
					}
				}
			}

			public static void AssignNillable(XmlNode xnode,bool isNull)
			{
				XmlAttribute attr = (XmlAttribute)xnode.Attributes.GetNamedItem("xsi:nil");
				if(attr!=null) 
				{
					attr.Value=isNull?"true":"false";								
					xnode.Attributes.SetNamedItem(attr);
				}
			}
		
            public static void PutComment(XmlNode eNode, string warning, bool overwrite)
            {
                XmlDocument sDoc;
                XmlNode parent;
                if (eNode is XmlDocument)
                {
                    sDoc = (XmlDocument)eNode;
                    parent = sDoc;
                }
                else
                {
                    sDoc = eNode.OwnerDocument;
                    parent = eNode.ParentNode;
                }
                if (eNode.PreviousSibling is XmlComment)
                {
                    if (overwrite)
                        eNode.PreviousSibling.InnerText = warning;
                    else
                        eNode.PreviousSibling.InnerText += warning;
                }
                else
                {
                    XmlComment comment = sDoc.CreateComment(warning);
                    parent.InsertBefore(comment, eNode);
                }
            }
		}

		public class EntryFormatter
		{ 
			public static string DateParse(DateTime dt) 
			{
				string sDt=String.Format("{0:d4}-{1:d2}-{2:d2}",dt.Year,dt.Month,dt.Day);
				return sDt;
			}
			public static string ValueParse(Decimal dec,Int16 fractionDigit,string DecimalSeparator) 
			{
				String sVal=dec.ToString();				
				Int32  indx=sVal.IndexOf(DecimalSeparator,0,sVal.Length);
				if(indx>=0) 
				{
					sVal=sVal.PadRight(fractionDigit+indx,'0');
					if(DecimalSeparator!=".")
						sVal=sVal.Replace(DecimalSeparator,".");
				}
				else
				{
					sVal=sVal.Insert(sVal.Length,".");
					sVal=sVal.PadRight(fractionDigit+sVal.Length,'0');
				}
				return sVal;
			}
			public static System.Int32 DiskSizeStr2Value(object oDiskSize)
			{
				string DiskSize=oDiskSize.ToString();
				int i=DiskSize.IndexOf("KB");
				if(i>0)
					return Int32.Parse(DiskSize.Substring(0,i-1));
				else
				{
					i=DiskSize.IndexOf("MB");
					if(i>0)
						return Int32.Parse(DiskSize.Substring(0,i-1))*1024;
					else
					{
						i=DiskSize.IndexOf("GB");
						if(i>0)
							return Int32.Parse(DiskSize.Substring(0,i-1))*1024*1024;
						throw new Exception("Unknown string format " + DiskSize);
					}
				}
			}
		}
	}
}
