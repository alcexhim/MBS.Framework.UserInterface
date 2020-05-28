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
					Application.Engine.Timer_Start(this);
				}
				else
				{
					Application.Engine.Timer_Stop(this);
				}
			}
		}

		public int Duration { get; set; } = 0;

		public event EventHandler Tick;

		internal void _OnTick()
		{
			Tick?.Invoke(this, EventArgs.Empty);
		}
	}
}
