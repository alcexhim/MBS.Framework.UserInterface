using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing.Drawing2D;

namespace MBS.Framework.UserInterface.Drawing
{
	public class Pen
	{
		public DashStyle DashStyle { get; set; }
		public Measurement Width { get; set; }
		public Color Color { get; set; }
		public LineCapStyles LineCapStyle { get; set; } = LineCapStyles.Flat;

		public Pen(Color color, Measurement width = default(Measurement), DashStyle dashStyle = null, LineCapStyles lineCapStyle = LineCapStyles.Flat)
		{
			DashStyle = dashStyle;
			if (width.Equals(default(Measurement)))
			{
				width = new Measurement(1.0, MeasurementUnit.Pixel);
			}
			Width = width;
			Color = color;
			LineCapStyle = lineCapStyle;
		}
	}
}
