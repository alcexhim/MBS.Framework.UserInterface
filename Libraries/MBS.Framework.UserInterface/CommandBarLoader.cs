//
//  CommandBarLoader.cs
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
using System.Collections.Generic;
using MBS.Framework.UserInterface.Controls;
using UniversalEditor.ObjectModels.Markup;

namespace MBS.Framework.UserInterface
{
	public static class CommandBarLoader
	{
		private static Dictionary<CommandBar, List<Toolbar>> _ToolbarsForCommandBar = new Dictionary<CommandBar, List<Toolbar>>();
		private static bool RegisterCommandBar(CommandBar cb, Toolbar tb)
		{
			if (!_ToolbarsForCommandBar.ContainsKey(cb))
			{
				_ToolbarsForCommandBar[cb] = new List<Toolbar>();
			}
			if (_ToolbarsForCommandBar[cb].Contains(tb))
				return false;

			_ToolbarsForCommandBar[cb].Add(tb);
			return true;
		}

		private static Toolbar[] ToolbarsForCommandBar(CommandBar cb)
		{
			if (!_ToolbarsForCommandBar.ContainsKey(cb))
				return null;
			return _ToolbarsForCommandBar[cb].ToArray();
		}

		private static void tsbCommand_Click(object sender, EventArgs e)
		{
			ToolbarItemButton tsb = (sender as ToolbarItemButton);
			CommandReferenceCommandItem crci = tsb.GetExtraData<CommandReferenceCommandItem>("crci");
			((UIApplication)Application.Instance).ExecuteCommand(crci.CommandID);
		}

		public static ToolbarItem[] LoadCommandBarItem(CommandItem ci)
		{
			System.Diagnostics.Contracts.Contract.Assert(ci != null);

			if (ci is SeparatorCommandItem)
			{
				return new ToolbarItem[] { new ToolbarItemSeparator() };
			}
			else if (ci is CommandReferenceCommandItem)
			{
				CommandReferenceCommandItem crci = (ci as CommandReferenceCommandItem);
				Command cmd = ((UIApplication)Application.Instance).Commands[crci.CommandID];
				if (cmd == null) return null;

				ToolbarItemButton tsb = new ToolbarItemButton(cmd.ID, (StockType)cmd.StockType);
				if (!String.IsNullOrEmpty(cmd.ImageFileName))
				{
					string fullPath = ((UIApplication)Application.Instance).ExpandRelativePath(cmd.ImageFileName);
					if (fullPath == null)
					{
						tsb.Image = Drawing.Image.FromName(cmd.ImageFileName, 16);
					}
					else
					{
						tsb.Image = Drawing.Image.FromFile(cmd.ImageFileName);
					}
				}
				tsb.SetExtraData<CommandReferenceCommandItem>("crci", crci);
				tsb.Click += tsbCommand_Click;
				tsb.Title = cmd.Title;
				return new ToolbarItem[] { tsb };
			}
			else if (ci is GroupCommandItem)
			{
				GroupCommandItem gci = (ci as GroupCommandItem);
				List<ToolbarItem> list = new List<ToolbarItem>();
				for (int i = 0; i < gci.Items.Count; i++)
				{
					ToolbarItem[] items = LoadCommandBarItem(gci.Items[i]);
					list.AddRange(items);
				}
				return list.ToArray();
			}
			throw new NotImplementedException(String.Format("type of CommandItem '{0}' not implemented", ci.GetType()));
		}

		public static Toolbar LoadCommandBar(CommandBar cb)
		{
			Toolbar tb = new Toolbar();
			for (int i = 0; i < cb.Items.Count; i++)
			{
				ToolbarItem[] items = LoadCommandBarItem(cb.Items[i]);
				if (items != null && items.Length > 0)
				{
					for (int j = 0; j < items.Length; j++)
					{
						tb.Items.Add(items[j]);
						if (cb.Items[i] is CommandReferenceCommandItem && tb.Items[j] is ToolbarItemButton)
						{
							((UIApplication)Application.Instance).AssociateCommandWithNativeObject(Application.Instance.FindCommand((cb.Items[i] as CommandReferenceCommandItem).CommandID), items[j] as ToolbarItemButton);
						}
					}
				}
			}
			RegisterCommandBar(cb, tb);
			return tb;
		}

		public static CommandBar LoadCommandBarXML(MarkupTagElement tag)
		{
			MarkupAttribute attID = tag.Attributes["ID"];
			if (attID == null) return null;

			CommandBar cb = new CommandBar();
			cb.ID = attID.Value;

			MarkupAttribute attTitle = tag.Attributes["Title"];
			if (attTitle != null)
			{
				cb.Title = attTitle.Value;
			}
			else
			{
				cb.Title = cb.ID;
			}

			MarkupTagElement tagItems = tag.Elements["Items"] as MarkupTagElement;
			if (tagItems != null)
			{
				foreach (MarkupElement elItem in tagItems.Elements)
				{
					MarkupTagElement tagItem = (elItem as MarkupTagElement);
					if (tagItem == null) continue;

					CommandItem item = CommandItemLoader.FromMarkup(tagItem);
					item.AddToCommandBar(null, cb);
				}
			}

			return cb;
		}
	}
}
