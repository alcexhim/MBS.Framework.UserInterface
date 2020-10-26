//
//  Timer.cs
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
using System.Collections.Generic;

namespace MBS.Framework.UserInterface
{
	/// <summary>
	/// Provides a facility for running timer operations on the GUI thread in a cross-platform manner.
	/// </summary>
	public class Timer
	{
		private bool _Enabled = false;
		public bool Enabled
		{
			get { return _Enabled; }
			set
			{
				_Enabled = value;
				if (_Enabled)
				{
					Start();
				}
				else
				{
					Stop();
				}
			}
		}

		public int Duration { get; set; } = 0;

		public event EventHandler Tick;

		internal void _OnTick()
		{
			Tick?.Invoke(this, EventArgs.Empty);
		}

		public void Start()
		{
			((UIApplication)Application.Instance).Engine.Timer_Start(this);
		}
		public void Stop()
		{
			((UIApplication)Application.Instance).Engine.Timer_Stop(this);
		}



		private static Dictionary<Timer, Action<object[]>> actionsForTimer = new Dictionary<Timer, Action<object[]>>();
		private static Dictionary<Timer, object[]> paramsForTimer = new Dictionary<Timer, object[]>();

		public static Timer SetTimeout(double delay, Action<object[]> action, params object[] parameters)
		{
			Timer tmr = new Timer();
			tmr.Tick += tmr_Tick;
			actionsForTimer.Add(tmr, action);
			paramsForTimer.Add(tmr, parameters);
			tmr.Duration = (int)delay;
			tmr.Start();
			return tmr;
		}

		private static void tmr_Tick(object sender, EventArgs e)
		{
			Timer tmr = (sender as Timer);
			if (!actionsForTimer.ContainsKey(tmr)) return;

			Action<object[]> action = actionsForTimer[tmr];
			object[] parameters = paramsForTimer[tmr];
			action(parameters);

			tmr.Stop();

			actionsForTimer.Remove(tmr);
		}

		public static bool ClearTimeout(Timer tmr)
		{
			if (!tmr.Enabled) return false;
			tmr.Stop();

			actionsForTimer.Remove(tmr);
			paramsForTimer.Remove(tmr);
			return true;
		}
	}
}
