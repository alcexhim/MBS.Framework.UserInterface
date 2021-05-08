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

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(ProgressBar))]
	public class ProgressBarImplementation : GTKNativeImplementation, MBS.Framework.UserInterface.Controls.Native.IProgressBarControlImplementation
	{
		public ProgressBarImplementation(Engine engine, ProgressBar control) : base(engine, control)
		{
		}

		protected override string GetControlTextInternal(Control control)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			if (Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_get_show_text(handle))
			{
				string ch = Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_get_text(handle);
				return ch;
			}
			return null;
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			if (text == null)
			{
				Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_set_show_text(handle, false);
			}
			else
			{
				Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_set_text(handle, text);
				Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_set_show_text(handle, true);
			}
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			IntPtr handle = Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_new();

			ProgressBar pb = (control as ProgressBar);
			if (pb.Text != null)
			{
				Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_set_text(handle, pb.Text);
				Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_set_show_text(handle, true);
			}
			__SetValues(handle, pb.Minimum, pb.Maximum, pb.Value);

			return new GTKNativeControl(handle);
		}

		public void SetValues(double minimum, double maximum, double value)
		{
			__SetValues((Handle as GTKNativeControl).Handle, minimum, maximum, value);
		}
		private void __SetValues(IntPtr handle, double minimum, double maximum, double value)
		{
			ProgressBar pb = (Control as ProgressBar);
			if (pb.Marquee)
			{
			}
			else
			{
				// FIXME: calculations
				double frac = minimum + (value / (maximum - minimum));
				Internal.GTK.Methods.GtkProgressBar.gtk_progress_bar_set_fraction(handle, frac);
			}
		}
	}
}
