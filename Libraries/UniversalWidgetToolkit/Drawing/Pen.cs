using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Drawing
{
	public enum PenStyle
	{

	}
	public struct Pen
	{
		private PenStyle mvarStyle;
		public PenStyle Style { get { return mvarStyle; } set { mvarStyle = value; } }

		private double mvarWidth;
		public double Width { get { return mvarWidth; } set { mvarWidth = value; } }

		private Color mvarColor;
		public Color Color { get { return mvarColor; } set { mvarColor = value; } }

		public Pen(PenStyle style, double width, Color color)
		{
			mvarStyle = style;
			mvarWidth = width;
			mvarColor = color;
		}
	}
}
