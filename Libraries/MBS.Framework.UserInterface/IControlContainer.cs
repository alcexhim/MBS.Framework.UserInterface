using System;
namespace MBS.Framework.UserInterface
{
	public interface IControlContainer : IControl
	{
		Control[] GetAllControls();
		Control.ControlCollection Controls { get; }
	}
}
