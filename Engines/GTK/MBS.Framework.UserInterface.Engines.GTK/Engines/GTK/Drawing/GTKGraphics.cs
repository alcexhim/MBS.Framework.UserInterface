using System;
using System.Diagnostics;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Engines.GTK.Drawing
{
	public class GTKGraphics : Graphics
	{
		private IntPtr mvarCairoContext = IntPtr.Zero;

		public GTKGraphics(IntPtr cairoContext)
		{
			mvarCairoContext = cairoContext;
		}

		protected override void DrawImageInternal(Image image, double x, double y, double width, double height)
		{

		}

		protected override void DrawLineInternal(Pen pen, double x1, double y1, double x2, double y2)
		{
			SelectPen(pen);

			Internal.Cairo.Methods.cairo_move_to(mvarCairoContext, x1, y1);
			CheckStatus();

			Internal.Cairo.Methods.cairo_line_to(mvarCairoContext, x2, y2);
			CheckStatus();

			Internal.Cairo.Methods.cairo_stroke(mvarCairoContext);
			CheckStatus();
		}

		protected override void DrawTextInternal(string value, Font font, Rectangle rectangle, Brush brush, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
		{
			SelectBrush(brush);

			Internal.Cairo.Methods.cairo_move_to(mvarCairoContext, rectangle.X, rectangle.Y);
			CheckStatus();

			Internal.Cairo.Methods.cairo_select_font_face(mvarCairoContext, font.FamilyName, (font.Italic ? Internal.Cairo.Constants.CairoFontSlant.Italic : Internal.Cairo.Constants.CairoFontSlant.Normal), (font.Weight == 800 ? Internal.Cairo.Constants.CairoFontWeight.Bold : Internal.Cairo.Constants.CairoFontWeight.Normal));
			CheckStatus();

			Internal.Cairo.Methods.cairo_set_font_size(mvarCairoContext, font.Size);
			CheckStatus();

			// this is for RENDERING text - textually
			Internal.Cairo.Methods.cairo_show_text(mvarCairoContext, value);
			CheckStatus();

			/*
			 // this is for DRAWING text - graphically
			Internal.Cairo.Methods.cairo_text_path(mvarCairoContext, value);
			CheckStatus();

			Internal.Cairo.Methods.cairo_fill(mvarCairoContext);
			CheckStatus();
			*/

			// there IS a difference - textually-rendered text is selectable when rendering PDFs; graphically-drawn text is not
		}

		[DebuggerNonUserCode()]
		private void CheckStatus()
		{
			Internal.Cairo.Constants.CairoStatus status = Internal.Cairo.Methods.cairo_status(mvarCairoContext);
			switch (status)
			{
				case Internal.Cairo.Constants.CairoStatus.Success:
					return;
			}
			throw new Exception(status.ToString());
		}

		private void SelectBrush(Brush brush)
		{
			if (brush is SolidBrush)
			{
				SolidBrush sb = (brush as SolidBrush);
				IntPtr hPattern = Internal.Cairo.Methods.cairo_pattern_create_rgba(sb.Color.R, sb.Color.G, sb.Color.B, sb.Color.A);
				Internal.Cairo.Methods.cairo_set_source(mvarCairoContext, hPattern);
			}
			else
			{
				throw new NotImplementedException("Brush type " + brush.GetType().FullName);
			}
			CheckStatus();
		}
		private void SelectPen(Pen pen)
		{
			IntPtr hPattern = Internal.Cairo.Methods.cairo_pattern_create_rgba(pen.Color.R, pen.Color.G, pen.Color.B, pen.Color.A);
			Internal.Cairo.Methods.cairo_set_source(mvarCairoContext, hPattern);
			Internal.Cairo.Methods.cairo_set_line_width(mvarCairoContext, pen.Width.ConvertTo(MeasurementUnit.Pixel).Value);

			CheckStatus();
		}

		protected override void DrawRectangleInternal(Pen pen, double x, double y, double width, double height)
		{
			SelectPen(pen);

			Internal.Cairo.Methods.cairo_rectangle(mvarCairoContext, x, y, width, height);
			CheckStatus();

			Internal.Cairo.Methods.cairo_stroke(mvarCairoContext);
			CheckStatus();
		}

		protected override void FillRectangleInternal(Brush brush, double x, double y, double width, double height)
		{
			SelectBrush(brush);

			Internal.Cairo.Methods.cairo_rectangle(mvarCairoContext, x, y, width, height);
			CheckStatus();

			Internal.Cairo.Methods.cairo_fill(mvarCairoContext);
			CheckStatus();
		}
	}
}
