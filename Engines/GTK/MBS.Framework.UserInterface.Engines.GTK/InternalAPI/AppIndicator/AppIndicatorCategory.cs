//
//  AppIndicatorCategory.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker's Software
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
namespace MBS.Framework.UserInterface.Engines.GTK.InternalAPI.AppIndicator
{
	/// <summary>
	/// The category provides grouping for the indicators so that users can find indicators that are similar
	/// together.
	/// </summary>
	public enum AppIndicatorCategory
	{
		/// <summary>
		/// The indicator is used to display the status of the application.
		/// </summary>
		ApplicationStatus,
		/// <summary>
		/// The application is used for communication with other people.
		/// </summary>
		Communications,
		/// <summary>
		/// A system indicator relating to something in the user's system.
		/// </summary>
		SystemServices,
		/// <summary>
		/// An indicator relating to the user's hardware.
		/// </summary>
		Hardware,
		/// <summary>
		/// Something not defined in this enum, please don't use unless you really need it.
		/// </summary>
		Other
	}
}
