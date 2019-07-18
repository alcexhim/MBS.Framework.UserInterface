using System;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Layouts;

using MBS.Framework.Drawing;

namespace UniversalWidgetToolkit.TestProject
{
	public class TestCustomControl : CustomControl
	{
		private int timesPainted = 0;

		public TestCustomControl()
		{
			this.Size = new Dimension2D(200, 200);
		}

		public bool ShowGreenBox { get; set; } = false;

		public override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.DrawText("Sample", Font.FromFamily("Liberation Sans", 26), new Rectangle(64, 64, 200, 200), Brushes.White, HorizontalAlignment.Center, VerticalAlignment.Middle);

			e.Graphics.FillRectangle(Brushes.Black, new Rectangle(0, 0, 200, 200));
			e.Graphics.DrawRectangle(Pens.Red, new Rectangle(64, 64, 200 - 128, 200 - 128));

			if (ShowGreenBox)
				e.Graphics.FillRectangle(Brushes.Green, new Rectangle(64, 64, 200 - 128, 200 - 128));
		}
	}
}
