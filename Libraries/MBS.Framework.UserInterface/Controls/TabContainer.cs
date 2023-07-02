using System;
using System.Collections.Generic;

namespace MBS.Framework.UserInterface.Controls
{
	namespace Native
	{
		public interface ITabContainerControlImplementation
		{
			void InsertTabPage(int index, TabPage item);
			void RemoveTabPage(TabPage tabPage);
			void ClearTabPages();

			void SetTabPageReorderable(TabPage page, bool value);
			void SetTabPageDetachable(TabPage page, bool value);

			TabPage GetSelectedTab();
			void SetSelectedTab(TabPage page);

			void SetTabText(TabPage page, string text);

			void SetTabPosition(TabPosition position);

			bool GetScrollable();
			void SetScrollable(bool value);
		}
	}
	public class TabContainer : SystemControl, IVirtualControlContainer, ITabPageContainer
	{
		private bool _Scrollable = false;
		/// <summary>
		/// Sets whether the tab label area will have arrows for scrolling if there are too many tabs to fit in the area.
		/// </summary>
		/// <value><c>true</c> if scrollable; otherwise, <c>false</c>.</value>
		public bool Scrollable
		{
			get
			{
				Native.ITabContainerControlImplementation impl = (ControlImplementation as Native.ITabContainerControlImplementation);
				if (IsCreated && impl != null)
				{
					return impl.GetScrollable();
				}
				return _Scrollable;
			}
			set
			{
				_Scrollable = value;
				(ControlImplementation as Native.ITabContainerControlImplementation)?.SetScrollable(value);
			}
		}

		private TabPosition _TabPosition = TabPosition.Top;
		public TabPosition TabPosition
		{
			get { return _TabPosition; }
			set
			{
				_TabPosition = value;
				(ControlImplementation as Native.ITabContainerControlImplementation)?.SetTabPosition(value);
			}
		}

		public Control[] GetAllControls()
		{
			List<Control> list = new List<Control>();
			for (int i = 0; i < TabPages.Count; i++)
			{
				Control[] ctls = TabPages[i].GetAllControls();
				list.AddRange(ctls);
			}
			return list.ToArray();
		}

		public event TabContainerSelectedTabChangedEventHandler SelectedTabChanged;
		protected void OnSelectedTabChanged(TabContainerSelectedTabChangedEventArgs e)
		{
			SelectedTabChanged?.Invoke(this, e);
		}

		public TabContainerTabStyle TabStyle { get; set; } = TabContainerTabStyle.Default;

		private TabPage.TabPageCollection mvarTabPages = null;
		public TabPage.TabPageCollection TabPages { get { return mvarTabPages; } }

		private TabPage _SelectedTab = null;
		public TabPage SelectedTab
		{
			get
			{
				if (IsCreated)
				{
					Native.ITabContainerControlImplementation impl = (ControlImplementation as Native.ITabContainerControlImplementation);
					if (impl != null)
					{
						return impl.GetSelectedTab();
					}
				}
				return _SelectedTab;
			}
			set
			{
				(ControlImplementation as Native.ITabContainerControlImplementation)?.SetSelectedTab(value);
				_SelectedTab = value;
			}
		}

		private Control.ControlCollection mvarTabTitleControls = new Control.ControlCollection();
		/// <summary>
		/// Gets the controls displayed for each tab.
		/// </summary>
		/// <value>The tab title controls.</value>
		public Control.ControlCollection TabTitleControls { get { return mvarTabTitleControls; } }

		public string GroupName { get; set; } = null;

		public event TabPageDetachedEventHandler TabPageDetached;
		protected virtual void OnTabPageDetached(TabPageDetachedEventArgs e)
		{
			TabPageDetached?.Invoke(this, e);
		}

		public event BeforeTabContextMenuEventHandler BeforeTabContextMenu;
		protected virtual void OnBeforeTabContextMenu(BeforeTabContextMenuEventArgs e)
		{
			BeforeTabContextMenu?.Invoke(this, e);
		}

		public TabContainer()
		{
			mvarTabPages = new TabPage.TabPageCollection (this);
		}
	}
}
