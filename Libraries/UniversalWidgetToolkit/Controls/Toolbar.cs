//
//  Toolbar.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 
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
namespace UniversalWidgetToolkit.Controls
{
	public class ToolbarItemButton
		: ToolbarItem
	{
		public ToolbarItemButton(string name, string title = "")
			: base(name, title)
		{

		}
	}
	public abstract class ToolbarItem
	{
		public class ToolbarItemCollection
			: System.Collections.ObjectModel.Collection<ToolbarItem>
		{

		}

		public string Name { get; set; } = String.Empty;
		public string Title { get; set; } = String.Empty;

		public ToolbarItem(string name, string title = "")
		{
			Name = name;
			Title = title;
		}
	}
	public class Toolbar : SystemControl
	{
		public Toolbar()
		{
			this.Items = new ToolbarItem.ToolbarItemCollection();
		}
		public ToolbarItem.ToolbarItemCollection Items { get; private set; } = null;
	}
}
