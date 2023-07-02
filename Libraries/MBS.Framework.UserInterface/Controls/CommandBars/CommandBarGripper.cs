//
//  CommandBarGripper.cs
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
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.CommandBars
{
	public class CommandBarGripper : CustomControl
	{
		private Container CTBOX = null;
		private double ox = 0, oy = 0, oh = 0, cx = 0, cy = 0, dx = 0, dy = 0;

		public CommandBarRaftingContainer RaftingContainer { get { return (CommandBarRaftingContainer)CTBOX.Parent; } }

		public CommandBarGripper(Container ctBox)
		{
			HorizontalAdjustment.ScrollType = AdjustmentScrollType.Never;
			VerticalAdjustment.ScrollType = AdjustmentScrollType.Never;
			MinimumSize = new Dimension2D(8, 8);
			Name = "TiInternalGripper";
			CTBOX = ctBox;
		}

		protected internal override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			//CTBOX.BeginMoveDrag(e.Buttons, e.X, e.Y, DateTime.Now);

			if (e.Buttons == MouseButtons.Primary)
			{
				RaftingContainer.BeginCommandBarDrag(CTBOX);
			}
		}

		protected internal override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Cursor = Cursors.Move;

			if (e.Buttons == MouseButtons.Primary)
			{
				RaftingContainer.ContinueCommandBarDrag(CTBOX);
			}
		}

		protected internal override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			//e.Graphics.Clear();`

			for (int i = 0; i < this.Size.Height; i += 4)
			{
				e.Graphics.DrawLine(new Pen(Colors.Gray), 4, i, this.Size.Width - 8, i);
			}
		}
	}
}
