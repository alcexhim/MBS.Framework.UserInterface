using System;
namespace UniversalWidgetToolkit.Engines.GTK
{
	public class GTKNativeControl : NativeControl
	{
		public IntPtr Handle { get; private set; } = IntPtr.Zero;

		public GTKNativeControl(IntPtr handle)
		{
			this.Handle = handle;
		}
	}
}
