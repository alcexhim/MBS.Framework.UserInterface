//
//  KeyboardSettingsControl.cs
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
using MBS.Framework.UserInterface.Controls.ListView;

namespace MBS.Framework.UserInterface.Internal.Controls.KeyboardSettings
{
	[ContainerLayout(typeof(KeyboardSettingsControl), "MBS.Framework.UserInterface.Internal.Controls.KeyboardSettings.KeyboardSettingsControl.glade")]
	public class KeyboardSettingsControl : Container
	{
		private ListViewControl lvCommandBindings;

		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			foreach (CommandBinding binding in (Application.Instance as UIApplication).CommandBindings)
			{
				Command cmd = Application.Instance.FindCommand(binding.CommandID);
				Context ctx = Application.Instance.FindContext(binding.ContextID ?? Guid.Empty);
				TreeModelRow row = new TreeModelRow(new TreeModelRowColumn[]
				{
					new TreeModelRowColumn(lvCommandBindings.Model.Columns[0], cmd?.Title.Replace("_", String.Empty)),
					new TreeModelRowColumn(lvCommandBindings.Model.Columns[1], binding.ToString()),
					new TreeModelRowColumn(lvCommandBindings.Model.Columns[2], ctx?.Name)
				});
				row.SetExtraData<CommandBinding>("binding", binding);
				lvCommandBindings.Model.Rows.Add(row);
			}
		}

		private Menu mnuContext = null;

		[EventHandler(nameof(lvCommandBindings), nameof(Control.BeforeContextMenu))]
		private void lvCommandBindings_BeforeContextMenu(object sender, EventArgs e)
		{
			if (mnuContext == null)
			{
				mnuContext = new Menu();
				mnuContext.Items.Add(new CommandMenuItem("_Change", null, mnuContextChange_Click));
				mnuContext.Items.Add(new SeparatorMenuItem());
				mnuContext.Items.Add(new CommandMenuItem("_Reset", null, mnuContextReset_Click));
			}
			lvCommandBindings.ContextMenu = mnuContext;
		}
		[EventHandler(nameof(lvCommandBindings), nameof(ListViewControl.RowActivated))]
		private void lvCommandBindings_RowActivated(object sender, ListViewRowActivatedEventArgs e)
		{
			if (e.Row == null) return;
			mnuContextChange_Click(sender, e);
		}

		private void mnuContextChange_Click(object sender, EventArgs e)
		{
			KeyboardSettingsChangeBindingDialog dlg = new KeyboardSettingsChangeBindingDialog();
			dlg.Binding = lvCommandBindings.SelectedRows[0].GetExtraData<CommandBinding>("binding");

			Command cmd = Application.Instance.FindCommand(dlg.Binding.CommandID);
			dlg.Command = cmd;
			if (dlg.ShowDialog(ParentWindow) == DialogResult.OK)
			{
			}
		}
		private void mnuContextReset_Click(object sender, EventArgs e)
		{

		}
	}
}
