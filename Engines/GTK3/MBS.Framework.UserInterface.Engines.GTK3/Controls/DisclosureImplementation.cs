//
//  DisclosureImplementation.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker's Software
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
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Engines.GTK3.Controls
{
	[ControlImplementation(typeof(Disclosure))]
	public class DisclosureImplementation : ContainerImplementation
	{
		public DisclosureImplementation(Engine engine, Disclosure control) : base(engine, control)
		{
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			IntPtr handle = Internal.GTK.Methods.GtkExpander.gtk_expander_new_with_mnemonic(control.Text);
			NativeControl ctlContainer = base.CreateControlInternal(control);

			Internal.GTK.Methods.GtkContainer.gtk_container_add(handle, (ctlContainer as GTKNativeControl).Handle);

			return new GTKNativeControl(handle);
		}
	}
}
