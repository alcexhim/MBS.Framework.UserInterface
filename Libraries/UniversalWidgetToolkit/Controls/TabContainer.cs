using System;

namespace UniversalWidgetToolkit.Controls
{
	public class TabContainer : Control
	{
		private TabPage.TabPageCollection mvarTabPages = null;
		public TabPage.TabPageCollection TabPages { get { return mvarTabPages; } }

		public TabContainer()
		{
			mvarTabPages = new TabPage.TabPageCollection (this);
		}
	}
}

