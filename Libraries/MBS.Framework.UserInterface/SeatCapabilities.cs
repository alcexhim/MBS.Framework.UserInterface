//
//  SeatCapabilities.cs
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
	public enum SeatCapabilities
	{
		/// <summary>
		/// No input capabilities
		/// </summary>
		None = 0,
		/// <summary>
		/// The seat has a pointer (e.g.mouse)
		/// </summary>
		Pointer,
		/// <summary>
		/// The seat has touchscreen(s) attached
		/// </summary>
		Touch,
		/// <summary>
		/// The seat has drawing tablet(s) attached
		/// </summary>
		TabletStylus,
		/// <summary>
		/// The seat has keyboard(s) attached
		/// </summary>
		Keyboard,
		/// <summary>
		/// The union of all pointing capabilities
		/// </summary>
		AllPointing = Pointer | Touch | TabletStylus,
		/// <summary>
		/// The union of all capabilities
		/// </summary>
		All = AllPointing | Keyboard
	}
}
