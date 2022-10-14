//
//  GtkSeparator.cs
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
	internal class GtkTextBuffer
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_text_buffer_new(IntPtr /*GtkTextTagTable*/ table);

		[DllImport(Gtk.LIBRARY_FILENAME, EntryPoint = "gtk_text_buffer_get_text")]
		private static extern IntPtr gtk_text_buffer_get_text_impl(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter /*GtkTextIter*/ start, ref Structures.GtkTextIter /*GtkTextIter*/ end, bool include_hidden_chars);
		public static string gtk_text_buffer_get_text(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter /*GtkTextIter*/ start, ref Structures.GtkTextIter /*GtkTextIter*/ end, bool include_hidden_chars)
		{
			IntPtr ptr = gtk_text_buffer_get_text_impl(buffer, ref start, ref end, include_hidden_chars);
			string val = Marshal.PtrToStringAuto(ptr);
			return val;
		}


		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_set_text(IntPtr /*GtkTextBuffer*/ buffer, string text, int len);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_get_start_iter(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter iter);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_get_end_iter(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter iter);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_get_iter_at_offset(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter iter, int offset);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_get_iter_at_line_offset(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter iter, int line, int offset);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_insert(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter iter, string text, int len);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_insert_at_cursor(IntPtr /*GtkTextBuffer*/ buffer, string text, int len);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_delete(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter start, ref Structures.GtkTextIter end);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern bool gtk_text_buffer_get_selection_bounds(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter start, ref Structures.GtkTextIter end);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkTextTagTable*/ gtk_text_buffer_get_tag_table(IntPtr /*GtkTextBuffer*/ buffer);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_apply_tag(IntPtr /*GtkTextBuffer*/ buffer, IntPtr /*GtkTextTag*/ tag, ref Structures.GtkTextIter start, ref Structures.GtkTextIter end);

		/// <summary>
		/// Initializes <paramref name="iter" /> to the start of the given line.
		/// If <paramref name="line_number"/> is greater than the number of lines
		/// in the buffer , the end iterator is returned.
		/// </summary>
		/// <param name="buffer">a <see cref="GtkTextBuffer" />.</param>
		/// <param name="iter">iterator to initialize.</param>
		/// <param name="line_number">line number counting from 0.</param>
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_text_buffer_get_iter_at_line(IntPtr /*GtkTextBuffer*/ buffer, ref Structures.GtkTextIter iter, int line_number);
	}
}
