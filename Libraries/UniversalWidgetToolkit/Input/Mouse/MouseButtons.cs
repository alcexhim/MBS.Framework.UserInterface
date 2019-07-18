using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Input.Mouse
{
	[Flags()]
    public enum MouseButtons
    {
        None = 0,
        Primary = 1,
        Wheel = 2,
        Secondary = 4,
        XButton1 = 8,
        XButton2 = 16
    }
}
