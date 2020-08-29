using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK.Internal.WebKit
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "webkit2gtk-4.0";

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ webkit_web_view_new();

		[DllImport(LIBRARY_FILENAME)]
		public static extern void webkit_web_view_load_html(IntPtr handle, string content, string base_uri);

		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.WebKitScriptDialogType webkit_script_dialog_get_dialog_type(IntPtr /*WebKitScriptDialog*/ dialog);
		[DllImport(LIBRARY_FILENAME, EntryPoint = "webkit_script_dialog_get_message")]
		private static extern IntPtr _webkit_script_dialog_get_message(IntPtr /*WebKitScriptDialog*/ dialog);

		public static string webkit_script_dialog_get_message(IntPtr /*WebKitScriptDialog*/ dialog)
		{
			IntPtr h = _webkit_script_dialog_get_message(dialog);
			return Marshal.PtrToStringAuto(h);
		}
	}
}
