using System;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GIO
{
	internal class Delegates
	{
		public delegate void ActivateDelegate (IntPtr action, IntPtr parameter, IntPtr user_data);

		public delegate void ChangeStateDelegate (IntPtr action, IntPtr parameter, IntPtr user_data);
	}
}

