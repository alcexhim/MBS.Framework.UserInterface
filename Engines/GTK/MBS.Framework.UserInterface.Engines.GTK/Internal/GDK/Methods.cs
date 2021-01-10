using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK.Internal.GDK
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "gdk-3";

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdk_display_get_default();
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdk_display_beep(IntPtr /*GdkDisplay*/ display);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkMonitor*/ gdk_display_get_monitor(IntPtr /*GdkDisplay*/ display, int index);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkDisplay*/ gdk_screen_get_display(IntPtr /*GdkScreen*/ screen);

		[DllImport(LIBRARY_FILENAME)]
		public static extern int gdk_screen_get_n_monitors(IntPtr screen);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gdk_screen_get_primary_monitor(IntPtr screen);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdk_screen_get_default();

		[DllImport(LIBRARY_FILENAME)]
		public static extern double gdk_screen_get_resolution(IntPtr /*GdkScreen*/ screen);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkCursor*/ gdk_cursor_new_from_name(IntPtr /*GdkDisplay*/ display, string name);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkDisplay*/ gdk_window_get_display(IntPtr /*GdkWindow*/ window);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkCursor*/ gdk_window_get_cursor(IntPtr /*GdkWindow*/ window);

		[DllImport(LIBRARY_FILENAME)]
		public static extern int gdk_monitor_get_scale_factor(IntPtr /*GdkMonitor*/ monitor);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdk_monitor_get_geometry(IntPtr handle, ref Structures.GdkRectangle geom);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdk_window_set_cursor(IntPtr /*GdkWindow*/ window, IntPtr /*GdkCursor*/ cursor);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdk_monitor_get_workarea(IntPtr /*GdkMonitor*/ handle, ref Structures.GdkRectangle workarea);

		// pixbuf
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdk_pixbuf_new(Constants.GdkColorspace colorspace, bool has_alpha, int bits_per_sample, int width, int height);

		// pixbuf loader
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkPixbufLoader*/ gdk_pixbuf_loader_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkPixbufLoader*/ gdk_pixbuf_loader_new_with_type(string image_type, ref IntPtr /*GError*/ error);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gdk_pixbuf_loader_write(IntPtr /*GdkPixbufLoader*/ loader, byte[] buffer, int count, ref IntPtr /*GError*/ error);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gdk_pixbuf_loader_close(IntPtr /*GdkPixbufLoader*/ loader, ref IntPtr /*GError*/ error);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdkPixbuf*/ gdk_pixbuf_loader_get_pixbuf(IntPtr /*GdkPixbufLoader*/ loader);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_t*/ gdk_drawing_context_get_cairo_context(IntPtr /*GdkDrawingContext*/ context);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_t*/ gdk_cairo_create(IntPtr /*GdkWindow*/ window);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdk_cairo_set_source_pixbuf(IntPtr /*cairo_t*/ cr, IntPtr /*const GdkPixbuf*/ pixbuf, double x, double y);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdk_pixbuf_new_from_data(byte[] data, Constants.GdkColorspace colorspace, bool has_alpha, int bits_per_sample, int width, int height, int rowstride, Action<byte[], IntPtr> destroy_fn, IntPtr destroy_fn_data);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdk_pixbuf_new_from_data(IntPtr data, Constants.GdkColorspace colorspace, bool has_alpha, int bits_per_sample, int width, int height, int rowstride, Action<byte[], IntPtr> destroy_fn, IntPtr destroy_fn_data);

		[DllImport(LIBRARY_FILENAME)]
		public static extern int gdk_pixbuf_get_width(IntPtr /*GdkPixbuf*/ pixbuf);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gdk_pixbuf_get_height(IntPtr /*GdkPixbuf*/ pixbuf);
	}
}

