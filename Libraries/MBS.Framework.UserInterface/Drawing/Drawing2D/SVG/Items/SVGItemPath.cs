//
//  SVGItemPath.cs
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
using MBS.Framework.Collections.Generic;

namespace MBS.Framework.UserInterface.Drawing.Drawing2D.SVG.Items
{
	public class SVGItemPath : SVGItem
	{
		public SVGPoint.SVGPointCollection Points { get; } = new SVGPoint.SVGPointCollection();

		public SVGItemPath(SVGPoint[] points)
		{
			Points.AddRange(points);
		}

		protected override void RenderInternal(Graphics graphics)
		{
			MBS.Framework.Drawing.Vector2D[] points = new Framework.Drawing.Vector2D[Points.Count];
			for (int i = 0; i < points.Length; i++)
			{
				points[i] = new Framework.Drawing.Vector2D(Points[i].X.GetValue(Framework.Drawing.MeasurementUnit.Pixel), Points[i].Y.GetValue(Framework.Drawing.MeasurementUnit.Pixel));
			}
			graphics.DrawPolygon(Pens.Black, points);
		}
	}
}
