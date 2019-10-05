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
		public MenuItem.MenuItemCollection Items { get; private set; } = null;

		public bool Visible { get; set; } = true;

		internal Window _Parent { get; private set; } = null;

		internal void InsertMenuItem(int index, MenuItem item)
		{
			(_Parent?.ControlImplementation as Native.IWindowNativeImplementation)?.InsertMenuItem(index, item);
		}
		internal void ClearMenuItems()
		{
			(_Parent?.ControlImplementation as Native.IWindowNativeImplementation)?.ClearMenuItems();
		}
		internal void RemoveMenuItem(MenuItem item)
		{
			(_Parent?.ControlImplementation as Native.IWindowNativeImplementation)?.RemoveMenuItem(item);
		}

		public Menu()
		{
			Items = new MenuItem.MenuItemCollection(this);
		}
		internal Menu(Window parent)
		{
			_Parent = parent;
			Items = new MenuItem.MenuItemCollection(this);
		}
	}
}

