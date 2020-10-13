using System;
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface
{
	public interface IControlContainer : IControl
	{
		Control[] GetAllControls();
		Control.ControlCollection Controls { get; }
		Layout Layout { get; set; }
		Rectangle Bounds { get; }
	}
}
