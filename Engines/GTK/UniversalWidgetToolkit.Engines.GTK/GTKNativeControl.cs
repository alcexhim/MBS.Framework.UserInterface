using System;
using System.Collections.Generic;

namespace UniversalWidgetToolkit.Engines.GTK
{
	public class GTKNativeControl : NativeControl
	{
		public IntPtr Handle { get; private set; } = IntPtr.Zero;
		public IntPtr[] AdditionalHandles { get; private set; } = new IntPtr[0];

		private Dictionary<string, IntPtr> _NamedHandles = new Dictionary<string, IntPtr>();
		public void SetNamedHandle(string name, IntPtr value)
		{
			_NamedHandles[name] = value;
		}
		public IntPtr GetNamedHandle(string name, IntPtr defaultValue = default(IntPtr))
		{
			if (_NamedHandles.ContainsKey(name)) return _NamedHandles[name];
			return defaultValue;
		}

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
