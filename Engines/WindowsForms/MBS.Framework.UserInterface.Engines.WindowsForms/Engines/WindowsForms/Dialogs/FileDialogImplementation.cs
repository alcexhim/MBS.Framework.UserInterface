//
//  FileDialogImplementation.cs
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
using MBS.Framework.UserInterface.Dialogs;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Engines.WindowsForms.Dialogs
{
	[ControlImplementation(typeof(FileDialog))]
	public class FileDialogImplementation : WindowsFormsNativeImplementation
	{
		public FileDialogImplementation(Engine engine, FileDialog control) : base(engine, control)
		{
		}

		protected override void SetControlVisibilityInternal(bool visible)
		{
			System.Windows.Forms.CommonDialog dlg = (Handle as WindowsFormsNativeDialog).Handle;
			if (visible)
			{
				dlg.ShowDialog();
			}
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			FileDialog dlg = (control as FileDialog);
			switch (dlg.Mode)
			{
				case FileDialogMode.CreateFolder:
				{
					break;
				}
				case FileDialogMode.Open:
				{
					System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
					return new WindowsFormsNativeDialog(ofd);
				}
				case FileDialogMode.Save:
				{
					System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
					return new WindowsFormsNativeDialog(sfd);
				}
				case FileDialogMode.SelectFolder:
				{
					System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
					return new WindowsFormsNativeDialog(fbd);
				}
			}
			throw new NotSupportedException();
		}
	}
}
