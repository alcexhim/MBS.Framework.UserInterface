using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UniversalWidgetToolkit.Engines.Win32Windowed
{
    public class Win32WindowedEngine : Engine
    {
		protected override int StartInternal()
		{
			Internal.Windows.Structures.MSG msg;
			int ret = 0;

			while ((ret = Internal.Windows.Methods.GetMessage(out msg, IntPtr.Zero, 0, 0)) != 0)
			{
				Internal.Windows.Methods.TranslateMessage(msg);
				Internal.Windows.Methods.DispatchMessage(msg);
			}

			return msg.wParam.ToInt32();
		}
		protected override void StopInternal()
		{

		}

		private Dictionary<Type, string> ControlClassNames = new Dictionary<Type, string>();
		public Win32WindowedEngine()
		{
			ControlClassNames.Add(typeof(Window), "Window");
		}

		private void EnsureWindowClassRegistered(string className)
		{
			Internal.Windows.Structures.WNDCLASSEX lpwcx = new Internal.Windows.Structures.WNDCLASSEX();
			lpwcx.cbSize = (uint)Marshal.SizeOf(lpwcx);
			lpwcx.lpszClassName = className;
			lpwcx.lpfnWndProc = new Internal.Windows.Delegates.WindowProc(_WindowProc);
			IntPtr atom = Internal.Windows.Methods.RegisterClassEx(lpwcx);
		}

		private static int _WindowProc(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam)
		{
			return Internal.Windows.Methods.DefWindowProc(hwnd, uMsg, wParam, lParam);
		}

		protected override void CreateControlInternal(Control control)
		{
			string className = ControlClassNames[control.GetType()];

			if (control is Window)
			{
				Window window = (control as Window);
				EnsureWindowClassRegistered(window.ClassName);
				Internal.Windows.Methods.CreateWindowEx(Internal.Windows.Constants.WindowStylesEx.None, window.ClassName, window.Title, Internal.Windows.Constants.WindowStyles.WS_OVERLAPPEDWINDOW | Internal.Windows.Constants.WindowStyles.WS_VISIBLE, 0, 0, 640, 480, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			}
		}
    }
}
