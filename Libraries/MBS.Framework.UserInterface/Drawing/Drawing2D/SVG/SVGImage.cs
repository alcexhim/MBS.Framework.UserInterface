using System;
namespace MBS.Framework.UserInterface.Drawing.Drawing2D.SVG
{
	public class SVGImage : Image
	{
		public SVGItem.SVGItemCollection Items { get; } = new SVGItem.SVGItemCollection();
	}
}
