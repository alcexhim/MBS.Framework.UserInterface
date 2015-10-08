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
			else if (value.StartsWith("#") && value.Length == 7)
			{
				string RRGGBB = value.Substring(1);
				byte RR = Byte.Parse(RRGGBB.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				byte GG = Byte.Parse(RRGGBB.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
				byte BB = Byte.Parse(RRGGBB.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
				return Color.FromRGBAByte(RR, GG, BB);
			}
			else if (value.StartsWith("rgb(") && value.EndsWith(")"))
			{
				string r_g_b = value.Substring(3, value.Length - 4);
				string[] rgb = r_g_b.Split(new char[] { ',' });
				if (rgb.Length == 3)
				{
					byte r = Byte.Parse(rgb[0].Trim());
					byte g = Byte.Parse(rgb[1].Trim());
					byte b = Byte.Parse(rgb[2].Trim());
					return Color.FromRGBAByte(r, g, b);
				}
			}
			else if (value.StartsWith("rgba(") && value.EndsWith(")"))
			{

			}
			else
			{
				/*
				try
				{
					System.Drawing.Color color = System.Drawing.Color.FromName(value);
					return color;
				}
				catch
				{

				}
				*/
			}
			return Color.Empty;
		}

		public byte GetRedByte()
		{
			return (byte)(mvarR * 255);
		}
		public byte GetGreenByte()
		{
			return (byte)(mvarG * 255);
		}
		public byte GetBlueByte()
		{
			return (byte)(mvarB * 255);
		}
		public byte GetAlphaByte()
		{
			return (byte)(mvarA * 255);
		}

		public int ToInt32()
		{
			return BitConverter.ToInt32(new byte[] { (byte)mvarA, (byte)mvarB, (byte)mvarG, (byte)mvarR }, 0);
		}
	}
}
