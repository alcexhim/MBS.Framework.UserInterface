using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.WebKit
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

		/// <summary>
		/// Register <paramref name="scheme" /> in context, so that when a
		/// URI request with <paramref name="scheme" /> is made in the
		/// WebKitWebContext, the <see cref="Delegates.WebKitURISchemeRequestCallback" />
		/// registered will be called with a WebKitURISchemeRequest.
		/// </summary>
		/// <remarks>
		/// It is possible to handle URI scheme requests asynchronously, by
		/// calling g_object_ref() on the WebKitURISchemeRequest and calling
		/// webkit_uri_scheme_request_finish() later when the data of the
		/// request is available or webkit_uri_scheme_request_finish_error()
		/// in case of error.
		/// </remarks>
		/// <param name="context">Context.</param>
		/// <param name="scheme">Scheme.</param>
		/// <param name="callback">Callback.</param>
		/// <param name="user_data">User data.</param>
		/// <param name="user_data_destroy_func">User data destroy func.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern void webkit_web_context_register_uri_scheme(IntPtr /*WebKitWebContext*/ context, string scheme, Delegates.WebKitURISchemeRequestCallback callback, IntPtr user_data, GObject.Delegates.GDestroyNotify user_data_destroy_func);

	}
}
