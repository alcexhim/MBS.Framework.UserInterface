//
//  ListViewColumn.cs
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
using System.Collections.Generic;

namespace MBS.Framework.UserInterface.Controls.ListView
{
	public class ListViewColumn : ICellRendererContainer
	{
		public class ListViewColumnCollection
			: System.Collections.ObjectModel.Collection<ListViewColumn>
		{
			private ListViewControl _parent = null;
			public ListViewColumnCollection(ListViewControl parent)
			{
				_parent = parent;
			}

			protected override void InsertItem(int index, ListViewColumn item)
			{
				base.InsertItem(index, item);
				item.Parent = _parent;
			}
			protected override void ClearItems()
			{
				for (int i = 0; i < Count; i++)
					this[i].Parent = null;
				base.ClearItems();
			}
			protected override void RemoveItem(int index)
			{
				this[index].Parent = null;
				base.RemoveItem(index);
			}
		}

		public CellRenderer.CellRendererCollection Renderers { get; } = new CellRenderer.CellRendererCollection();

		public ListViewControl Parent { get; private set; } = null;

		private TreeModelColumn mvarColumn = null;
		public TreeModelColumn Column { get { return mvarColumn; } set { mvarColumn = value; } }

		private string mvarTitle = String.Empty;
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private bool _Resizable = true;
		public bool Resizable
		{
			get
			{

				if (Parent != null)
				{
					if (Parent.IsCreated)
					{
						_Resizable = ((Parent.ControlImplementation as Native.IListViewNativeImplementation)?.IsColumnResizable(this)).GetValueOrDefault(_Resizable);
					}
				}
				return _Resizable;
			}
			set
			{
				if (Parent != null)
				{
					if (Parent.IsCreated)
					{
						(Parent.ControlImplementation as Native.IListViewNativeImplementation)?.SetColumnResizable(this, value);
					}
				}
				_Resizable = value;
			}
		}

		private bool _Reorderable = true;
		public bool Reorderable
		{
			get
			{
				if (Parent != null)
				{
					if (Parent.IsCreated)
					{
						_Reorderable = ((Parent.ControlImplementation as Native.IListViewNativeImplementation)?.IsColumnReorderable(this)).GetValueOrDefault(_Reorderable);
					}
				}
				return _Reorderable;
			}
			set
			{
				if (Parent != null)
				{
					if (Parent.IsCreated)
					{
						(Parent.ControlImplementation as Native.IListViewNativeImplementation)?.SetColumnReorderable(this, value);
					}
				}
				_Reorderable = value;
			}
		}

		private bool _Editable = false;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:MBS.Framework.UserInterface.Controls.ListViewColumn"/>
		/// is editable.
		/// </summary>
		/// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
		public bool Editable
		{
			get { return _Editable; }
			set
			{
				(Parent?.ControlImplementation as Native.IListViewNativeImplementation)?.SetColumnEditable(this, value);
				_Editable = value;
			}
		}

		public ListViewColumn(string title = "", IEnumerable<CellRenderer> renderers = null)
		{
			mvarTitle = title;
			if (renderers != null)
			{
				foreach (CellRenderer renderer in renderers)
				{
					Renderers.Add(renderer);
				}
			}
		}
	}
}
