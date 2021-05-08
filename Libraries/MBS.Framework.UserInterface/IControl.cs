using System;
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface
{
	public interface IControl
	{
		bool IsCreated { get; }
		bool Visible { get; set; }
		ControlImplementation ControlImplementation { get; }
		IVirtualControlContainer Parent { get; }
		Padding Padding { get; set; }
		Rectangle Bounds { get; }
	}
}
