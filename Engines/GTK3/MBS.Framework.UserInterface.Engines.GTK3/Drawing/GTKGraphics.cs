using System;
using System.Collections.Generic;
using System.Diagnostics;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Drawing.Drawing2D;
using MBS.Framework.UserInterface.Engines.GTK3.Internal.Cairo;

namespace MBS.Framework.UserInterface.Engines.GTK3.Drawing
{
	public class GTKGraphics : Graphics
	{
		private IntPtr mvarCairoContext = IntPtr.Zero;
		private IntPtr mvarPangoContext = IntPtr.Zero;

		public GTKGraphics(IntPtr cairoContext)
		{
			mvarCairoContext = cairoContext;
		}
		public GTKGraphics(IntPtr cairoContext, Rectangle clipRectangle) : base(clipRectangle)
		{
			mvarCairoContext = cairoContext;
		}
		public GTKGraphics(IntPtr cairoContext, IntPtr pangoContext, Rectangle clipRectangle) : base(clipRectangle)
		{
			mvarCairoContext = cairoContext;
			mvarPangoContext = pangoContext;
		}

		protected override TextMeasurement MeasureTextInternal(string text, Font font)
		{
			Internal.Cairo.Structures.cairo_text_extents_t extents = new Internal.Cairo.Structures.cairo_text_extents_t();
			Internal.Cairo.Methods.cairo_text_extents(mvarCairoContext, text, ref extents);
			return new TextMeasurement(extents.width, extents.height, extents.x_bearing, extents.y_bearing, extents.x_advance, extents.y_advance);
		}

		protected override void RotateInternal(double angle)
		{
			Internal.Cairo.Methods.cairo_translate(mvarCairoContext, RotationCenterpoint.X, RotationCenterpoint.Y);
			CheckStatus();

			double angleRadians = (Math.PI / 180) * angle;
			Internal.Cairo.Methods.cairo_rotate(mvarCairoContext, angleRadians);
			CheckStatus();

			Internal.Cairo.Methods.cairo_translate(mvarCairoContext, -RotationCenterpoint.X, -RotationCenterpoint.Y);
			CheckStatus();
		}

		private void SelectPath(Vector2D[] points)
		{
			Internal.Cairo.Methods.cairo_move_to(mvarCairoContext, points[0].X, points[0].Y);
			CheckStatus();

			for (int i = 1; i < points.Length; i++)
			{
				Internal.Cairo.Methods.cairo_line_to(mvarCairoContext, points[i].X, points[i].Y);
				CheckStatus();
			}
		}

		protected override void DrawPolygonInternal(Pen pen, Vector2D[] points)
		{
			SelectPen(pen);
			SelectPath(points);

			Internal.Cairo.Methods.cairo_stroke(mvarCairoContext);
			CheckStatus();
		}
		protected override void FillPolygonInternal(Brush brush, Vector2D[] points)
		{
			SelectBrush(brush);
			SelectPath(points);

			Internal.Cairo.Methods.cairo_fill(mvarCairoContext);
			CheckStatus();
		}

