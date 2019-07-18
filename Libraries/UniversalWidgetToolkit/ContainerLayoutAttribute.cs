﻿//
//  WindowLayoutAttribute.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 
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
namespace UniversalWidgetToolkit
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ContainerLayoutAttribute : Attribute
	{
		public string PathName { get; private set; } = String.Empty;
		public string ClassName { get; private set; } = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UniversalWidgetToolkit.WindowLayoutAttribute"/> class.
		/// </summary>
		/// <param name="pathName">Path name.</param>
		public ContainerLayoutAttribute(string pathName, string className = null)
		{
			PathName = pathName;
			ClassName = className;
		}
	}
}
