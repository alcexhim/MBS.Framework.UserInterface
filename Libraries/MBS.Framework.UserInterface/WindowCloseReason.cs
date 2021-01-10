//
//  WindowCloseReason.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker
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
	public enum WindowCloseReason
	{
		Unknown = -1,

		// definitions from System.Windows.Forms.CloseReason
		/// <summary>
		/// The window is being closed due to a call to <see cref="UIApplication.Stop(int)" />.
		/// </summary>
		ApplicationStop = 6,
		/// <summary>
		/// The window is being closed because its owner window is closing.
		/// </summary>
		OwnerClosing = 5,
		/// <summary>
		/// The window is being closed because its MDI parent window is closing.
		/// </summary>
		MdiParentClosing = 2,
		None = 0,
		/// <summary>
		/// The window is being closed forcibly (e.g. due to a Task Manager or kill(1) call).
		/// </summary>
		ForcedClosing = 4,
		/// <summary>
		/// The window is being closed due to an explicit user action.
		/// </summary>
		UserClosing = 3,
		/// <summary>
		/// The window is being closed due to the operating system shutting down.
		/// </summary>
		SystemShutdown = 1
	}
}
