using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Layouts
{
	public class FlowLayout : Layout
	{
		public class Constraints : UserInterface.Constraints
		{
		}

		public SelectionMode SelectionMode { get; set; }

		private Dictionary<Control, int> mvarControlPriorities = new Dictionary<Control, int>();
		private Dictionary<Control, Rectangle> mvarControlBounds = new Dictionary<Control, Rectangle>();

		protected override void ResetControlBoundsInternal(Control ctl = null)
		{
			if (ctl == null)
			{
				mvarControlBounds.Clear();
			}
			else
			{
				if (mvarControlBounds.ContainsKey(ctl)) mvarControlBounds.Remove(ctl);
			}
		}

		protected override Rectangle GetControlBoundsInternal(Control ctl)
		{
			if (!mvarControlBounds.ContainsKey(ctl))
			{
				Rectangle rect = new Rectangle(0, 0, 0, 0);
				if (ctl.Parent != null)
				{
					Control[] ctls = ctl.Parent.GetAllControls();
					for (int i = 0; i < ctls.Length; i++)
					{
						if (ctls[i] == ctl) break;

						// TODO:	figure out how to get rid of the dependency on ctl.ParentWindow (it should be just ctl.Parent since
						//			we are looking at it strictly from a Container point of view)
						Rectangle rect2 = GetControlBounds(ctls[i]);
						rect.X += rect2.Right;

						if (rect.Right > (ctl.ParentWindow.Bounds.Width - ctl.ParentWindow.Padding.Left - ctl.ParentWindow.Padding.Right - ctl.Margin.Left - ctl.Margin.Right))
						{
							rect.X = 0;
							rect.Y += (rect2.Height + ctls[i].Margin.Bottom);
						}
					}
				}
				mvarControlBounds.Add(ctl, rect);
			}

			return mvarControlBounds[ctl];
		}
	}
}
