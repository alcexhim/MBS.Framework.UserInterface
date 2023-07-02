using System;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface
{
	public class DragManagerHitTestEventArgs : EventArgs
	{
		public DragManagerHitTestEventArgs(Vector2D location, MouseButtons buttons)
		{
			Location = location;
			Buttons = buttons;
		}

		public object Hit { get; set; } = null;
		public bool Handled { get; set; } = false;
		public Vector2D Location { get; }
		public MouseButtons Buttons { get; }
	}
}
