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

			DockingItem GetCurrentItem();
			void SetCurrentItem (DockingItem item);
		}
	}

	public class DockingContainer : SystemControl
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

		public DockingContainer()
		{
			mvarItems = new DockingItem.DockingItemCollection(this);
		}
	}
}
