using System;
namespace UniversalWidgetToolkit.Engines.GTK
{
	public class GTKNativeControl : NativeControl
	{
		public IntPtr Handle { get; private set; } = IntPtr.Zero;
		public IntPtr[] AdditionalHandles { get; private set; } = new IntPtr[0];

		public GTKNativeControl(IntPtr handle, params IntPtr[] additionalHandles)
		{
			this.Handle = handle;
			this.AdditionalHandles = additionalHandles;
		}

		public override string ToString()
		{
			return Handle.ToString();
		}
	}
}
