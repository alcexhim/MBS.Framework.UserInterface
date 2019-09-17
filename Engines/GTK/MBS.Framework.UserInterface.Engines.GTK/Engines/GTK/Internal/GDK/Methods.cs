using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK.Internal.GDK
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "gdk-3";

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdk_display_get_default();

		[DllImport(LIBRARY_FILENAME)]
		public static extern int gdk_screen_get_n_monitors(IntPtr screen);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gdk_screen_get_primary_monitor(IntPtr screen);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdk_screen_get_default();
	}
}

