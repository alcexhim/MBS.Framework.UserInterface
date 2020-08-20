//
//  GDKScreen.cs
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
namespace MBS.Framework.UserInterface.Engines.GTK
{
	public class GDKScreen : Screen
	{
		public IntPtr Handle { get; private set; } = IntPtr.Zero;

		protected override int GetMonitorCountInternal()
		{
			return Internal.GDK.Methods.gdk_screen_get_n_monitors(Handle);
		}

		protected override Monitor GetPrimaryMonitorInternal()
		{
			IntPtr hScreenDefault = (Screen.Default as GDKScreen).Handle;
			IntPtr hDisplay = Internal.GDK.Methods.gdk_screen_get_display(hScreenDefault);

			int nMonitorDefault = Internal.GDK.Methods.gdk_screen_get_primary_monitor(hScreenDefault);
			IntPtr hMonitor = Internal.GDK.Methods.gdk_display_get_monitor(hDisplay, nMonitorDefault);

			return new GDKMonitor(hMonitor);
		}

		protected override double GetDpiInternal()
		{
			return Internal.GDK.Methods.gdk_screen_get_resolution(Handle);
		}

		internal GDKScreen(IntPtr handle)
		{
			Handle = handle;
		}
	}
}
