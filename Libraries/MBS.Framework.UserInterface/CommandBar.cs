using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBS.Framework.Collections.Generic;
using MBS.Framework.UserInterface;
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface
{
	public class CommandBar
	{
		public class CommandBarCollection
			: System.Collections.ObjectModel.Collection<CommandBar>
		{
			private Window _ParentWindow = null;
			public CommandBarCollection()
			{
			}
			public CommandBarCollection(Window parent)
			{
				_ParentWindow = parent;
			}

			public CommandBar Add(string id, string title)
			{
				CommandBar cb = new CommandBar();
				cb.ID = id;
				cb.Title = title;
				Add(cb);
				return cb;
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				if (_ParentWindow != null)
				{
					if (_ParentWindow.ControlImplementation is Native.IWindowNativeImplementation)
					{
						(_ParentWindow.ControlImplementation as Native.IWindowNativeImplementation).ClearCommandBars();
					}
				}
			}
			protected override void InsertItem(int index, CommandBar item)
			{
				base.InsertItem(index, item);
				if (_ParentWindow != null)
				{
					if (_ParentWindow.ControlImplementation is Native.IWindowNativeImplementation)
					{
						(_ParentWindow.ControlImplementation as Native.IWindowNativeImplementation).InsertCommandBar(index, item);
					}
				}
			}
			protected override void RemoveItem(int index)
			{
				if (_ParentWindow != null)
				{
					if (_ParentWindow.ControlImplementation is Native.IWindowNativeImplementation)
					{
						(_ParentWindow.ControlImplementation as Native.IWindowNativeImplementation).RemoveCommandBar(this[index]);
					}
				}
				base.RemoveItem(index);
			}
		}

		public CommandBar()
		{
		}
		public CommandBar(string id, string title, CommandItem[] items)
		{
			ID = id;
			Title = title;
			Items.AddRange(items);
		}

		private string mvarID = String.Empty;
		public string ID { get { return mvarID; } set { mvarID = value; } }

		private string mvarTitle = String.Empty;
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private CommandItem.CommandItemCollection mvarItems = new CommandItem.CommandItemCollection();
		public CommandItem.CommandItemCollection Items { get { return mvarItems; } }
	}
}
