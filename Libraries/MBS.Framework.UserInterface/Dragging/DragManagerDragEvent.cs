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

namespace MBS.Framework.UserInterface.Dragging
{
	public class DragManagerDragEventArgs : EventArgs
	{
		public double StartX { get; private set; }
		public double StartY { get; private set; }
		public Vector2D StartPoint => new Vector2D(StartX, StartY);

		public double EndX { get; private set; }
		public double EndY { get; private set; }
		public Vector2D EndPoint => new Vector2D(EndX, EndY);

		public Rectangle SelectionRectangle
		{
			get { return new Rectangle(StartX, StartY, EndX - StartX, EndY - StartY); }
			set { StartX = value.X; StartY = value.Y; EndX = value.Right; EndY = value.Bottom; }
		}

		public Rectangle ObjectRectangle { get; set; } = Rectangle.Empty;

		public double ObjectX { get { return ObjectRectangle.X + DeltaX; } }
		public double ObjectY { get { return ObjectRectangle.Y + DeltaY; } }

		public Color BorderColor { get; set; } = Color.Empty;
		public Color FillColor { get; set; } = Color.Empty;

		public bool Dragging { get; private set; } = false;
		public DragResizable Resizable { get; set; } = DragResizable.None;

		public double DeltaX { get { return EndX - StartX; } }
		public double DeltaY { get { return EndY - StartY; } }
		public Vector2D DeltaPoint { get { return new Vector2D(DeltaX, DeltaY); } }

		public DragManagerDragEventArgs(double startX, double startY, double endX, double endY, bool dragging = true)
		{
			StartX = startX;
			StartY = startY;
			EndX = endX;
			EndY = endY;
			Dragging = dragging;
		}
	}
}