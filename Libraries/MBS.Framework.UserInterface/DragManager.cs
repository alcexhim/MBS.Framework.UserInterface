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
using MBS.Framework.UserInterface.Dragging;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface
{
	public class DragManager
	{
		/// <summary>
		/// Gets the value of the horizontal coordinate at the moment the drag operation began.
		/// </summary>
		/// <value>The value of the horizontal coordinate at the moment the drag operation began.</value>
		public double InitialX { get; private set; }
		/// <summary>
		/// Gets the value of the vertical coordinate at the moment the drag operation began.
		/// </summary>
		/// <value>The value of the vertical coordinate at the moment the drag operation began.</value>
		public double InitialY { get; private set; }

		/// <summary>
		/// Gets the value of the horizontal coordinate at the current moment.
		/// </summary>
		/// <value>The value of the horizontal coordinate at the current moment.</value>
		public double CurrentX { get; set; }
		/// <summary>
		/// Gets the value of the vertical coordinate at the current moment.
		/// </summary>
		/// <value>The value of the vertical coordinate at the current moment.</value>
		public double CurrentY { get; set; }

		/// <summary>
		/// Computes the difference between the values of the horizontal coordinate at the current moment compared to the moment the drag operation began.
		/// </summary>
		/// <value>The difference between the values of the horizontal coordinate at the current moment compared to the moment the drag operation began.</value>
		public double DeltaX { get { return CurrentX - InitialX; } }
		/// <summary>
		/// Computes the difference between the values of the vertical coordinate at the current moment compared to the moment the drag operation began.
		/// </summary>
		/// <value>The difference between the values of the vertical coordinate at the current moment compared to the moment the drag operation began.</value>
		public double DeltaY { get { return CurrentY - InitialY; } }

		/// <summary>
		/// Gets or sets a <see cref="Rectangle" /> specifying the drag limit boundary.
		/// </summary>
		/// <value>The drag limit boundary.</value>
		public Rectangle DragLimitBounds { get; set; } = Rectangle.Empty;

		public DragArea.DragAreaCollection DragAreas { get; } = new DragArea.DragAreaCollection();

		private Control _control = null;

		private bool _Enabled = true;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="DragManager" /> handles drag events.
		/// </summary>
		/// <value><c>true</c> if this <see cref="DragManager" /> should handle drag events; otherwise, <c>false</c>.</value>
		public bool Enabled { get { return _Enabled; } set { _Enabled = value; _control.Refresh(); } }

		/// <summary>
		/// The event that is raised before the <see cref="DragManager" /> paints over the associated <see cref="Control" />. <see cref="Control" />s making use of
		/// <see cref="DragManager" /> should connect to this event and perform all of their drawing logic inside the <see cref="BeforeControlPaint" /> event handler so
		/// that selection rectangles and other overlays can be properly drawn on top of the associated <see cref="Control" />.
		/// </summary>
		public event PaintEventHandler BeforeControlPaint;
		/// <summary>
		/// Raises the <see cref="DragManager.BeforeControlPaint" /> event.
		/// </summary>
		/// <param name="e">A <see cref="PaintEventArgs" /> instance.</param>
		protected virtual void OnBeforeControlPaint(PaintEventArgs e)
		{
			BeforeControlPaint?.Invoke(this, e);
		}

		public void Register(Control control)
		{
			_control = control;
			control.MouseDown += control_MouseDown;
			control.MouseMove += control_MouseMove;
			control.MouseUp += control_MouseUp;
			control.Paint += control_Paint;
		}

		private Pen pSelectionBorder = new Pen(SystemColors.HighlightBackground);
		private SolidBrush bSelectionFill = new SolidBrush(Color.FromRGBADouble(SystemColors.HighlightBackground.R, SystemColors.HighlightBackground.G, SystemColors.HighlightBackground.B, 0.5));

		private void control_MouseDown(object sender, Input.Mouse.MouseEventArgs e)
		{
			if (e.Buttons == Input.Mouse.MouseButtons.Primary)
			{
				InitialX = e.X;
				InitialY = e.Y;
				CurrentX = InitialX;
				CurrentY = InitialY;

				if (Enabled)
				{
					DragManagerDragEventArgs ee = new DragManagerDragEventArgs(InitialX, InitialY, CurrentX, CurrentY);
					ee.BorderColor = SystemColors.HighlightBackground;
					ee.FillColor = Color.FromRGBADouble(SystemColors.HighlightBackground.R, SystemColors.HighlightBackground.G, SystemColors.HighlightBackground.B, 0.5);

					OnDragStarting(ee);

					pSelectionBorder = new Pen(ee.BorderColor);
					bSelectionFill = new SolidBrush(ee.FillColor);

					_SelectionRectangle = ee.SelectionRectangle;

					Dragging = true;
					_control.Refresh();
				}
			}
		}

		private Rectangle _SelectionRectangle = Rectangle.Empty;
		private Rectangle _ObjectRectangle = Rectangle.Empty;
		private DragOperation dragOperation = DragOperation.None;

		private void control_MouseMove(object sender, Input.Mouse.MouseEventArgs e)
		{
			CurrentX = e.X;
			CurrentY = e.Y;

			if (e.Buttons == Input.Mouse.MouseButtons.Primary)
			{
				if (DragLimitBounds != Rectangle.Empty)
				{
					if (DragLimitBounds.X > -1)
					{
						if (CurrentX <= DragLimitBounds.X)
							CurrentX = DragLimitBounds.X;
					}
					if (DragLimitBounds.Y > -1)
					{
						if (CurrentY <= DragLimitBounds.Y)
							CurrentY = DragLimitBounds.Y;
					}
					if (DragLimitBounds.Width > -1)
					{
						if (CurrentX >= DragLimitBounds.Right)
							CurrentX = DragLimitBounds.Right;
					}
					if (DragLimitBounds.Height > -1)
					{
						if (CurrentY >= DragLimitBounds.Bottom)
							CurrentY = DragLimitBounds.Bottom;
					}
				}
			}

			DragManagerDragEventArgs ee = new DragManagerDragEventArgs(InitialX, InitialY, CurrentX, CurrentY, e.Buttons == Input.Mouse.MouseButtons.Primary);
			ee.ObjectRectangle = _ObjectRectangle;
			OnDragMoving(ee);

			_SelectionRectangle = ee.SelectionRectangle;
			if (ee.ObjectRectangle != Rectangle.Empty)
			{
				_ObjectRectangle = ee.ObjectRectangle;
				int initX1 = (int)(ee.ObjectRectangle.X);
				int initX2 = (int)(ee.ObjectRectangle.Right);
				int initY1 = (int)(ee.ObjectRectangle.Y);
				int initY2 = (int)(ee.ObjectRectangle.Bottom);

				if (e.X >= initX1 && e.X <= initX1 + ResizeMargin && ((ee.Resizable & DragResizable.HorizontalStart) == DragResizable.HorizontalStart))
				{
					dragOperation = DragOperation.ResizeHorizontalStart;
				}
				else if (e.X >= initX2 - ResizeMargin && e.X <= initX2 && ((ee.Resizable & DragResizable.HorizontalEnd) == DragResizable.HorizontalEnd))
				{
					dragOperation = DragOperation.ResizeHorizontalEnd;
				}
				else if (e.Y >= initY1 && e.Y <= initY1 + ResizeMargin && ((ee.Resizable & DragResizable.VerticalStart) == DragResizable.VerticalStart))
				{
					dragOperation = DragOperation.ResizeVerticalStart;
				}
				else if (e.Y >= initY2 - ResizeMargin && e.X <= initY2 && ((ee.Resizable & DragResizable.VerticalEnd) == DragResizable.VerticalEnd))
				{
					dragOperation = DragOperation.ResizeVerticalEnd;
				}
				else
				{
					dragOperation = DragOperation.Move;
				}

				switch (dragOperation)
				{
					case DragOperation.Move:
					{
						_control.Cursor = Cursors.Move;
						break;
					}
					case DragOperation.ResizeHorizontalEnd:
					{
						_control.Cursor = Cursors.ResizeE;
						break;
					}
					case DragOperation.ResizeHorizontalStart:
					{
						_control.Cursor = Cursors.ResizeW;
						break;
					}
					case DragOperation.ResizeVerticalEnd:
					{
						_control.Cursor = Cursors.ResizeS;
						break;
					}
					case DragOperation.ResizeVerticalStart:
					{
						_control.Cursor = Cursors.ResizeN;
						break;
					}
				}
			}
			else
			{
				_ObjectRectangle = Rectangle.Empty;
				_control.Cursor = DefaultCursor;
			}

			if (Enabled)
			{
				_control.Refresh();
			}
		}

		public Cursor DefaultCursor { get; set; } = Cursors.Default;

		private void control_MouseUp(object sender, Input.Mouse.MouseEventArgs e)
		{
			if (Enabled)
			{
				if (Dragging)
				{
					OnDragComplete(new DragManagerDragEventArgs(InitialX, InitialY, e.X, e.Y));
				}

				Dragging = false;
				_control.Refresh();
			}
		}

		public event EventHandler<DragManagerDragEventArgs> DragStarting;
		protected virtual void OnDragStarting(DragManagerDragEventArgs e)
		{
			DragStarting?.Invoke(this, e);
		}

		public event EventHandler<DragManagerDragEventArgs> DragMoving;
		protected virtual void OnDragMoving(DragManagerDragEventArgs e)
		{
			DragMoving?.Invoke(this, e);
		}

		public event EventHandler<DragManagerDragEventArgs> DragComplete;
		protected virtual void OnDragComplete(DragManagerDragEventArgs e)
		{
			DragComplete?.Invoke(this, e);
		}

		private void control_Paint(object sender, PaintEventArgs e)
		{
			OnBeforeControlPaint(e);
			if (Dragging && Enabled)
			{
				if (DrawSelection)
				{
					e.Graphics.DrawRectangle(pSelectionBorder, _SelectionRectangle);
					e.Graphics.FillRectangle(bSelectionFill, _SelectionRectangle);
				}
				if (_ObjectRectangle != Rectangle.Empty)
				{
					Console.WriteLine("Object rectangle is {0}", _ObjectRectangle);
					e.Graphics.DrawRectangle(pSelectionBorder, _ObjectRectangle);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the user is currently dragging on the associated control.
		/// </summary>
		/// <value><c>true</c> if dragging; otherwise, <c>false</c>.</value>
		public bool Dragging { get; private set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the selection rectangle should be drawn.
		/// </summary>
		/// <value><c>true</c> if the selection rectangle should be drawn; otherwise, <c>false</c>.</value>
		public bool DrawSelection { get; set; } = true;

		public double ObjectInitialX { get; set; } = 0.0;
		public double ObjectInitialY { get; set; } = 0.0;
		public double ObjectDeltaX { get { return ObjectInitialX + DeltaX; } }
		public double ObjectDeltaY { get { return ObjectInitialY + DeltaY; } }

		/// <summary>
		/// Gets or sets the size, in pixels, of the margin around a selectable object reserved for resize handles.
		/// </summary>
		/// <value>The size, in pixels, of the margin around a selectable object reserved for resize handles.</value>
		public int ResizeMargin { get; set; } = 6;
	}
}
