using System;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GObject
{
	internal static class Delegates
	{
		public delegate void GCallback(IntPtr handle, IntPtr data);

		public delegate void GClosureNotify(IntPtr data, IntPtr closure);
	}
}

