using System;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Input.Mouse;

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
		void BeginMoveDrag(MouseButtons buttons, double x, double y, DateTime now);
	}
}
