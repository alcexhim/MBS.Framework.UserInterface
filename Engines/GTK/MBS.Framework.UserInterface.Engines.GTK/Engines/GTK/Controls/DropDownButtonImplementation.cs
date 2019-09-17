//
//  DropDownButtonImplementation.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
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
using MBS.Framework.UserInterface.Dialogs;
using MBS.Framework.UserInterface.Controls.Native;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(DropDownButton), true)]
	public class DropDownButtonImplementation : ButtonImplementation, IDropDownButtonImplementation
	{
		public DropDownButtonImplementation (Engine engine, Control control)
			: base(engine, control)
		{
		}

		public void OpenDropDown()
		{
			IntPtr hPopOver = (Handle as GTKNativeControl).GetNamedHandle("popover");
			Internal.GTK.Methods.GtkPopover.gtk_popover_popup (hPopOver);
		}
		public void CloseDropDown()
		{
			IntPtr hPopOver = (Handle as GTKNativeControl).GetNamedHandle("popover");
			Internal.GTK.Methods.GtkPopover.gtk_popover_popdown (hPopOver);
		}

		protected internal virtual void OnDropDownClosed(EventArgs e)
		{
			InvokeMethod(Control, "OnDropDownClosed", e);
		}

		protected override void OnClick (EventArgs e)
		{
			base.OnClick (e);

			if (popup != null)
				popup.Show ();
		}

		private PopupWindow popup = null;

		protected override void OnCreated (EventArgs e)
		{
			base.OnCreated (e);
			popup = new PopupWindow ();

			DropDownButton ctl = (Control as DropDownButton);
			if (ctl == null)
				return;

			popup.Owner = ctl;
			Engine.CreateControl (popup);

			popup.Controls.Add (ctl.Container);
		}
	}
}

