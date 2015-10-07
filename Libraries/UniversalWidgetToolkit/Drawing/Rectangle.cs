using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Drawing
{
	public struct Rectangle
	{
		public static readonly Rectangle Empty = new Rectangle();
		
		public Rectangle(Vector2D location, Dimension2D size)
		{
			mvarX = location.X;
			mvarY = location.Y;
			mvarWidth = size.Width;
			mvarHeight = size.Height;
		}
		public Rectangle(double x, double y, double width, double height)
		{
			mvarX = x;
			mvarY = y;
			mvarWidth = width;
			mvarHeight = height;
		}

		private double mvarX;
		public double X { get { return mvarX; } set { mvarX = value; } }

		private double mvarY;
		public double Y { get { return mvarY; } set { mvarY = value; } }

		private double mvarWidth;
		public double Width { get { return mvarWidth; } set { mvarWidth = value; } }

		private double mvarHeight;
		public double Height { get { return mvarHeight; } set { mvarHeight = value; } }

		public Vector2D Location
		{
			get { return new Vector2D(mvarX, mvarY); }
			set { mvarX = value.X; mvarY = value.Y; }
		}
		public Dimension2D Size
		{
			get { return new Dimension2D(mvarWidth, mvarHeight); }
			set { mvarWidth = value.Width; mvarHeight = value.Height; }
		}

		public double Right { get { return mvarX + mvarWidth; } set { mvarWidth = value - mvarX; } }
		public double Bottom { get { return mvarY + mvarHeight; } set { mvarHeight = value - mvarY; } }
	}
}
