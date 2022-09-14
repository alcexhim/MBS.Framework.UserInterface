//
//  KeyboardSettingsChangeBindingDialog.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2021 Mike Becker's Software
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
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Internal.Controls.KeyboardSettings
{
	[ContainerLayout(typeof(KeyboardSettingsChangeBindingDialog), "MBS.Framework.UserInterface.Internal.Controls.KeyboardSettings.KeyboardSettingsChangeBindingDialog.glade")]
	public class KeyboardSettingsChangeBindingDialog : CustomDialog
	{
		private Button cmdOK;
		private Label lblCommandName;
		private Label lblShortcut;
		private ImageView img;

		[EventHandler(nameof(cmdOK), nameof(Control.Click))]
		private void cmdOK_Click(object sender, EventArgs e)
		{
			if (Binding != null)
			{
				Binding.Key = _Key;
				Binding.MouseButtons = _MouseButtons;
				Binding.ModifierKeys = _ModifierKeys;
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		public CommandBinding Binding { get; set; } = null;
		public Command Command { get; set; } = null;

		private KeyboardKey _Key = KeyboardKey.None;
		private MouseButtons _MouseButtons = MouseButtons.None;
		private KeyboardModifierKey _ModifierKeys = KeyboardModifierKey.None;

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);
			lblCommandName.Text = Command?.Title;
		}

		protected internal override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Key == KeyboardKey.LMenu || e.Key == KeyboardKey.RMenu || e.Key == KeyboardKey.LShiftKey || e.Key == KeyboardKey.RShiftKey || e.Key == KeyboardKey.LControlKey || e.Key == KeyboardKey.RControlKey)
			{
				return;
			}
			lblShortcut.Text = CommandBinding.GetString(e.Key, e.ModifierKeys);
			_Key = e.Key;
			_ModifierKeys = e.ModifierKeys;
			_MouseButtons = MouseButtons.None;
			e.Cancel = true;

			cmdOK.Enabled = true;
		}
		[EventHandler(nameof(img), nameof(Control.MouseDown))]
		private void img_MouseDown(object sender, MouseEventArgs e)
		{
			lblShortcut.Text = CommandBinding.GetString(e.Buttons, e.ModifierKeys);
			_Key = KeyboardKey.None;
			_ModifierKeys = e.ModifierKeys;
			_MouseButtons = e.Buttons;
			e.Cancel = true;

			cmdOK.Enabled = true;
		}
	}
}
