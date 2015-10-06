using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Drawing
{
	public abstract class Graphics
	{
		protected abstract void DrawLineInternal(double x1, double y1, double x2, double y2);
		public void DrawLine(double x1, double y1, double x2, double y2)
		{
			DrawLineInternal(x1, y1, x2, y2);
		}

		public void DrawRectangle(double x, double y, double width, double height)
		{
			DrawLine(x, y, x + width, y);
			DrawLine(x, y, x, y + height);
			DrawLine(x, y + height, x + width, y + height);
			DrawLine(x + width, y, x + width, y + height);
		}
		public void DrawRectangle(Rectangle rect)
		{
			DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void Clear(Color color)
		{
			
		}
	}
}
