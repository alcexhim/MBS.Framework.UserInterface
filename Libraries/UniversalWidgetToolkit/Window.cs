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
		private MenuBar mvarMenuBar = new MenuBar();
		public MenuBar MenuBar { get { return mvarMenuBar; } }

		private Rectangle mvarBounds = Rectangle.Empty;
		public Rectangle Bounds { get { return mvarBounds; } set { mvarBounds = value; } }

		public event EventHandler Activate;
		public virtual void OnActivate(EventArgs e)
		{
			if (Activate != null) Activate (this, e);
		}

		public event EventHandler Deactivate;
		public virtual void OnDeactivate(EventArgs e)
		{
			if (Deactivate != null) Deactivate (this, e);
		}

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
