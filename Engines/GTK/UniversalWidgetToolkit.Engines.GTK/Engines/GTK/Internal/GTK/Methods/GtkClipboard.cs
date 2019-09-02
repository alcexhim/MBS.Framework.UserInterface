﻿//
//  GtkClipboard.cs
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
	internal static class GtkClipboard
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkClipboard*/ gtk_clipboard_get_default(IntPtr /*GdkDisplay*/ display);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_clipboard_clear(IntPtr /*GtkClipboard*/ clipboard);

		[DllImport(Gtk.LIBRARY_FILENAME)]	
		public static extern void gtk_clipboard_request_text(IntPtr handle, Action<IntPtr /*GtkClipboard*/, string, IntPtr> text_received_func, IntPtr user_data);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_clipboard_set_text(IntPtr /*GtkClipboard*/ clipboard, string text, int len);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern string gtk_clipboard_wait_for_text(IntPtr /*GtkClipboard*/ handle);
	}
}
