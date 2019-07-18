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
using UniversalWidgetToolkit.Controls;
namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[NativeImplementation(typeof(UniversalWidgetToolkit.Controls.Toolbar))]
	public class ToolbarImplementation : GTKNativeImplementation
	{
		public ToolbarImplementation(Engine engine, Control control)
			: base(engine, control)
		{
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Toolbar ctl = (control as Toolbar);

			IntPtr handle = Internal.GTK.Methods.gtk_toolbar_new();
			Internal.GTK.Methods.gtk_toolbar_set_show_arrow(handle, false);

			foreach (ToolbarItem item in ctl.Items)
			{
				IntPtr hItem = IntPtr.Zero;
				if (item is ToolbarItemButton)
				{
					hItem = Internal.GTK.Methods.gtk_toggle_tool_button_new();  // IntPtr.Zero, item.Title);
				}
				if (hItem != IntPtr.Zero)
				{
					Internal.GTK.Methods.gtk_tool_button_set_label(hItem, item.Title);

					int index = ctl.Items.IndexOf(item);
					Internal.GTK.Methods.gtk_toolbar_insert(handle, hItem, index);
				}
			}

			return new GTKNativeControl(handle);
		}
	}
}
