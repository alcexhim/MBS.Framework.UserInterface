//
//  GtkWidgetPath.cs
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
	internal static class GtkWidgetPath
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidgetPath*/ gtk_widget_path_new();
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_widget_path_append_type(IntPtr /*GtkWidgetPath*/ path, GType type);
		[DllImport(Gtk.LIBRARY_FILENAME), Obsolete("gtk_widget_path_iter_add_region has been deprecated since version 3.14 and should not be used in newly-written code. The use of regions is deprecated.")]
		public static extern void gtk_widget_path_iter_add_region(IntPtr /*GtkWidgetPath*/ path, int pos, string name, int flags);
	}
}
