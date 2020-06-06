//
//  TimerImplementation.cs
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
	public class GTKTimerImplementation : TimerImplementation
	{
		[System.Runtime.InteropServices.DllImport("glib-2.0")]
		private static extern void g_timeout_add(uint interval, Func<IntPtr, bool> func, IntPtr data);

		private Func<IntPtr, bool> Timer_Callback_D = null;
		private bool Timer_Callback(IntPtr data)
		{
			OnTick();
			return Timer.Enabled;
		}

		public GTKTimerImplementation(Timer timer) : base(timer)
		{
			Timer_Callback_D = new Func<IntPtr, bool>(Timer_Callback);
		}

		protected override void StartInternal()
		{
			g_timeout_add((uint)Timer.Duration, Timer_Callback_D, IntPtr.Zero);
		}
		protected override void StopInternal()
		{
			// GTK implementation handles this by watching the Enabled property
		}
	}
}
