using System;
using System.Collections.Generic;

using MBS.Framework.UserInterface.Controls.ListView;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Layouts;

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

	public class DockingContainerControl : SystemControl, IVirtualControlContainer, IDockingItemContainer
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
				if (item is DockingWindow)
				{
					if ((item as DockingWindow).ChildControl is IVirtualControlContainer)
					{
						Control[] childControls = ((IVirtualControlContainer)(item as DockingWindow).ChildControl).GetAllControls();
						foreach (Control ctlChild in childControls)
						{
							list.Add(ctlChild);
						}
					}
					list.Add((item as DockingWindow).ChildControl);
				}
				else if (item is DockingContainer)
				{
					foreach (DockingItem item2 in (item as DockingContainer).Items)
					{

					}
				}
			}
			return list.ToArray();
		}

		public DockingContainerControl()
		{
			mvarItems = new DockingItem.DockingItemCollection(this);
		}

		private class WindowListPopupWindow : Window
		{
			private ListViewControl lvWindows;
			private DefaultTreeModel tmWindows;

			private DockingContainerControl dccParent = null;
			public WindowListPopupWindow(DockingContainerControl dcc)
			{
				dccParent = dcc;

				Layout = new BoxLayout(Orientation.Vertical);
				Size = new Framework.Drawing.Dimension2D(500, 200);

				tmWindows = new DefaultTreeModel(new Type[] { typeof(string) });

				lvWindows = new ListViewControl();
				lvWindows.Columns.Add(new ListViewColumn("Window", new CellRenderer[] { new CellRendererText(tmWindows.Columns[0]) }));
				lvWindows.HeaderStyle = ColumnHeaderStyle.None;
				lvWindows.Model = tmWindows;
				Controls.Add(lvWindows, new BoxLayout.Constraints(true, true));
			}

			private int i = -1;
			public void Next()
			{
				Console.WriteLine("__wwC -> next()");

				i++;
				if (i >= lvWindows.Model.Rows.Count)
				{
					i = 0;
				}

				if (i >= 0 && i < lvWindows.Model.Rows.Count)
				{
					lvWindows.Select(lvWindows.Model.Rows[i]);
				}
			}
			public void Prev()
			{
				Console.WriteLine("__wwC -> prev()");

				i--;
				if (i < 0)
				{
					i = lvWindows.Model.Rows.Count - 1;
				}

				if (i >= 0 && i < lvWindows.Model.Rows.Count)
				{
					lvWindows.Select(lvWindows.Model.Rows[i]);
				}
			}

			public DockingItem GetSelectedItem()
			{
				if (lvWindows.SelectedRows.Count == 1)
				{
					return lvWindows.SelectedRows[0].GetExtraData<DockingItem>("item");
				}
				return null;
			}

			protected internal override void OnKeyDown(KeyEventArgs e)
			{
				base.OnKeyDown(e);

				if (e.Key == KeyboardKey.Tab && (e.ModifierKeys & KeyboardModifierKey.Alt) == KeyboardModifierKey.Alt)
				{
					Console.WriteLine("switching window");
				}
			}

			protected internal override void OnShown(EventArgs e)
			{
				base.OnShown(e);

				Console.WriteLine("dcc: showing window list");

				tmWindows.Rows.Clear();
				foreach (DockingItem item in dccParent.Items)
				{
					if (item.Placement == DockingItemPlacement.Center)
					{
						TreeModelRow row = new TreeModelRow(new TreeModelRowColumn[]
						{
							new TreeModelRowColumn(tmWindows.Columns[0], item.Title)
						});
						row.SetExtraData<DockingItem>("item", item);
						tmWindows.Rows.Add(row);
					}
				}
			}
		}
		private WindowListPopupWindow __wwC = null;

		public void ShowWindowListPopup()
		{
			if (__wwC == null)
			{
				__wwC = new WindowListPopupWindow(this);
			}
			__wwC.Show();
			__wwC.Next();
		}
		public void HideWindowListPopup()
		{
			if (__wwC != null)
			{
				Console.WriteLine("dcc: hiding window list");

				DockingItem item = __wwC.GetSelectedItem();
				this.CurrentItem = item;

				__wwC.Hide();
			}
		}
		public void ShowWindowListPopupDialog()
		{
			Console.WriteLine("dcc: showing window list (modal)");
		}
	}
}
