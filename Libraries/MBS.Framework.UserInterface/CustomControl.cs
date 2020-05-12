using System.Collections.Generic;

namespace MBS.Framework.UserInterface
{
	public class CustomControl : Control, IControlContainer
	{
		public Layout Layout { get; set; } = null;
		public ControlCollection Controls { get; } = null;

		public Control[] GetAllControls()
		{
			return (new List<Control>(Controls)).ToArray();
		}

		public CustomControl()
		{
			Controls = new ControlCollection(this);
		}
	}
}
