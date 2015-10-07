using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Drawing
{
	public abstract class Vector
	{
	}
	public class Vector2D
	{
		private double mvarX = 0.0;
		public double X { get { return mvarX; } set { mvarX = value; } }
		private double mvarY = 0.0;
		public double Y { get { return mvarY; } set { mvarY = value; } }

		public Vector2D()
		{
		}
		public Vector2D(double x, double y)
		{
			mvarX = x;
			mvarY = y;
		}
	}
}
