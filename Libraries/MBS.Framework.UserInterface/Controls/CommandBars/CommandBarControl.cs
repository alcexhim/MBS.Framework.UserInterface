using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Controls.CommandBars
{
	public class CommandBarControl : Container
	{
		public Control Control { get; } = null;

		public CommandBarControl(Control ctl)
		{
			this.Layout = new BoxLayout(Orientation.Horizontal);
			this.Controls.Add(new CommandBarGripper(this), new BoxLayout.Constraints(false, false));

			Control = ctl;
			this.Controls.Add(ctl, new BoxLayout.Constraints(true, true));
		}

	}
}