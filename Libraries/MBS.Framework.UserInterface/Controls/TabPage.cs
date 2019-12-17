using System;

namespace MBS.Framework.UserInterface.Controls
{
	public class TabPage : Container
	{
		public class TabPageCollection
			: System.Collections.ObjectModel.Collection<TabPage>
		{
			private TabContainer _parentContainer = null;

			protected override void ClearItems ()
			{
				for (int i = 0; i < Count; i++)
				{
					this[i].Parent = null;
				}
				base.ClearItems ();
				(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.ClearTabPages();
			}
			protected override void InsertItem (int index, TabPage item)
			{
				base.InsertItem (index, item);
				item.Parent = _parentContainer;
				(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.InsertTabPage(index, item);
			}
			protected override void RemoveItem (int index)
			{
				(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.RemoveTabPage(this[index]);
				this[index].Parent = null;
				base.RemoveItem (index);
			}
			protected override void SetItem (int index, TabPage item)
			{
				if (index >= 0 && index < this.Count) {
					(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.RemoveTabPage(this[index]);
				}
				base.SetItem (index, item);
				item.Parent = _parentContainer;
				(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.InsertTabPage(index, item);
			}

			public TabPageCollection(TabContainer parentContainer) {
				_parentContainer = parentContainer;
			}
		}

		public TabPage(string text = null, Control[] controls = null)
		{
			if (text != null)
				this.Text = text;
			if (controls != null) {
				foreach (Control ctl in controls) {
					this.Controls.Add (ctl);
				}
			}
		}

		public new TabContainer Parent { get; private set; } = null;

		private bool _Reorderable = false;
		public bool Reorderable
		{
			get { return _Reorderable; }
			set
			{
				((Parent as TabContainer)?.ControlImplementation as Native.ITabContainerControlImplementation)?.SetTabPageReorderable(this, value);
				_Reorderable = value;
			}
		}

		private bool _Detachable = false;
		public bool Detachable
		{
			get { return _Detachable; }
			set
			{
				((Parent as TabContainer)?.ControlImplementation as Native.ITabContainerControlImplementation)?.SetTabPageDetachable(this, value);
				_Detachable = value;
			}
		}

		public override string ToString()
		{
			return this.Text;
		}

	}
}

