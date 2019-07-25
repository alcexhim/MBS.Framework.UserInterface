//
//  GtkNotebook.cs
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
	internal class GtkNotebook
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_notebook_new();
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern int gtk_notebook_append_page(IntPtr hNotebook, IntPtr hChild, IntPtr hTabLabel);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern int gtk_notebook_get_n_pages(IntPtr hNotebook);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern int gtk_notebook_page_num(IntPtr hNotebook, IntPtr hChild);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_notebook_remove_page(IntPtr hNotebook, int page_num);
	}
}

