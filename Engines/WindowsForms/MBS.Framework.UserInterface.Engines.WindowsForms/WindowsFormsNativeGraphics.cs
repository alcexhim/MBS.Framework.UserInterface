//
//  WindowsFormsNativeGraphics.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public class WindowsFormsNativeGraphics : Graphics
	{
		private const string FONTFAMILY_MONOSPACE = "Courier New";

		public System.Drawing.Graphics Handle { get; private set; } = null;
		public WindowsFormsNativeGraphics(System.Drawing.Graphics g)
		{
			Handle = g;
		}
		public WindowsFormsNativeGraphics(System.Drawing.Graphics g, Rectangle clipRectangle) : base(clipRectangle)
		{
			Handle = g;
		}

		private System.Drawing.Image ImageToNativeImage(Image image)
		{
			return (image as WindowsFormsNativeImage).Handle;
		}
		private System.Drawing.Pen PenToNativePen(Pen pen)
		{
			return new System.Drawing.Pen(ColorToNativeColor(pen.Color), (float) pen.Width.GetValue(MeasurementUnit.Pixel));
		}

		protected override void DrawPolygonInternal(Pen pen, Vector2D[] points)
		{
			Handle.DrawPolygon(PenToNativePen(pen), Vector2DToNativePointF(points));
		}
		protected override void FillPolygonInternal(Brush brush, Vector2D[] points)
		{
			Handle.FillPolygon(BrushToNativeBrush(brush), Vector2DToNativePointF(points));
		}

		protected override void DrawImageInternal(Image image, double x, double y, double width, double height)
		{
			Handle.DrawImage(ImageToNativeImage(image), new System.Drawing.Rectangle((int)x, (int)y, (int)width, (int)height));
		}

		protected override void DrawLineInternal(Pen pen, double x1, double y1, double x2, double y2)
		{
			Handle.DrawLine(PenToNativePen(pen), (float)x1, (float)y1, (float)x2, (float)y2);
		}

		protected override void DrawRectangleInternal(Pen pen, double x, double y, double width, double height)
		{
			Handle.DrawRectangle(PenToNativePen(pen), new System.Drawing.Rectangle((int)x, (int)y, (int)width, (int)height));
		}

		protected override void FillRectangleInternal(Brush brush, double x, double y, double width, double height)
		{
			Handle.FillRectangle(BrushToNativeBrush(brush), new System.Drawing.Rectangle((int)x, (int)y, (int)width, (int)height));
		}

		private System.Drawing.StringAlignment AlignmentToStringAlignment(HorizontalAlignment alignment)
		{
			switch (alignment)
			{
				case HorizontalAlignment.Left: return System.Drawing.StringAlignment.Near;
				case HorizontalAlignment.Center: return System.Drawing.StringAlignment.Center;
				case HorizontalAlignment.Right: return System.Drawing.StringAlignment.Far;
			}
			return System.Drawing.StringAlignment.Near;
		}
		private System.Drawing.StringAlignment AlignmentToStringAlignment(VerticalAlignment alignment)
		{
			switch (alignment)
			{
				case VerticalAlignment.Top: return System.Drawing.StringAlignment.Near;
				case VerticalAlignment.Middle: return System.Drawing.StringAlignment.Center;
				case VerticalAlignment.Bottom: return System.Drawing.StringAlignment.Far;
			}
			return System.Drawing.StringAlignment.Near;
		}

		private System.Drawing.Color ColorToNativeColor(Color color)
		{
			return System.Drawing.Color.FromArgb(color.GetAlphaByte(), color.GetRedByte(), color.GetGreenByte(), color.GetBlueByte());
		}
		private System.Drawing.Font FontToNativeFont(Font font)
		{
			if (font == null) return System.Drawing.SystemFonts.DialogFont;
			if (font == SystemFonts.Monospace)
			{
				return new System.Drawing.Font(FONTFAMILY_MONOSPACE, 10.0f);
			}
			return new System.Drawing.Font(font.FamilyName, (float)font.Size);
		}
		private System.Drawing.Brush BrushToNativeBrush(Brush brush)
		{
			if (brush is SolidBrush)
			{
				return new System.Drawing.SolidBrush(ColorToNativeColor((brush as SolidBrush).Color));
			}
			return null;
		}

		private System.Drawing.PointF Vector2DToNativePointF(Vector2D point)
		{
			return new System.Drawing.PointF((float)point.X, (float)point.Y);
		}
		private System.Drawing.PointF[] Vector2DToNativePointF(Vector2D[] points)
		{
			System.Drawing.PointF[] ptf = new System.Drawing.PointF[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				ptf[i] = new System.Drawing.PointF((float)points[i].X, (float)points[i].Y);
			}
			return ptf;
		}

		protected override void DrawTextInternal(string value, Font font, Vector2D location, Dimension2D size, Brush brush, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
		{
			System.Drawing.StringFormat format = new System.Drawing.StringFormat();
			format.Alignment = AlignmentToStringAlignment(horizontalAlignment);
			format.LineAlignment = AlignmentToStringAlignment(verticalAlignment);
			// Console.WriteLine("coordinate rect : {0}", rectangle);

			if (size != null)
			{
				System.Drawing.RectangleF rect = new System.Drawing.RectangleF((float)location.X, (float)location.Y, (float)size.Width, (float)size.Height);
				System.Drawing.PointF pt = new System.Drawing.PointF((float)location.X, (float)location.Y);
				pt.Y -= 12; // I don't even know
				pt.X -= 2;
				Handle.DrawString(value, FontToNativeFont(font), BrushToNativeBrush(brush), pt, format);
			}
			else
			{
				System.Windows.Forms.TextRenderer.DrawText(Handle, value, FontToNativeFont(font), new System.Drawing.Point((int)location.X, (int)location.Y), (BrushToNativeBrush(brush) as System.Drawing.SolidBrush).Color, System.Windows.Forms.TextFormatFlags.Default);
			}
		}

		protected override void DrawFocusInternal(double x, double y, double width, double height, Control styleReference)
		{
			// FIXME: this does not apply to certain controls (e.g. text boxes) which draw their focus differently than buttons, etc.
			System.Windows.Forms.ControlPaint.DrawFocusRectangle(Handle, new System.Drawing.Rectangle((int)x, (int)y, (int)width, (int)height));
		}

		protected override void RotateInternal(double angle)
		{
			throw new NotImplementedException();
		}
	}
}
