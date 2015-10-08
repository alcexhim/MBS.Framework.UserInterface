using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Drawing
{
	public class LinearGradientBrush : Brush
	{
		public LinearGradientBrush(Rectangle bounds, )
	}
	public class SolidBrush : Brush
	{
		private Color mvarColor = Color.Empty;
		public Color Color { get { return mvarColor; } }
		public SolidBrush(Color color)
		{
			mvarColor = color;
		}
	}
	public abstract class Brush
	{
	}
}
