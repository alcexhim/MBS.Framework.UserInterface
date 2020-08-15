//
//  ListView.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker
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
using System.Drawing;
using System.Windows.Forms;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.ListView
{
	public class ListView : System.Windows.Forms.ListView
	{
		private ListViewImplementation _impl;
		public ListView(ListViewImplementation impl)
		{
			OwnerDraw = true;
			DoubleBuffered = true;

			StateImageList = new ImageList();
			StateImageList.ImageSize = new Size(ItemHeight, ItemHeight);

			VirtualMode = true;
			_impl = impl;

			AllowColumnReorder = true;
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == 0x0F)
			{
				OnRealPaint(new System.Windows.Forms.PaintEventArgs(CreateGraphics(), ClientRectangle));
			}
		}

		protected virtual void OnRealPaint(System.Windows.Forms.PaintEventArgs e)
		{
			// Theming.Theme.CurrentTheme.DrawListViewBackground(e.Graphics, new Rectangle(0, 0, base.Width, base.Height));
			// this paints OVER the already-painted owner draw stuff
		}

		private TreeModelRow mvarHoverItem = null;
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			System.Windows.Forms.ListViewHitTestInfo info = HitTest(e.Location);
			if (info.Item != null)
			{
				if (mvarHoverItem != info.Item.Tag as TreeModelRow)
				{
					mvarHoverItem = info.Item.Tag as TreeModelRow;
					Invalidate();
				}
			}
			else
			{
				if (mvarHoverItem != null)
				{
					mvarHoverItem = null;
					Invalidate();
				}
			}
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			if (mvarHoverItem != null)
			{
				mvarHoverItem = null;
				Invalidate();
			}
		}

		protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
		{
			base.OnRetrieveVirtualItem(e);

			e.Item = new ListViewItem();

			TreeModelRow row = _impl.GetVirtualListRow(e.ItemIndex);
			if (row == null)
			{
				Console.WriteLine("GetVirtualListRow returned NULL for item index {0}", e.ItemIndex);
				return;
			}

			e.Item.Tag = row;

			if (row.RowColumns.Count > 0)
			{
				e.Item.Text = row.RowColumns[0].Value?.ToString();

				for (int i = 1; i < Columns.Count; i++)
				{
					ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();

					UserInterface.Controls.ListView.ListViewColumn tvc = Columns[i].Tag as UserInterface.Controls.ListView.ListViewColumn;
					if (tvc != null)
					{
						if (row.RowColumns[tvc.Column] != null)
						{
							lvsi.Tag = row.RowColumns[tvc.Column];
							lvsi.Text = row.RowColumns[tvc.Column].Value?.ToString() ?? String.Empty;
						}
					}

					e.Item.SubItems.Add(lvsi);
				}
			}
		}

		private int ItemHeight = 18;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left)
			{
				System.Windows.Forms.ListViewHitTestInfo info = HitTest(e.Location);
				if (info.Item != null)
				{
					TreeModelRow row = (info.Item.Tag as TreeModelRow);
					if (row == null)
					{
						return;
					}

					int x = (row.RowLevel * metrics_listview_tree_glyph_size_width);
					if (e.X >= x && e.X <= (x + metrics_listview_tree_glyph_size_width))
					{
						row.Expanded = !row.Expanded;
						Refresh();
					}
				}
			}
		}

		static int metrics_listview_tree_glyph_size = 12;
		static int metrics_listview_tree_glyph_size_width = metrics_listview_tree_glyph_size;
		static int metrics_listview_tree_glyph_size_height = metrics_listview_tree_glyph_size;

		protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
		{
			base.OnDrawColumnHeader(e);
			e.DrawDefault = true;
		}
		protected override void OnDrawItem(DrawListViewItemEventArgs e)
		{
			base.OnDrawItem(e);

			if (e.Item == null) return;

			TreeModelRow row = (e.Item.Tag as TreeModelRow);
			if (row == null) return;

			int level = row.RowLevel;

			int x = e.Bounds.X, y = e.Bounds.Y;
			x += (level * metrics_listview_tree_glyph_size_width);

			int w = e.Bounds.Width - metrics_listview_tree_glyph_size_width;
			if (FullRowSelect)
			{
				w = Size.Width - x;
			}

			Rectangle rectBoundsBackground = new Rectangle(e.Bounds.X, e.Bounds.Y, w, e.Bounds.Height);
			if (!FullRowSelect)
			{
				rectBoundsBackground.X = x + metrics_listview_tree_glyph_size_width;
			}

			Rectangle rectBoundsItem = new Rectangle(x + metrics_listview_tree_glyph_size_width, y, w, e.Bounds.Height);

			// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.listview.drawitem?view=netcore-3.1
			if (row == mvarHoverItem)
			{
				Theming.Theme.CurrentTheme.DrawListItemBackground(e.Graphics, rectBoundsBackground, UserInterface.Theming.ControlState.Hover, e.Item.Selected, Focused);
			}
			else
			{
				Theming.Theme.CurrentTheme.DrawListItemBackground(e.Graphics, rectBoundsBackground, UserInterface.Theming.ControlState.Normal, e.Item.Selected, Focused);
			}

			if (row.Rows.Count > 0)
			{
				Theming.Theme.CurrentTheme.DrawListViewTreeGlyph(e.Graphics, new Rectangle(x, y + (int)((double)(ItemHeight - metrics_listview_tree_glyph_size_height) / 2), metrics_listview_tree_glyph_size_width, metrics_listview_tree_glyph_size_height), UserInterface.Theming.ControlState.Normal, row.Expanded);
			}

			for (int i = 0; i < e.Item.SubItems.Count; i++)
			{
				Rectangle rectSubItemBounds = e.Item.SubItems[i].Bounds;
				rectSubItemBounds.Width = Columns[i].Width;
				RenderSubItem(e.Graphics, e.Item, e.Item.SubItems[i], rectSubItemBounds);
			}
		}

		private void RenderSubItem(Graphics g, System.Windows.Forms.ListViewItem lvi, System.Windows.Forms.ListViewItem.ListViewSubItem si, Rectangle bounds)
		{
			if (lvi == null) return;

			TreeModelRow row = (lvi.Tag as TreeModelRow);
			if (row == null) return;

			string text = si.Text ?? String.Empty;

			int level = row.RowLevel;

			int x = bounds.X, y = bounds.Y;
			x += (level * metrics_listview_tree_glyph_size_width);

			int w = bounds.Width - metrics_listview_tree_glyph_size_width;

			Rectangle rectBoundsItem = new Rectangle(x + metrics_listview_tree_glyph_size_width, y, w, bounds.Height);

			TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter;
			TextRenderer.DrawText(g, text, si.Font, rectBoundsItem, si.ForeColor, flags);
		}
		protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
		{
			base.OnDrawSubItem(e);
			// RenderSubItem(e.Graphics, e.Item, e.SubItem);
		}
	}
}
