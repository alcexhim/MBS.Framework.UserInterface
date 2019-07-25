//
//  GtkWidget.cs
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
	internal class GtkWidget
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_hexpand(IntPtr /*GtkWidget*/ widget, bool expand);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_vexpand(IntPtr /*GtkWidget*/ widget, bool expand);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_widget_translate_coordinates(IntPtr /*GtkWidget*/ src_widget, IntPtr /*GtkWidget*/ dest_widget, int src_x, int src_y, ref int dest_x, ref int dest_y);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_widget_get_type();

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_show(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_show_all(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_show_now(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_hide(IntPtr widget);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_queue_draw(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_queue_draw_area(IntPtr widget, int x, int y, int width, int height);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_realize(IntPtr /*GtkWidget*/ widget);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_size_request(IntPtr widget, int width, int height);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_get_size_request(IntPtr widget, out int width, out int height);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_sensitive(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_widget_is_sensitive(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_sensitive(IntPtr widget, bool value);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_can_focus(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_can_focus(IntPtr widget, bool value);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_can_default(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_can_default(IntPtr widget, bool value);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_receives_default(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_receives_default(IntPtr widget, bool value);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_grab_default(IntPtr widget);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern Constants.GtkAlign gtk_widget_set_halign(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_halign(IntPtr widget, Constants.GtkAlign value);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern Constants.GtkAlign gtk_widget_set_valign(IntPtr widget);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_valign(IntPtr widget, Constants.GtkAlign value);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_destroy(IntPtr widget);
	}
}

