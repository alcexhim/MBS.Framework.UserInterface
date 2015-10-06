using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit
{
	public class Monitor
	{
		private string mvarDeviceName = String.Empty;
		public string DeviceName { get { return mvarDeviceName; } }

		private Rectangle mvarBounds = Rectangle.Empty;
		public Rectangle Bounds { get { return mvarBounds; } }

		private Rectangle mvarWorkingArea = Rectangle.Empty;
		public Rectangle WorkingArea { get { return mvarWorkingArea; } }

		public Monitor(string deviceName, Rectangle bounds, Rectangle workingArea)
		{
			mvarDeviceName = deviceName;
			mvarBounds = bounds;
			mvarWorkingArea = workingArea;
		}

		public static Monitor[] Get()
		{
			return Application.Engine.GetMonitors();
		}
	}
}
