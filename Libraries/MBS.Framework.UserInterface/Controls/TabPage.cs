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
				base.ClearItems ();
				(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.ClearTabPages();
			}
			protected override void InsertItem (int index, TabPage item)
			{
				base.InsertItem (index, item);
				(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.InsertTabPage(index, item);
			}
			protected override void RemoveItem (int index)
			{
				(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.RemoveTabPage(this[index]);
				base.RemoveItem (index);
			}
			protected override void SetItem (int index, TabPage item)
			{
				if (index >= 0 && index < this.Count) {
					(_parentContainer.ControlImplementation as Native.ITabContainerControlImplementation)?.RemoveTabPage(this[index]);
				}
				base.SetItem (index, item);
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

		public override string ToString()
		{
			return this.Text;
		}

	}
}

