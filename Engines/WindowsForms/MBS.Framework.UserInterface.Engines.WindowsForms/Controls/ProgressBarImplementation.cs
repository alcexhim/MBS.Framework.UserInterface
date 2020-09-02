//
//  ProgressBarImplementation.cs
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
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Native;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls
{
	[ControlImplementation(typeof(ProgressBar))]
	public class ProgressBarImplementation : WindowsFormsNativeImplementation, IProgressBarControlImplementation
	{
		public ProgressBarImplementation(Engine engine, ProgressBar control) : base(engine, control)
		{
		}

		public void SetValues(double minimum, double maximum, double value)
		{
			System.Windows.Forms.ProgressBar handle = ((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.ProgressBar);
			ProgressBar pb = (Control as ProgressBar);

			InvokeIfRequired(handle, new Action(delegate ()
			{
				handle.Minimum = 0;
				handle.Maximum = 100;
				handle.Value = (int)((minimum + (value / (maximum - minimum))) * 100);

				if (pb.Marquee)
				{
					handle.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
				}
				else
				{
					handle.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
				}
			}));
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			System.Windows.Forms.ProgressBar handle = new System.Windows.Forms.ProgressBar();
			ProgressBar pb = (control as ProgressBar);

			handle.Minimum = 0;
			handle.Maximum = 100;
			handle.Value = (int)((pb.Minimum + (pb.Value / (pb.Maximum - pb.Minimum))) * 100);
			if (pb.Marquee)
			{
				handle.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			}
			else
			{
				handle.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			}

			return new WindowsFormsNativeControl(handle);
		}
	}
}
