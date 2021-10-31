using System;
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface.Layouts
{
	public class StackLayout : Layout
	{
		public class Constraints : MBS.Framework.UserInterface.Constraints
		{
			public string Name { get; set; }
			public string Title { get; set; }

			public Constraints(string name = null, string title = null)
			{
				Name = name;
				Title = title;
			}
		}

		protected override Rectangle GetControlBoundsInternal(Control ctl)
		{
			return Rectangle.Empty;
		}

		protected override void ResetControlBoundsInternal(Control ctl = null)
		{
		}
	}
}
