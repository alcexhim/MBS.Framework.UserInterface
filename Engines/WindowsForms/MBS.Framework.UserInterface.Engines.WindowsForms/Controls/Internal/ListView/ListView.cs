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
		public ListView()
		{
			OwnerDraw = true;
		}

		private ListViewItem mvarHoverItem = null;
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaintBackground(e);

			Theming.Theme.CurrentTheme.DrawListViewBackground(e.Graphics, new Rectangle(0, 0, base.Width, base.Height));
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (HitTest(e.Location).Item != null)
			{
				mvarHoverItem = HitTest(e.Location).Item;
				Invalidate();
			}
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			mvarHoverItem = null;
			Invalidate();
		}
		protected override void OnDrawItem(DrawListViewItemEventArgs e)
		{
			base.OnDrawItem(e);

			if (e.Item == mvarHoverItem)
			{
				Theming.Theme.CurrentTheme.DrawListItemBackground(e.Graphics, e.Bounds, UserInterface.Theming.ControlState.Hover, e.Item.Selected, Focused);
			}
			else
			{
				Theming.Theme.CurrentTheme.DrawListItemBackground(e.Graphics, e.Bounds, UserInterface.Theming.ControlState.Normal, e.Item.Selected, Focused);
			}

			e.DrawText();
		}
	}
}
