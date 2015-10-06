using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Drawing
{
	public struct Rectangle
	{
		public static readonly Rectangle Empty = new Rectangle();
		
		private bool mvarIsEmpty;
		public bool IsEmpty { get { return mvarIsEmpty; } }

		public Rectangle(double x, double y, double width, double height)
		{
			mvarX = x;
			mvarY = y;
			mvarWidth = width;
			mvarHeight = height;
			mvarIsEmpty = false;
		}

		private double mvarX;
		public double X { get { return mvarX; } set { mvarX = value; mvarIsEmpty = false; } }

		private double mvarY;
		public double Y { get { return mvarY; } set { mvarY = value; mvarIsEmpty = false; } }

		private double mvarWidth;
		public double Width { get { return mvarWidth; } set { mvarWidth = value; mvarIsEmpty = false; } }

		private double mvarHeight;
		public double Height { get { return mvarHeight; } set { mvarHeight = value; mvarIsEmpty = false; } }
	}
}
