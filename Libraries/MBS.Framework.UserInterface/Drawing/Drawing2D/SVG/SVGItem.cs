//
//  SVGItem.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2021 Mike Becker's Software
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

namespace MBS.Framework.UserInterface.Drawing.Drawing2D.SVG
{
	public abstract class SVGItem
	{
		public class SVGItemCollection
			: System.Collections.ObjectModel.Collection<SVGItem>
		{

		}

		public SVGItem()
		{
		}

		public SVGStyle Style { get; set; } = null;

		/// <summary>
		/// Creates a <see cref="Pen" /> containing the stroke styles specifed in <see cref="Style" />.
		/// </summary>
		/// <returns>The pen created from the <see cref="Style" />.</returns>
		protected Pen StyleToPen()
		{
			if (Style == null)
				return Pens.Black;

			Color strokeColor = Style.Properties["stroke"].Value.Parse<Color>();
			Measurement strokeWidth = Style.Properties["stroke-width"].Value.Parse<Measurement>();
			// SVGStrokeLineCap strokeLineCap = Style.Properties["stroke-linecap"].Value.Parse<SVGStrokeLineCap>();
			// SVGStrokeLineJoin strokeLineJoin = Style.Properties["stroke-linejoin"].Value.Parse<SVGStrokeLineJoin>();
			double strokeOpacity = Style.Properties["stroke-opacity"].Value.Parse<double>();

			return new Pen(strokeColor.Alpha(strokeOpacity), strokeWidth, PenStyle.Solid);
		}

		protected abstract void RenderInternal(Graphics graphics);
		public void Render(Graphics graphics)
		{
			RenderInternal(graphics);
		}
	}
}
