using System;

namespace UniversalWidgetToolkit
{
	public class MenuBar
	{
		private MenuItem.MenuItemCollection mvarItems = new MenuItem.MenuItemCollection();
		public MenuItem.MenuItemCollection Items { get { return mvarItems; } }
	}
}

