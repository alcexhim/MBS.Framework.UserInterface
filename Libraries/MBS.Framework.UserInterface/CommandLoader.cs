//
//  CommandLoader.cs
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
using MBS.Framework.UserInterface.Input.Keyboard;
using UniversalEditor.ObjectModels.Markup;

namespace MBS.Framework.UserInterface
{
	public class CommandLoader
	{
		public static Command FromMarkup(MarkupTagElement tagCommand)
		{
			UICommand cmd = new UICommand();
			cmd.ID = tagCommand.Attributes["ID"]?.Value;

			MarkupAttribute attDefaultCommandID = tagCommand.Attributes["DefaultCommandID"];
			if (attDefaultCommandID != null)
			{
				cmd.DefaultCommandID = attDefaultCommandID.Value;
			}

			MarkupAttribute attCommandStockType = tagCommand.Attributes["StockType"];
			if (attCommandStockType != null)
			{
				StockType stockType = StockType.None;
				string[] names = Enum.GetNames(typeof(StockType));
				int[] values = (int[])Enum.GetValues(typeof(StockType));
				for (int i = 0; i < names.Length; i++)
				{
					if (names[i].Equals(attCommandStockType.Value))
					{
						stockType = (StockType)values[i];
						break;
					}
				}
				cmd.StockType = stockType;
			}

			MarkupAttribute attTitle = tagCommand.Attributes["Title"];
			if (attTitle != null)
			{
				cmd.Title = attTitle.Value;
			}
			else
			{
				cmd.Title = cmd.ID;
			}

			MarkupAttribute attEnabled = tagCommand.Attributes["Enabled"];
			if (attEnabled != null)
			{
				cmd.Enabled = (attEnabled.Value.ToLower() == "true");
			}

			MarkupTagElement tagShortcut = (tagCommand.Elements["Shortcut"] as MarkupTagElement);
			if (tagShortcut != null)
			{
				MarkupAttribute attModifiers = tagShortcut.Attributes["Modifiers"];
				MarkupAttribute attKey = tagShortcut.Attributes["Key"];
				if (attKey != null)
				{
					KeyboardModifierKey modifiers = KeyboardModifierKey.None;
					if (attModifiers != null)
					{
						string[] strModifiers = attModifiers.Value.Split(new char[] { ',' });
						foreach (string strModifier in strModifiers)
						{
							switch (strModifier.Trim().ToLower())
							{
								case "alt":
								{
									modifiers |= KeyboardModifierKey.Alt;
									break;
								}
								case "control":
								{
									modifiers |= KeyboardModifierKey.Control;
									break;
								}
								case "meta":
								{
									modifiers |= KeyboardModifierKey.Meta;
									break;
								}
								case "shift":
								{
									modifiers |= KeyboardModifierKey.Shift;
									break;
								}
								case "super":
								{
									modifiers |= KeyboardModifierKey.Super;
									break;
								}
							}
						}
					}

					KeyboardKey value = KeyboardKey.None;
					if (!Enum.TryParse<KeyboardKey>(attKey.Value, out value))
					{
						Console.WriteLine("ue: ui: unable to parse keyboard key '{0}'", attKey.Value);
					}

					cmd.Shortcut = new Shortcut(value, modifiers);
				}
			}

			MarkupTagElement tagItems = (tagCommand.Elements["Items"] as MarkupTagElement);
			if (tagItems != null)
			{
				foreach (MarkupElement el in tagItems.Elements)
				{
					MarkupTagElement tag = (el as MarkupTagElement);
					if (tag == null) continue;

					UIApplication.InitializeCommandBarItem(tag, cmd, null);
				}
			}
			return cmd;
		}
	}
}
