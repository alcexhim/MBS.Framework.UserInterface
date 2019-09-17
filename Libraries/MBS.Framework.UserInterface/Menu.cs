using System;

namespace MBS.Framework.UserInterface
{
	public class Menu
	{
		/// <summary>
		/// Determines whether this <see cref="Menu" /> should provide the ability to be torn off from its parent container and displayed as a floating window.
		/// </summary>
		/// <value><c>true</c> if enable tearoff; otherwise, <c>false</c>.</value>
		public bool EnableTearoff { get; set; } = true;

		private MenuItem.MenuItemCollection mvarItems = new MenuItem.MenuItemCollection();
		public MenuItem.MenuItemCollection Items { get { return mvarItems; } }
	}
}

