//
//  DragManager.cs
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
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface
{
	public class DragManager
	{
		private double cx, cy, dx, dy;

		private Control _control = null;

		private bool _Enabled = true;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="DragManager" /> handles drag events.
		/// </summary>
		/// <value><c>true</c> if this <see cref="DragManager" /> should handle drag events; otherwise, <c>false</c>.</value>
		public bool Enabled { get { return _Enabled; } set { _Enabled = value; _control.Refresh(); } }

		public void Register(Control control)
		{
			_control = control;
			control.MouseDown += control_MouseDown;
			control.MouseMove += control_MouseMove;
			control.MouseUp += control_MouseUp;
			control.Paint += control_Paint;
		}

		private void control_MouseDown(object sender, Input.Mouse.MouseEventArgs e)
		{
			if (e.Buttons == Input.Mouse.MouseButtons.Primary)
			{
				cx = e.X;
				cy = e.Y;
				dx = cx;
				dy = cy;

				if (Enabled)
				{
					_Dragging = true;
					_control.Refresh();
				}
			}
		}

		private void control_MouseMove(object sender, Input.Mouse.MouseEventArgs e)
		{
			if (e.Buttons == Input.Mouse.MouseButtons.Primary)
			{
				dx = e.X;
				dy = e.Y;

				if (Enabled)
				{
					_control.Refresh();
				}
			}
		}

		private void control_MouseUp(object sender, Input.Mouse.MouseEventArgs e)
		{
			if (Enabled)
			{
				_Dragging = false;
				_control.Refresh();
			}
		}

		private void control_Paint(object sender, PaintEventArgs e)
		{
			if (Dragging && Enabled)
			{
				Pen pSelectionBorder = new Pen(SystemColors.HighlightBackground);
				SolidBrush bSelectionFill = new SolidBrush(Color.FromRGBADouble(SystemColors.HighlightBackground.R, SystemColors.HighlightBackground.G, SystemColors.HighlightBackground.B, 0.5));

				e.Graphics.DrawRectangle(pSelectionBorder, new Rectangle(cx, cy, dx - cx, dy - cy));
				e.Graphics.FillRectangle(bSelectionFill, new Rectangle(cx, cy, dx - cx, dy - cy));
			}
		}

		private bool _Dragging = false;
		public bool Dragging { get { return _Dragging; } }
	}
}
