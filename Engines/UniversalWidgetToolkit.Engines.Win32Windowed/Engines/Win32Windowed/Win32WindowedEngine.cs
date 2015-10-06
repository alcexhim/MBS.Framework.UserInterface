using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Dialogs;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Engines.Win32Windowed.Drawing;

namespace UniversalWidgetToolkit.Engines.Win32Windowed
{
    public class Win32WindowedEngine : Engine
    {
		private Control mvarPressedControl = null;

		protected override int StartInternal(Window waitForClose = null)
		{
			mvarMainWindow = waitForClose;

			Internal.Windows.Structures.User32.MSG msg;
			int ret = 0;

			while ((ret = Internal.Windows.Methods.User32.GetMessage(out msg, IntPtr.Zero, 0, 0)) != 0)
			{

				Control ctl = GetControlByHandle(msg.hwnd);
				if (!(ctl is Window) && (ctl != null))
				{
					switch (msg.message)
					{
						case Internal.Windows.Constants.User32.WindowMessages.LeftMouseButtonDown:
						{
							mvarPressedControl = ctl;
							break;
						}
						case Internal.Windows.Constants.User32.WindowMessages.LeftMouseButtonUp:
						{
							if (ctl == mvarPressedControl)
							{
								ctl.OnClick(EventArgs.Empty);
							}
							mvarPressedControl = null;
							break;
						}
					}
				}

				Internal.Windows.Methods.User32.TranslateMessage(ref msg);
				Internal.Windows.Methods.User32.DispatchMessage(ref msg);
			}

			return msg.wParam.ToInt32();
		}

		private bool quitting = false;
		protected override void StopInternal(int exitCode)
		{
			if (!quitting)
			{
				Internal.Windows.Methods.User32.PostQuitMessage(exitCode);
				quitting = true;
			}
		}

		private Window mvarMainWindow = null;

		protected override void SetControlVisibilityInternal(Control control, bool visible)
		{
			CreateControl(control);

			IntPtr hWnd = handlesByControl[control];
			
		}

		private Dictionary<Type, string> ControlClassNames = new Dictionary<Type, string>();
		public Win32WindowedEngine()
		{
			ControlClassNames.Add(typeof(Window), "Window");
			ControlClassNames.Add(typeof(Button), "Button");
		}

		private void EnsureWindowClassRegistered(string className)
		{
			Internal.Windows.Structures.User32.WNDCLASSEX lpwcx = new Internal.Windows.Structures.User32.WNDCLASSEX();
			lpwcx.cbSize = (uint)Marshal.SizeOf(lpwcx);
			lpwcx.lpszClassName = className;
			lpwcx.lpfnWndProc = new Internal.Windows.Delegates.WindowProc(_WindowProc);
			lpwcx.hCursor = Internal.Windows.Methods.User32.LoadCursor(IntPtr.Zero, Internal.Windows.Constants.User32.Cursors.Arrow);
			lpwcx.hbrBackground = Internal.Windows.Methods.User32.GetSysColorBrush(Internal.Windows.Constants.User32.SystemColors.ThreeDFace);
			IntPtr atom = Internal.Windows.Methods.User32.RegisterClassEx(ref lpwcx);
		}

		private Dictionary<IntPtr, Control> controlsByHandle = new Dictionary<IntPtr, Control>();
		private Dictionary<Control, IntPtr> handlesByControl = new Dictionary<Control, IntPtr>();

