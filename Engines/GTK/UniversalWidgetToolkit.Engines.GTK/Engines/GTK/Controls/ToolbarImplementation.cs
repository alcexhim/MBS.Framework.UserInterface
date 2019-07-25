//
//  ToolbarImplementation.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[NativeImplementation(typeof(UniversalWidgetToolkit.Controls.Toolbar))]
	public class ToolbarImplementation : GTKNativeImplementation
	{
		private Internal.GObject.Delegates.GCallbackV1I gc_clicked_handler = null;
		public ToolbarImplementation(Engine engine, Control control)
			: base(engine, control)
		{
			gc_clicked_handler = new Internal.GObject.Delegates.GCallbackV1I(gc_clicked);
		}

		private Dictionary<IntPtr, ToolbarItem> _itemsByHandle = new Dictionary<IntPtr, ToolbarItem>();
		private Dictionary<ToolbarItem, IntPtr> _handlesByItem = new Dictionary<ToolbarItem, IntPtr>();
		protected void RegisterToolbarItemHandle(ToolbarItem item, IntPtr handle)
		{
			_itemsByHandle[handle] = item;
			_handlesByItem[item] = handle;
		}

		protected IntPtr GetHandleForItem(ToolbarItem item)
		{
			if (!_handlesByItem.ContainsKey(item)) return IntPtr.Zero;
			return _handlesByItem[item];
		}
		protected ToolbarItem GetItemByHandle(IntPtr handle)
		{
			if (!_itemsByHandle.ContainsKey(handle)) return null;
			return _itemsByHandle[handle];
		}

		private void gc_clicked(IntPtr handle)
		{
			ToolbarItemButton tsb = (GetItemByHandle(handle) as ToolbarItemButton);
			if (tsb != null) InvokeMethod(tsb, "OnClick", EventArgs.Empty);
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Toolbar ctl = (control as Toolbar);

			IntPtr handle = Internal.GTK.Methods.GtkToolbar.gtk_toolbar_new();
			Internal.GTK.Methods.GtkToolbar.gtk_toolbar_set_show_arrow(handle, false);

			foreach (ToolbarItem item in ctl.Items)
			{
				IntPtr hItem = IntPtr.Zero;
				if (item is ToolbarItemButton)
				{
					ToolbarItemButton tsb = (item as ToolbarItemButton);
					if (tsb.CheckOnClick)
					{
						hItem = Internal.GTK.Methods.GtkToggleToolButton.gtk_toggle_tool_button_new();  // IntPtr.Zero, item.Title);
					}
					else
					{
						string title = (item as ToolbarItemButton).Title;
						IntPtr iconWidget = IntPtr.Zero;
						if (tsb.StockType != StockType.None)
						{
							string stockTypeID = Engine.StockTypeToString(tsb.StockType);
							iconWidget = Internal.GTK.Methods.GtkImage.gtk_image_new_from_icon_name(stockTypeID);

							Internal.GTK.Structures.GtkStockItem stock = new Internal.GTK.Structures.GtkStockItem();
							bool hasStock = Internal.GTK.Methods.GtkStock.gtk_stock_lookup(stockTypeID, ref stock);
							if (hasStock)
							{
								// fill info from GtkStockItem struct
								title = Marshal.PtrToStringAuto(stock.label);
							}
						}
						hItem = Internal.GTK.Methods.GtkToolButton.gtk_tool_button_new(iconWidget, title);
						if (hItem != IntPtr.Zero)
						{
							Internal.GObject.Methods.g_signal_connect(hItem, "clicked", gc_clicked_handler);
						}
					}
					if (hItem != IntPtr.Zero)
					{
						RegisterToolbarItemHandle(item, hItem);
						Internal.GTK.Methods.GtkToolButton.gtk_tool_button_set_label(hItem, item.Title);
					}
				}
				else if (item is ToolbarItemSeparator)
				{
					hItem = Internal.GTK.Methods.GtkSeparatorToolItem.gtk_separator_tool_item_new();
				}
				if (hItem != IntPtr.Zero)
				{
					int index = ctl.Items.IndexOf(item);
					Internal.GTK.Methods.GtkToolbar.gtk_toolbar_insert(handle, hItem, index);
				}
			}

			return new GTKNativeControl(handle);
		}
	}
}
