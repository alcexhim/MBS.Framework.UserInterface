//
//  Screen.cs
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
	public abstract class Screen
	{
		private static Screen _Default = null;
		public static Screen Default
		{
			get
			{
				if (_Default == null)
				{
					_Default = ((UIApplication)Application.Instance).Engine.GetDefaultScreen();
				}
				return _Default;
			}
		}

		protected abstract int GetMonitorCountInternal();
		internal int GetMonitorCount() { return GetMonitorCountInternal(); }

		public Monitor.MonitorCollection Monitors { get; private set; } = null;

		protected abstract Monitor GetPrimaryMonitorInternal();

		protected abstract double GetDpiInternal();
		public double Dpi => GetDpiInternal();

		private Monitor _PrimaryMonitor = null;
		public Monitor PrimaryMonitor
		{
			get
			{
				if (_PrimaryMonitor == null)
					_PrimaryMonitor = GetPrimaryMonitorInternal();
				return _PrimaryMonitor;
			}
		}
	}
}
