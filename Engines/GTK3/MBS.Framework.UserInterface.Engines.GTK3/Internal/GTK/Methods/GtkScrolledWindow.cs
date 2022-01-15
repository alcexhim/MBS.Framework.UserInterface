//
//  GtkScrolledWindow.cs
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
	internal class GtkScrolledWindow
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_scrolled_window_new(IntPtr /*GtkAdjustment*/ hadjustment, IntPtr /*GtkAdjustment*/ vadjustment);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_scrolled_window_get_policy(IntPtr hScroller, ref Constants.GtkPolicyType hscrollbar_policy, ref Constants.GtkPolicyType vscrollbar_policy);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_scrolled_window_set_policy(IntPtr hScroller, Constants.GtkPolicyType hscrollbar_policy, Constants.GtkPolicyType vscrollbar_policy);

		// new in V4, replaces gtk_container_add
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_scrolled_window_set_child(IntPtr scrolledWindow, IntPtr handle);
	}
}
