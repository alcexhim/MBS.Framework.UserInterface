using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Drawing
{
	public static class Colors
	{
		private static Color mvarBlack = Color.FromRGBAByte(0, 0, 0);
		public static Color Black { get { return mvarBlack; } set { mvarBlack = value; } }

		private static Color mvarWhite = Color.FromRGBAByte(255, 255, 255);
		public static Color White { get { return mvarWhite; } }
	}
}