		public Control GetControlByHandle(IntPtr handle)
		{
			if (handle == IntPtr.Zero) return null;
			if (controlsByHandle.ContainsKey(handle)) return controlsByHandle[handle];
			return null;
		}
		public IntPtr GetHandleByControl(Control ctl)
		{
			if (ctl == null) return IntPtr.Zero;
			if (handlesByControl.ContainsKey(ctl)) return handlesByControl[ctl];
			return IntPtr.Zero;
		}
		private int _WindowProc(IntPtr hwnd, Internal.Windows.Constants.User32.WindowMessages uMsg, IntPtr wParam, IntPtr lParam)
		{
			Control ctl = GetControlByHandle(hwnd);
			if (ctl != null)
			{
				switch (uMsg)
				{
					case Internal.Windows.Constants.User32.WindowMessages.LeftMouseButtonDown:
					{
						break;
					}
					case Internal.Windows.Constants.User32.WindowMessages.LeftMouseButtonUp:
					{
						ctl.OnClick(EventArgs.Empty);
						break;
					}
					case Internal.Windows.Constants.User32.WindowMessages.Close:
					{
						CancelEventArgs e = new CancelEventArgs();
						if (ctl is Window)
						{
							(ctl as Window).OnClosing(e);
							if (e.Cancel) return 1;
						}
						break;
					}
					case Internal.Windows.Constants.User32.WindowMessages.DESTROY:
					{
						if (ctl is Window)
						{
							(ctl as Window).OnClosed(EventArgs.Empty);
							if ((ctl as Window) == mvarMainWindow)
							{
								Stop();
							}
						}
						break;
					}
					case Internal.Windows.Constants.User32.WindowMessages.Paint:
					{
						Graphics graphics = CreateGraphics(ctl);
						PaintEventArgs e = new PaintEventArgs(graphics);
						ctl.OnPaint(e);
						break;
					}
				}
			}
			return Internal.Windows.Methods.User32.DefWindowProc(hwnd, uMsg, wParam, lParam);
		}

		private Graphics CreateGraphics(Control ctl)
		{
			Graphics graphics = new Win32WindowedGraphics(GetHandleByControl(ctl));
			return graphics;
		}

