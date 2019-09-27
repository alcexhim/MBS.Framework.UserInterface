using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Engines.WindowsForms.Printing;
using MBS.Framework.UserInterface.Printing;

namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public class WindowsFormsEngine : Engine
	{
		protected override int Priority => (System.Environment.OSVersion.Platform == PlatformID.Win32NT ? 1 : -1);

		protected override void DestroyControlInternal(Control control)
		{
			throw new NotImplementedException();
		}
		protected override NativeControl CreateControlInternal(Control control)
		{
			return base.CreateControlInternal(control);
		}

		protected override void TabContainer_ClearTabPagesInternal(TabContainer parent)
		{
			throw new NotImplementedException();
		}
		protected override void TabContainer_RemoveTabPageInternal(TabContainer parent, TabPage tabPage)
		{
			throw new NotImplementedException();
		}
		protected override void TabContainer_InsertTabPageInternal(TabContainer parent, int index, TabPage tabPage)
		{
			throw new NotImplementedException();
		}

		protected override void UpdateControlLayoutInternal(Control control)
		{
			throw new NotImplementedException();
		}
		protected override void UpdateControlPropertiesInternal(Control control, NativeControl handle)
		{
			throw new NotImplementedException();
		}
		protected override void UpdateNotificationIconInternal(NotificationIcon nid, bool updateContextMenu)
		{
			throw new NotImplementedException();
		}

		protected override bool WindowHasFocusInternal(Window window)
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
				{
					IntPtr hWndActive = Internal.Windows.Methods.GetActiveWindow();
					if (window.ControlImplementation != null)
					{
						if (window.ControlImplementation.Handle is Win32NativeControl)
						{
							return (window.ControlImplementation.Handle as Win32NativeControl).Handle.Equals(hWndActive);
						}
						else if (window.ControlImplementation.Handle is WindowsFormsNativeControl)
						{
							return (window.ControlImplementation.Handle as WindowsFormsNativeControl).Handle.Handle.Equals(hWndActive);
						}
					}
					else
					{
						NativeControl nc = GetHandleForControl(window);
						IntPtr hWnd = IntPtr.Zero;
						if (nc is Win32NativeControl)
						{
							hWnd = (nc as Win32NativeControl).Handle;
						}
						else if (nc is WindowsFormsNativeControl)
						{
							hWnd = (nc as WindowsFormsNativeControl).Handle.Handle;
						}
						return hWnd.Equals(hWndActive);
					}
					break;
				}
			}
			return false;
		}

		protected override void InvalidateControlInternal(Control control, int x, int y, int width, int height)
		{
			throw new NotImplementedException();
		}

		protected override DialogResult ShowDialogInternal(Dialog dialog, Window parent)
		{
			if (dialog is FileDialog)
			{
				FileDialog fd = (dialog as FileDialog);
				switch (fd.Mode)
				{
					case FileDialogMode.CreateFolder:
					case FileDialogMode.SelectFolder:
					{
						System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
						System.Windows.Forms.DialogResult result = fbd.ShowDialog();
						return SWFDialogResultToDialogResult(result);
					}
					case FileDialogMode.Open:
					{
						System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
						ofd.Title = fd.Text;
						ofd.FileName = fd.SelectedFileNames[fd.SelectedFileNames.Count - 1];
						System.Windows.Forms.DialogResult result = ofd.ShowDialog();
						foreach (string fn in ofd.FileNames)
						{
							fd.SelectedFileNames.Add(fn);
						}
						return SWFDialogResultToDialogResult(result);
					}
					case FileDialogMode.Save:
					{
						System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
						sfd.Title = fd.Text;
						sfd.FileName = fd.SelectedFileNames[fd.SelectedFileNames.Count - 1];
						System.Windows.Forms.DialogResult result = sfd.ShowDialog();
						foreach (string fn in sfd.FileNames)
						{
							fd.SelectedFileNames.Add(fn);
						}
						return SWFDialogResultToDialogResult(result);
					}
				}
			}
			else
			{
				System.Windows.Forms.Form f = new System.Windows.Forms.Form();
				System.Windows.Forms.DialogResult result = f.ShowDialog();
				return SWFDialogResultToDialogResult(result);
			}
			throw new NotImplementedException();
		}

		internal DialogResult SWFDialogResultToDialogResult(System.Windows.Forms.DialogResult result)
		{
			switch (result)
			{
				case System.Windows.Forms.DialogResult.Abort: return DialogResult.Abort;
				case System.Windows.Forms.DialogResult.Cancel: return DialogResult.Cancel;
				case System.Windows.Forms.DialogResult.Ignore: return DialogResult.Ignore;
				case System.Windows.Forms.DialogResult.No: return DialogResult.No;
				case System.Windows.Forms.DialogResult.None: return DialogResult.None;
				case System.Windows.Forms.DialogResult.OK: return DialogResult.OK;
				case System.Windows.Forms.DialogResult.Retry: return DialogResult.Retry;
				case System.Windows.Forms.DialogResult.Yes: return DialogResult.Yes;
			}
			return (DialogResult)((int)result);
		}

		private List<Window> _GetToplevelWindowsRetval = null;
		private Window[] GTK_GetToplevelWindowsInternal()
		{
			if (_GetToplevelWindowsRetval != null)
			{
				// should not happen
				throw new InvalidOperationException();
			}

			_GetToplevelWindowsRetval = new List<Window>();
			IntPtr hList = Internal.Linux.GTK.Methods.gtk_window_list_toplevels();
			Internal.Linux.GLib.Methods.g_list_foreach(hList, GTK_AddToList, IntPtr.Zero);

			Window[] retval = _GetToplevelWindowsRetval.ToArray();
			Internal.Linux.GLib.Methods.g_list_free(hList);

			_GetToplevelWindowsRetval = null;
			return retval;
		}
		private void /*GFunc*/ GTK_AddToList(IntPtr data, IntPtr user_data)
		{
			if (_GetToplevelWindowsRetval == null)
			{
				throw new InvalidOperationException("_AddToList called before initializing the list");
			}

			Control ctl = null; // GetControlByHandle(data);
			Window window = (ctl as Window);

			if (window == null)
			{
				window = new Window();
				RegisterControlHandle(window, new Internal.Linux.GTKNativeControl(data));
			}

			_GetToplevelWindowsRetval.Add(window);
		}

		protected override Window [] GetToplevelWindowsInternal ()
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Unix: return GTK_GetToplevelWindowsInternal();
				case PlatformID.Win32NT: return W32_GetToplevelWindowsInternal();
				default: throw new PlatformNotSupportedException();
			}
		}

		private Internal.Windows.Delegates.EnumWindowsProc W32_GetToplevelWindowsCallback;
		public WindowsFormsEngine()
		{
			W32_GetToplevelWindowsCallback = new Internal.Windows.Delegates.EnumWindowsProc(W32_GetToplevelWindowsCallbackFunc);
		}

		private bool W32_GetToplevelWindowsCallbackFunc(IntPtr hWnd, IntPtr lParam)
		{
			if (_GetToplevelWindowsRetval != null)
			{
				Window window = new Window();
				RegisterControlHandle(window, new Win32NativeControl(hWnd));

				_GetToplevelWindowsRetval.Add(window);
			}
			return true;
		}

		private Window[] W32_GetToplevelWindowsInternal()
		{
			if (_GetToplevelWindowsRetval != null)
				return new Window[0];

			_GetToplevelWindowsRetval = new List<Window>();
			bool success = Internal.Windows.Methods.EnumWindows(W32_GetToplevelWindowsCallback, IntPtr.Zero);
			Window[] list = _GetToplevelWindowsRetval.ToArray();
			_GetToplevelWindowsRetval = null;
			return list;
		}

		protected override int StartInternal (Window waitForClose = null)
		{
			if (waitForClose != null) {
				WindowsFormsNativeControl ncWaitForClose = (GetHandleForControl (waitForClose) as WindowsFormsNativeControl);
				System.Windows.Forms.Application.Run (ncWaitForClose.Handle as System.Windows.Forms.Form);
			} else {
				System.Windows.Forms.Application.Run ();
			}
			return 0;
		}
		protected override void StopInternal (int exitCode)
		{
			System.Windows.Forms.Application.Exit();
		}

		protected override void ShowNotificationPopupInternal (NotificationPopup popup)
		{
			throw new NotImplementedException ();
		}

		protected override void SetControlEnabledInternal (Control control, bool value)
		{
			throw new NotImplementedException ();
		}

		protected override Vector2D ClientToScreenCoordinatesInternal (Vector2D point)
		{
			throw new NotImplementedException ();
		}

		protected override void DoEventsInternal ()
		{
			System.Windows.Forms.Application.DoEvents ();
		}

		protected override Monitor [] GetMonitorsInternal ()
		{
			List<Monitor> list = new List<Monitor> ();
			foreach (System.Windows.Forms.Screen scr in System.Windows.Forms.Screen.AllScreens) {
				list.Add (new Monitor (scr.DeviceName, SDRectangleToUWTRectangle(scr.Bounds), SDRectangleToUWTRectangle(scr.WorkingArea)/*, scr.Primary*/));
			}
			return list.ToArray ();
		}

		private Rectangle SDRectangleToUWTRectangle (System.Drawing.Rectangle rect)
		{
			return new Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		protected override void RepaintCustomControl (CustomControl control, int x, int y, int width, int height)
		{
			throw new NotImplementedException ();
		}

		protected override bool IsControlDisposedInternal (Control ctl)
		{
			if (!IsControlCreated (ctl)) return true;
			return (GetHandleForControl (ctl) as WindowsFormsNativeControl).Handle.IsDisposed;
		}
		protected override bool IsControlEnabledInternal (Control control)
		{
			return (GetHandleForControl (control) as WindowsFormsNativeControl).Handle.Enabled;
		}

		protected override bool InitializeInternal ()
		{
			System.Windows.Forms.Application.EnableVisualStyles ();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault (false);
			return true;
		}

		protected override Printer[] GetPrintersInternal()
		{
			List<Printer> list = new List<Printer>();
			foreach (string p in PrinterSettings.InstalledPrinters)
			{
				list.Add(new WindowsFormsPrinter(p));
			}
			return list.ToArray();
		}

		protected override void PrintInternal(PrintJob job)
		{
			PrintDocument doc = new PrintDocument();
			doc.BeginPrint += Doc_BeginPrint;
			doc.PrintPage += Doc_PrintPage;
			doc.Print();
		}

		void Doc_PrintPage(object sender, PrintPageEventArgs e)
		{
		}


		void Doc_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
		}

		protected override NativeTreeModel CreateTreeModelInternal(TreeModel model)
		{
			return new WindowsFormsNativeTreeModel();
		}

		private static WindowsFormsClipboard _Clipboard = null;
		protected override Clipboard GetDefaultClipboardInternal()
		{
			if (_Clipboard == null)
			{
				_Clipboard = new WindowsFormsClipboard();
			}
			return _Clipboard;
		}

		internal static Dimension2D SystemDrawingSizeToDimension2D(System.Drawing.Size size)
		{
			return new Dimension2D(size.Width, size.Height);
		}

	}
}