		protected override void DrawImageInternal(Image image, double x, double y, double width, double height)
		{
			double width_ratio = width / image.Width;
			double height_ratio = height / image.Height;

			Internal.Cairo.Methods.cairo_save(mvarCairoContext);
			CheckStatus();

			Internal.Cairo.Methods.cairo_translate(mvarCairoContext, x, y);
			CheckStatus();

			Internal.Cairo.Methods.cairo_scale(mvarCairoContext, width_ratio, height_ratio);
			CheckStatus();

			Internal.GDK.Methods.gdk_cairo_set_source_pixbuf(mvarCairoContext, (image as GDKPixbufImage).Handle, 0, 0);
			CheckStatus();

			Internal.Cairo.Methods.cairo_paint(mvarCairoContext);
			CheckStatus();

			Internal.Cairo.Methods.cairo_restore(mvarCairoContext);
			CheckStatus();
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

		protected override void DrawTextInternal(string value, Font font, Vector2D location, Dimension2D size, Brush brush, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
		{
			SelectBrush(brush);
			SelectFont(ref font);

			Measurement sz = font.Size;
			if (sz == Measurement.Empty)
			{
				sz = new Measurement(11, MeasurementUnit.Point);
			}

			double x = location.X;
			double y_orig = location.Y + sz.GetValue(MeasurementUnit.Point);
			double y = y_orig;

			TextMeasurement textMeasurement = MeasureText(value, font);

			// FIXME: use cairo_glyph_extents to measure the string
			if (horizontalAlignment == HorizontalAlignment.Center)
			{
				x = location.X + ((size.Width - textMeasurement.Size.Width) / 2);
			}
			if (verticalAlignment == VerticalAlignment.Middle)
			{
				y = y_orig + ((size.Height - textMeasurement.Size.Height) / 2);
			}

			Internal.Cairo.Methods.cairo_move_to(mvarCairoContext, x, y);
			CheckStatus();

			if (mvarPangoContext != IntPtr.Zero)
			{
				IntPtr layout = Internal.Pango.Methods.pango_layout_new(mvarPangoContext);

				if (size != default(Dimension2D))
				{
					//Internal.Pango.Methods.pango_layout_set_width(layout, (int)size.Width);
					//Internal.Pango.Methods.pango_layout_set_height(layout, (int)size.Height);
				}

				if (horizontalAlignment == HorizontalAlignment.Left)
				{
					Internal.Pango.Methods.pango_layout_set_alignment(layout, Internal.Pango.Constants.PangoAlignment.Left);
				}
				else if (horizontalAlignment == HorizontalAlignment.Center)
				{
					Internal.Pango.Methods.pango_layout_set_alignment(layout, Internal.Pango.Constants.PangoAlignment.Center);
				}
				else if (horizontalAlignment == HorizontalAlignment.Right)
				{
					Internal.Pango.Methods.pango_layout_set_alignment(layout, Internal.Pango.Constants.PangoAlignment.Right);
				}
				else if (horizontalAlignment == HorizontalAlignment.Justify)
				{
					Internal.Pango.Methods.pango_layout_set_justify(layout, true);
				}
				if (verticalAlignment == VerticalAlignment.Middle)
				{
					//y = y_orig + ((size.Height - textMeasurement.Size.Height) / 2);
				}

				Internal.Pango.Methods.pango_layout_set_text(layout, value, value.Length);

				Internal.PangoCairo.Methods.pango_cairo_show_layout(mvarCairoContext, layout);
			}
			else
			{
				// OLD rendering code, crufty and idk

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
		}

		private void SelectFont(ref Font font)
		{
			if (font == null)
				font = Font.FromFamily("Sans", new Measurement(10, MeasurementUnit.Point));

			if (font == SystemFonts.Monospace)
			{
				font = Font.FromFamily("Monospace", new Measurement(10, MeasurementUnit.Point));
			}

			Internal.Cairo.Methods.cairo_select_font_face(mvarCairoContext, font.FamilyName, (font.Italic ? Internal.Cairo.Constants.CairoFontSlant.Italic : Internal.Cairo.Constants.CairoFontSlant.Normal), (font.Weight == 800 ? Internal.Cairo.Constants.CairoFontWeight.Bold : Internal.Cairo.Constants.CairoFontWeight.Normal));
			CheckStatus();

			if (font.Size != null)
			{
				double fsz = font.Size.Value;
				if (false) // Application.DpiAwareness == DpiAwareness.Default && System.Environment.OSVersion.Platform == PlatformID.Unix)
				{
					fsz *= Screen.Default.PrimaryMonitor.ScaleFactor;
				}
				Internal.Cairo.Methods.cairo_set_font_size(mvarCairoContext, fsz);
			}
			CheckStatus();
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
			IntPtr hPattern = IntPtr.Zero;
			if (brush is SolidBrush)
			{
				SolidBrush sb = (brush as SolidBrush);
				hPattern = Internal.Cairo.Methods.cairo_pattern_create_rgba(sb.Color.R, sb.Color.G, sb.Color.B, sb.Color.A);
				CheckStatus();
			}
			else if (brush is TextureBrush)
			{
				TextureBrush tb = (brush as TextureBrush);
				if (tb.Image == null)
				{
					throw new NullReferenceException();
				}

				PatternIndices++;
				hPattern = Internal.Cairo.Methods.cairo_pattern_create_raster_source(new IntPtr(PatternIndices), Internal.Cairo.Constants.CairoContent.Both, tb.Image.Width, tb.Image.Height);
				CheckStatus();

				PatternTextures[PatternIndices] = tb;

				Internal.Cairo.Methods.cairo_raster_source_pattern_set_acquire(hPattern, hPattern_Acquire, hPattern_Release);
				CheckStatus();
			}
			else if (brush is LinearGradientBrush)
			{
				LinearGradientBrush lgb = (brush as LinearGradientBrush);
				hPattern = Internal.Cairo.Methods.cairo_pattern_create_linear(lgb.Bounds.X, lgb.Bounds.Y, lgb.Bounds.Right, lgb.Bounds.Bottom);
				CheckStatus();

				foreach (LinearGradientBrushColorStop colorStop in lgb.ColorStops)
				{
					Internal.Cairo.Methods.cairo_pattern_add_color_stop_rgba(hPattern, colorStop.Position.GetValue(MeasurementUnit.Percentage), colorStop.Color.R, colorStop.Color.G, colorStop.Color.B, colorStop.Color.A);
					CheckStatus();
				}
			}

			if (hPattern == IntPtr.Zero)
			{
				throw new NotImplementedException("Brush type " + brush.GetType().FullName);
			}
			else
			{
				Internal.Cairo.Methods.cairo_set_source(mvarCairoContext, hPattern);
				CheckStatus();
			}
		}

		private int PatternIndices = 0;
		private static Dictionary<int, TextureBrush> PatternTextures = new Dictionary<int, TextureBrush>();

		private static IntPtr hPattern_Acquire(IntPtr /*cairo_pattern_t*/ pattern, IntPtr /*void*/ callback_data, IntPtr /*cairo_surface_t*/ target,  Internal.Cairo.Structures.cairo_rectangle_int_t extents)
		{
			TextureBrush tb = PatternTextures[callback_data.ToInt32()];
			IntPtr hImage = (tb.Image as CairoImage).Handle;

			return hImage;
		}
		private static void hPattern_Release(IntPtr /*cairo_pattern_t*/ pattern, IntPtr /*void*/ callback_data, IntPtr /*cairo_surface_t*/ target)
		{
		}

		private void SelectPen(Pen pen)
		{
			IntPtr hPattern = Internal.Cairo.Methods.cairo_pattern_create_rgba(pen.Color.R, pen.Color.G, pen.Color.B, pen.Color.A);

			DashStyle dashStyle = pen.DashStyle;
			if (dashStyle == null)
				dashStyle = DashStyles.Solid;

			Internal.Cairo.Methods.cairo_set_dash(mvarCairoContext, dashStyle.Dashes, dashStyle.Dashes.Length, 0);

			Internal.Cairo.Methods.cairo_set_line_cap(mvarCairoContext, LineCapStylesToCairoLineCap(pen.LineCapStyle));
			Internal.Cairo.Methods.cairo_set_source(mvarCairoContext, hPattern);
			Internal.Cairo.Methods.cairo_set_line_width(mvarCairoContext, pen.Width.GetValue(MeasurementUnit.Pixel));

			CheckStatus();
		}

		private static Constants.CairoLineCap LineCapStylesToCairoLineCap(LineCapStyles lineCapStyle)
		{
			switch (lineCapStyle)
			{
				case LineCapStyles.Flat: return Constants.CairoLineCap.Butt;
				case LineCapStyles.Round: return Constants.CairoLineCap.Round;
				case LineCapStyles.Square: return Constants.CairoLineCap.Square;
			}
			return Constants.CairoLineCap.Butt;
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

		private static IntPtr _hStyleContext = IntPtr.Zero;
		protected override void DrawFocusInternal(double x, double y, double width, double height, Control styleReference)
		{
			IntPtr hStyleContext = _hStyleContext;
			if (_hStyleContext == IntPtr.Zero)
			{
				IntPtr hTextBox = Internal.GTK.Methods.GtkTreeView.gtk_tree_view_new();
				_hStyleContext = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(hTextBox);
				hStyleContext = _hStyleContext;
			}

			if (styleReference != null)
			{
				if (!styleReference.IsCreated)
				{
					((Application.Instance as UIApplication).Engine as GTK3Engine).CreateControl(styleReference);
				}
				GTKNativeControl handle = ((Application.Instance as UIApplication).Engine as GTK3Engine).GetHandleForControl(styleReference) as GTKNativeControl;
				if (handle != null)
				{
					hStyleContext = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(handle.Handle);
				}
			}
			Internal.GTK.Methods.Gtk.gtk_render_focus(hStyleContext, mvarCairoContext, x, y, width, height);
		}
	}
}
