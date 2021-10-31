//
//  SVGItemLine.cs
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

namespace MBS.Framework.UserInterface.Drawing.Drawing2D.SVG.Items
{
	public class SVGItemLine : SVGItem
	{
		public Measurement X1 { get; set; } = Measurement.Empty;
		public Measurement Y1 { get; set; } = Measurement.Empty;
		public Measurement X2 { get; set; } = Measurement.Empty;
		public Measurement Y2 { get; set; } = Measurement.Empty;

		protected override void RenderInternal(Graphics graphics)
		{
			graphics.DrawLine(StyleToPen(), X1.GetValue(MeasurementUnit.Pixel), Y1.GetValue(MeasurementUnit.Pixel), X2.GetValue(MeasurementUnit.Pixel), Y2.GetValue(MeasurementUnit.Pixel));
		}
	}
}
