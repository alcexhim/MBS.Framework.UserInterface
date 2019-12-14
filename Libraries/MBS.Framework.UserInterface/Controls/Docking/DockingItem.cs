using System;
namespace MBS.Framework.UserInterface.Controls.Docking
{
	/// <summary>
	/// A dock item is a container widget that can be docked at different place.
	/// It accepts a single child and adds a grip allowing the user to click on
	/// it to drag and drop the widget.
	/// </summary>
	public class DockingItem
	{
		public class DockingItemCollection :  System.Collections.ObjectModel.Collection<DockingItem>
		{
			private DockingContainer _parent = null;
			public DockingItemCollection(DockingContainer parent)
			{
				_parent = parent;
			}

			protected override void ClearItems()
			{
				(_parent.ControlImplementation as Native.IDockingContainerNativeImplementation).ClearDockingItems();
				base.ClearItems();
			}
			protected override void InsertItem(int index, DockingItem item)
			{
				if (_parent.ControlImplementation != null) (_parent.ControlImplementation as Native.IDockingContainerNativeImplementation).InsertDockingItem(item, index);
				item.Parent = _parent;
				base.InsertItem(index, item);
			}
			protected override void RemoveItem(int index)
			{
				if (_parent.ControlImplementation != null) (_parent.ControlImplementation as Native.IDockingContainerNativeImplementation).RemoveDockingItem(this[index]);
				this[index].Parent = null;
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, DockingItem item)
			{
				if (_parent.ControlImplementation != null) (_parent.ControlImplementation as Native.IDockingContainerNativeImplementation).SetDockingItem(index, item);
				this[index].Parent = null;
				item.Parent = _parent;
				base.SetItem(index, item);
			}
		}

		public DockingContainer Parent { get; private set; } = null;

		private DockingItemPlacement mvarPlacement = DockingItemPlacement.Center;
		public DockingItemPlacement Placement {  get { return mvarPlacement;  } set { mvarPlacement = value; } }

		private string mvarName = String.Empty;
		public string Name { get { return mvarName; } set { mvarName = value; } }

		private string mvarTitle = String.Empty;
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private Control mvarChildControl = null;
		public Control ChildControl {  get { return mvarChildControl;  } set { mvarChildControl = value; mvarChildControl.SetParent(Parent?.Parent); } }

		private DockingItemBehavior mvarBehavior = DockingItemBehavior.Normal;
		public DockingItemBehavior Behavior {  get { return mvarBehavior;  } set { mvarBehavior = value; } }

		public bool AutoHide { get; set; } = false;

		public DockingItem(string title, Control child)
			: this(title, title, child)
		{
		}
		public DockingItem(string name, string title, Control child)
		{
			mvarName = name;
			mvarTitle = title;
			mvarChildControl = child;
		}
	}
}
