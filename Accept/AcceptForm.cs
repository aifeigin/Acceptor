using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;
using System.Reflection;  // Module
using System.Security.Permissions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Accept
{	
		/// <summary>
		/// Summary description for Form1.
		/// </summary>
		public class fmAcceptForm : System.Windows.Forms.Form
		{
			private System.Windows.Forms.NotifyIcon notifyIcon;
			private System.Windows.Forms.ContextMenu contextMenu;
			private System.Windows.Forms.OpenFileDialog openFileDlg;
			private System.Windows.Forms.MenuItem miRecv_0;
			private System.Windows.Forms.MenuItem menuItem1;
			private System.Windows.Forms.MenuItem miExit;
			private System.ComponentModel.IContainer components;

			public fmAcceptForm()
			{
				//
				// Required for Windows Form Designer support
				//
				InitializeComponent();

				//
				// TODO: Add any constructor code after InitializeComponent call
				//
			}

			/// <summary>
			/// Clean up any resources being used.
			/// </summary>
			protected override void Dispose( bool disposing )
			{
				if( disposing )
				{
					if (components != null) 
					{
						components.Dispose();
					}
				}
				base.Dispose( disposing );
			}

			#region Windows Form Designer generated code
			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this.components = new System.ComponentModel.Container();
				System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(fmAcceptForm));
				this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
				this.contextMenu = new System.Windows.Forms.ContextMenu();
				this.miRecv_0 = new System.Windows.Forms.MenuItem();
				this.menuItem1 = new System.Windows.Forms.MenuItem();
				this.miExit = new System.Windows.Forms.MenuItem();
				this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
				// 
				// notifyIcon
				// 
				this.notifyIcon.ContextMenu = this.contextMenu;
				this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
				this.notifyIcon.Text = "ЄІС Акцептор";
				this.notifyIcon.Visible = true;
				// 
				// contextMenu
				// 
				this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.miRecv_0,
																							this.menuItem1,
																							this.miExit});
				// 
				// miRecv_0
				// 
				this.miRecv_0.Index = 0;
				this.miRecv_0.Text = "Прийняти файл уніфікованого формату ";
				this.miRecv_0.Click += new System.EventHandler(this.miRecFile_Click);
				// 
				// menuItem1
				// 
				this.menuItem1.Index = 1;
				this.menuItem1.Text = "-";
				// 
				// miExit
				// 
				this.miExit.Index = 2;
				this.miExit.Text = "Вихід";
				this.miExit.Click += new System.EventHandler(this.miExit_Click);
				// 
				// fmAcceptForm
				// 
				this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
				this.BackColor = System.Drawing.SystemColors.ControlDark;
				this.ClientSize = new System.Drawing.Size(528, 37);
				this.Enabled = false;
				this.Name = "fmAcceptForm";
				this.Text = "Програма перевірки сумісності  та реєстрації \"Акцептор\"";
				this.Closing += new System.ComponentModel.CancelEventHandler(this.fmAcceptForm_Closing);

			}
			#endregion

			public const int SUCCESS = 1;
			public const int ERR_UNKNOWN = 0;	
			public const int ERR_CALL = -1;		
			public const int ERR_CONFIG = -2;		
			public const int ERR_VALIDATE = -3;		
			public const int ERR_REGISTRATION = -4;
			public const int ERR_DEFAULT_NS_MISSED = -5;		
			public const int ERR_CARGO_TYPE_MISSED = -6;		
			public const int ERR_CARGO_TYPE_UNSUPPORTED = -7;		



			const uint WAIT_ABANDONED	= 80;
			const uint WAIT_FAILED		= 0xFFFFFFFF;
			const uint WAIT_OBJECT_0	= 0;
			const uint WAIT_TIMEOUT		= 0x102;

			internal enum StartFlags : uint
			{
				STARTF_USESHOWWINDOW    = 0x00000001,
				STARTF_USESIZE          = 0x00000002,
				STARTF_USEPOSITION      = 0x00000004,
				STARTF_USECOUNTCHARS    = 0x00000008,
				STARTF_USEFILLATTRIBUTE = 0x00000010,
				STARTF_RUNFULLSCREEN    = 0x00000020,  // ignored for non-x86 platforms
				STARTF_FORCEONFEEDBACK  = 0x00000040,
				STARTF_FORCEOFFFEEDBACK = 0x00000080,
				STARTF_USESTDHANDLES    = 0x00000100
			}

			/**
			 * Flags that can be used in StartupInfo.wShowWindow
			 */
			[Flags()]
				internal enum ShowWindowFlags : ushort
			{
				SW_HIDE             = 0,
				SW_SHOWNORMAL       = 1,
				SW_NORMAL           = 1,
				SW_SHOWMINIMIZED    = 2,
				SW_SHOWMAXIMIZED    = 3,
				SW_MAXIMIZE         = 3,
				SW_SHOWNOACTIVATE   = 4,
				SW_SHOW             = 5,
				SW_MINIMIZE         = 6,
				SW_SHOWMINNOACTIVE  = 7,
				SW_SHOWNA           = 8,
				SW_RESTORE          = 9,
				SW_SHOWDEFAULT      = 10,
				SW_FORCEMINIMIZE    = 11,
				SW_MAX              = 11,
			}

			/**
			 * Flags that can be used in CreateProcess.
			 */
			[Flags()]
				internal enum CreationFlags : uint
			{
				CREATE_BREAKAWAY_FROM_JOB   = 0x01000000,
				CREATE_DEFAULT_ERROR_MODE   = 0x04000000,
				CREATE_FORCE_DOS            = 0x00002000,
				CREATE_NEW_CONSOLE          = 0x00000010,
				CREATE_NEW_PROCESS_GROUP    = 0x00000200,
				CREATE_NO_WINDOW            = 0x08000000,
				CREATE_SEPARATE_WOW_VDM     = 0x00000800,
				CREATE_SHARED_WOW_VDM       = 0x00001000,
				CREATE_SUSPENDED            = 0x00000004,
				CREATE_UNICODE_ENVIRONMENT  = 0x00000400,
				DEBUG_PROCESS               = 0x00000001,
				DEBUG_ONLY_THIS_PROCESS     = 0x00000002,
				DETACHED_PROCESS            = 0x00000008,
				// --- Priority class stuff
				ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
				BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
				HIGH_PRIORITY_CLASS         = 0x00000080,
				IDLE_PRIORITY_CLASS         = 0x00000040,
				NORMAL_PRIORITY_CLASS       = 0x00000020,
				REALTIME_PRIORITY_CLASS     = 0x00000100
			}

			internal struct StartupInfo
			{
				/** 
				 * ``cb'' should be equal to 68 
				 * (the result of ``printf ("%i\n", sizeof(STARTUPINFO));'' in C++)
				 */
				public UInt32 cb;
				public UInt32 lpReserved;
				public UInt32 lpDesktop;
				public UInt32 lpTitle;
				public UInt32 dwX;
				public UInt32 dwY;
				public UInt32 dwXSize;
				public UInt32 dwYSize;
				public UInt32 dwXCountChars;
				public UInt32 dwYCountChars;
				public UInt32 dwFillAttribute;
				public UInt32 dwFlags;
				public UInt16 wShowWindow;
				public UInt16 cbReserved2;
				public UInt32 lpReserved2;
				public UInt32 hStdInput;
				public UInt32 hStdOutput;
				public UInt32 hStdError;
			}

			/** Information about a process created via CreateProcess(). */
			[StructLayout(LayoutKind.Sequential)]
				internal struct ProcessInfo
			{
				public UInt32 hProcess;
				public UInt32 hThread;
				public UInt32 dwProcessId;
				public UInt32 dwThreadId;
			}

			[DllImport("kernel32", EntryPoint="CreateProcess")] 
			private static extern Boolean _nCreateProcess (
				string appName, string cmdLine, uint psaProcess, uint psaThread, 
				bool bInheritHandles, uint dwCreationFlags, uint pEnv, 
				string currentDir, ref StartupInfo si, out ProcessInfo pi);	

			[DllImport("kernel32", EntryPoint="CloseHandle")] 
			private static extern bool _nCloseHandle(uint handle);	

			[DllImport("kernel32", EntryPoint="GetLastError")] 
			private static extern int _nGetLastError();

			[DllImport("kernel32", EntryPoint="ZeroMemory")] 
		    private static extern void _nZeroMemory(
				out StartupInfo si,
				uint Length
				);
			[DllImport("kernel32", EntryPoint="ZeroMemory")] 
			private static extern void _nZeroMemory(
				out ProcessInfo pi,
				uint Length
				);
			[DllImport("kernel32", EntryPoint="WaitForSingleObject")] 
			private static extern uint _nWaitForSingleObject(
				uint hHandle,
				uint dwMilliseconds);

			[DllImport("kernel32", EntryPoint="GetExitCodeProcess")] 
			private static extern bool _nGetExitCodeProcess(
				uint hProcess,
				out int lpExitCode
				);


			/// <summary>
			/// The main entry point for the application.
			/// </summary>
			[STAThread]
			static void Main() 
			{
				fmAcceptForm fmAForm;
				fmAForm=new fmAcceptForm();
				Application.Run();			
			}

			private void fmAcceptForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
			{
				((Control)sender).Hide();
				e.Cancel = true;   
			}

			private void miRecFile_Click(object sender, System.EventArgs e)
			{
				Stream myStream;				
				string currentDir = Path.GetDirectoryName(Application.ExecutablePath);
				try
				{
					OpenFileDialog openFileDlg = new OpenFileDialog();
					try 
					{
						openFileDlg.InitialDirectory = currentDir+"\\Examples" ;
						openFileDlg.Filter = "xml files (*.xml)|*.xml" ;
						openFileDlg.FilterIndex = 2 ;
						openFileDlg.RestoreDirectory = true ;

						if(openFileDlg.ShowDialog() == DialogResult.OK)
						{
							if((myStream = openFileDlg.OpenFile())!= null)
							{
								// Insert code to read the stream here.
							
								myStream.Close();

								int pos=contextMenu.MenuItems.IndexOf((MenuItem)sender);						
								if(pos==-1) throw new Exception("Невідомий тип файлу"); 
								
								string Npp=pos.ToString();

								string path = openFileDlg.FileName;
						
								StartupInfo si = new StartupInfo ();
								ProcessInfo pi; 

								_nZeroMemory( out si, 0 );
								si.cb = (uint)System.Runtime.InteropServices.Marshal.SizeOf(si);
								si.dwFlags = (uint) StartFlags.STARTF_USESHOWWINDOW;
								si.wShowWindow = (ushort) ShowWindowFlags.SW_NORMAL;
								_nZeroMemory( out pi, 0 );
																			

								string fName = " -v "+ '"' + path + '"' + " " + '"' + currentDir + '"'; 
								string cmdLine = currentDir+"\\Acceptor.exe"; 
								if(_nCreateProcess(cmdLine,fName,
									0, 0,
									false,
									0,										// creation flags
									0,										// new environment block	
									currentDir,								// current directory name
									ref si,									// startup information
									out pi))
								{
									// GUI process was created; clean up our resources.								
									int ret;
									try 
									{
										uint dwRes=_nWaitForSingleObject(pi.hProcess, 60000);

										switch(dwRes) 
										{
											case WAIT_FAILED	: throw new Exception("Системна помилка : " + _nGetLastError()); 
											case WAIT_OBJECT_0: 
											{
												if(_nGetExitCodeProcess(pi.hProcess,out ret)) 
																  {
																	  switch(ret)
																	  {
																		  case ERR_UNKNOWN :	throw new Exception("Невідома помилка виконання "); 
																		  case ERR_CALL	:	throw new Exception("Помилка виклику [" + fName+"]"); 
																		  case ERR_CONFIG : 	throw new Exception("Помилка конфігупаціїї "); 
																		  case ERR_VALIDATE : throw new Exception("Помилка перевірки сумісності "); 
																		  case ERR_REGISTRATION : throw new Exception("Помилка регістрації "); 
																		  case ERR_DEFAULT_NS_MISSED : throw new Exception("Помилка розпізнання типу документу (default ns is missed)"); 
																		  case ERR_CARGO_TYPE_MISSED : throw new Exception("Помилка розпізнання типу документу (not found)"); 
																		  case ERR_CARGO_TYPE_UNSUPPORTED : throw new Exception("Помилка розпізнання типу документу (not supported)"); 
																	  }
																	  if(ret==SUCCESS) 
																	  {
																		  MessageBox.Show("Регістрація файлу пройшла успішно");
																	  }
																  } 
												break; 
											}
											case WAIT_TIMEOUT	:  throw new Exception("Помилка таймауту. Повторіть операцію ще раз."); 
										}
									}
									finally 
									{
									}
								}
								else
								{
									int s=_nGetLastError();
									throw new Exception("Помилка виконання " + s.ToString() + "[" + cmdLine + " " + fName + "]");
								}

							}
						}	
					}
					finally
					{
						openFileDlg.Dispose();
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show(ex.Message);
				}

			}

			private void miExit_Click(object sender, System.EventArgs e)
			{
				Application.Exit();
			}

			private void menuItem4_Click(object sender, System.EventArgs e)
			{
			
			}

		}
	}

