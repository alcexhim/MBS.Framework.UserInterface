using System;

namespace UniversalWidgetToolkit.Controls
{
	namespace Native
	{
		public interface ISplitContainerImplementation
		{
			int GetSplitterPosition();
			void SetSplitterPosition(int value);
		}
	}
	public class SplitContainer : SystemControl
	{
		private Container mvarPanel1 = new Container ();

		public Container Panel1 { get { return mvarPanel1; } }

		private Container mvarPanel2 = new Container ();

		public Container Panel2 { get { return mvarPanel2; } }

		private Orientation mvarOrientation = Orientation.Horizontal;
		/// <summary>
		/// The orientation of the splitter in the SplitContainer. When vertical, panels are on the left and right; when
		/// horizontal, panels are on the top and bottom.
		/// </summary>
		/// <value>The orientation of the splitter in this <see cref="SplitContainer" />.</value>
		public Orientation Orientation { get { return mvarOrientation; } set { mvarOrientation = value; } }

		private int mvarSplitterPosition = 0;
		public int SplitterPosition
		{
			get
			{
				Native.ISplitContainerImplementation impl = (ControlImplementation as Native.ISplitContainerImplementation);
				if (impl != null) {
					mvarSplitterPosition = impl.GetSplitterPosition ();
				}
				return mvarSplitterPosition;
			}
			set
			{
				(ControlImplementation as Native.ISplitContainerImplementation)?.SetSplitterPosition (value);
				mvarSplitterPosition = value;
			}
		}

		public SplitContainer (Orientation orientation = Orientation.Horizontal)
		{
			mvarOrientation = orientation;
		}
	}
}

