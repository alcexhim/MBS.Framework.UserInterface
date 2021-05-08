//
//  CommandItemLoader.cs
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
using UniversalEditor.ObjectModels.Markup;

namespace MBS.Framework.UserInterface
{
	public class CommandItemLoader
	{public static CommandItem FromMarkup(MarkupTagElement tag)
		{
			CommandItem item = null;

			MarkupAttribute attInsertAfter = tag.Attributes["InsertAfter"];
			MarkupAttribute attInsertBefore = tag.Attributes["InsertBefore"];

			switch (tag.FullName)
			{
				case "CommandReference":
				{
					MarkupAttribute attCommandID = tag.Attributes["CommandID"];
					if (attCommandID != null)
					{
						item = new CommandReferenceCommandItem(attCommandID.Value);
					}

					MarkupAttribute attHorizontalAlignment = tag.Attributes["HorizontalAlignment"];
					if (attHorizontalAlignment != null)
					{
						/*
						if (Enum.TryParse<MenuItemHorizontalAlignment>(attHorizontalAlignment.Value, out MenuItemHorizontalAlignment value))
						{
							item.HorizontalAlignment = value;
						}
						*/
					}
					break;
				}
				case "CommandPlaceholder":
				{
					MarkupAttribute attPlaceholderID = tag.Attributes["PlaceholderID"];
					if (attPlaceholderID != null)
					{
						item = new CommandPlaceholderCommandItem(attPlaceholderID.Value);
					}
					break;
				}
				case "Separator":
				{
					item = new SeparatorCommandItem();
					break;
				}
				case "Group":
				{
					item = new GroupCommandItem();

					MarkupTagElement tagItems = (tag.Elements["Items"] as MarkupTagElement);
					if (tagItems != null)
					{
						for (int i = 0; i < tagItems.Elements.Count; i++)
						{
							MarkupTagElement tag1 = (tagItems.Elements[i] as MarkupTagElement);
							if (tag1 == null) continue;

							CommandItem childItem = FromMarkup(tag1);
							(item as GroupCommandItem).Items.Add(childItem);
						}
					}
					break;
				}
				default:
				{
					if (System.Diagnostics.Debugger.IsAttached)
					{
						throw new ArgumentException(String.Format("unrecognized CommandBar Item type '{0}'", tag.FullName));
					}
					break;
				}
			}

			if (item != null)
			{
				if (attInsertAfter != null)
					item.InsertAfterID = attInsertAfter.Value;
				if (attInsertBefore != null)
					item.InsertBeforeID = attInsertBefore.Value;
			}
			return item;
		}
	}
}
