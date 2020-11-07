//
//  AppIndicatorStatus.cs
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
	/// These are the states that the indicator can be on in the user's panel. The indicator by default starts
	/// in the state @APP_INDICATOR_STATUS_PASSIVE and can be shown by setting it to @APP_INDICATOR_STATUS_ACTIVE.
	/// </summary>
	public enum AppIndicatorStatus
	{
		/// <summary>
		/// The indicator should not be shown to the user.
		/// </summary>
		Passive,
		/// <summary>
		/// The indicator should be shown in it's default state.
		/// </summary>
		Active,
		/// <summary>
		/// The indicator should show it's attention icon.
		/// </summary>
		Attention
	}
}
