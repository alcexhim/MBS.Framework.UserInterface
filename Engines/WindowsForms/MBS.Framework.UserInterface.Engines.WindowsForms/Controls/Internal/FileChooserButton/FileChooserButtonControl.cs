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
namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.FileChooserButton
{
	public class FileChooserButtonControl : System.Windows.Forms.UserControl
	{
		internal System.Windows.Forms.TextBox txt = null;
		private System.Windows.Forms.Button btn = null;

		public FileChooserButtonControl()
		{
			txt = new System.Windows.Forms.TextBox();
			txt.Dock = System.Windows.Forms.DockStyle.Fill;
			txt.ReadOnly = true;
			txt.Text = "(empty)";
			txt.TextChanged += txt_TextChanged;
			Controls.Add(txt);

			btn = new System.Windows.Forms.Button();
			btn.Dock = System.Windows.Forms.DockStyle.Right;
			btn.Width = 16;
			btn.Text = "...";
			btn.Click += btn_Click;
			Controls.Add(btn);
		}

		internal bool InhibitTextChanged = false;
		internal bool InhibitSelectedFileNameChanged = false;

		private void txt_TextChanged(object sender, EventArgs e)
		{
			if (InhibitTextChanged) return;

			InhibitSelectedFileNameChanged = true;
			(Tag as UserInterface.Controls.FileChooserButton).SelectedFileName = txt.Text;
			InhibitSelectedFileNameChanged = false;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			txt.ReadOnly = (Tag as UserInterface.Controls.FileChooserButton).RequireExistingFile;
		}

		private void btn_Click(object sender, EventArgs e)
		{
			UserInterface.Controls.FileChooserButton fcb = (Tag as UserInterface.Controls.FileChooserButton);
			switch (fcb.DialogMode)
			{
				case UserInterface.Dialogs.FileDialogMode.CreateFolder:
				case UserInterface.Dialogs.FileDialogMode.SelectFolder:
				{
					Dialogs.Internal.FolderBrowserDialog.V2.FolderSelectDialog fbd = new Dialogs.Internal.FolderBrowserDialog.V2.FolderSelectDialog();
					fbd.Title = fcb.DialogTitle ?? "Select Folder";
					if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						fcb.SelectedFileName = fbd.FileName;
					}
					break;
				}
				case UserInterface.Dialogs.FileDialogMode.Open:
				{
					System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
					ofd.AutoUpgradeEnabled = true;
					ofd.Title = fcb.DialogTitle ?? "Open File";
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						fcb.SelectedFileName = ofd.FileName;
						txt.Text = ofd.FileName;
					}
					break;
				}
				case UserInterface.Dialogs.FileDialogMode.Save:
				{
					System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
					sfd.AutoUpgradeEnabled = true;
					sfd.Title = fcb.DialogTitle ?? "Save File";
					if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						fcb.SelectedFileName = sfd.FileName;
						txt.Text = sfd.FileName;
					}
					break;
				}
			}
		}
	}
}
