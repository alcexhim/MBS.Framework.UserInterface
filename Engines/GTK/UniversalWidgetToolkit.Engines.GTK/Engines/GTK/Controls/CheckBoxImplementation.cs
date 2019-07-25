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
using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[ControlImplementation(typeof(CheckBox))]
	public class CheckBoxImplementation : GTKNativeImplementation
	{
		public CheckBoxImplementation(Engine engine, Control control)
			: base(engine, control)
		{
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
			
			return new GTKNativeControl(handle);
		}
	}
}
