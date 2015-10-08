using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Theming;

namespace UniversalWidgetToolkit.Drawing
{
	public struct Color
	{
		public static readonly Color Empty;

		private double mvarR;
		public double R { get { return mvarR; } set { mvarR = value; } }

		private double mvarG;
		public double G { get { return mvarG; } set { mvarG = value; } }

		private double mvarB;
		public double B { get { return mvarB; } set { mvarB = value; } }

		private double mvarA;
		public double A { get { return mvarA; } set { mvarA = value; } }

		public static Color FromRGBADouble(double r, double g, double b, double a = 1.0)
		{
			Color color = new Color();
			color.R = r;
			color.G = g;
			color.B = b;
			color.A = a;
			return color;
		}

		public static Color FromRGBAByte(byte r, byte g, byte b, byte a = 255)
		{
			Color color = new Color();
			color.R = ((double)r / 255);
			color.G = ((double)g / 255);
			color.B = ((double)b / 255);
			color.A = ((double)a / 255);
			return color;
		}

		public static Color FromString(string value)
		{
			if (value.StartsWith("@"))
			{
				if (ThemeManager.CurrentTheme != null) return ThemeManager.CurrentTheme.GetColorFromString(value);
			}
			return Color.Empty;
		}
	}
}
