using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.Docking.AwesomeControls
{
	public delegate void WindowClosingEventHandler(object sender, WindowClosingEventArgs e);
	public class WindowClosingEventArgs : CancelEventArgs
	{
		private DockingWindow mvarWindow = null;
		public DockingWindow Window { get { return mvarWindow; } }

		public WindowClosingEventArgs(DockingWindow window)
		{
			mvarWindow = window;
		}
	}
}
