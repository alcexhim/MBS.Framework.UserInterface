﻿using System;
using System.Collections.Generic;

namespace MBS.Framework.UserInterface.Controls.Docking
{
	namespace Native
	{
		public interface IDockingContainerNativeImplementation
		{
			void ClearDockingItems();
			void InsertDockingItem(DockingItem item, int index);
			void RemoveDockingItem(DockingItem item);
			void SetDockingItem(int index, DockingItem item);

			DockingItem GetCurrentItem();
			void SetCurrentItem (DockingItem item);

			void UpdateDockingItemName(DockingItem item, string text);
			void UpdateDockingItemTitle(DockingItem item, string text);
		}
	}

	public class DockingContainer : SystemControl, IVirtualControlContainer
	{
		private DockingItem mvarCurrentItem = null;
		public DockingItem CurrentItem
		{
			get {
				Native.IDockingContainerNativeImplementation impl = (ControlImplementation as Native.IDockingContainerNativeImplementation);
				if (impl != null)
					mvarCurrentItem = impl.GetCurrentItem ();
				return mvarCurrentItem; 
			}
			set {
				Native.IDockingContainerNativeImplementation impl = (ControlImplementation as Native.IDockingContainerNativeImplementation);
				if (impl != null)
					impl.SetCurrentItem (value);
				mvarCurrentItem = value;
			}
		}

		private DockingItem.DockingItemCollection mvarItems = null;
		public DockingItem.DockingItemCollection Items {  get { return mvarItems; } }

		public event EventHandler SelectionChanged;
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			SelectionChanged?.Invoke(this, e);
		}

		public Control[] GetAllControls()
		{
			List<Control> list = new List<Control>();
			foreach (DockingItem item in mvarItems)
			{
				if (item.ChildControl is IVirtualControlContainer)
				{
					Control[] childControls = ((IVirtualControlContainer)item.ChildControl).GetAllControls();
					foreach (Control ctlChild in childControls)
					{
						list.Add(ctlChild);
					}
				}
				list.Add(item.ChildControl);
			}
			return list.ToArray();
		}

		public DockingContainer()
		{
			mvarItems = new DockingItem.DockingItemCollection(this);
		}
	}
}
