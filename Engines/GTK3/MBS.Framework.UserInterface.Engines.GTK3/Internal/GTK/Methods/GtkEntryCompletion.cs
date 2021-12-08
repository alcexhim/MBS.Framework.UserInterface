//
//  GtkEntryCompletion.cs - declares P/Invoke methods for the GtkEntryCompletion control
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
	/// <summary>
	/// Declares P/Invoke methods for the GtkEntryCompletion control.
	/// </summary>
	internal static class GtkEntryCompletion
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_entry_completion_new();

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_entry_completion_set_model(IntPtr /*GtkEntryCompletion*/ completion, IntPtr /*GtkTreeModel*/ treeModel);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_entry_completion_set_text_column(IntPtr /*GtkEntryCompletion*/ completion, int column);
	}
}
