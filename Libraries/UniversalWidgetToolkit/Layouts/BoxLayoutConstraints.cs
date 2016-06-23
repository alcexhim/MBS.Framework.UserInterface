using System;

namespace UniversalWidgetToolkit
{
	public class BoxLayoutConstraints : Constraints
	{
		private bool mvarExpand = false;
		public bool Expand { get { return mvarExpand; } set { mvarExpand = value; } }

		private bool mvarFill = false;
		public bool Fill { get { return mvarFill; } set { mvarFill = value; } }

		public BoxLayoutConstraints(bool expand = false, bool fill = false)
		{
			mvarExpand = expand;
			mvarFill = fill;
		}
	}
}

