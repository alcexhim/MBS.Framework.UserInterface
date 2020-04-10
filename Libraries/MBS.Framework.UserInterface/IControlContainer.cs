using System;
namespace MBS.Framework.UserInterface
{
	public interface IControlContainer
	{
		Control[] GetAllControls();
		Control.ControlCollection Controls { get; }
	}
}
