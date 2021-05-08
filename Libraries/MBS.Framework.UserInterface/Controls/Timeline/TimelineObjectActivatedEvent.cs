using System;
namespace MBS.Framework.UserInterface.Controls.Timeline
{
	public class TimelineObjectActivatedEventArgs : EventArgs
	{
		public TimelineObject Object { get; private set; } = null;
		public TimelineObjectActivatedEventArgs(TimelineObject obj)
		{
			Object = obj;
		}
	}
}
