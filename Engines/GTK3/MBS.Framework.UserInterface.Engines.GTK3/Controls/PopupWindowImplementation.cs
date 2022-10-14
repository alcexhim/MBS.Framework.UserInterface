//
//  PopupWindowImplementation.cs
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
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface.Engines.GTK3.Controls
{
	[ControlImplementation(typeof(PopupWindow))]
	public class PopupWindowImplementation : WindowImplementation
	{
		public PopupWindowImplementation (Engine engine, PopupWindow control) : base(engine, control)
		{
		}

		protected override void PresentWindowInternal(DateTime timestamp)
		{
			// base.PresentWindowInternal(timestamp);
			// do nothing, because a GtkPopover is not actually a GtkWindow
			Internal.GTK.Methods.GtkPopover.gtk_popover_popup((Handle as GTKNativeControl).Handle);
		}
		protected override string GetControlTextInternal(Control control)
		{
			// return base.GetControlTextInternal(control);
			// do nothing, because a GtkPopover is not actually a GtkWindow
			return String.Empty;
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			// base.SetControlTextInternal(control, text);
			// do nothing, because a GtkPopover is not actually a GtkWindow
			// calling gtk_window_set_title on a non-GtkWindow crashes beautifully ;(
		}
		protected override Rectangle GetControlBoundsInternal()
		{
			// return base.GetControlBoundsInternal();
			return Rectangle.Empty;
		}
		protected override Dimension2D GetControlSizeInternal()
		{
			// return base.GetControlSizeInternal();
			return Dimension2D.Empty;
		}
		protected override void SetControlBoundsInternal(Rectangle bounds)
		{
			// base.SetControlBoundsInternal(bounds);
			Internal.GDK.Structures.GdkRectangle rect = new Internal.GDK.Structures.GdkRectangle();
			rect.x = (int)bounds.X;
			rect.y = (int)bounds.Y;

			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkPopover.gtk_popover_set_pointing_to(handle, ref rect);
		}

		static PopupWindowImplementation()
		{
			popover_closed_handler = new Action<IntPtr>(popover_closed);
		}

		private static Action<IntPtr> popover_closed_handler = null;
		private static void popover_closed(IntPtr handle)
		{
			PopupWindow ctl = ((((UIApplication)Application.Instance).Engine as GTK3Engine).GetControlByHandle (handle) as PopupWindow);
			if (ctl == null)
				return;

			// InvokeMethod (ctl.ControlImplementation, "OnClosed", EventArgs.Empty);
		}

		protected override NativeControl CreateControlInternal (Control control)
		{
			PopupWindow ctl = (Control as PopupWindow);

			IntPtr hCtrlParent = IntPtr.Zero;
			if (ctl.Owner != null) {
				if (ctl.Owner.ControlImplementation.Handle is GTKNativeControl) {
					hCtrlParent = (ctl.Owner.ControlImplementation.Handle as GTKNativeControl).Handle;
				} else {
				}
			}
			IntPtr handle = Internal.GTK.Methods.GtkPopover.gtk_popover_new (hCtrlParent);

			Internal.GObject.Methods.g_signal_connect (handle, "closed", popover_closed_handler);

			IntPtr hLayout = CreateLayout(ctl.Layout, ctl);

			foreach (Control ctl1 in ctl.Controls) {
				if (!ctl1.IsCreated) Engine.CreateControl (ctl1);
				if (!ctl1.IsCreated) continue;

				ApplyLayout(hLayout, ctl1, ctl.Layout);

				IntPtr hCtrl1 = (Engine.GetHandleForControl(ctl1) as GTKNativeControl).Handle;
				Internal.GTK.Methods.GtkWidget.gtk_widget_show_all (hCtrl1);
				// Internal.GTK.Methods.GtkContainer.gtk_container_add (handle, hCtrl1);
			}
			Internal.GTK.Methods.GtkContainer.gtk_container_add(handle, hLayout);

			Internal.GTK.Methods.GtkPopover.gtk_popover_set_position(handle, GTK3Engine.CardinalDirectionToGtkPositionType(ctl.PopupDirection));
			Internal.GDK.Structures.GdkRectangle rect = new Internal.GDK.Structures.GdkRectangle()
			{
				x = (int)ctl.Location.X,
				y = (int)ctl.Location.Y,
				width = 1,
				height = 1
			};
			Internal.GTK.Methods.GtkPopover.gtk_popover_set_pointing_to(handle, ref rect);

			Internal.GTK.Methods.GtkPopover.gtk_popover_set_modal (handle, ctl.Modal);

			Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(handle);
			return new GTKNativeControl (handle);
		}

		protected override void SetLocationInternal(Vector2D location)
		{
			// base.SetLocationInternal(location);
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GDK.Structures.GdkRectangle rect = new Internal.GDK.Structures.GdkRectangle()
			{
				x = (int)location.X,
				y = (int)location.Y,
				width = 1,
				height = 1
			};
			Internal.GTK.Methods.GtkPopover.gtk_popover_set_pointing_to(handle, ref rect);
		}

		protected override void SetControlVisibilityInternal (bool visible)
		{
			if (Handle == null) return;

			PopupWindow ctl = (Control as PopupWindow);
			IntPtr handle = (Handle as GTKNativeControl).Handle;

			if (ctl.Owner != null) {
				IntPtr hCtrlParent = (Engine.GetHandleForControl(ctl.Owner) as GTKNativeControl).Handle;
				Internal.GTK.Methods.GtkPopover.gtk_popover_set_relative_to (handle, hCtrlParent);
			}

			if (visible)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(handle);
				Internal.GTK.Methods.GtkPopover.gtk_popover_popup (handle);
			} else {
				Internal.GTK.Methods.GtkPopover.gtk_popover_popdown (handle);
			}
		}
	}
}
