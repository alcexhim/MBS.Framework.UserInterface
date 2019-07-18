using System;
using System.Diagnostics.Contracts;

using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[NativeImplementation(typeof(Button))]
	public class ButtonImplementation : GTKNativeImplementation
	{
		private Internal.GObject.Delegates.GCallback gc_Button_Clicked = null;
		public ButtonImplementation(Engine engine, Button control) : base(engine, control)
		{
			gc_Button_Clicked = new Internal.GObject.Delegates.GCallback(Button_Clicked);
		}

		private void Button_Clicked(IntPtr handle, IntPtr data)
		{
			Button button = (Application.Engine.GetControlByHandle(handle) as Button);
			// maybe it's the button not the tabpage?
			if (button != null)
			{
				EventArgs e = new EventArgs();
				button.OnClick(e);
			}
		}

		protected override string GetControlTextInternal(Control control)
		{
			IntPtr handle = Engine.GetHandleForControl(control);
			return Internal.GTK.Methods.gtk_button_get_label(handle);
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			IntPtr handle = Engine.GetHandleForControl(control);
			Internal.GTK.Methods.gtk_button_set_label(handle, text);
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Button ctl = (control as Button);
			Contract.Assert(ctl != null);

			IntPtr handle = Internal.GTK.Methods.gtk_button_new();
			Internal.GTK.Methods.gtk_button_set_always_show_image(handle, ctl.AlwaysShowImage);
			switch (ctl.BorderStyle)
			{
				case ButtonBorderStyle.None:
				{ 
					Internal.GTK.Methods.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.None);
					break;
				}
				case ButtonBorderStyle.Half:
				{ 
					Internal.GTK.Methods.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.Half);
					break;
				}
				case ButtonBorderStyle.Normal:
				{ 
					Internal.GTK.Methods.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.Normal);
					break;
				}
			}

			// DON'T SET THIS... only Dialog buttons should get this by default
			// Internal.GTK.Methods.gtk_widget_set_can_default (handle, true);

			Internal.GObject.Methods.g_signal_connect(handle, "clicked", gc_Button_Clicked, new IntPtr(0xDEADBEEF));
			return new GTKNativeControl(handle);
		}
	}
}
