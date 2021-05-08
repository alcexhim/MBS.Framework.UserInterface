using System;

namespace MBS.Framework.UserInterface
{
	public interface IControlContainer : IVirtualControlContainer
	{
		Control.ControlCollection Controls { get; }
		Layout Layout { get; set; }
	}
}
