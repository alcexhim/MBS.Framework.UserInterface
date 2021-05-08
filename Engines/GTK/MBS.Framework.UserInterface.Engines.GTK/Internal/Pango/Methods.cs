using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK.Internal.Pango
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "pango-1.0";

		[DllImport(LIBRARY_FILENAME)]
		public static extern string pango_font_family_get_name(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string pango_font_face_get_face_name(IntPtr handle);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr pango_attr_list_new();

		[DllImport(LIBRARY_FILENAME)]
		public static extern void pango_attr_list_insert(IntPtr /*PangoAttrList*/ list, IntPtr /*PangoAttribute*/ attr);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*PangoAttribute*/ pango_attr_scale_new(double scale_factor);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*PangoFontDescription*/ pango_context_get_font_description(IntPtr /*PangoContext*/ context);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void pango_context_set_font_description(IntPtr /*PangoContext*/ context, IntPtr /*PangoFontDescription*/ desc);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*PangoFontDescription*/ pango_font_description_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern void pango_font_description_set_family(IntPtr /*PangoFontDescription*/ desc, string familyname);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void pango_font_description_set_size(IntPtr /*PangoFontDescription*/ desc, int size);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void pango_font_description_set_weight(IntPtr /*PangoFontDescription*/ desc, int weight);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*PangoAttrFontDesc*/ pango_attr_font_desc_new(IntPtr font);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*PangoAttrFontDesc*/ pango_attr_size_new(int size);
	}
}
