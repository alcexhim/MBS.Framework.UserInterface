using System;
using System.Diagnostics.Contracts;

using MBS.Framework.UserInterface;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Native;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK3.Controls
{
	[ControlImplementation(typeof(Button))]
	public class ButtonImplementation : GTKNativeImplementation, IButtonControlImplementation
	{
		private Action<IntPtr, IntPtr> gc_Button_Clicked = null;
		public ButtonImplementation(Engine engine, Control control) : base(engine, control)
		{
			gc_Button_Clicked = new Action<IntPtr, IntPtr>(Button_Clicked);
		}

		private void Button_Clicked(IntPtr handle, IntPtr data)
		{
			EventArgs e = new EventArgs();
			base.OnClick(e);
		}

		protected override void OnClick(EventArgs e)
		{
			// Button clicks get handled by the OS in Button_Clicked handler
			// base.OnClick(e);
		}

		protected override string GetControlTextInternal(Control control)
		{
			IntPtr handle = (Engine.GetHandleForControl(control) as GTKNativeControl).Handle;
			IntPtr hTitle = Internal.GTK.Methods.GtkButton.gtk_button_get_label (handle);
			return Marshal.PtrToStringAuto (hTitle);
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			IntPtr hTitle = Marshal.StringToHGlobalAuto (text);
			Internal.GTK.Methods.GtkButton.gtk_button_set_label(handle, hTitle);
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Button ctl = (control as Button);
			Contract.Assert(ctl != null);

			IntPtr handle = IntPtr.Zero;
			if (ctl.CheckOnClick)
			{
				handle = Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_new();
			}
			else
			{
				handle = Internal.GTK.Methods.GtkButton.gtk_button_new();
			}
			if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
			{
			}
			else
			{
				Internal.GTK.Methods.GtkButton.gtk_button_set_always_show_image(handle, ctl.AlwaysShowImage);
			}
			if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V4)
			{

			}
			else
			{
				switch (ctl.BorderStyle)
				{
					case ButtonBorderStyle.None:
					{
						Internal.GTK.Methods.GtkButton.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.None);
						break;
					}
					case ButtonBorderStyle.Half:
					{
						Internal.GTK.Methods.GtkButton.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.Half);
						break;
					}
					case ButtonBorderStyle.Normal:
					{
						Internal.GTK.Methods.GtkButton.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.Normal);
						break;
					}
				}
			}

			if (ctl.StylePreset == CommandStylePreset.Destructive)
			{
				IntPtr hStyleContext = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(handle);
				Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class(hStyleContext, "destructive-action");
			}
			else if (ctl.StylePreset == CommandStylePreset.Suggested)
			{
				IntPtr hStyleContext = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(handle);
				Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class(hStyleContext, "suggested-action");
			}

			UserInterface.Drawing.Image image = null;
			// ImageView imageView = null;
			if (ctl.StockType != StockType.None)
			{
				// imageView = new ImageView();
				image = UserInterface.Drawing.Image.FromName(Engine.StockTypeToString(ctl.StockType), (int)ctl.ImageSize.Width);
			}
			else if (ctl.Image != null)
			{
				// imageView = new ImageView();
				image = ctl.Image;
			}
			if (image != null)
			{
				IntPtr hImageView = IntPtr.Zero;
				// if (Engine.CreateControl(imageView))
				// {
				//     hImageView = (Engine.GetHandleForControl(image) as GTKNativeControl).Handle;
				// }

				if (image is GDKPixbufImage)
				{
					IntPtr hImage = (image as GDKPixbufImage).Handle;
					hImageView = Internal.GTK.Methods.GtkImage.gtk_image_new_from_pixbuf(hImage);
				}
				else if (image is CairoImage)
				{

				}

				if (hImageView != IntPtr.Zero)
				{
					if (ctl.ImageSize != Framework.Drawing.Dimension2D.Empty)
					{
						Internal.GTK.Methods.GtkImage.gtk_image_set_pixel_size(hImageView, (int) ctl.ImageSize.Width);
					}
					Internal.GTK.Methods.GtkButton.gtk_button_set_image(handle, hImageView);

					Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_start(hImageView, ctl.ImageMargin.Left);
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_top(hImageView, ctl.ImageMargin.Top);
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_bottom(hImageView, ctl.ImageMargin.Bottom);
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_end(hImageView, ctl.ImageMargin.Right);
				}
			}

			// DON'T SET THIS... only Dialog buttons should get this by default
			// Internal.GTK.Methods.Methods.gtk_widget_set_can_default (handle, true);

			Internal.GObject.Methods.g_signal_connect(handle, "clicked", gc_Button_Clicked, new IntPtr(0xDEADBEEF));

			Internal.GTK.Methods.GtkButton.gtk_button_set_image_position (handle, (Engine as GTK3Engine).RelativePositionToGtkPositionType(ctl.ImagePosition));

			if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V3)
			{
				switch (ctl.HorizontalAlignment)
				{
					case HorizontalAlignment.Left:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.Start);
						break;
					}
					case HorizontalAlignment.Center:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.Center);
						break;
					}
					case HorizontalAlignment.Right:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.End);
						break;
					}
				}
			}

			// we do this to support older versions of Gtk+ that may not handle gtk_widget_set_focus_on_click
			// Internal.GTK.Methods.GtkButton.gtk_button_set_focus_on_click (handle, ctl.FocusOnClick);

			return new GTKNativeControl(handle);
		}

		public RelativePosition GetImagePosition()
		{
			if (Handle == null) return RelativePosition.Default;

			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Constants.GtkPositionType value = Internal.GTK.Methods.GtkButton.gtk_button_get_image_position (handle);
			return (Engine as GTK3Engine).GtkPositionTypeToRelativePosition(value);
		}
		public void SetImagePosition(RelativePosition value)
		{
			if (Handle == null) return;

			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Constants.GtkPositionType value2 = (Engine as GTK3Engine).RelativePositionToGtkPositionType(value);
			Internal.GTK.Methods.GtkButton.gtk_button_set_image_position (handle, value2);
		}

		public bool GetChecked()
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			return Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_get_active(handle);
		}
		public void SetChecked(bool value)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_set_active(handle, value);
		}
	}
}
