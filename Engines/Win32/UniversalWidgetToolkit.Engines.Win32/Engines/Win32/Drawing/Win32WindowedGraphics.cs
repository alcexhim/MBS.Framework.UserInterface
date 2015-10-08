using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit.Engines.Win32.Drawing
{
	public class Win32WindowedGraphics : Graphics
	{
		private IntPtr mvarHwnd = IntPtr.Zero;
		private IntPtr mvarHdc = IntPtr.Zero;

		public Win32WindowedGraphics(IntPtr hWnd)
		{
			mvarHwnd = hWnd;
			mvarHdc = Internal.Windows.Methods.User32.GetDC(mvarHwnd);
		}

		protected override void DrawLineInternal(double x1, double y1, double x2, double y2)
		{
			Internal.Windows.Structures.User32.POINT pt = new Internal.Windows.Structures.User32.POINT();
			Internal.Windows.Methods.GDI.MoveToEx(mvarHdc, (int)x1, (int)y1, ref pt);
			Internal.Windows.Methods.GDI.LineTo(mvarHdc, (int)x2, (int)y2);
		}
	}
}
