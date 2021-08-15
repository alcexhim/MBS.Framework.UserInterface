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
using System.Collections.Generic;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Controls
{
	public class ToolbarItemSeparator
		: ToolbarItem
	{
	}
	public class ToolbarItemButton
		: ToolbarItem
	{
		public ToolbarItemButton(string name, string title = "", EventHandler onClick = null)
			: base(name, title)
		{
			if (onClick != null)
				Click += onClick;
		}
		public ToolbarItemButton(string name, StockType stockType, EventHandler onClick = null)
			: base(name, stockType)
		{
			if (onClick != null)
				Click += onClick;
		}

		public event EventHandler Click;
		protected virtual void OnClick(EventArgs e)
		{
			Click?.Invoke(this, e);
		}

		public ToolbarItemIconSize IconSize { get ; set; } = ToolbarItemIconSize.Default;
		public ToolbarItemDisplayStyle DisplayStyle { get; set; } = ToolbarItemDisplayStyle.Default;
		public bool CheckOnClick { get; set; } = false;
		public bool Checked { get; set; } = false;
	}
	public abstract class ToolbarItem : ISupportsExtraData
	{
		public class ToolbarItemCollection
			: System.Collections.ObjectModel.Collection<ToolbarItem>
		{
			private Dictionary<string, ToolbarItem> _itemsByName = new Dictionary<string, ToolbarItem>();
			public ToolbarItem this[string name]
			{
				get
				{
					if (_itemsByName.ContainsKey(name))
						return _itemsByName[name];
					return null;
				}
			}

			public bool Contains(string name)
			{
				return _itemsByName.ContainsKey(name);
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				_itemsByName.Clear();
			}
			protected override void InsertItem(int index, ToolbarItem item)
			{
				base.InsertItem(index, item);
				if (item.Name != null)
					_itemsByName[item.Name] = item;
			}
			protected override void RemoveItem(int index)
			{
				if (_itemsByName.ContainsKey(this[index].Name))
					_itemsByName.Remove(this[index].Name);
				base.RemoveItem(index);
			}
		}

		public string Name { get; set; } = String.Empty;
		public string Title { get; set; } = String.Empty;
		public StockType StockType { get; set; } = StockType.None;
		public Image Image { get; set; } = null;

		private bool _Enabled = true;
		public bool Enabled { get { return _Enabled; } set { _Enabled = value; (Application.Instance as UIApplication).Engine.SetToolbarItemEnabled(this, value); } }

		public ToolbarItem(string name = "", string title = "")
		{
			Name = name;
			Title = title;
		}
		public ToolbarItem(string name, StockType stockType)
		{
			Name = name;
			StockType = stockType;
		}

		#region ISupportsExtraData members
		private Dictionary<string, object> _ExtraData = new Dictionary<string, object>();
		public T GetExtraData<T>(string key, T defaultValue = default(T))
		{
			if (_ExtraData.ContainsKey(key)) return (T)_ExtraData[key];
			return defaultValue;
		}
		public object GetExtraData(string key, object defaultValue = null)
		{
			return GetExtraData<object>(key, defaultValue);
		}
		public void SetExtraData<T>(string key, T value)
		{
			_ExtraData[key] = value;
		}
		public void SetExtraData(string key, object value)
		{
			SetExtraData<object>(key, value);
		}
		#endregion
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
