//
//  CheckBoxImplementation.cs
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
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Native;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(CheckBox))]
	public class CheckBoxImplementation : ButtonImplementation, ICheckBoxImplementation
	{
		public CheckBoxImplementation(Engine engine, Control control)
			: base(engine, control)
		{
			toggled_d = new Action<IntPtr>(toggled);
			state_set_d = new Action<IntPtr, bool>(state_set);
		}

		public bool GetChecked()
		{
			CheckBox ctl = (Control as CheckBox);
			IntPtr handle = (Engine.GetHandleForControl(ctl) as GTKNativeControl).Handle;

			if (ctl.DisplayStyle == CheckBoxDisplayStyle.CheckBox)
			{
				return Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_get_active(handle);
			}
			else if (ctl.DisplayStyle == CheckBoxDisplayStyle.Switch)
			{
				return Internal.GTK.Methods.GtkSwitch.gtk_switch_get_state(handle);
			}
			return false;
		}
		public void SetChecked(bool value)
		{
			CheckBox ctl = (Control as CheckBox);
			IntPtr handle = (Engine.GetHandleForControl(ctl) as GTKNativeControl).Handle;

			if (ctl.DisplayStyle == CheckBoxDisplayStyle.CheckBox)
			{
				Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_set_active(handle, value);
			}
			else if (ctl.DisplayStyle == CheckBoxDisplayStyle.Switch)
			{
				bool state = Internal.GTK.Methods.GtkSwitch.gtk_switch_get_state(handle);
				bool changed = (state != value);

				if (changed)
				{
					_inhibit_state_set = true;
					Internal.GObject.Methods.g_signal_emit_by_name(handle, "activate");
					_inhibit_state_set = false;
				}
				// Internal.GTK.Methods.GtkSwitch.gtk_switch_set_active(handle, value);
			}
		}

		private Action<IntPtr> toggled_d = null;
		private void toggled(IntPtr handle)
		{
			if (_inhibit_state_set) return;
			OnChanged(EventArgs.Empty);
		}
		private bool _inhibit_state_set = false;
		private Action<IntPtr, bool> state_set_d = null;
		private void state_set(IntPtr handle, bool state)
		{
			if (_inhibit_state_set) return;
			if (state != (Control as CheckBox).Checked)
			{
				CheckBoxChangingEventArgs ee = new CheckBoxChangingEventArgs((Control as CheckBox).Checked, state);
				OnChanging(ee);

				_inhibit_state_set = true;
				if (ee.Cancel)
				{
					(Control as CheckBox).Checked = ee.OldValue;
				}
				else
				{
					(Control as CheckBox).Checked = ee.NewValue;
				}
				_inhibit_state_set = false;

				if (!ee.Cancel)
					OnChanged(EventArgs.Empty);
			}
		}

		protected virtual void OnChanging(EventArgs e)
		{
			InvokeMethod(Control as CheckBox, "OnChanging", new object[] { e });
		}
		protected virtual void OnChanged(EventArgs e)
		{
			InvokeMethod(Control as CheckBox, "OnChanged", new object[] { e });
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			CheckBox ctl = (control as CheckBox);
			IntPtr handle = IntPtr.Zero;

			if (ctl.DisplayStyle == CheckBoxDisplayStyle.CheckBox)
			{
				if (ctl.UseMnemonic)
				{
					handle = Internal.GTK.Methods.GtkCheckButton.gtk_check_button_new_with_mnemonic(ctl.Text);
				}
				else
				{
					handle = Internal.GTK.Methods.GtkCheckButton.gtk_check_button_new_with_label(ctl.Text);
				}
				Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_set_active(handle, ctl.Checked);

				Internal.GObject.Methods.g_signal_connect(handle, "toggled", toggled_d);
			}
			else if (ctl.DisplayStyle == CheckBoxDisplayStyle.Switch)
			{
				handle = Internal.GTK.Methods.GtkSwitch.gtk_switch_new();
				Internal.GTK.Methods.GtkSwitch.gtk_switch_set_state(handle, ctl.Checked);
				Internal.GObject.Methods.g_signal_connect(handle, "state_set", state_set_d);
				Internal.GObject.Methods.g_signal_connect(handle, "activate", toggled_d);
			}
			return new GTKNativeControl(handle);
		}
	}
}
