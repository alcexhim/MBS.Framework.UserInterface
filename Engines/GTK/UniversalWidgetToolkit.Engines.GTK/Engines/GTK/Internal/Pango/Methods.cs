using System;
using System.Runtime.InteropServices;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.Pango
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "pango-1.0";

		[DllImport(LIBRARY_FILENAME)]
		public static extern string pango_font_family_get_name(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string pango_font_face_get_face_name(IntPtr handle);
	}
}

