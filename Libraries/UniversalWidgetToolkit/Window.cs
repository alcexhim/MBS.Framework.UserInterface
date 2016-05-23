using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit
{
	public class Window : Container
	{
		private Rectangle mvarBounds = Rectangle.Empty;
		public Rectangle Bounds { get { return mvarBounds; } set { mvarBounds = value; } }

		public event CancelEventHandler Closing;
		public virtual void OnClosing(CancelEventArgs e)
		{
			if (Closing != null) Closing(this, e);
		}

		public event EventHandler Closed;
		public virtual void OnClosed(EventArgs e)
		{
			if (Closed != null) Closed(this, e);
		}
	}
}
