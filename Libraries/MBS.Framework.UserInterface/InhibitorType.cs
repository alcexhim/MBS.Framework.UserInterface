//
//  InhibitorType.cs - defines various types of inhibitors that can be registered
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

namespace MBS.Framework.UserInterface
{
	/// <summary>
	/// Types of user actions that may be blocked by adding an
	/// <see cref="Inhibitor" /> to <see cref="UIApplication.Inhibitors" />.
	/// </summary>
	/// <remarks>
	/// Certain operating systems may not support all inhibitor types.
	/// </remarks>
	[Flags()]
	public enum InhibitorType
	{
		None = 0,
		/// <summary>
		/// Inhibit the session being marked as idle (and possibly locked)
		/// </summary>
		SystemIdle,
		/// <summary>
		/// Inhibit ending the user session by logging out or by shutting down the computer
		/// </summary>
		SystemLogout,
		/// <summary>
		/// Inhibit suspending the session or computer
		/// </summary>
		SystemSuspend,
		/// <summary>
		/// Inhibit user switching
		/// </summary>
		SystemUserSwitch
	}
}
