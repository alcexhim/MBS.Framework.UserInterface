//
//  GtkTextView.cs
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
using MBS.Framework.UserInterface.Engines.GTK3.Internal.GDK;

namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.GTK.Methods
{
	internal class GtkTextView
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_text_view_new();
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern GType gtk_text_view_get_type();
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_text_view_get_buffer(IntPtr /*GtkTextView*/ text_view);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_view_set_buffer(IntPtr /*GtkTextView*/ text_view, IntPtr /*GtkTextBuffer*/ buffer);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_view_set_wrap_mode(IntPtr /*GtkTextView*/ text_view, Constants.GtkWrapMode mode);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_text_view_get_editable(IntPtr /*GtkTextView*/ text_view);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_view_set_editable(IntPtr /*GtkTextView*/ text_view, bool editable);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_view_get_iter_location(IntPtr handle, ref Structures.GtkTextIter iter, ref GDK.Structures.GdkRectangle rect);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_view_window_to_buffer_coords(IntPtr handle, Constants.GtkTextWindowType text, int x, int y, ref int buffer_x, ref int buffer_y);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_view_get_iter_at_position(IntPtr handle, ref Structures.GtkTextIter iter, ref int trailing, int buffer_x, int buffer_y);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_view_get_cursor_locations(IntPtr handle, ref Structures.GtkTextIter iter, ref GDK.Structures.GdkRectangle strong, ref GDK.Structures.GdkRectangle weak);
	}
}
