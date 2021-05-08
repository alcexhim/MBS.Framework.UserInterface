//
//  GtkImage.cs
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

namespace MBS.Framework.UserInterface.Engines.GTK.Internal.GTK.Methods
{
	internal class GtkImage
	{
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_image_new();
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_image_new_from_file(string filename);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_image_new_from_icon_name(string iconName);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_image_new_from_pixbuf(IntPtr /*GdkPixbuf*/ pixbuf);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_image_set_pixel_size(IntPtr /*GtkImage*/ image, int pixel_size);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_image_get_pixbuf(IntPtr /*GtkImage*/ image);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_image_set_from_pixbuf(IntPtr /*GtkImage*/ image, IntPtr /*GdkPixbuf*/ pixbuf);
	}
}
