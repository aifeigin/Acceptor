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
				UKConsole.WriteLine("Параметри запуску :");
                UKConsole.WriteLine("1-й. Команда (-v : перевiрка сумiсностi, -r : реєстрацiя)");
                UKConsole.WriteLine("2-й. Тип форми (0 : Квартальна 2-ТП (повiтря), 1 : рiчна 2-ТП (повiтря), 2 : ПОД-2, 3: Інвентаризація, 4: 1-ТП (повiтря), 5-Документи, що обгрунтують обсяги викидів для об'єктів 1-ї групи, 6-Документи, що обгрунтують обсяги викидів для об'єктів 2-ї групи, 7-Документи, що обгрунтують обсяги викидів для об'єктів 3-ї групи");
                UKConsole.WriteLine("3-й. Найменування XML файлу");
                UKConsole.WriteLine("4-й. Шлях до конфiгурацiйно-системної директорiї");
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
