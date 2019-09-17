﻿using System;
namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public class WindowsFormsNativeControl : NativeControl
	{
		public System.Windows.Forms.Control Handle { get; private set; } = null;

		public WindowsFormsNativeControl (System.Windows.Forms.Control handle)
		{
			Handle = handle;
		}
	}
}