using System;
namespace MBS.Framework.UserInterface
{
	public interface IControl
	{
		bool IsCreated { get; }
		bool Visible { get; set; }
		ControlImplementation ControlImplementation { get; }
		IControlContainer Parent { get; }
		MBS.Framework.Drawing.Padding Padding { get; set; }
	}
}
