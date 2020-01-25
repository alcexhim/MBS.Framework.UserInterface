//
//  GenericDialogImplementation.cs
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Dialogs
{
	[ControlImplementation(typeof(CustomDialog))]
	public class GenericDialogImplementation : WindowsFormsDialogImplementation
	{
		public GenericDialogImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		protected override bool AcceptInternal()
		{
			return true;
		}

		private class __wmG : System.Windows.Forms.CommonDialog
		{
			private Dialog _dialog = null;
			private System.Windows.Forms.Form f;
			public __wmG(Dialog dialog, System.Windows.Forms.Form f)
			{
				_dialog = dialog;
				this.f = f;
			}
			public override void Reset()
			{
			}

			protected override bool RunDialog(IntPtr hwndOwner)
			{
				if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					return true;
				return false;
			}
		}

		protected override WindowsFormsNativeDialog CreateDialogInternal(Dialog dialog, List<Button> buttons)
		{
			NativeControl hContainer = (new Controls.ContainerImplementation(Engine, dialog)).CreateControl(dialog);

			System.Windows.Forms.Control ctl = (hContainer as WindowsFormsNativeControl).Handle;
			System.Windows.Forms.Form f = new System.Windows.Forms.Form();
			ctl.Dock = System.Windows.Forms.DockStyle.Fill;
			f.Controls.Add(ctl);

			WindowsFormsNativeDialog nc = new WindowsFormsNativeDialog(new __wmG(dialog, f));
			Engine.RegisterControlHandle(dialog, nc);
			return nc;
		}
	}
}
