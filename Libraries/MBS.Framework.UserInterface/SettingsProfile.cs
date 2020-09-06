//
//  SettingsProfile.cs
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
namespace MBS.Framework.UserInterface
{
	public class SettingsProfile
	{
		public class SettingsProfileCollection
			: System.Collections.ObjectModel.Collection<SettingsProfile>
		{

		}

		public static readonly Guid AllUsersGUID = new Guid("{6c1e84c6-7cb8-4798-b000-349dba816114}");
		public static readonly Guid ThisUserGUID = new Guid("{a550229d-05e1-4a93-96a6-98ae1c69b847}");

		public Guid ID { get; set; } = Guid.Empty;
		public string Title { get; set; } = null;
	}
}
