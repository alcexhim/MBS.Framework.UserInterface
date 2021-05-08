//
//  MessageDialogImplementation.cs
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
using System.Windows.Forms;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Dialogs
{
	[ControlImplementation(typeof(MessageDialog))]
	public class MessageDialogImplementation : WindowsFormsDialogImplementation
	{
		public MessageDialogImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		protected override bool AcceptInternal()
		{
			// nothing to do here
			return true;
		}

		private class __wmM : System.Windows.Forms.CommonDialog
		{
			private Dialog _dialog = null;
			public __wmM(Dialog dialog)
			{
				_dialog = dialog;
			}
			public override void Reset()
			{
			}

			protected override bool RunDialog(IntPtr hwndOwner)
			{
				throw new NotImplementedException();
			}
		}

		protected override WindowsFormsNativeDialog CreateDialogInternal(Dialog dialog, List<UserInterface.Controls.Button> buttons)
		{
			return new WindowsFormsNativeDialog(new __wmM(dialog));
		}
		public override DialogResult Run(IWin32Window parentHandle)
		{
			MessageDialog dlg = (Control as MessageDialog);

			System.Windows.Forms.MessageBoxIcon messageType = System.Windows.Forms.MessageBoxIcon.None;
			switch (dlg.Icon)
			{
				case MessageDialogIcon.Error:
					{
						messageType = System.Windows.Forms.MessageBoxIcon.Error;
						break;
					}
				case MessageDialogIcon.Information:
					{
						messageType = System.Windows.Forms.MessageBoxIcon.Information;
						break;
					}
				case MessageDialogIcon.Warning:
					{
						messageType = System.Windows.Forms.MessageBoxIcon.Warning;
						break;
					}
				case MessageDialogIcon.Question:
					{
						messageType = System.Windows.Forms.MessageBoxIcon.Question;
						break;
					}
			}

			System.Windows.Forms.MessageBoxButtons buttonsType = System.Windows.Forms.MessageBoxButtons.OK;
			switch (dlg.Buttons)
			{
				case MessageDialogButtons.AbortRetryIgnore:
					{
						buttonsType = System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore;
						break;
					}
				case MessageDialogButtons.CancelTryContinue:
					{
						// TODO: call out to Win32 to show this one
						buttonsType = System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore;
						break;
					}
				case MessageDialogButtons.RetryCancel:
					{
						buttonsType = System.Windows.Forms.MessageBoxButtons.RetryCancel;
						break;
					}
				case MessageDialogButtons.YesNoCancel:
					{
						buttonsType = System.Windows.Forms.MessageBoxButtons.YesNoCancel;
						break;
					}
				case MessageDialogButtons.OK:
					{
						buttonsType = System.Windows.Forms.MessageBoxButtons.OK;
						break;
					}
				case MessageDialogButtons.OKCancel:
					{
						buttonsType = System.Windows.Forms.MessageBoxButtons.OKCancel;
						break;
					}
				case MessageDialogButtons.YesNo:
					{
						buttonsType = System.Windows.Forms.MessageBoxButtons.YesNo;
						break;
					}
			}

			if (parentHandle != null && dlg.Parent != null)
			{
				parentHandle = ((Engine as WindowsFormsEngine).GetHandleForControl(dlg.Parent) as WindowsFormsNativeControl).Handle;
			}

			if (dlg.Buttons == MessageDialogButtons.CancelTryContinue)
			{
				try
				{
					WindowsForms.Internal.Windows.Methods.MessageBox((parentHandle?.Handle).GetValueOrDefault(IntPtr.Zero), dlg.Content, dlg.Text, (uint)(6 | 0x00002000L | (uint)messageType));
					return DialogResult.None;
				}
				catch (Exception ex)
				{

				}
			}

			if (dlg.AutoUpgradeEnabled && System.Environment.OSVersion.Platform == PlatformID.Win32NT && System.Environment.OSVersion.Version.Major >= 6)
			{
				TaskDialog dlg2 = new TaskDialog();
				switch (dlg.Icon)
				{
					case MessageDialogIcon.Error:
					{
						dlg2.Icon = TaskDialogIcon.Error;
						break;
					}
					case MessageDialogIcon.Information:
					{
						dlg2.Icon = TaskDialogIcon.Information;
						break;
					}
					case MessageDialogIcon.Question:
					{
						dlg2.Icon = TaskDialogIcon.Question;
						break;
					}
					case MessageDialogIcon.Warning:
					{
						dlg2.Icon = TaskDialogIcon.Warning;
						break;
					}
				}
				dlg2.Content = dlg.Content;
				dlg2.Text = dlg.Text;
				switch (buttonsType)
				{
					case MessageBoxButtons.OK:
					{
						dlg2.ButtonsPreset = TaskDialogButtons.OK;
						break;
					}
					case MessageBoxButtons.YesNo:
					{
						dlg2.ButtonsPreset = TaskDialogButtons.Yes | TaskDialogButtons.No;
						break;
					}
					case MessageBoxButtons.OKCancel:
					{
						dlg2.ButtonsPreset = TaskDialogButtons.OK | TaskDialogButtons.Cancel;
						break;
					}
					case MessageBoxButtons.YesNoCancel:
					{
						dlg2.ButtonsPreset = TaskDialogButtons.Yes | TaskDialogButtons.No | TaskDialogButtons.Cancel;
						break;
					}
					case MessageBoxButtons.AbortRetryIgnore:
					{
						dlg2.ButtonsPreset = TaskDialogButtons.Custom;
						dlg2.Buttons.Add(new UserInterface.Controls.Button("Abort", DialogResult.Abort));
						dlg2.Buttons.Add(new UserInterface.Controls.Button("Retry", DialogResult.Abort));
						dlg2.Buttons.Add(new UserInterface.Controls.Button("Ignore", DialogResult.Abort));
						break;
					}
					case MessageBoxButtons.RetryCancel:
					{
						dlg2.ButtonsPreset = TaskDialogButtons.Retry | TaskDialogButtons.Cancel;
						break;
					}
				}
				return dlg2.ShowDialog();
			}

			System.Windows.Forms.DialogResult nativeResult = System.Windows.Forms.MessageBox.Show(dlg.Content, dlg.Text, buttonsType, messageType);
			return WindowsFormsEngine.SWFDialogResultToDialogResult(nativeResult);
		}
	}
}
