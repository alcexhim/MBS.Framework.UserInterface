//
//  SVGStyleProperty.cs
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

namespace MBS.Framework.UserInterface.Drawing.Drawing2D.SVG
{
	public class SVGStyleProperty
	{
		public class SVGStylePropertyCollection
			: System.Collections.ObjectModel.Collection<SVGStyleProperty>
		{
			private Dictionary<string, SVGStyleProperty> _itemsByName = new Dictionary<string, SVGStyleProperty>();
			protected override void ClearItems()
			{
				base.ClearItems();
				_itemsByName.Clear();
			}
			protected override void InsertItem(int index, SVGStyleProperty item)
			{
				base.InsertItem(index, item);
				_itemsByName[item.Name] = item;
			}
			protected override void RemoveItem(int index)
			{
				_itemsByName.Remove(this[index].Name);
				base.RemoveItem(index);
			}

			public SVGStyleProperty this[string name]
			{
				get
				{
					if (!_itemsByName.ContainsKey(name))
						return null;
					return _itemsByName[name];
				}
			}
		}

		public string Name { get; set; } = null;
		public string Value { get; set; } = null;

		public SVGStyleProperty(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}
