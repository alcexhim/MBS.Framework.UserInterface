//
//  MainWindow.cs
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
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface
{
	public class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeMainMenu();
		}

		private void InitializeMainMenu()
		{
			foreach (CommandItem ci in Application.MainMenu.Items)
			{
				MenuItem[] mi = MenuItem.LoadMenuItem(ci, MainWindow_MenuBar_Item_Click);
				if (mi == null || mi.Length == 0)
					continue;

				for (int i = 0; i < mi.Length; i++)
				{
					this.MenuBar.Items.Add(mi[i]);
				}
			}
		}

		private void MainWindow_MenuBar_Item_Click(object sender, EventArgs e)
		{
			CommandMenuItem mi = (sender as CommandMenuItem);
			if (mi == null)
				return;

			Application.ExecuteCommand(mi.Name);
		}

		protected internal override void OnCreating(EventArgs e)
		{
			base.OnCreating(e);

			for (int i = 0; i < Application.QuickAccessToolbarItems.Count; i++)
			{
				Button button = new Button();
				CommandReferenceCommandItem cmdr = (Application.QuickAccessToolbarItems[i] as CommandReferenceCommandItem);
				if (cmdr != null)
				{
					Command cmd = Application.Commands[cmdr.CommandID];
					if (cmd != null)
					{
						if (cmd.StockType != StockType.None)
						{
							button.Image = Image.FromStock(cmd.StockType, 16);
						}
						else if (cmd.ImageFileName != null)
						{
							button.Image = Image.FromName(cmd.ImageFileName, 16);
						}
						button.TooltipText = cmd.Title.Replace("_", String.Empty);
					}
				}
				button.SetExtraData<CommandItem>("item", Application.QuickAccessToolbarItems[i]);
				button.Click += delegate (object sender, EventArgs ee)
				{
					CommandItem ci = (sender as Button).GetExtraData<CommandItem>("item");
					if (ci is CommandReferenceCommandItem)
					{
						Application.ExecuteCommand((ci as CommandReferenceCommandItem).CommandID);
					}
				};
				TitleBarButtons.Add(button);
			}
		}
	}
}
