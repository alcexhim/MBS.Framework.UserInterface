using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.GTK.Methods
{
	internal static class GtkPrintSettings
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkPrintSettings*/ gtk_print_settings_new();
	}
}
