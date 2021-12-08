//
//  GtkSwitch.cs
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

namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.GTK.Methods
{
	internal static class GtkSwitch
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_switch_new();
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_switch_set_active(IntPtr /*GtkSwitch*/ sw, bool is_active);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_switch_get_active(IntPtr /*GtkSwitch*/ sw);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_switch_set_state(IntPtr /*GtkSwitch*/ sw, bool state);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_switch_get_state(IntPtr /*GtkSwitch*/ sw);
	}
}
