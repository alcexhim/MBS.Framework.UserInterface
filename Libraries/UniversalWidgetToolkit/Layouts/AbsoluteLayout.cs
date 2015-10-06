using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit.Layouts
{
	public class AbsoluteLayout : Layout
	{
		private Dictionary<Control, Rectangle> mvarControlBounds = new Dictionary<Control, Rectangle>();

		public void SetControlBounds(Control ctl, Rectangle bounds)
		{
			mvarControlBounds[ctl] = bounds;
		}

		protected override Rectangle GetControlBoundsInternal(Control ctl)
		{
			if (mvarControlBounds.ContainsKey(ctl)) return mvarControlBounds[ctl];
			return Rectangle.Empty;
		}
	}
}
