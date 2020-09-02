//
//  FileChooserButtonImplementation.cs
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
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls
{
	[ControlImplementation(typeof(FileChooserButton))]
	public class FileChooserButtonImplementation : WindowsFormsNativeImplementation, UserInterface.Controls.Native.IFileChooserButtonImplementation
	{
		public FileChooserButtonImplementation(Engine engine, FileChooserButton control) : base(engine, control)
		{
		}

		public void FileNamesChanged()
		{
			Internal.FileChooserButton.FileChooserButtonControl handle = (Handle as WindowsFormsNativeControl).Handle as Internal.FileChooserButton.FileChooserButtonControl;
			if (handle.InhibitSelectedFileNameChanged) return;

			handle.InhibitTextChanged = true;
			handle.txt.Text = (Control as FileChooserButton).SelectedFileName;
			handle.InhibitTextChanged = false;
		}

		private bool _RequireExistingFile = true;
		public bool GetRequireExistingFile()
		{
			return _RequireExistingFile;
		}
		public void SetRequireExistingFile(bool value)
		{
			_RequireExistingFile = value;
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			FileChooserButton btn = (control as FileChooserButton);
			Internal.FileChooserButton.FileChooserButtonControl handle = new Internal.FileChooserButton.FileChooserButtonControl();
			handle.Tag = btn;
			return new WindowsFormsNativeControl(handle);
		}
	}
}
