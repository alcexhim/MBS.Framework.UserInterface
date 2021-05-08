using System;
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(NumericTextBox))]
	public class NumericTextBoxImplementation : GTKNativeImplementation, MBS.Framework.UserInterface.Controls.Native.INumericTextBoxControlImplementation
	{
		public NumericTextBoxImplementation(Engine engine, NumericTextBox control) : base(engine, control)
		{
			value_changed_d = new Internal.GObject.Delegates.GCallbackV1I(value_changed);
		}

		public double GetMaximum()
		{
			double min = 0.0, max = 0.0;
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_range((Handle as GTKNativeControl).Handle, out min, out max);
			return max;
		}

		public double GetMinimum()
		{
			double min = 0.0, max = 0.0;
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_range((Handle as GTKNativeControl).Handle, out min, out max);
			return min;
		}

		public double GetValue()
		{
			return Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_value((Handle as GTKNativeControl).Handle);
		}

		public double GetLargeIncrement()
		{
			double step = 0.0, page = 0.0;
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_increments((Handle as GTKNativeControl).Handle, out step, out page);
			return page;
		}

		public double GetStep()
		{
			double step = 0.0, page = 0.0;
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_increments((Handle as GTKNativeControl).Handle, out step, out page);
			return step;
		}

		public int GetDecimalPlaces()
		{
			return (int)Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_digits((Handle as GTKNativeControl).Handle);
		}
		public void SetDecimalPlaces(int value)
		{
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_digits((Handle as GTKNativeControl).Handle, (uint)value);
		}

		public void SetMaximum(double value)
		{
			double min = 0.0, max = 0.0;
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_range((Handle as GTKNativeControl).Handle, out min, out max);
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_range((Handle as GTKNativeControl).Handle, min, value);
		}

		public void SetMinimum(double value)
		{
			double min = 0.0, max = 0.0;
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_range((Handle as GTKNativeControl).Handle, out min, out max);
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_range((Handle as GTKNativeControl).Handle, value, max);
		}

		public void SetValue(double value)
		{
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_value((Handle as GTKNativeControl).Handle, value);
		}

		public void SetStep(double value)
		{
			double step = 0.0, page = 0.0;
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_increments((Handle as GTKNativeControl).Handle, out step, out page);
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_increments((Handle as GTKNativeControl).Handle, value, page);
		}

		public void SetLargeIncrement(double value)
		{
			double step = 0.0, page = 0.0;
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_get_increments((Handle as GTKNativeControl).Handle, out step, out page);
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_increments((Handle as GTKNativeControl).Handle, step, value);
		}

		private Internal.GObject.Delegates.GCallbackV1I value_changed_d;
		private void value_changed(IntPtr handle)
		{
			InvokeMethod(Control, "OnChanged", EventArgs.Empty);
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			NumericTextBox txt = (control as NumericTextBox);
			IntPtr h = Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_new_with_range(txt.Minimum, txt.Maximum, txt.LargeIncrement);
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_value(h, txt.Value);
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_digits(h, (uint)txt.DecimalPlaces);
			Internal.GTK.Methods.GtkSpinButton.gtk_spin_button_set_increments(h, txt.SmallIncrement, txt.LargeIncrement);
			Internal.GObject.Methods.g_signal_connect(h, "value_changed", value_changed_d);
			return new GTKNativeControl(h);
		}
	}
}
