using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit
{
	public abstract class Layout
	{
		protected abstract Rectangle GetControlBoundsInternal(Control ctl);
		public Rectangle GetControlBounds(Control ctl)
		{
			return GetControlBoundsInternal(ctl);
		}
	}
}
