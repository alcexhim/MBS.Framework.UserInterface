using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK.Internal.GTK.Methods
{
	internal static class GtkListBox
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_list_box_new();
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_list_box_row_new();

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_list_box_set_selection_mode(IntPtr /*GtkListBox*/ box, Constants.GtkSelectionMode mode);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_list_box_insert(IntPtr hContainer, IntPtr hListBoxRow, int position = -1);
	}
}
