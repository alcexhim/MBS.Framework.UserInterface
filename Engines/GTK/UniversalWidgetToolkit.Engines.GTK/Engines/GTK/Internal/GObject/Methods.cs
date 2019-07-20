using System;
using System.Runtime.InteropServices;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GObject
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME = "gobject-2.0";

		public static uint g_signal_connect(IntPtr instance, string detailed_signal, Delegates.GCallback c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}
		public static uint g_signal_connect(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkGlAreaRenderFunc c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool g_type_check_instance_is_a(IntPtr /*GTypeInstance*/ instance, GType iface_type);

		public static bool G_TYPE_CHECK_INSTANCE_TYPE(IntPtr handle, GType typeEntry)
		{
			IntPtr __inst = handle;
			GType __t = typeEntry;
			bool __r;
			if (__inst == IntPtr.Zero)
			{
				__r = false;
			}
			/*
			else if (__inst->g_class && __inst->g_class->g_type == __t)
			{
				__r = true;
			}
			*/
			else
			{
				__r = g_type_check_instance_is_a(__inst, __t);
			}
			return __r;
		}

		public static uint g_signal_connect(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkWidgetEvent c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}

		public static uint g_signal_connect(IntPtr instance, string detailed_signal, Delegates.GCallback c_handler, IntPtr data)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, data, null, Constants.GConnectFlags.None);
		}
		public static uint g_signal_connect_after(IntPtr instance, string detailed_signal, Delegates.GCallback c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.ConnectAfter);
		}
		public static uint g_signal_connect_after(IntPtr instance, string detailed_signal, Delegates.GCallback c_handler, IntPtr data)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, data, null, Constants.GConnectFlags.ConnectAfter);
		}

		#region GtkTreeView
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkTreeViewFunc c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);

		public static uint g_signal_connect(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkTreeViewFunc c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}
		public static uint g_signal_connect_after(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkTreeViewFunc c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.ConnectAfter);
		}
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkTreeViewRowActivatedFunc c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);
		public static uint g_signal_connect(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkTreeViewRowActivatedFunc c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}
		public static uint g_signal_connect_after(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkTreeViewRowActivatedFunc c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.ConnectAfter);
		}
		#endregion

		#region Cairo
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data(IntPtr instance, string detailed_signal, Delegates.DrawFunc c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);
		public static uint g_signal_connect(IntPtr instance, string detailed_signal, Delegates.DrawFunc c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkGlAreaRenderFunc c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);

		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkWidgetEvent c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);
		public static uint g_signal_connect_after(IntPtr instance, string detailed_signal, Internal.GTK.Delegates.GtkWidgetEvent c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.ConnectAfter);
		}


		#endregion


#region drag
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data(IntPtr instance, string detailed_signal, GTK.Delegates.GtkDragEvent c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);
		public static uint g_signal_connect(IntPtr instance, string detailed_signal, GTK.Delegates.GtkDragEvent c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}
		public static uint g_signal_connect_after(IntPtr instance, string detailed_signal, GTK.Delegates.GtkDragEvent c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.ConnectAfter);
		}
#endregion
#region GtkDragDataGetEvent
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data(IntPtr instance, string detailed_signal, GTK.Delegates.GtkDragDataGetEvent c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);
		public static uint g_signal_connect(IntPtr instance, string detailed_signal, GTK.Delegates.GtkDragDataGetEvent c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.None);
		}
		public static uint g_signal_connect_after(IntPtr instance, string detailed_signal, GTK.Delegates.GtkDragDataGetEvent c_handler)
		{
			return g_signal_connect_data(instance, detailed_signal, c_handler, IntPtr.Zero, null, Constants.GConnectFlags.ConnectAfter);
		}
#endregion

		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_data(IntPtr instance, string detailed_signal, Delegates.GCallback c_handler, IntPtr data, Delegates.GClosureNotify destroy_data, Constants.GConnectFlags connect_flags);
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint g_signal_connect_object(IntPtr instance, string detailed_signal, Delegates.GCallback c_handler, IntPtr gobject, Constants.GConnectFlags connect_flags);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool g_type_check_instance_is_a(IntPtr instance, IntPtr instance_type);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void g_type_init();

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_init(ref GLib.Structures.Value val, IntPtr gtype);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_boolean(ref GLib.Structures.Value val, bool data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_uchar(ref GLib.Structures.Value val, byte data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_char(ref GLib.Structures.Value val, sbyte data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_boxed(ref GLib.Structures.Value val, IntPtr data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_double(ref GLib.Structures.Value val, double data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_float(ref GLib.Structures.Value val, float data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_int(ref GLib.Structures.Value val, int data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_int64(ref GLib.Structures.Value val, long data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_uint64(ref GLib.Structures.Value val, ulong data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_object(ref GLib.Structures.Value val, IntPtr data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_param(ref GLib.Structures.Value val, IntPtr data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_pointer(ref GLib.Structures.Value val, IntPtr data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_string(ref GLib.Structures.Value val, IntPtr data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_uint(ref GLib.Structures.Value val, uint data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_enum(ref GLib.Structures.Value val, int data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_set_flags(ref GLib.Structures.Value val, uint data);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool g_value_get_boolean(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern byte g_value_get_uchar(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern sbyte g_value_get_char(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr g_value_get_boxed(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern double g_value_get_double(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern float g_value_get_float(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int g_value_get_int(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern long g_value_get_int64(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void g_value_unset(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong g_value_get_uint64(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr g_value_get_object(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr g_value_get_param(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr g_value_get_pointer(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr g_value_get_string(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint g_value_get_uint(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern int g_value_get_enum(ref GLib.Structures.Value val);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint g_value_get_flags(ref GLib.Structures.Value val);


		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern string g_type_name(IntPtr raw);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr g_type_from_name(string name);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool g_type_is_a(IntPtr type, IntPtr is_a_type);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr g_strv_get_type();

	}
}

