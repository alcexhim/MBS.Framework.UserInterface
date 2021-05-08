using System;
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface.Layouts
{
	public class ListLayout : Layout
	{
		public SelectionMode SelectionMode { get; set; } = SelectionMode.Single;

		protected override Rectangle GetControlBoundsInternal(Control ctl)
		{
			return Rectangle.Empty;
		}

		protected override void ResetControlBoundsInternal(Control ctl = null)
		{

		}
	}
}
