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
using System.Runtime.InteropServices;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(FileChooserButton))]
	public class FileChooserButtonImplementation : GTKNativeImplementation, UserInterface.Controls.Native.IFileChooserButtonImplementation
	{
		public FileChooserButtonImplementation(Engine engine, FileChooserButton control) : base(engine, control)
		{
			file_set_d = new Action<IntPtr>(file_set);
		}

		private Action<IntPtr> file_set_d = null;
		private void file_set(IntPtr /*GtkFileChooserButton*/ fc)
		{
			_InhibitFileNamesChanged = true;
			FileChooserButton fcb = (Control as FileChooserButton);
			IntPtr lstFileNames = Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_get_filenames(fc);
			uint length = Internal.GLib.Methods.g_list_length(lstFileNames);
			for (uint i = 0; i < length; i++)
			{
				IntPtr hFileName = Internal.GLib.Methods.g_list_nth_data(lstFileNames, i);
				string filename = Marshal.PtrToStringAuto(hFileName);
				fcb.SelectedFileNames.Add(filename);
			}
			InvokeMethod(fcb, "OnChanged", new object[] { EventArgs.Empty });
			_InhibitFileNamesChanged = false;
		}

		private TextBox fallbackTextBox = null;

		protected override NativeControl CreateControlInternal(Control control)
		{
			FileChooserButton btn = (control as FileChooserButton);

			IntPtr handle = Internal.GTK.Methods.GtkBox.gtk_box_new(Internal.GTK.Constants.GtkOrientation.Vertical);


			string title = btn.DialogTitle;
			Internal.GTK.Constants.GtkFileChooserAction fca = Internal.GTK.Constants.GtkFileChooserAction.Open;
			switch (btn.DialogMode)
			{
				case FileDialogMode.CreateFolder:
				{
					fca = Internal.GTK.Constants.GtkFileChooserAction.CreateFolder;
					if (title == null)
						title = "Create Folder";
					break;
				}
				case FileDialogMode.Open:
				{
					fca = Internal.GTK.Constants.GtkFileChooserAction.Open;
					if (title == null)
						title = "Open";
					break;
				}
				case FileDialogMode.Save:
				{
					fca = Internal.GTK.Constants.GtkFileChooserAction.Save;
					if (title == null)
						title = "Save";
					break;
				}
				case FileDialogMode.SelectFolder:
				{
					fca = Internal.GTK.Constants.GtkFileChooserAction.SelectFolder;
					if (title == null)
						title = "Select Folder";
					break;
				}
			}

			IntPtr hFCB = Internal.GTK.Methods.GtkFileChooserButton.gtk_file_chooser_button_new(title, fca);
			Internal.GObject.Methods.g_signal_connect(handle, "file_set", file_set_d);
			Internal.GTK.Methods.GtkBox.gtk_box_pack_start(handle, hFCB, true, true, 0);

			Container fallbackContainer = new Container();
			fallbackContainer.Layout = new Layouts.BoxLayout(Orientation.Horizontal);
			fallbackTextBox = new TextBox();
			fallbackTextBox.Changed += FallbackTextBox_Changed;
			fallbackContainer.Controls.Add(fallbackTextBox, new Layouts.BoxLayout.Constraints(true, true));

			Button fallbackButton = new Button();
			fallbackButton.AlwaysShowImage = true;
			fallbackButton.Image = UserInterface.Drawing.Image.FromName("gtk-file", 8);
			fallbackButton.TooltipText = "Browse";
			fallbackButton.Click += FallbackButton_Click;
			fallbackContainer.Controls.Add(fallbackButton, new Layouts.BoxLayout.Constraints(false, false));

			Engine.CreateControl(fallbackContainer);
			IntPtr hFallback = (Engine.GetHandleForControl(fallbackContainer) as GTKNativeControl).Handle;

			Internal.GTK.Methods.GtkBox.gtk_box_pack_start(handle, hFallback, true, true, 0);
			Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hFallback);

			return new GTKNativeControl(handle, new System.Collections.Generic.KeyValuePair<string, IntPtr>[]
			{
				new System.Collections.Generic.KeyValuePair<string, IntPtr>("fcb", hFCB),
				new System.Collections.Generic.KeyValuePair<string, IntPtr>("fallback", hFallback)
			});
		}

		private string _OldFallbackTextBoxText = null;
		void FallbackTextBox_Changed(object sender, EventArgs e)
		{
			bool changed = (_OldFallbackTextBoxText != (sender as TextBox).Text);
			if (changed)
			{
				InvokeMethod((Control as FileChooserButton), "OnChanged", new object[] { EventArgs.Empty });
			}
		}


		void FallbackButton_Click(object sender, EventArgs e)
		{
			FileChooserButton btn = (Control as FileChooserButton);

			FileDialog dlg = new FileDialog();
			dlg.Mode = btn.DialogMode;
			dlg.Text = btn.DialogTitle;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				btn.SelectedFileName = dlg.SelectedFileName;
			}
		}


		protected override void OnRealize(EventArgs e)
		{
			base.OnRealize(e);

			// weird, but it gets the job done...
			IntPtr hFCB = (Handle as GTKNativeControl).GetNamedHandle("fcb");
			IntPtr hFallback = (Handle as GTKNativeControl).GetNamedHandle("fallback");

			if (_RequireExistingFile)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hFallback);
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hFCB);
			}
			else
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hFCB);
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hFallback);
			}
		}

		private bool _InhibitFileNamesChanged = false;
		public void FileNamesChanged()
		{
			if (_InhibitFileNamesChanged) return;

			IntPtr hFCB = (Handle as GTKNativeControl).GetNamedHandle("fcb");
			FileChooserButton fcb = (Control as FileChooserButton);
			Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_set_filename(hFCB, fcb.SelectedFileName);

			fallbackTextBox.Text = fcb.SelectedFileName;
		}

		private bool _RequireExistingFile = true;
		public bool GetRequireExistingFile()
		{
			return _RequireExistingFile;
		}
		public void SetRequireExistingFile(bool value)
		{
			_RequireExistingFile = value;

			IntPtr hFCB = (Handle as GTKNativeControl).GetNamedHandle("fcb");
			IntPtr hFallback = (Handle as GTKNativeControl).GetNamedHandle("fallback");

			if (_RequireExistingFile)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hFallback);
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hFCB);
			}
			else
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hFCB);
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hFallback);
			}
		}
	}
}
