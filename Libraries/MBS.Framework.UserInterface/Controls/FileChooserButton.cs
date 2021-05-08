//
//  FileChooserButton.cs
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
using MBS.Framework.UserInterface.Dialogs;

namespace MBS.Framework.UserInterface.Controls
{
	namespace Native
	{
		public interface IFileChooserButtonImplementation
		{
			void FileNamesChanged();
			bool GetRequireExistingFile();
			void SetRequireExistingFile(bool value);
		}
	}
	public class FileChooserButton : SystemControl
	{
		public FileChooserButton()
		{
			SelectedFileNames.CollectionChanged += SelectedFileNames_CollectionChanged;
		}

		void SelectedFileNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			(ControlImplementation as Native.IFileChooserButtonImplementation)?.FileNamesChanged();
		}

		public System.Collections.ObjectModel.ObservableCollection<string> SelectedFileNames { get; } = new System.Collections.ObjectModel.ObservableCollection<string>();
		public string SelectedFileName
		{
			get
			{
				if (SelectedFileNames.Count > 0)
				{
					return SelectedFileNames[SelectedFileNames.Count - 1];
				}
				return null;
			}
			set
			{
				SelectedFileNames.Clear();
				SelectedFileNames.Add(value);
			}
		}

		public FileDialogMode DialogMode { get; set; } = FileDialogMode.Open;
		public string DialogTitle { get; set; } = null;

		private bool _RequireExistingFile = true;
		public bool RequireExistingFile
		{
			get
			{
				if (IsCreated)
				{
					Native.IFileChooserButtonImplementation impl = (ControlImplementation as Native.IFileChooserButtonImplementation);
					if (impl != null)
						return impl.GetRequireExistingFile();
				}
				return _RequireExistingFile;
			}
			set
			{
				if (IsCreated)
				{
					Native.IFileChooserButtonImplementation impl = (ControlImplementation as Native.IFileChooserButtonImplementation);
					impl?.SetRequireExistingFile(value);
				}
				_RequireExistingFile = value;
			}
		}

		public event EventHandler Changed;
		protected virtual void OnChanged(EventArgs e)
		{
			Changed?.Invoke(this, e);
		}
	}
}
