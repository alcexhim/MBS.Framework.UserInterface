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
				Application.Engine.TabContainer_ClearTabPages (_parentContainer);
			}
			protected override void InsertItem (int index, TabPage item)
			{
				base.InsertItem (index, item);
				Application.Engine.TabContainer_InsertTabPage (_parentContainer, index, item);
			}
			protected override void RemoveItem (int index)
			{
				Application.Engine.TabContainer_RemoveTabPage (_parentContainer, this [index]);
				base.RemoveItem (index);
			}
			protected override void SetItem (int index, TabPage item)
			{
				if (index >= 0 && index < this.Count) {
					Application.Engine.TabContainer_RemoveTabPage (_parentContainer, this [index]);
				}
				base.SetItem (index, item);
				Application.Engine.TabContainer_InsertTabPage (_parentContainer, index, item);
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

