using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[NativeImplementation(typeof(TextBox))]
	public class TextBoxImplementation : GTKNativeImplementation
	{
		public TextBoxImplementation(Engine engine, Control control) : base(engine, control)
		{
			TextBox_Changed_Handler = new Internal.GObject.Delegates.GCallback(TextBox_Changed);
		}

		protected override string GetControlTextInternal(Control control)
		{
			IntPtr handle = Engine.GetHandleForControl(control);

			GType typeEntry = Internal.GTK.Methods.gtk_entry_get_type();
			bool isTextBox = Internal.GObject.Methods.G_TYPE_CHECK_INSTANCE_TYPE(handle, typeEntry);

			TextBox ctl = (control as TextBox);
			/*
			if (textboxChanged.ContainsKey (handle)) {
				if (!textboxChanged [handle])
					return null;
			}
			*/
			// textboxChanged [handle] = false;

			string value = String.Empty;

			if (ctl.Multiline)
			{
				// handle points to the ScrolledWindow
				IntPtr hTextBox = Internal.GTK.Methods.gtk_container_get_focus_child(handle);
				IntPtr hBuffer = Internal.GTK.Methods.gtk_text_view_get_buffer(hTextBox);
				// return Internal.GTK.Methods.gtk_text_buffer_get_text(hBuffer, 0, 0, false);
			}
			else
			{
				ushort textLength = Internal.GTK.Methods.gtk_entry_get_text_length(handle);
				if (textLength > 0)
				{
					try
					{
						value = Internal.GTK.Methods.gtk_entry_get_text(handle);
					}
					catch (Exception ex)
					{
						Console.Error.WriteLine(ex.Message);
					}
				}
			}
			return value;
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			TextBox ctl = (control as TextBox);
			if (ctl.Multiline)
			{
				IntPtr hScrolledWindow = Engine.GetHandleForControl(control);
				IntPtr hList = Internal.GTK.Methods.gtk_container_get_children(hScrolledWindow);
				IntPtr hTextBox = Internal.GLib.Methods.g_list_nth_data(hList, 0);

				IntPtr hBuffer = Internal.GTK.Methods.gtk_text_view_get_buffer(hTextBox);
				Internal.GTK.Methods.gtk_text_buffer_set_text(hBuffer, text, text.Length);
			}
			else
			{
				// this isn't working.. why not?
				IntPtr hTextBox = Engine.GetHandleForControl(control);
				Internal.GTK.Methods.gtk_entry_set_text(hTextBox, text);
			}
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Contract.Assert(control is TextBox);

			TextBox ctl = (control as TextBox);
			IntPtr handle = IntPtr.Zero;
			if (ctl.Multiline)
			{
				handle = Internal.GTK.Methods.gtk_text_view_new();
			}
			else
			{
				handle = Internal.GTK.Methods.gtk_entry_new();
			}
			Internal.GObject.Methods.g_signal_connect(handle, "changed", TextBox_Changed_Handler);

			string ctlText = ctl.Text;
			if (ctlText != null)
			{
				if (ctl.Multiline)
				{
					IntPtr hTextTagTable = Internal.GTK.Methods.gtk_text_tag_table_new();
					IntPtr hBuffer = Internal.GTK.Methods.gtk_text_buffer_new(hTextTagTable);
					Internal.GTK.Methods.gtk_text_buffer_set_text(hBuffer, ctlText, ctlText.Length);
					Internal.GTK.Methods.gtk_text_view_set_buffer(handle, hBuffer);
				}
				Internal.GTK.Methods.gtk_entry_set_text(handle, ctlText);
			}

			if (ctl.MaxLength > -1)
			{
				Internal.GTK.Methods.gtk_entry_set_max_length(handle, ctl.MaxLength);
			}
			if (ctl.WidthChars > -1)
			{
				Internal.GTK.Methods.gtk_entry_set_width_chars(handle, ctl.WidthChars);
			}
			Internal.GTK.Methods.gtk_entry_set_activates_default(handle, true);
			Internal.GTK.Methods.gtk_entry_set_visibility(handle, !ctl.UseSystemPasswordChar);

			Internal.GTK.Methods.gtk_editable_set_editable(handle, ctl.Editable);

			if (ctl.Multiline)
			{
				IntPtr hHAdjustment = Internal.GTK.Methods.gtk_adjustment_new(0, 0, 100, 1, 10, 10);
				IntPtr hVAdjustment = Internal.GTK.Methods.gtk_adjustment_new(0, 0, 100, 1, 10, 10);

				IntPtr hScrolledWindow = Internal.GTK.Methods.gtk_scrolled_window_new(hHAdjustment, hVAdjustment);
				Internal.GTK.Methods.gtk_container_add(hScrolledWindow, handle);
				return new GTKNativeControl(hScrolledWindow);
			}
			else
			{
				return new GTKNativeControl(handle);
			}
		}

		private Dictionary<IntPtr, bool> textboxChanged = new Dictionary<IntPtr, bool>();
		private Internal.GObject.Delegates.GCallback TextBox_Changed_Handler;
		private void TextBox_Changed(IntPtr handle, IntPtr data)
		{
			TextBox ctl = Application.Engine.GetControlByHandle(handle) as TextBox;
			Contract.Assert(ctl != null);

			textboxChanged[handle] = true;
			InvokeMethod(ctl, "OnChanged", EventArgs.Empty);
		}
	}
}
