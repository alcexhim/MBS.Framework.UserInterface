using System;
using System.Runtime.InteropServices;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.Cairo
{
	internal static class Methods
	{
		const string LIBRARY_FILENAME = "cairo";

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_rectangle(IntPtr /*cairo_t*/ cc, double x, double y, double width, double height);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_fill(IntPtr /*cairo_t*/ cc);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_stroke(IntPtr /*cairo_t*/ cc);

		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.CairoStatus cairo_status(IntPtr /*cairo_t*/ cc);

		[DllImport(LIBRARY_FILENAME)]
		public static extern string cairo_status_to_string(Constants.CairoStatus status);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_paint(IntPtr /*cairo_t*/ cr);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_show_page(IntPtr /*cairo_t*/ cr);

		#region Pattern
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_pattern_t*/ cairo_pattern_create_rgb(double red, double green, double blue);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_select_font_face(IntPtr /*cairo_t*/ cr, string familyName, Constants.CairoFontSlant slant, Constants.CairoFontWeight weight);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_set_font_size(IntPtr /*cairo_t*/ cr, double size);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_pattern_t*/ cairo_pattern_create_rgba(double red, double green, double blue, double alpha);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_pattern_t*/ cairo_pattern_create_for_surface(IntPtr /*cairo_surface_t*/ surface);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_pattern_t*/ cairo_pattern_create_linear(double x0, double y0, double x1, double y1);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_pattern_t*/ cairo_pattern_create_radial(double cx0, double cy0, double radius0, double cx1, double cy1, double radius1);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_pattern_t*/ cairo_pattern_create_mesh();

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*cairo_pattern_t*/ cairo_pattern_reference(IntPtr /*cairo_pattern_t*/ pattern);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void /*cairo_pattern_t*/ cairo_pattern_destroy(IntPtr /*cairo_pattern_t*/ pattern);
		#endregion
		#region Source
		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_set_source(IntPtr /*cairo_t*/ cr, IntPtr /*cairo_pattern_t*/ source);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_set_source_rgb(IntPtr /*cairo_t*/ cr, double red, double green, double blue);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_set_source_rgba(IntPtr /*cairo_t*/ cr, double red, double green, double blue, double alpha);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_set_source_surface(IntPtr /*cairo_t*/ cr, IntPtr /*cairo_surface_t*/ surface, double x, double y);
		#endregion

		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_text_path(IntPtr /*cairo_t*/ cr, string value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void cairo_show_text(IntPtr /*cairo_t*/ cr, string value);

	}
}
