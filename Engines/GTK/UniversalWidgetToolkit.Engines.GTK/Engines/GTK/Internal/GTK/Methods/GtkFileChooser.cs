//
//  GtkFileChooser.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GTK.Methods
{
	internal class GtkFileChooser
	{
		// Preview widget
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_preview_widget(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkWidget*/ preview_widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_file_chooser_get_preview_widget(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_preview_widget_active(IntPtr /*GtkFileChooser*/ chooser, bool active);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_file_chooser_get_preview_widget_active(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_use_preview_label(IntPtr /*GtkFileChooser*/ chooser, bool use_label);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_file_chooser_get_use_preview_label(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern string gtk_file_chooser_get_preview_filename(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern string gtk_file_chooser_get_preview_uri(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GFile*/ gtk_file_chooser_get_preview_file(IntPtr /*GtkFileChooser*/ chooser);

		// Extra widget
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_extra_widget(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkWidget*/ extra_widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_file_chooser_get_extra_widget(IntPtr /*GtkFileChooser*/ chooser);

		// List of user selectable filters
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_add_filter(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkFileFilter*/ filter);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_remove_filter(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkFileFilter*/ filter);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GSList*/ gtk_file_chooser_list_filters(IntPtr /*GtkFileChooser*/ chooser);

		// Current filter
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_filter(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkFileFilter*/ filter);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkFileFilter*/ gtk_file_chooser_get_filter(IntPtr /*GtkFileChooser*/ chooser);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_file_chooser_get_filenames(IntPtr chooser);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_file_chooser_get_select_multiple(IntPtr chooser);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_select_multiple(IntPtr chooser, bool value);

		/// <summary>
		/// Retrieves the file names from the GtkFileChooser with the specified handle, in a managed string[] array.
		/// </summary>
		/// <returns>The file chooser get filenames managed.</returns>
		/// <param name="handle">Handle.</param>
		public static string [] gtk_file_chooser_get_filenames_managed (IntPtr handle)
		{
			List<string> list = new List<string> ();
			IntPtr gslist = Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_get_filenames (handle);

			uint length = Internal.GLib.Methods.g_slist_length (gslist);
			for (uint i = 0; i < length; i++) {
				// WE MUST DO THIS IN ORDER TO MANUALLY FREE THE MEMORY AT THE END
				IntPtr hFileNameStr = Internal.GLib.Methods.g_slist_nth_data (gslist, i);

				// This is now a managed pointer to a managed string. We're all good, so...
				string fileName = System.Runtime.InteropServices.Marshal.PtrToStringAuto (hFileNameStr);
				list.Add (fileName);

				// DESTROY THE UNMANAGED POINTER NOW THAT WE'RE DONE!!!
				Internal.GLib.Methods.g_free (hFileNameStr);

				// this fixes a bug in Universal Editor where the FileDialog could only be opened a few times before crashing the application...
				// but DOES NOT fix the bug in Concertroid :(
			}

			Internal.GLib.Methods.g_slist_free (gslist);
			return list.ToArray ();
		}
	}
}

