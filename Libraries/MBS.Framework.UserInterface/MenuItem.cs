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
				if (item != null)
				{
					item.Parent = _parent;
					_parent?.InsertMenuItem(index, item);
				}
			}
			protected override void ClearItems()
			{
				foreach (MenuItem item in this)
				{
					item.Parent = null;
				}
				base.ClearItems();
				_parent?.ClearMenuItems();
			}
			protected override void RemoveItem(int index)
			{
				this[index].Parent = null;
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

		public Menu Parent { get; private set; } = null;

		private object mvarData = null;
		/// <summary>
		/// Extra data to attach to this <see cref="MenuItem" />.
		/// </summary>
		/// <value>The data.</value>
		public object Data { get { return mvarData; } set { mvarData = value; } }

		private MenuItemHorizontalAlignment mvarHorizontalAlignment = MenuItemHorizontalAlignment.Left;
		public MenuItemHorizontalAlignment HorizontalAlignment { get { return mvarHorizontalAlignment; } set { mvarHorizontalAlignment = value; } }

		private bool _Visible = true;
		public bool Visible { get { return _Visible; } set { _Visible = value; Application.Engine.SetMenuItemVisibility(this, value); } }

		public static MenuItem LoadMenuItem(CommandItem ci, EventHandler onclick = null)
		{
			if (ci is CommandReferenceCommandItem)
			{
				CommandReferenceCommandItem crci = (ci as CommandReferenceCommandItem);

				Command cmd = Application.Commands[crci.CommandID];
				if (cmd != null)
				{
					CommandMenuItem mi = new CommandMenuItem(cmd.Title);
					mi.Name = cmd.ID;
					mi.Shortcut = cmd.Shortcut;
					if (cmd.Items.Count > 0)
					{
						foreach (CommandItem ci1 in cmd.Items)
						{
							MBS.Framework.UserInterface.MenuItem mi1 = LoadMenuItem(ci1, onclick);
							mi.Items.Add(mi1);
						}
					}
					else
					{
						if (onclick != null)
							mi.Click += onclick;
					}
					return mi;
				}
				else
				{
					Console.WriteLine("attempted to load unknown cmd '" + crci.CommandID + "'");
				}
				return null;
			}
			else if (ci is SeparatorCommandItem)
			{
				return new MBS.Framework.UserInterface.SeparatorMenuItem();
			}
			return null;
		}
	}
}

