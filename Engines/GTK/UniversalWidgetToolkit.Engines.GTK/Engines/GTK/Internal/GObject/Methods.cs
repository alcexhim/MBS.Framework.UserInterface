using System;
using System.Runtime.InteropServices;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GObject
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "gobject-2.0";

		public static uint g_signal_connect (IntPtr instance, string detailed_signal, Delegates.GCallback c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}
		public static uint g_signal_connect (IntPtr instance, string detailed_signal, Delegates.GCallback c_handler, IntPtr data)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, data, null, Constants.GConnectFlags.None);
		}
		public static uint g_signal_connect_after (IntPtr instance, string detailed_signal, Delegates.GCallback c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.ConnectAfter);
		}
		public static uint g_signal_connect_after (IntPtr instance, string detailed_signal, Delegates.GCallback c_handler, IntPtr data)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, data, null, Constants.GConnectFlags.ConnectAfter);
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data (IntPtr instance, string detailed_signal, Delegates.GCallback c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_object(IntPtr instance, string detailed_signal, Delegates.GCallback c_handler, IntPtr gobject, Constants.GConnectFlags connect_flags);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool g_type_check_instance_is_a(IntPtr instance, IntPtr instance_type);

	}
}

