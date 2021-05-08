using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface
{
	public abstract class Monitor
	{
		public class MonitorCollection
		{
			public MonitorCollection(Screen screen)
			{
				Screen = screen;
			}
			public Screen Screen { get; private set; } = null;

			public Monitor this[int index]
			{
				get
				{
					return null;
				}
			}

			public int Count { get { return Screen.GetMonitorCount(); } }
		}

		protected abstract Rectangle GetBoundsInternal();
		public Rectangle Bounds { get { return GetBoundsInternal(); } }

		protected abstract Rectangle GetWorkingAreaInternal();
		public Rectangle WorkingArea { get { return GetWorkingAreaInternal(); } }

		protected abstract string GetDeviceNameInternal();
		public string DeviceName { get { return GetDeviceNameInternal(); } }

		protected abstract double GetScaleFactorInternal();
		public double ScaleFactor { get { return GetScaleFactorInternal(); } }

		public static Monitor[] Get()
		{
			return ((UIApplication)Application.Instance).Engine.GetMonitors();
		}
	}
}
