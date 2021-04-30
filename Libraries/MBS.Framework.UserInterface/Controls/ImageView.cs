using System;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Controls
{
	public enum ImageViewDisplayMode
	{
		Center,
		Stretch,
		Zoom
	}
	public class ImageView : CustomControl
	{

		public ImageView()
		{
		}

		private double _ZoomFactor = 1.0;
		public double ZoomFactor { get { return _ZoomFactor; } set { _ZoomFactor = value; Refresh(); } }

		private double _OffsetX = 0.0;
		public double OffsetX { get { return _OffsetX; } set { _OffsetX = value; Refresh(); } }
		private double _OffsetY = 0.0;
		public double OffsetY { get { return _OffsetY; } set { _OffsetY = value; Refresh(); } }

		public ImageViewDisplayMode DisplayMode { get; set; } = ImageViewDisplayMode.Center;

		private Image _Image = null;
		public Image Image { get { return _Image; } set { _Image = value; Refresh(); } }

		public bool AutoSize { get; set; } = true;

		public override Dimension2D Size
		{
			get
			{
				if (AutoSize && Image != null)
				{
					return Image.Size;
				}
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}

		protected internal override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (BackgroundBrush != null)
			{
				e.Graphics.FillRectangle(BackgroundBrush, new MBS.Framework.Drawing.Rectangle(0, 0, Size.Width, Size.Height));
			}

			if (AutoSize)
			{
				Size = Image.Size;
				ScrollBounds = Size;
			}

			if (Image == null)
				return;

			switch (DisplayMode)
			{
				case ImageViewDisplayMode.Center:
				{
					double x = (Size.Width - _Image.Width) / 2;
					double y = (Size.Height - _Image.Height) / 2;

					e.Graphics.DrawImage(Image, x, y);
					break;
				}
				case ImageViewDisplayMode.Zoom:
				{
					double w = Image.Width;
					double h = Image.Height;

					// Scale
					double width_ratio = Size.Width / Image.Width;
					double height_ratio = Size.Height / Image.Height;
					double scale_xy = Math.Min(height_ratio, width_ratio);

					if (ZoomFactor < 1) ZoomFactor = 1.0;

					w = Image.Width * scale_xy * ZoomFactor;
					h = Image.Height * scale_xy * ZoomFactor;
					double x = (Size.Width - w) / 2;
					double y = 0; // (Size.Height - h) / 2;

					x += OffsetX;
					y += OffsetY;

					e.Graphics.DrawImage(Image, x, y, w, h);
					break;
				}
				case ImageViewDisplayMode.Stretch:
				{
					e.Graphics.DrawImage(Image, 0, 0, Size.Width, Size.Height);
					break;
				}
			}
		}
	}
}
