//
//  StackSidebarImplementation.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[ControlImplementation(typeof(StackSidebar))]
	public class StackSidebarImplementation : GTKNativeImplementation
	{
		public StackSidebarImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		protected override NativeControl CreateControlInternal (Control control)
		{
			StackSidebar ctl = (control as StackSidebar);

			IntPtr handle = Internal.GTK.Methods.GtkBox.gtk_box_new (Internal.GTK.Constants.GtkOrientation.Horizontal);
			IntPtr hSidebar = Internal.GTK.Methods.GtkStackSidebar.gtk_stack_sidebar_new ();
			IntPtr hStack = Internal.GTK.Methods.GtkStack.gtk_stack_new ();
			Internal.GTK.Methods.GtkStackSidebar.gtk_stack_sidebar_set_stack (hSidebar, hStack);

			foreach (Control child in ctl.Controls) {
				bool created = Engine.CreateControl (child);
				if (created) {
					IntPtr hChild = Engine.GetHandleForControl (child);
					// IntPtr hName = Marshal.StringToHGlobalAuto (child.Name);
					// IntPtr hTitle = Marshal.StringToHGlobalAuto (child.Text);
					Internal.GTK.Methods.GtkStack.gtk_stack_add_titled (hStack, hChild, child.Name, child.Text);
				}
			}

			Internal.GTK.Methods.GtkBox.gtk_box_pack_start (handle, hSidebar, false, false, 0);
			Internal.GTK.Methods.GtkBox.gtk_box_pack_start (handle, hStack, true, true, 0);

			return new GTKNativeControl (handle, new KeyValuePair<string, IntPtr>[]
			{
				new KeyValuePair<string, IntPtr>("Sidebar", hSidebar),
				new KeyValuePair<string, IntPtr>("Stack", hStack)
			});
		}

		protected override void OnCreated (EventArgs e)
		{
			base.OnCreated (e);

			IntPtr hCtrl = (Handle as GTKNativeControl).GetNamedHandle("Sidebar");
			IntPtr hStyleContext = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context (hCtrl);
			foreach (ControlStyleClass cls in Control.Style.Classes) {
				Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class (hStyleContext, cls.Value);
			}
		}
	}
}

