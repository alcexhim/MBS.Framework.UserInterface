using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(Label))]
	public class LabelImplementation : GTKNativeImplementation
	{
		public LabelImplementation(Engine engine, Control control) : base(engine, control)
		{
			activate_link_d = new Func<IntPtr, string, bool>(activate_link);
		}

		private Func<IntPtr /*GtkLabel*/, string, bool> activate_link_d;
		private bool activate_link(IntPtr /*GtkLabel*/ label, string uri)
		{
			LinkClickedEventArgs ee = new LinkClickedEventArgs(uri);
			OnLinkClicked(ee);
			return ee.Cancel;
		}

		protected virtual void OnLinkClicked(LinkClickedEventArgs e)
		{
			InvokeMethod((Control as Label), "OnLinkClicked", new object[] { e });
		}

		protected override string GetControlTextInternal(Control control)
		{
			IntPtr handle = (Engine.GetHandleForControl(control) as GTKNativeControl).GetNamedHandle("Control");
			IntPtr hLabelText = Internal.GTK.Methods.GtkLabel.gtk_label_get_label(handle);

			string value = System.Runtime.InteropServices.Marshal.PtrToStringAuto(hLabelText);
			return value;
		}
		private Dictionary<Control, IntPtr> _ctlTextHandles = new Dictionary<Control, IntPtr>();
		protected override void SetControlTextInternal(Control control, string text)
		{
			IntPtr handle = (Engine.GetHandleForControl(control) as GTKNativeControl).GetNamedHandle("Control");

			// GTK fucks this up by passing a pointer directly to the guts of the GtkLabel
			// so, we cannot simply implicitly pass strings to and from GTK
			//
			// we need to go through this rigamarole to ensure that *we* own the pointer to the label text
			// unfortunately, this means we are also responsible for free()ing it...
			if (_ctlTextHandles.ContainsKey(control))
			{
				System.Runtime.InteropServices.Marshal.FreeHGlobal(_ctlTextHandles[control]);
			}
			_ctlTextHandles[control] = System.Runtime.InteropServices.Marshal.StringToHGlobalAuto(text);

			Internal.GTK.Methods.GtkLabel.gtk_label_set_label(handle, _ctlTextHandles[control]);
		}
		protected override NativeControl CreateControlInternal(Control control)
		{
			Contract.Assert(control is Label);

			Label ctl = (control as Label);
			IntPtr handle = Internal.GTK.Methods.GtkLabel.gtk_label_new_with_mnemonic(ctl.Text);

			IntPtr hAttrList = Internal.Pango.Methods.pango_attr_list_new();
			if (ctl.Attributes.ContainsKey("scale"))
			{
				double scale_factor = (double)ctl.Attributes["scale"];
				IntPtr hAttr = Internal.Pango.Methods.pango_attr_scale_new(scale_factor);
				Internal.Pango.Methods.pango_attr_list_insert(hAttrList, hAttr);
			}

			if (Control.Font != null)
			{
				IntPtr hCtx = Internal.GTK.Methods.GtkWidget.gtk_widget_get_pango_context(handle);
				IntPtr hDesc = Internal.Pango.Methods.pango_context_get_font_description(hCtx);
				if (!String.IsNullOrEmpty(Control.Font.FamilyName))
				{
					Internal.Pango.Methods.pango_font_description_set_family(hDesc, Control.Font.FamilyName);
				}
				if (control.Font.Size != null)
				{
					Internal.Pango.Methods.pango_font_description_set_size(hDesc, (int)(Control.Font.Size * Internal.Pango.Constants.PangoScale));
				}

				IntPtr hStyleCtx = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(handle);
				if (Control.Font.Weight != null)
				{
					Internal.Pango.Methods.pango_font_description_set_weight(hDesc, (int)Control.Font.Weight);
				}
				Internal.Pango.Methods.pango_context_set_font_description(hCtx, hDesc);

				IntPtr hAttr = Internal.Pango.Methods.pango_attr_font_desc_new(hDesc);
				Internal.Pango.Methods.pango_attr_list_insert(hAttrList, hAttr);
			}

			if (ctl.WordWrap == WordWrapMode.Always)
			{
				Internal.GTK.Methods.GtkLabel.gtk_label_set_line_wrap(handle, true);
			}
			else if (ctl.WordWrap == WordWrapMode.Never)
			{
				Internal.GTK.Methods.GtkLabel.gtk_label_set_line_wrap(handle, false);
			}

			if (ctl.WidthChars > -1)
			{
				Internal.GTK.Methods.GtkLabel.gtk_label_set_width_chars(handle, ctl.WidthChars);
			}

			switch (ctl.HorizontalAlignment)
			{
				case HorizontalAlignment.Center:
				{
					Internal.GTK.Methods.GtkLabel.gtk_label_set_xalign(handle, 0.5f);
					break;
				}
				case HorizontalAlignment.Left:
				{
					Internal.GTK.Methods.GtkLabel.gtk_label_set_xalign(handle, 0.0f);
					break;
				}
				case HorizontalAlignment.Right:
				{
					Internal.GTK.Methods.GtkLabel.gtk_label_set_xalign(handle, 1.0f);
					break;
				}
			}

			Internal.GTK.Methods.GtkLabel.gtk_label_set_attributes(handle, hAttrList);
			Internal.GObject.Methods.g_signal_connect(handle, "activate_link", activate_link_d);

			Internal.GTK.Methods.GtkLabel.gtk_label_set_use_markup(handle, ctl.UseMarkup);

			IntPtr hEventBox = Internal.GTK.Methods.GtkEventBox.gtk_event_box_new();
			Internal.GTK.Methods.GtkContainer.gtk_container_add(hEventBox, handle);

			return new GTKNativeControl(hEventBox, new KeyValuePair<string, IntPtr>[]
			{
				new KeyValuePair<string, IntPtr>("EventBox", hEventBox),
				new KeyValuePair<string, IntPtr>("Control", handle)
			});
		}
	}
}
