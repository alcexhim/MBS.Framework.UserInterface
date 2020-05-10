//
//  HexEditorHighlightArea.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
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
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface.Controls.HexEditor
{
	public class HexEditorHighlightArea
	{
		public class HexEditorHighlightAreaCollection
			: System.Collections.ObjectModel.Collection<HexEditorHighlightArea>
		{
			private HexEditorControl _parent = null;
			internal HexEditorHighlightAreaCollection(HexEditorControl parent)
			{
				_parent = parent;
			}

			private Dictionary<string, HexEditorHighlightArea> _itemsByName = new Dictionary<string, HexEditorHighlightArea>();
			public HexEditorHighlightArea this[string name]
			{
				get
				{
					if (_itemsByName.ContainsKey(name))
						return _itemsByName[name];
					return null;
				}
				set
				{
					if (Contains(value))
					{
					}
					else
					{
						Add(value);
					}

					_itemsByName[name] = value;
				}
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				_itemsByName.Clear();
				_parent.Refresh();
			}
			protected override void InsertItem(int index, HexEditorHighlightArea item)
			{
				base.InsertItem(index, item);

				item._parent = this;
				_itemsByName[item.Name] = item;
				_parent.Refresh();
			}
			protected override void RemoveItem(int index)
			{
				this[index]._parent = null;
				_itemsByName.Remove(this[index].Name);

				base.RemoveItem(index);
				_parent.Refresh();
			}

			internal void UpdateName(HexEditorHighlightArea area, string name)
			{
				_itemsByName.Remove(area.Name);
				_itemsByName[name] = area;
			}
		}

		public HexEditorHighlightArea()
		{
		}
		public HexEditorHighlightArea(string name, string title, int start, int length, Color color)
		{
			Name = name;
			Title = title;
			Start = start;
			Length = length;
			BackColor = color;
		}

		private HexEditorHighlightAreaCollection _parent = null;

		private string _Name = String.Empty;
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_parent != null) _parent.UpdateName(this, value);
				_Name = value;
			}
		}
		public string Title { get; set; } = String.Empty;

		public int Start { get; set; } = 0;
		public int Length { get; set; } = 0;

		public Color BackColor { get; set; } = Color.Empty;
		public Color ForeColor { get; set; } = Color.Empty;
	}
}
