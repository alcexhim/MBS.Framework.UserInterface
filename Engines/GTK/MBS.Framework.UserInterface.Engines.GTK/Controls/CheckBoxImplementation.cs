﻿//
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
		}

		public bool GetChecked()
		{
			CheckBox ctl = (Control as CheckBox);
			IntPtr handle = (Engine.GetHandleForControl(ctl) as GTKNativeControl).Handle;

			return Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_get_active (handle);
		}
		public void SetChecked(bool value)
		{
			CheckBox ctl = (Control as CheckBox);
			IntPtr handle = (Engine.GetHandleForControl(ctl) as GTKNativeControl).Handle;

			Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_set_active (handle, value);
		}

		private Action<IntPtr> toggled_d = null;
		private void toggled(IntPtr handle)
		{
			OnChanged(EventArgs.Empty);
		}

		protected virtual void OnChanged(EventArgs e)
		{
			InvokeMethod(Control as CheckBox, "OnChanged", new object[] { e });
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			CheckBox ctl = (control as CheckBox);
			IntPtr handle = IntPtr.Zero;

			if (ctl.UseMnemonic)
			{
				handle = Internal.GTK.Methods.GtkCheckButton.gtk_check_button_new_with_mnemonic(ctl.Text);
			}
			else
			{
				handle = Internal.GTK.Methods.GtkCheckButton.gtk_check_button_new_with_label(ctl.Text);
			}
			Internal.GTK.Methods.GtkToggleButton.gtk_toggle_button_set_active (handle, ctl.Checked);

			Internal.GObject.Methods.g_signal_connect(handle, "toggled", toggled_d);
			
			return new GTKNativeControl(handle);
		}
	}
}
