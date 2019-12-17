using System;

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
		}
	}
	public class TabContainer : SystemControl
	{
		private TabPage.TabPageCollection mvarTabPages = null;
		public TabPage.TabPageCollection TabPages { get { return mvarTabPages; } }

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

		public TabContainer()
		{
			mvarTabPages = new TabPage.TabPageCollection (this);
		}
	}
}

