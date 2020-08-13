//
//  CollectionListView.cs
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
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls
{
	public class CollectionListView : Container
	{
		private Toolbar tbItems;
		private ListView lvItems;

		public ListView ListView { get { return lvItems; } }

		private void InitializeComponent()
		{
			Layout = new BoxLayout(Orientation.Vertical);

			tbItems = new Toolbar();
			tbItems.Items.Add(new ToolbarItemButton("tsbItemAdd", StockType.Add, tsbItemAdd_Click));
			tbItems.Items.Add(new ToolbarItemButton("tsbItemEdit", StockType.Edit, tsbItemEdit_Click));
			tbItems.Items.Add(new ToolbarItemButton("tsbItemRemove", StockType.Remove, tsbItemRemove_Click));
			tbItems.Items.Add(new ToolbarItemSeparator());
			tbItems.Items.Add(new ToolbarItemButton("tsbItemMoveUp", StockType.ArrowUp, tsbItemMoveUp_Click));
			tbItems.Items.Add(new ToolbarItemButton("tsbItemMoveDown", StockType.ArrowDown, tsbItemMoveDown_Click));
			tbItems.Items.Add(new ToolbarItemSeparator());
			tbItems.Items.Add(new ToolbarItemButton("tsbItemClear", StockType.Clear, tsbItemClear_Click));

			Controls.Add(tbItems, new BoxLayout.Constraints(false, false));

			lvItems = new ListView();
			Controls.Add(lvItems, new BoxLayout.Constraints(true, true));
		}

		public event EventHandler ItemAdding;
		protected virtual void OnItemAdding(EventArgs e)
		{
			ItemAdding?.Invoke(this, e);
		}
		private void tsbItemAdd_Click(object sender, EventArgs e)
		{
			OnItemAdding(e);
		}

		public event EventHandler ItemEditing;
		protected virtual void OnItemEditing(EventArgs e)
		{
			ItemEditing?.Invoke(this, e);
		}
		private void tsbItemEdit_Click(object sender, EventArgs e)
		{
			OnItemEditing(e);
		}

		public event EventHandler ItemRemoving;
		protected virtual void OnItemRemoving(EventArgs e)
		{
			ItemRemoving?.Invoke(this, e);
		}
		private void tsbItemRemove_Click(object sender, EventArgs e)
		{
			OnItemRemoving(e);
		}

		private void tsbItemMoveUp_Click(object sender, EventArgs e)
		{

		}
		private void tsbItemMoveDown_Click(object sender, EventArgs e)
		{

		}

		private void tsbItemClear_Click(object sender, EventArgs e)
		{

		}

		public CollectionListView()
		{
			InitializeComponent();
		}
	}
}
