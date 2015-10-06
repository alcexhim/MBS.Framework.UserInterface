using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UniversalWidgetToolkit.Engines.Win32Windowed.Internal.Windows
{
	internal static class Delegates
	{
		public delegate int WindowProc
		(
			[In()] IntPtr hwnd,
			[In()] uint uMsg,
			[In()] IntPtr wParam,
			[In()] IntPtr lParam
		);
	}
}
