//
//  GtkLayout.cs
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

namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.GTK.Methods
{
	internal static class GtkLayout
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkLayout*/ gtk_layout_new(IntPtr /*GtkAdjustment*/ hadjustment, IntPtr /*GtkAdjustment*/ vadjustment);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_layout_set_size(IntPtr /*GtkLayout*/ layout, uint width, uint height);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_layout_get_size(IntPtr /*GtkLayout*/ layout, out uint width, out uint height);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkWindow*/ gtk_layout_get_bin_window(IntPtr /*GtkLayout*/ layout);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_layout_put(IntPtr /*GtkLayout*/ layout, IntPtr /*GtkWidget*/ child_widget, int x, int y);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_layout_move(IntPtr /*GtkLayout*/ layout, IntPtr /*GtkWidget*/ child_widget, int x, int y);
	}
}
