//
//  WindowsFormsTimerImplementation.cs
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
namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public class WindowsFormsTimerImplementation : TimerImplementation
	{
		private System.Windows.Forms.Timer _timer = null;
		public WindowsFormsTimerImplementation(Timer timer) : base(timer)
		{
			_timer = new System.Windows.Forms.Timer();
			_timer.Tick += _timer_Tick;
		}

		void _timer_Tick(object sender, EventArgs e)
		{
			OnTick();
		}


		protected override void StartInternal()
		{
			_timer.Start();
		}
		protected override void StopInternal()
		{
			_timer.Stop();
		}
	}
}
