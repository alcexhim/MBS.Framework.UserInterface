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

		private object mvarData = null;
		/// <summary>
		/// Extra data to attach to this <see cref="MenuItem" />.
		/// </summary>
		/// <value>The data.</value>
		public object Data { get { return mvarData; } set { mvarData = value; } }

		private MenuItemHorizontalAlignment mvarHorizontalAlignment = MenuItemHorizontalAlignment.Left;
		public MenuItemHorizontalAlignment HorizontalAlignment { get { return mvarHorizontalAlignment; } set { mvarHorizontalAlignment = value; } }
	}
}

