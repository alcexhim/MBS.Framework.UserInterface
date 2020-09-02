using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls
{
	[ControlImplementation(typeof(NumericTextBox))]
	public class NumericTextBoxImplementation : WindowsFormsNativeImplementation, MBS.Framework.UserInterface.Controls.Native.INumericTextBoxControlImplementation
	{
		public NumericTextBoxImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		public double GetMaximum()
		{
			return (double)(((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown)?.Maximum);
		}

		public double GetMinimum()
		{
			return (double)(((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown)?.Minimum);
		}

		public double GetStep()
		{
			return (double)(((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown)?.Increment);
		}

		public double GetValue()
		{
			return (double)(((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown)?.Value);
		}

		public int GetDecimalPlaces()
		{
			return (((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown)?.DecimalPlaces).GetValueOrDefault(0);
		}

		public void SetMaximum(double value)
		{
			System.Windows.Forms.NumericUpDown nud = ((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown);
			if (nud != null)
			{
				nud.Maximum = CDoubleToDecimal(value);
			}
		}

		public void SetMinimum(double value)
		{
			System.Windows.Forms.NumericUpDown nud = ((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown);
			if (nud != null)
			{
				nud.Minimum = CDoubleToDecimal(value);
			}
		}

		public void SetDecimalPlaces(int value)
		{
			System.Windows.Forms.NumericUpDown handle = ((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown);
			if (handle != null)
			{
				handle.DecimalPlaces = value;
			}
		}

		private decimal CDoubleToDecimal(double value)
		{
			decimal val = 0.0M;
			try
			{
				val = (decimal)value;
			}
			catch (OverflowException ex)
			{
				if (value == double.MinValue)
				{
					val = decimal.MinValue;
				}
				else if (value == double.MaxValue)
				{
					val = decimal.MaxValue;
				}
				else
				{
					//ah, screw it
					val = 0;
				}
			}
			return val;
		}

		public void SetStep(double value)
		{
			System.Windows.Forms.NumericUpDown nud = ((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown);
			if (nud != null) nud.Increment = CDoubleToDecimal(value);
		}

		public void SetValue(double value)
		{
			System.Windows.Forms.NumericUpDown nud = ((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.NumericUpDown);
			if (nud != null)
			{
				if (value <= (Control as NumericTextBox).Maximum && value >= (Control as NumericTextBox).Minimum)
				{
					nud.Value = CDoubleToDecimal(value);
				}
			}
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			NumericTextBox ctl = (control as NumericTextBox);

			System.Windows.Forms.NumericUpDown txt = new System.Windows.Forms.NumericUpDown();
			txt.Maximum = CDoubleToDecimal(ctl.Maximum);
			txt.Minimum = CDoubleToDecimal(ctl.Minimum);
			if (ctl.Value < ctl.Minimum || ctl.Value > ctl.Maximum)
			{
				ctl.Value = ctl.Minimum;
			}
			txt.Value = CDoubleToDecimal(ctl.Value);
			txt.DecimalPlaces = ctl.DecimalPlaces;
			txt.ValueChanged += txt_ValueChanged;

			return new WindowsFormsNativeControl(txt);
		}

		private void txt_ValueChanged(object sender, EventArgs e)
		{
			InvokeMethod((Control as NumericTextBox), "OnChanged", new object[] { EventArgs.Empty });
		}

	}
}