		protected override void CreateControlInternal(Control control)
		{
			if (handlesByControl.ContainsKey(control)) return;

			string className = control.ClassName;
			bool classNameRequiresRegistration = true;
			if (className == null)
			{
				if (ControlClassNames.ContainsKey(control.GetType()))
				{
					className = ControlClassNames[control.GetType()];
					classNameRequiresRegistration = false;
				}
			}
			else if (ControlClassNames.ContainsValue(className))
			{
				classNameRequiresRegistration = false;
			}

			if (classNameRequiresRegistration)
			{
				EnsureWindowClassRegistered(className);
			}


			IntPtr hWndParent = IntPtr.Zero;
			if (control.Parent != null)
			{
				hWndParent = handlesByControl[control.Parent];
			}

			Rectangle bounds = new Rectangle();
			if (control is Window)
			{
				bounds = (control as Window).Bounds;
			}
			else
			{
				bounds = control.Parent.Layout.GetControlBounds(control);
			}

			IntPtr handle = Internal.Windows.Methods.User32.CreateWindowEx(GetWindowStylesExForControl(control), className, control.Title, GetWindowStylesForControl(control), (int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height, hWndParent, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

			if (handle != IntPtr.Zero)
			{
				IntPtr hFont = GetHandleByFont(Font.FromFamily("Tahoma", 8.24));
				Internal.Windows.Methods.User32.SendMessage(handle, Internal.Windows.Constants.User32.WindowMessages.SetFont, hFont, new IntPtr(1));

				controlsByHandle[handle] = control;
				handlesByControl[control] = handle;
			}
		}

		private IntPtr GetHandleByFont(Font font)
		{
			Internal.Windows.Structures.GDI.LOGFONT lplf = new Internal.Windows.Structures.GDI.LOGFONT();
			lplf.lfFaceName = font.FamilyName;
			lplf.lfItalic = (byte)(font.Italic ? 1 : 0);
			lplf.lfHeight = (byte)(font.Size * 72);
			lplf.lfWeight = (byte)font.Weight;
			
			IntPtr retval = Internal.Windows.Methods.GDI.CreateFontIndirect(ref lplf);
			return retval;
		}

		private Internal.Windows.Constants.User32.WindowStylesEx GetWindowStylesExForControl(Control control)
		{
			Internal.Windows.Constants.User32.WindowStylesEx styles = Internal.Windows.Constants.User32.WindowStylesEx.None;
			styles |= Internal.Windows.Constants.User32.WindowStylesEx.NoParentNotify;
			return styles;
		}

		private Internal.Windows.Constants.User32.WindowStyles GetWindowStylesForControl(Control control)
		{
			Internal.Windows.Constants.User32.WindowStyles styles = Internal.Windows.Constants.User32.WindowStyles.None;
			if (control is Window)
			{
				styles |= Internal.Windows.Constants.User32.WindowStyles.OverlappedWindow;
			}
			if (control.Visible)
			{
				styles |= Internal.Windows.Constants.User32.WindowStyles.Visible;
			}
			if (control.Parent != null)
			{
				styles |= Internal.Windows.Constants.User32.WindowStyles.Child;
			}
			styles |= Internal.Windows.Constants.User32.WindowStyles.TabStop;
			return styles;
		}

		protected override CommonDialogResult ShowDialogInternal(CommonDialog dialog)
		{
			if (dialog is MessageDialog)
			{
				return ShowMessageDialogInternal(dialog as MessageDialog);
			}
			return CommonDialogResult.None;
		}

		private CommonDialogResult ShowMessageDialogInternal(MessageDialog dialog)
		{
			Internal.Windows.Constants.User32.MessageDialogStyles styles = Win32MessageDialog.GetMessageDialogStyles(dialog);

			IntPtr hWnd = GetHandleByControl(dialog.Parent);
			Internal.Windows.Constants.User32.MessageDialogResponses retval = Internal.Windows.Methods.User32.MessageBox(hWnd, dialog.Content, dialog.Title, styles);
			return CommonDialogResultFromWin32(retval);
		}

		private CommonDialogResult CommonDialogResultFromWin32(Internal.Windows.Constants.User32.MessageDialogResponses value)
		{
			switch (value)
			{
				case Internal.Windows.Constants.User32.MessageDialogResponses.Abort: return CommonDialogResult.Abort;
				case Internal.Windows.Constants.User32.MessageDialogResponses.Cancel: return CommonDialogResult.Cancel;
				case Internal.Windows.Constants.User32.MessageDialogResponses.Continue: return CommonDialogResult.Continue;
				case Internal.Windows.Constants.User32.MessageDialogResponses.Ignore: return CommonDialogResult.Ignore;
				case Internal.Windows.Constants.User32.MessageDialogResponses.No: return CommonDialogResult.No;
				case Internal.Windows.Constants.User32.MessageDialogResponses.OK: return CommonDialogResult.OK;
				case Internal.Windows.Constants.User32.MessageDialogResponses.Retry: return CommonDialogResult.Retry;
				case Internal.Windows.Constants.User32.MessageDialogResponses.TryAgain: return CommonDialogResult.TryAgain;
				case Internal.Windows.Constants.User32.MessageDialogResponses.Yes: return CommonDialogResult.Yes;
			}
			return CommonDialogResult.None;
		}

		private Monitor GetMonitorInternal(IntPtr hMonitor)
		{
			Internal.Windows.Structures.User32.MONITORINFOEX lpmi = new Internal.Windows.Structures.User32.MONITORINFOEX();
			lpmi.cbSize = Marshal.SizeOf(lpmi) - 4;
			Internal.Windows.Methods.User32.GetMonitorInfo(hMonitor, ref lpmi);

			Monitor monitor = new Monitor
			(
				lpmi.szDevice,
				RECTToRectangle(lpmi.rcMonitor),
				RECTToRectangle(lpmi.rcWork)
			);
			return monitor;
		}

		private Rectangle RECTToRectangle(Internal.Windows.Structures.User32.RECT rect)
		{
			return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
		}

		private bool GetMonitorsInternalCallback(IntPtr hMonitor, IntPtr hdcMonitor, Internal.Windows.Structures.User32.RECT lprcMonitor, IntPtr dwData)
		{
			Monitor monitor = GetMonitorInternal(hMonitor);
			_monitorList.Add(monitor);
			return true;
		}

		private List<Monitor> _monitorList = null;
		protected override Monitor[] GetMonitorsInternal()
		{
			if (_monitorList != null) throw new InvalidOperationException("Already in use");

			_monitorList = new List<Monitor>();

			Internal.Windows.Structures.User32.RECT rect = new Internal.Windows.Structures.User32.RECT(-32000, -32000, 64000, 64000);
			bool retval = Internal.Windows.Methods.User32.EnumDisplayMonitors(IntPtr.Zero, ref rect, GetMonitorsInternalCallback, IntPtr.Zero);

			Monitor[] items = _monitorList.ToArray();
			_monitorList = null;

			return items;
		}

    }
}
