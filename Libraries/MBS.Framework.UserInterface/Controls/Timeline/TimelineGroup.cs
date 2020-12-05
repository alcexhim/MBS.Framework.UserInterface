using System;
namespace MBS.Framework.UserInterface.Controls.Timeline
{
	public class TimelineGroup : TimelineItem
	{
		public class TimelineGroupCollection
			: System.Collections.ObjectModel.Collection<TimelineGroup>
		{
		}

		public TimelineGroup()
		{
			Objects = new TimelineObject.TimelineObjectCollection(this);
		}

		public string Title { get; set; } = null;
		public int? Height { get; set; } = null;
		public TimelineGroup.TimelineGroupCollection Groups { get; } = new TimelineGroup.TimelineGroupCollection();
		public TimelineObject.TimelineObjectCollection Objects { get; private set; } = null;
		public bool Expanded { get; set; } = false;
	}
}
