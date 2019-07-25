using System;
using System.Collections.Generic;

using UniversalWidgetToolkit.DragDrop;
using UniversalWidgetToolkit.Input.Keyboard;
using UniversalWidgetToolkit.Input.Mouse;

namespace UniversalWidgetToolkit
{
	/// <summary>
	/// Native implementation for the specified Control.
	/// </summary>
	public abstract class NativeImplementation : ControlImplementation
	{
		public NativeImplementation(Engine engine, Control control) : base(engine, control)
		{
		}
	}
}
