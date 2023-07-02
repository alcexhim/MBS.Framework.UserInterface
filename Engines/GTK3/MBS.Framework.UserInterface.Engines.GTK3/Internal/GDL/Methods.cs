using System;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.GDL
{
	internal static class Methods
	{
		private const string LIBRARY_FILENAME = "libgdl-3";

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdl_dock_new();

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gdl_dock_bar_new(IntPtr hDock);

		/// <summary>
		///
		/// </summary>
		/// <param name="dock">The GdlDock widget to bind it to. Note that this widget must be a type of GdlDock.</param>
		/// <param name="item">The item to bind.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_add_item(IntPtr /*GdlDock*/ dock, IntPtr /*GdlDockItem*/ item, Constants.GdlDockPlacement placement);

		/// <summary>
		/// Creates a new dock item widget.
		/// </summary>
		/// <returns>The newly created dock item widget.</returns>
		/// <param name="name">Unique name for identifying the dock object.</param>
		/// <param name="long_name">Human readable name for the dock object.</param>
		/// <param name="behavior">General behavior for the dock item (i.e. whether it can float, if it's locked, etc.), as specified by <see cref="Constants.GdlDockItemBehavior"/> flags.</param>
		[DllImport(LIBRARY_FILENAME, EntryPoint = "gdl_dock_item_new")]
		private static extern IntPtr _gdl_dock_item_new(IntPtr name, IntPtr long_name, Constants.GdlDockItemBehavior behavior);

		public static IntPtr gdl_dock_item_new(string name, string long_name, Constants.GdlDockItemBehavior behavior)
		{
			IntPtr hName = Marshal.StringToHGlobalAuto(name);
			IntPtr hLongName = Marshal.StringToHGlobalAuto(long_name);
			IntPtr h = _gdl_dock_item_new(hName, hLongName, behavior);
			return h;
		}

		/// <summary>
		/// Binds this dock item to a new dock master.
		/// </summary>
		/// <param name="item">The item to bind.</param>
		/// <param name="dock">The GdlDock widget to bind it to. Note that this widget must be a type of GdlDock.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_item_bind(IntPtr item, IntPtr dock);
		/// <summary>
		/// Binds this dock item to a new dock master.
		/// </summary>
		/// <param name="item">The item to bind.</param>
		/// <param name="dock">The GdlDock widget to bind it to. Note that this widget must be a type of GdlDock.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_item_unbind(IntPtr item);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_item_hide_item(IntPtr /*GdlDockItem*/ item);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_object_set_name(IntPtr /*GdlDockObject*/ item, string value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_master_remove(IntPtr /*GdlDockMaster*/ master, IntPtr /*GdlDockObject*/ obj);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_object_set_long_name(IntPtr /*GdlDockObject*/ item, string value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_object_dock(IntPtr /*GdlDockObject*/ hobject, IntPtr /*GdlDockObject*/ requestor, Constants.GdlDockPlacement position, IntPtr other_data);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_object_present(IntPtr hobject, IntPtr child);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gdl_dock_item_show_item(IntPtr /*GdlDockItem*/ item);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GdlDockMaster*/ gdl_dock_object_get_master(IntPtr /*GdlDockObject*/ hobject);
	}
}
