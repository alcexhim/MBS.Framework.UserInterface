using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit.Controls
{
	public class Label : SystemControl
	{

		public Label()
		{
		}
		public Label(string text)
		{
			this.Text = text;
		}

		public bool UseMnemonic { get; set; } = true;

		public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Default;
		public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Default;
		public WordWrapMode WordWrap { get; set; } = WordWrapMode.Default;

		public override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawText(Text, Font, ClientRectangle, Brushes.Black, HorizontalAlignment, VerticalAlignment);
		}
	}
}
