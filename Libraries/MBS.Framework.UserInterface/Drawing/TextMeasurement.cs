//
//  TextMeasurement.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2022 Mike Becker's Software
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

namespace MBS.Framework.UserInterface.Drawing
{
	public class TextMeasurement
	{
		public Dimension2D Size { get; private set; } = Dimension2D.Empty;
		public Vector2D Bearing { get; private set; } = Vector2D.Empty;
		public Vector2D Advance { get; private set; } = Vector2D.Empty;

		public TextMeasurement(double width, double height, double xBearing, double yBearing, double xAdvance, double yAdvance)
		{
			Size = new Dimension2D(width, height);
			Bearing = new Vector2D(xBearing, yBearing);
			Advance = new Vector2D(xAdvance, yAdvance);
		}
	}
}
