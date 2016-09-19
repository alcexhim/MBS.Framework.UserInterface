using System;
using System.Runtime.InteropServices;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GLib
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "glib-2.0";

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr g_variant_type_new (string type_string);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void g_slist_free	(IntPtr /*GSList*/ list);

		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_slist_length (IntPtr /*GSList*/ list);

		[DllImport(LIBRARY_FILENAME)]
		public static extern string g_slist_nth_data (IntPtr /*GSList*/ list, uint n);
		
		/// <summary>
		/// Save some memory by interning strings to GQuarks
		/// </summary>
		/// <param name="value">Value.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern string g_intern_string (string value);
		/// <summary>
		/// Save some memory by interning strings to GQuarks
		/// </summary>
		/// <param name="value">Value.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern string g_intern_static_string (string value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern string g_strlcat(System.Text.StringBuilder dest, string src, int dest_size);
	}
}

