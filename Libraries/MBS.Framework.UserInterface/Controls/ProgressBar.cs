//
//  ProgressBar.cs
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
namespace MBS.Framework.UserInterface.Controls
{
	namespace Native
	{
		public interface IProgressBarControlImplementation
		{
			void SetValues(double minimum, double maximum, double value);
		}
	}
	public class ProgressBar : SystemControl
	{
		private bool _Marquee = false;
		public bool Marquee
		{
			get { return _Marquee; }
			set { _Marquee = value; Update(); }
		}

		private double _Minimum = 0.0;
		public double Minimum
		{
			get { return _Minimum; }
			set { _Minimum = value; Update(); }
		}
		private double _Maximum = 100.0;
		public double Maximum
		{
			get { return _Maximum; }
			set { _Maximum = value; Update(); }
		}

		private double _Value = 0.0;
		public double Value
		{
			get { return _Value; }
			set { _Value = value; Update(); }
		}

		private void Update()
		{
			(ControlImplementation as Native.IProgressBarControlImplementation)?.SetValues(Minimum, Maximum, Value);
		}
	}
}
