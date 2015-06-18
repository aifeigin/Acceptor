using System;
using System.IO;
using EIS.XML;
using EIS.XML.Utils;

namespace Validator
{

	class Validator
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			//
			// TODO: Add code to start application here
			//
			//StreamWriter writer = new StreamWriter(Console.OpenStandardOutput(),System.Text.Encoding.Default);
			//writer.AutoFlush=true;
			//Console.SetOut(writer);
			if(args.Length!=3) 
			{
				UKConsole.WriteLine("��������� ������� :");
                UKConsole.WriteLine("1-�. ������� (-v : �����i��� ���i�����i, -r : �������i�)");
                UKConsole.WriteLine("2-�. ��� ����� (0 : ���������� 2-�� (���i���), 1 : �i��� 2-�� (���i���), 2 : ���-2, 3: ��������������, 4: 1-�� (���i���), 5-���������, �� ����������� ������ ������ ��� ��'���� 1-� �����, 6-���������, �� ����������� ������ ������ ��� ��'���� 2-� �����, 7-���������, �� ����������� ������ ������ ��� ��'���� 3-� �����");
                UKConsole.WriteLine("3-�. ������������ XML �����");
                UKConsole.WriteLine("4-�. ���� �� ����i�����i���-�������� ��������i�");
				for(int i=0;i<args.Length;i++)
                    UKConsole.WriteLine(args[i]);
                UKConsole.WaitForConfirm();
				return (int)RetVal.ERR_CALL;
			}
			else
			{
				ProcessEISXml eisXml = new ProcessEISXml();
				return (int)eisXml.Run(args);
			}
		}
	}
}
