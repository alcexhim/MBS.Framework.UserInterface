using System;
namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public abstract class WindowsFormsNativeImplementation : NativeImplementation
	{
		public WindowsFormsNativeImplementation (Engine engine, Control control) : base(engine, control)
		{
		}
	}
}
