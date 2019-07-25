using System;
using System.Diagnostics.Contracts;

using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[ControlImplementation(typeof(SplitContainer))]
	public class SplitContainerImplementation : GTKNativeImplementation
	{
		public SplitContainerImplementation(Engine engine, Control control) : base(engine, control)
		{

		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Contract.Assert(control is SplitContainer);

			SplitContainer ctl = (control as SplitContainer);
			Internal.GTK.Constants.GtkOrientation orientation = Internal.GTK.Constants.GtkOrientation.Horizontal;
			switch (ctl.Orientation)
			{
				case Orientation.Horizontal:
				{
					orientation = Internal.GTK.Constants.GtkOrientation.Horizontal;
					break;
				}
				case Orientation.Vertical:
				{
					orientation = Internal.GTK.Constants.GtkOrientation.Vertical;
					break;
				}
			}
			IntPtr handle = Internal.GTK.Methods.GtkPaned.gtk_paned_new(orientation);

			foreach (Control ctl1 in ctl.Panel1.Controls)
			{
				if (!Engine.IsControlCreated(ctl1)) Engine.CreateControl(ctl1);
				if (!Engine.IsControlCreated(ctl1)) continue;

				Internal.GTK.Methods.GtkPaned.gtk_paned_pack1(handle, Engine.GetHandleForControl(ctl1), true, true);
			}
			foreach (Control ctl1 in ctl.Panel2.Controls)
			{
				if (!Engine.IsControlCreated(ctl1)) Engine.CreateControl(ctl1);
				if (!Engine.IsControlCreated(ctl1)) continue;
				Internal.GTK.Methods.GtkPaned.gtk_paned_pack2(handle, Engine.GetHandleForControl(ctl1), true, true);
			}
			return new GTKNativeControl(handle);
		}
	}
}
