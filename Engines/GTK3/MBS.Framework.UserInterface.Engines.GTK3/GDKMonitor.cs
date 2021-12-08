//
//  GDKMonitor.cs
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

namespace MBS.Framework.UserInterface.Engines.GTK3
{
	public class GDKMonitor : Monitor
	{
		public GDKMonitor(IntPtr handle)
		{
			Handle = handle;
		}
		public IntPtr Handle { get; private set; }

		protected override string GetDeviceNameInternal()
		{
			return null;
		}

		protected override double GetScaleFactorInternal()
		{
			// FIXME: gdk_monitor_get_scale_factor returns an int ; how can we handle fractional scaling?
			return (double) Internal.GDK.Methods.gdk_monitor_get_scale_factor(Handle);
		}

		protected override Rectangle GetBoundsInternal()
		{
			Internal.GDK.Structures.GdkRectangle geom = new Internal.GDK.Structures.GdkRectangle();
			Internal.GDK.Methods.gdk_monitor_get_geometry(Handle, ref geom);
			return new Rectangle(geom.x, geom.y, geom.width, geom.height);
		}

		protected override Rectangle GetWorkingAreaInternal()
		{
			Internal.GDK.Structures.GdkRectangle workarea = new Internal.GDK.Structures.GdkRectangle();
			Internal.GDK.Methods.gdk_monitor_get_workarea(Handle, ref workarea);

			return new Rectangle(workarea.x, workarea.y, workarea.width, workarea.height);
		}
	}
}
