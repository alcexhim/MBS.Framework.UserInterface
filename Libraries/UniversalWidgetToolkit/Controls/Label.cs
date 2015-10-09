using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit.Controls
{
	public class Label : Control
	{
		private HorizontalAlignment mvarHorizontalAlignment = HorizontalAlignment.Left;
		public HorizontalAlignment HorizontalAlignment { get { return mvarHorizontalAlignment; } set { mvarHorizontalAlignment = value; } }

		private VerticalAlignment mvarVerticalAlignment = VerticalAlignment.Top;
		public VerticalAlignment VerticalAlignment { get { return mvarVerticalAlignment; } set { mvarVerticalAlignment = value; } }

		public override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawText(Text, Font, ClientRectangle, Colors.Black, mvarHorizontalAlignment, mvarVerticalAlignment);
		}
	}
}
