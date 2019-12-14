//
//  Constants.cs
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
namespace MBS.Framework.UserInterface.Engines.WindowsForms.Internal.Windows
{
	internal static class Constants
	{
		public enum ShowWindowCommand
		{
			/// <summary>
			/// Minimizes a window, even if the thread that owns the window is not responding. This flag should only be
			/// used when minimizing windows from a different thread.
			/// </summary>
			ForceMinimize = 11,
			/// <summary>
			/// Hides the window and activates another window.
			/// </summary>
			Hide = 0,
			/// <summary>
			/// Maximizes the specified window.
			/// </summary>
			Maximize = 3,
			/// <summary>
			/// Minimizes the specified window and activates the next top-level window in the Z order.
			/// </summary>
			Minimize = 6,
			/// <summary>
			/// Activates and displays the window. If the window is minimized or maximized, the system restores it to its
			/// original size and position. An application should specify this flag when restoring a minimized window.
			/// </summary>
			Restore = 9,
			/// <summary>
			/// Activates the window and displays it in its current size and position.
			/// </summary>
			Show = 5,
			/// <summary>
			/// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the
			/// CreateProcess function by the program that started the application.
			/// </summary>
			ShowDefault = 10,
			/// <summary>
			/// Activates the window and displays it as a maximized window.
			/// </summary>
			ShowMaximized = 3,
			/// <summary>
			/// Activates the window and displays it as a minimized window.
			/// </summary>
			ShowMinimized = 2,
			/// <summary>
			/// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window
			/// is not activated.
			/// </summary>
			ShowMInimizedNoActive = 7,
			/// <summary>
			/// Displays the window in its current size and position. This value is similar to SW_SHOW, except that the
			/// window is not activated.
			/// </summary>
			ShowNoActivate = 8,
			/// <summary>
			/// Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except
			/// that the window is not activated.
			/// </summary>
			ShowNormalNoActivate = 4,
			/// <summary>
			/// Activates and displays a window. If the window is minimized or maximized, the system restores it to its
			/// original size and position. An application should specify this flag when displaying the window for the
			/// first time. 
			/// </summary>
			ShowNormal = 1
		}
	}
}
