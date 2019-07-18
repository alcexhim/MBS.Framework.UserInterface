using System;
using UniversalWidgetToolkit.Controls.Docking;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[NativeImplementation(typeof(DockingContainer))]
	public class DockingContainerImplementation : GTKNativeImplementation, UniversalWidgetToolkit.Controls.Docking.Native.IDockingContainerNativeImplementation
	{
		public DockingContainerImplementation(Engine engine, DockingContainer control)
			: base(engine, control)
		{
		}

		public void ClearDockingItems()
		{
		}
		private void InsertDockingItem2(IntPtr handle, DockingItem item, int index)
		{
			IntPtr childHandle = CreateDockingItem(item);

			// TODO: fix this!
			if (!Engine.IsControlCreated(item.ChildControl))
				Engine.CreateControl(item.ChildControl);

			IntPtr childWidget = (item.ChildControl.NativeImplementation.Handle as GTKNativeControl).Handle;
			if (childWidget != IntPtr.Zero)
			{
				Internal.GTK.Methods.gtk_container_add(childHandle, childWidget);
			}
			else
			{
				IntPtr chdhclm = Internal.GTK.Methods.gtk_label_new("Content not specified");
				Internal.GTK.Methods.gtk_container_add(childHandle, chdhclm);
			}
			Internal.GDL.Methods.gdl_dock_add_item(handle, childHandle, UwtDockItemPlacementToGdlDockPlacement(item.Placement));
		}
		public void InsertDockingItem(DockingItem item, int index)
		{
			InsertDockingItem2(mvarDockHandle, item, index);
		}
		public void RemoveDockingItem(DockingItem item)
		{
		}
		public void SetDockingItem(int index, DockingItem item)
		{
		}

		private Internal.GDL.Constants.GdlDockItemBehavior UwtDockItemBehaviorToGtkDockItemBehavior(DockingItemBehavior value)
		{
			Internal.GDL.Constants.GdlDockItemBehavior retval = Internal.GDL.Constants.GdlDockItemBehavior.BEH_NORMAL;
			if ((value & DockingItemBehavior.Normal) == DockingItemBehavior.Normal) retval |= Internal.GDL.Constants.GdlDockItemBehavior.BEH_NORMAL;
			return retval;
		}

		public static Internal.GDL.Constants.GdlDockPlacement UwtDockItemPlacementToGdlDockPlacement(DockingItemPlacement placement)
		{
			switch (placement)
			{
				case DockingItemPlacement.Top: return Internal.GDL.Constants.GdlDockPlacement.GDL_DOCK_TOP;
				case DockingItemPlacement.Left: return Internal.GDL.Constants.GdlDockPlacement.GDL_DOCK_LEFT;
				case DockingItemPlacement.None: return Internal.GDL.Constants.GdlDockPlacement.GDL_DOCK_NONE;
				case DockingItemPlacement.Right: return Internal.GDL.Constants.GdlDockPlacement.GDL_DOCK_RIGHT;
				case DockingItemPlacement.Bottom: return Internal.GDL.Constants.GdlDockPlacement.GDL_DOCK_BOTTOM;
				case DockingItemPlacement.Center: return Internal.GDL.Constants.GdlDockPlacement.GDL_DOCK_CENTER;
				case DockingItemPlacement.Floating: return Internal.GDL.Constants.GdlDockPlacement.GDL_DOCK_FLOATING;
			}
			return Internal.GDL.Constants.GdlDockPlacement.GDL_DOCK_NONE;
		}

		private IntPtr mvarDockHandle = IntPtr.Zero;
		private IntPtr mvarDockBarHandle = IntPtr.Zero;

		private IntPtr CreateDockingItem(DockingItem item)
		{
			IntPtr handle = Internal.GDL.Methods.gdl_dock_item_new(item.Title, item.Title, UwtDockItemBehaviorToGtkDockItemBehavior(item.Behavior));
			return handle;
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			DockingContainer dock = (control as DockingContainer);
			IntPtr hBox = Internal.GTK.Methods.gtk_box_new(Internal.GTK.Constants.GtkOrientation.Horizontal, 0);

			IntPtr handle = Internal.GDL.Methods.gdl_dock_new();

			foreach (DockingItem item in dock.Items)
			{
				InsertDockingItem2(handle, item, dock.Items.Count - 1);
			}

			IntPtr hDockBar = Internal.GDL.Methods.gdl_dock_bar_new(handle);

			Internal.GTK.Methods.gtk_box_pack_start(hBox, hDockBar, false, false, 0);
			Internal.GTK.Methods.gtk_box_pack_end(hBox, handle, true, true, 0);

			Internal.GTK.Methods.gtk_widget_show(hBox);

			mvarDockHandle = handle;
			mvarDockBarHandle = hDockBar;
			return new GTKNativeControl(hBox);
		}
	}
}
