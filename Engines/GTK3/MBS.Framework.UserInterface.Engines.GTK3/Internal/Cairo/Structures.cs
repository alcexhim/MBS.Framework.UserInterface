//
//  Structures.cs
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
namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.Cairo
{
	internal static class Structures
	{
		public struct cairo_glyph_t
		{
			public uint index;
			public double x;
			public double y;
		}
		public struct cairo_rectangle_int_t
		{
			public int x, y;
			public int width, height;
		}
		public struct cairo_text_extents_t
		{
			public double x_bearing;
			public double y_bearing;
			public double width;
			public double height;
			public double x_advance;
			public double y_advance;
		}
	}
}
