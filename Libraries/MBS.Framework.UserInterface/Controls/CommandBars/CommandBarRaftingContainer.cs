//
//  CommandBarRaftingContainer.cs
//
//  Author:
//       beckermj <>
//
//  Copyright (c) 2023 ${CopyrightHolder}
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
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.CommandBars
{
	public class CommandBarRaftingContainer : Container
	{
		public CommandBarRaftingContainer()
		{
			this.Layout = new AbsoluteLayout();
			Rows = new CBRCRow.CBRCRowCollection(this);
		}

		private double ox = 0, oy = 0, oh = 0, cx = 0, cy = 0;

		internal void BeginCommandBarDrag(Container container)
		{
			ox = container.Location.X - this.Location.X; // idk what the 26 is for
			oy = container.Location.Y - this.Location.Y;
			oh = container.Size.Height;

			cx = MouseDevice.Default.Position.X;
			cy = MouseDevice.Default.Position.Y;

			UpdateRowsNeeded();
		}

		private class CBRCRow
		{
			public class CBRCRowCollection
			{
				public CommandBarRaftingContainer Parent { get; private set; }

				public CBRCRowCollection(CommandBarRaftingContainer parent)
				{
					Parent = parent;
				}

				public CBRCRow this[int index]
				{
					get
					{
						return new CBRCRow(index);
					}
				}
			}

			private int _CellCount = 0;
			public int CellCount { get { return _CellCount; } }

			private int _Index = 0;

			public CBRCRow(int index)
			{
				_Index = index;
			}
		}

		CBRCRow.CBRCRowCollection Rows { get; }

		private int rowsNeeded = 0;
		private void UpdateRowsNeeded()
		{
			rowsNeeded = 0;

			// FIXME:
			// we need to figure out how many CommandBars are on a single row
			// if there are more than 1, we increase rowsNeeded

			int lastRowIndex = 0;
			foreach (Control ctl in this.Controls)
			{
				int ri = GetRowIndex(ctl);
				if (ri > rowsNeeded)
					rowsNeeded = ri;
			}
			foreach (Control ctl in this.Controls)
			{
				int ri = GetRowIndex(ctl);
				if (ri != lastRowIndex)
				{
					lastRowIndex = ri;
					rowsNeeded++;
				}
			}
		}

		internal void ContinueCommandBarDrag(Container container)
		{
			double dx = MouseDevice.Default.Position.X - cx;
			double dy = MouseDevice.Default.Position.Y - cy;

			int nx = (int)(ox + dx), ny = (int)oy;
			if (nx < 0)
				nx = 0;

			int rowIndex = (int)Math.Round((oy + dy) / (oy + oh));
			if (rowIndex > rowsNeeded)
			{
				DetachCommandBar(container);
			}
			else
			{
				ny = (int)(oh * rowIndex);
			}

			Layout.SetControlConstraints(Controls, container, new AbsoluteLayout.Constraints(nx, ny, (int)this.Size.Width, (int)this.Size.Height));
		}

		private int GetRowIndex(Control ctl)
		{
			int rowIndex = (int)Math.Round((ctl.Location.Y - this.Location.Y) / ctl.Size.Height);
			return rowIndex;
		}

		public void DetachCommandBar(Container container)
		{
			Console.WriteLine("!!! DETACHING!!! ");
		}
	}
}
