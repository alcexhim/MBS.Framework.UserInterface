//
//  CommandItemExtensions.cs
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
namespace MBS.Framework.UserInterface
{
	public static class CommandItemExtensions
	{
		public static void AddToCommandBar(this CommandItem item, Command parent, CommandBar parentCommandBar)
		{
			CommandItem.CommandItemCollection coll = null;
			if (item != null)
			{
				if (parent == null)
				{
					if (parentCommandBar != null)
					{
						coll = parentCommandBar.Items;
					}
					else
					{
						coll = ((UIApplication)Application.Instance).MainMenu.Items;
					}
				}
				else
				{
					coll = parent.Items;
				}
			}

			if (coll != null)
			{
				int insertIndex = -1;
				if (item.InsertAfterID != null)
				{
					insertIndex = coll.IndexOf(item.InsertAfterID) + 1;
				}
				else if (item.InsertBeforeID != null)
				{
					insertIndex = coll.IndexOf(item.InsertBeforeID);
				}

				if (insertIndex != -1)
				{
					coll.Insert(insertIndex, item);
				}
				else
				{
					coll.Add(item);
				}
			}
		}
	}
}
