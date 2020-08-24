//
//  GtkProgressBar.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker's Software
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

namespace MBS.Framework.UserInterface.Engines.GTK.Internal.GTK.Methods
{
	internal static class GtkProgressBar
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_progress_bar_new();

		[DllImport(Gtk.LIBRARY_FILENAME, EntryPoint = "gtk_progress_bar_get_text")]
		private static extern IntPtr /*string*/ _gtk_progress_bar_get_text(IntPtr /*GtkProgressBar*/ handle);

		public static string gtk_progress_bar_get_text(IntPtr handle)
		{
			IntPtr h = _gtk_progress_bar_get_text(handle);
			string v = Marshal.PtrToStringAuto(h);
			return v;
		}

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_progress_bar_get_show_text(IntPtr /*GtkProgressBar*/ handle);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_progress_bar_set_show_text(IntPtr /*GtkProgressBar*/ handle, bool value);

		[DllImport(Gtk.LIBRARY_FILENAME, EntryPoint = "gtk_progress_bar_set_text")]
		private static extern void _gtk_progress_bar_set_text(IntPtr handle, IntPtr value);

		public static void gtk_progress_bar_set_text(IntPtr handle, string value)
		{
			IntPtr h = Marshal.StringToHGlobalAuto(value);
			_gtk_progress_bar_set_text(handle, h);
		}

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_progress_bar_set_fraction(IntPtr /*GtkProgressBar*/ handle, double fraction);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_progress_bar_pulse(IntPtr /*GtkProgressBar*/ handle);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_progress_bar_set_pulse_step(IntPtr handle, double v);
	}
}
