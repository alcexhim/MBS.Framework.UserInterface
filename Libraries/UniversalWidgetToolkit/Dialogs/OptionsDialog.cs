using System;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Layouts;

using MBS.Framework.Drawing;

namespace UniversalWidgetToolkit.Dialogs
{
	public class OptionsDialog : Dialog
	{
		public OptionsDialog()
		{
			this.Layout = new BoxLayout(Orientation.Vertical);

			this.Buttons.Add(new Button(ButtonStockType.OK, DialogResult.OK));
			this.Buttons.Add(new Button(ButtonStockType.Cancel, DialogResult.Cancel));

			this.DefaultButton = this.Buttons[0];

			SplitContainer vpaned = new SplitContainer(Orientation.Vertical);
			vpaned.Panel1.Layout = new Layouts.BoxLayout(Orientation.Horizontal);

			ListView tv = new ListView();
			tv.Model = new DefaultTreeModel(new Type[] { typeof(string) });

			vpaned.Panel1.Controls.Add(tv, new BoxLayout.Constraints(true, true));

			this.Controls.Add(vpaned, new BoxLayout.Constraints(true, true, 0, BoxLayout.PackType.Start));

			this.Title = "Options";
			this.Size = new Dimension2D(600, 400);
		}
	}
}
