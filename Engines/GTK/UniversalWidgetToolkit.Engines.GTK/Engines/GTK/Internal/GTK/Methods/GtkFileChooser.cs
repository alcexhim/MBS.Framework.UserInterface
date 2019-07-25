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
	}
}

