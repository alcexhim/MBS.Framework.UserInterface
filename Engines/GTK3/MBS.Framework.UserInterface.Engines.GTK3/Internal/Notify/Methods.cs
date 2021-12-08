using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.Notify
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "notify";

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool notify_init (string appname);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr notify_notification_new(string summary, string body, string icon);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool notify_notification_show(IntPtr notification, IntPtr error);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void notify_notification_add_action(IntPtr notification, string action, string label, Action<IntPtr, string, IntPtr> callback, IntPtr user_data, IntPtr free_func);
	}
}
