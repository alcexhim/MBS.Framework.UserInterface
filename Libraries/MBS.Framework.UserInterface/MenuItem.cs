using System;

namespace MBS.Framework.UserInterface
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

			private Menu _parent = null;
			public MenuItemCollection(Menu parent = null)
			{
				_parent = parent;
			}

			protected override void InsertItem(int index, MenuItem item)
			{
				base.InsertItem(index, item);
				_parent?.InsertMenuItem(index, item);
			}
			protected override void ClearItems()
			{
				base.ClearItems();
				_parent?.ClearMenuItems();
			}
			protected override void RemoveItem(int index)
			{
				_parent?.RemoveMenuItem(this[index]);
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, MenuItem item)
			{
				base.SetItem(index, item);
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

