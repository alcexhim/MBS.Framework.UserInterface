//
//  WindowsFormsMonitor.cs
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
using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public class WindowsFormsMonitor : Monitor
	{
		public System.Windows.Forms.Screen Handle { get; private set; }
		public WindowsFormsMonitor(System.Windows.Forms.Screen handle)
		{
			Handle = handle;
		}

		protected override double GetScaleFactorInternal()
		{
			// FIXME: how to get scale factor of current monitor on Windows?
			return 1.0;
		}

		protected override Rectangle GetBoundsInternal() => new Rectangle(Handle.Bounds.X, Handle.Bounds.Y, Handle.Bounds.Width, Handle.Bounds.Height);
		protected override string GetDeviceNameInternal() => Handle.DeviceName;
		protected override Rectangle GetWorkingAreaInternal() => new Rectangle(Handle.WorkingArea.X, Handle.WorkingArea.Y, Handle.WorkingArea.Width, Handle.WorkingArea.Height);
	}
}
