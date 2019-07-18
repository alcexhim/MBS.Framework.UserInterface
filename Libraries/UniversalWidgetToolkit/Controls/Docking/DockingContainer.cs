using System;
namespace UniversalWidgetToolkit.Controls.Docking
{
	namespace Native
	{
		public interface IDockingContainerNativeImplementation
		{
			void ClearDockingItems();
			void InsertDockingItem(DockingItem item, int index);
			void RemoveDockingItem(DockingItem item);
			void SetDockingItem(int index, DockingItem item);
		}
	}

	public class DockingContainer : SystemControl
	{
		private DockingItem mvarCurrentItem = null;
		public DockingItem CurrentItem {  get { return mvarCurrentItem; } }

		private DockingItem.DockingItemCollection mvarItems = null;
		public DockingItem.DockingItemCollection Items {  get { return mvarItems; } }

		public DockingContainer()
		{
			mvarItems = new DockingItem.DockingItemCollection(this);
		}
	}
}
