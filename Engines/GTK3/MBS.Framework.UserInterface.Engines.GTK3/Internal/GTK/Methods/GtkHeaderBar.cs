//
//  GtkHeaderBar.cs
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
	internal class GtkHeaderBar
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_header_bar_new();

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_header_bar_set_title(IntPtr /*GtkHeaderBar*/ bar, string title);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_header_bar_set_subtitle(IntPtr /*GtkHeaderBar*/ bar, string subtitle);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_header_bar_set_show_close_button(IntPtr /*GtkHeaderBar*/ bar, bool value);

		[DllImport(Gtk.LIBRARY_FILENAME, EntryPoint = "gtk_header_bar_get_subtitle")]
		private static extern IntPtr _gtk_header_bar_get_subtitle(IntPtr handle);
		public static string gtk_header_bar_get_subtitle(IntPtr handle)
		{
			IntPtr h = _gtk_header_bar_get_subtitle(handle);
			return Marshal.PtrToStringAuto(h);
		}

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_header_bar_pack_start(IntPtr /*GtkHeaderBar*/ bar, IntPtr /*GtkWidget*/ child);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_header_bar_pack_end(IntPtr /*GtkHeaderBar*/ bar, IntPtr /*GtkWidget*/ child);
	}
}
