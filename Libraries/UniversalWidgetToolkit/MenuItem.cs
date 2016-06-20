using System;

namespace UniversalWidgetToolkit
{
	public abstract class MenuItem
	{
		public class MenuItemCollection
			: System.Collections.ObjectModel.Collection<MenuItem>
		{
			public void AddRange (MenuItem[] menuItems)
			{
				foreach (MenuItem mi in menuItems) {
					Add (mi);
				}
			}
		}

		private string mvarName = String.Empty;
		public string Name { get { return mvarName; } set { mvarName = value; } }
	}
}

