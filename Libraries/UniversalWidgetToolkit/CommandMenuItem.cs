using System;

namespace UniversalWidgetToolkit
{
	public class CommandMenuItem : MenuItem
	{

		private MenuItem.MenuItemCollection mvarItems = new MenuItem.MenuItemCollection();
		public MenuItem.MenuItemCollection Items { get { return mvarItems; } }

		private string mvarText = String.Empty;
		public string Text { get { return mvarText; } set { mvarText = value; } }

		public event EventHandler Click;

		public void OnClick(EventArgs e) {
			if (Click != null) {
				Click (this, e);
			}
		}

		public CommandMenuItem(string text, MenuItem[] items = null, EventHandler onClick = null)
		{
			mvarText = text;
			if (items != null) {
				foreach (MenuItem item in items) {
					mvarItems.Add (item);
				}
			}
			if (onClick != null) {
				Click += onClick;
			}
		}

	}
}

