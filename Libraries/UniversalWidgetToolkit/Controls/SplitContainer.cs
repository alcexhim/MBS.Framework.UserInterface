using System;

namespace UniversalWidgetToolkit
{
	public class SplitContainer : Control
	{
		private Orientation mvarOrientation = Orientation.Horizontal;
		/// <summary>
		/// The orientation of the splitter in the SplitContainer. When vertical, panels are on the left and right; when
		/// horizontal, panels are on the top and bottom.
		/// </summary>
		/// <value>The orientation of the splitter in this <see cref="SplitContainer" />.</value>
		public Orientation Orientation { get { return mvarOrientation; } set { mvarOrientation = value; } }

		public SplitContainer (Orientation orientation = Orientation.Horizontal)
		{
			mvarOrientation = orientation;
		}
	}
}

