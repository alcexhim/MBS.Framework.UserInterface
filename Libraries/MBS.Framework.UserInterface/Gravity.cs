//
//  Gravity.cs
//
//  Author:
//       beckermj <>
//
//  Copyright (c) 2023 ${CopyrightHolder}
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
namespace MBS.Framework.UserInterface
{
	public enum Gravity
	{
		/// <summary>
		/// the reference point is at the top left corner.
		/// </summary>
		TopLeft,
		/// <summary>
		/// the reference point is in the middle of the top edge.
		/// </summary>
		TopCenter,
		/// <summary>
		/// the reference point is at the top right corner.
		/// </summary>
		TopRight,
		/// <summary>
		/// the reference point is at the middle of the left edge.
		/// </summary>
		CenterLeft,
		/// <summary>
		/// the reference point is at the center of the window.
		/// </summary>
		Center,
		/// <summary>
		/// the reference point is at the middle of the right edge.
		/// </summary>
		CenterRight,
		/// <summary>
		/// the reference point is at the lower left corner.
		/// </summary>
		BottomLeft,
		/// <summary>
		/// the reference point is at the middle of the lower edge.
		/// </summary>
		BottomCenter,
		/// <summary>
		/// the reference point is at the lower right corner.
		/// </summary>
		BottomRight,
		/// <summary>
		/// the reference point is at the top left corner of the window itself, ignoring window manager decorations.
		/// </summary>
		Static
	}
}
