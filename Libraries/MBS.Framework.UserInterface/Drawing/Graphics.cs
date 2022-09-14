using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing.Drawing2D.SVG;

namespace MBS.Framework.UserInterface.Drawing
{
	public abstract class Graphics
	{
		public Rectangle ClipRectangle { get; private set; }

		protected abstract TextMeasurement MeasureTextInternal(string text, Font font);
		public TextMeasurement MeasureText(string text, Font font)
		{
			return MeasureTextInternal(text, font);
		}

		public Graphics()
		{
		}
		public Graphics(Rectangle clipRectangle)
		{
			ClipRectangle = clipRectangle;
		}

		public static Graphics FromImage(Image image)
		{
			return ((UIApplication)Application.Instance).Engine.CreateGraphics(image);
		}

		private void DrawSVGImage(SVGImage image)
		{
			foreach (SVGItem item in image.Items)
			{
				item.Render(this);
			}
		}

		protected abstract void DrawImageInternal(Image image, double x, double y, double width, double height);
		public void DrawImage(Image image, double x, double y)
		{
			DrawImage(image, x, y, image.Width, image.Height);
		}
		public void DrawImage(Image image, double x, double y, double width, double height)
		{
			DpiScale(ref x, ref y, ref width, ref height);

			if (image is SVGImage)
			{
				DrawSVGImage((SVGImage)image);
				return;
			}
			DrawImageInternal(image, x, y, width, height);
		}

		protected abstract void DrawLineInternal(Pen pen, double x1, double y1, double x2, double y2);
		public void DrawLine(Pen pen, double x1, double y1, double x2, double y2)
		{
			DpiScale(ref x1, ref x2, ref y1, ref y2);
			DrawLineInternal(pen, x1, y1, x2, y2);
		}

		private Vector2D DpiScale(Vector2D point)
		{
			if (((UIApplication)Application.Instance).ShouldDpiScale)
			{
				double sf = Screen.Default.PrimaryMonitor.ScaleFactor;
				return new Vector2D(point.X * sf, point.Y * sf);
			}
			return point;
		}
		private Rectangle DpiScale(Rectangle rect)
		{
			if (((UIApplication)Application.Instance).ShouldDpiScale)
			{
				double sf = Screen.Default.PrimaryMonitor.ScaleFactor;
				return new Rectangle(rect.X * sf, rect.Y * sf, rect.Width * sf, rect.Height * sf);
			}
			return rect;
		}
		private void DpiScale(ref double x, ref double y, ref double w, ref double h)
		{
			if (((UIApplication)Application.Instance).ShouldDpiScale)
			{
				double sf = Screen.Default.PrimaryMonitor.ScaleFactor;
				x *= sf;
				y *= sf;
				w *= sf;
				h *= sf;
			}
		}

		protected abstract void DrawFocusInternal(double x, double y, double width, double height, Control styleReference);
		/// <summary>
		/// Renders a focus indicator on the rectangle determined by <paramref name="x" />, <paramref name="y" />,
		/// <paramref name="width" />, <paramref name="height" />.
		/// </summary>
		/// <param name="x">X origin of the rectangle.</param>
		/// <param name="y">Y origin of the rectangle.</param>
		/// <param name="width">Rectangle width.</param>
		/// <param name="height">Rectangle height.</param>
		/// <param name="styleReference">The control used as a reference for the focus rectangle style. For example,
		/// one could pass a TextBox to get a text box style focus rectangle, which looks different on certain themes.</param>
		public void DrawFocus(double x, double y, double width, double height, Control styleReference = null)
		{
			DrawFocusInternal(x, y, width, height, styleReference);
		}

		protected abstract void DrawRectangleInternal(Pen pen, double x, double y, double width, double height);
		public void DrawRectangle(Pen pen, double x, double y, double width, double height)
		{
			Rectangle rect2 = NormalizeRectangle(new Rectangle(x, y, width, height));
			rect2 = DpiScale(rect2);
			DrawRectangleInternal(pen, rect2.X, rect2.Y, rect2.Width, rect2.Height);
		}

		protected abstract void DrawPolygonInternal(Pen pen, Vector2D[] points);
		public void DrawPolygon(Pen pen, Vector2D[] points)
		{
			DrawPolygonInternal(pen, points);
		}

		protected abstract void FillPolygonInternal(Brush brush, Vector2D[] points);
		public void FillPolygon(Brush brush, Vector2D[] points)
		{
			FillPolygonInternal(brush, points);
		}

		public void DrawRectangle(Pen pen, Rectangle rect)
		{
			DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
		}

		protected abstract void FillRectangleInternal(Brush brush, double x, double y, double width, double height);
		public void FillRectangle(Brush brush, double x, double y, double width, double height)
		{
			Rectangle rect2 = NormalizeRectangle(new Rectangle(x, y, width, height));
			rect2 = DpiScale(rect2);
			FillRectangleInternal(brush, rect2.X, rect2.Y, rect2.Width, rect2.Height);
		}
		public void FillRectangle(Brush brush, Rectangle rect)
		{
			FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}

		protected abstract void RotateInternal(double angle);
		private double _Rotation = 0.0;
		public double Rotation { get { return _Rotation; } set { _Rotation = value; RotateInternal(value); } }

		public Vector2D RotationCenterpoint { get; set; } = Vector2D.Empty;

		private Rectangle NormalizeRectangle(Rectangle input)
		{
			double x = input.X;
			double y = input.Y;
			double width = input.Width;
			double height = input.Height;

			if (width < 0)
			{
				x = x + width;
				width = -width;
			}
			if (height < 0)
			{
				y = y + height;
				height = -height;
			}
			return new Rectangle(x, y, width, height);
		}

		public void Clear(Color color)
		{
			if (ClipRectangle != null)
			{
				FillRectangle(new SolidBrush(color), new Rectangle(0, 0, ClipRectangle.Width, ClipRectangle.Height));
			}
		}


		protected abstract void DrawTextInternal(string value, Font font, Vector2D location, Dimension2D size, Brush brush, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment);
		public void DrawText(string value, Font font, Vector2D location, Brush brush, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
		{
			location = DpiScale(location);
			DrawTextInternal(value, font, location, null, brush, horizontalAlignment, verticalAlignment);
		}
		public void DrawText(string value, Font font, Rectangle rectangle, Brush brush, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
		{
			rectangle = DpiScale(rectangle);
			DrawTextInternal(value, font, rectangle.Location, rectangle.Size, brush, horizontalAlignment, verticalAlignment);
		}
	}
}
