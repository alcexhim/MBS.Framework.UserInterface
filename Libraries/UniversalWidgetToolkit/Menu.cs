using System;

namespace UniversalWidgetToolkit
{
	public class Menu
	{
		private MenuItem.MenuItemCollection mvarItems = new MenuItem.MenuItemCollection();
		public MenuItem.MenuItemCollection Items { get { return mvarItems; } }
	}
}

