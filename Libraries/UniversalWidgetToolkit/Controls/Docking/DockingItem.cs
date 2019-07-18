﻿using System;
namespace UniversalWidgetToolkit.Controls.Docking
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
				(_parent.NativeImplementation as Native.IDockingContainerNativeImplementation).ClearDockingItems();
				base.ClearItems();
			}
			protected override void InsertItem(int index, DockingItem item)
			{
				if (_parent.NativeImplementation != null) (_parent.NativeImplementation as Native.IDockingContainerNativeImplementation).InsertDockingItem(item, index);
				base.InsertItem(index, item);
			}
			protected override void RemoveItem(int index)
			{
				if (_parent.NativeImplementation != null) (_parent.NativeImplementation as Native.IDockingContainerNativeImplementation).RemoveDockingItem(this[index]);
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, DockingItem item)
			{
				if (_parent.NativeImplementation != null) (_parent.NativeImplementation as Native.IDockingContainerNativeImplementation).SetDockingItem(index, item);
				base.SetItem(index, item);
			}
		}

		private DockingItemPlacement mvarPlacement = DockingItemPlacement.Center;
		public DockingItemPlacement Placement {  get { return mvarPlacement;  } set { mvarPlacement = value; } }

		private string mvarTitle = String.Empty;
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private Control mvarChildControl = null;
		public Control ChildControl {  get { return mvarChildControl;  } set { mvarChildControl = value; } }

		private DockingItemBehavior mvarBehavior = DockingItemBehavior.Normal;
		public DockingItemBehavior Behavior {  get { return mvarBehavior;  } set { mvarBehavior = value; } }

		public DockingItem(string title, Control child)
		{
			mvarTitle = title;
			mvarChildControl = child;
		}
	}
}
