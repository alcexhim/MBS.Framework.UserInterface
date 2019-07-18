using System;

namespace UniversalWidgetToolkit.Controls
{
	public class TabContainer : SystemControl
	{
		private TabPage.TabPageCollection mvarTabPages = null;
		public TabPage.TabPageCollection TabPages { get { return mvarTabPages; } }

		private Control.ControlCollection mvarTabTitleControls = new Control.ControlCollection();
		/// <summary>
		/// Gets the controls displayed for each tab.
		/// </summary>
		/// <value>The tab title controls.</value>
		public Control.ControlCollection TabTitleControls { get { return mvarTabTitleControls; } }

		public TabContainer()
		{
			mvarTabPages = new TabPage.TabPageCollection (this);
		}
	}
}

