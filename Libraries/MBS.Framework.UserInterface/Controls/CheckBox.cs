//
//  CheckBox.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019
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
using System.ComponentModel;

namespace MBS.Framework.UserInterface.Controls
{
	namespace Native
	{
		public interface ICheckBoxImplementation
		{
			bool GetChecked();
			void SetChecked(bool value);
		}
	}
	public class CheckBoxChangingEventArgs : CancelEventArgs
	{
		public bool OldValue { get; private set; } = false;
		public bool NewValue { get; set; } = false;
		public CheckBoxChangingEventArgs(bool oldvalue, bool newvalue)
		{
			OldValue = oldvalue;
			NewValue = newvalue;
		}
	}
	public enum CheckBoxDisplayStyle
	{
		CheckBox,
		Switch
	}
	public class CheckBox : SystemControl
	{
		public CheckBox()
		{
		}

		private bool mvarChecked = false;

		public event EventHandler<CheckBoxChangingEventArgs> Changing;
		protected virtual void OnChanging(CheckBoxChangingEventArgs e)
		{
			Changing?.Invoke(this, e);
		}
		public event EventHandler Changed;
		protected virtual void OnChanged(EventArgs e)
		{
			Changed?.Invoke(this, e);
		}

		public bool Checked
		{
			get
			{
				if (!((UIApplication)Application.Instance).Engine.IsControlCreated (this)) {
					return mvarChecked;
				}

				Native.ICheckBoxImplementation impl = (ControlImplementation as Native.ICheckBoxImplementation);
				if (impl != null)
					mvarChecked = impl.GetChecked ();
				return mvarChecked;
			}
			set
			{
				if (IsCreated) {
					(ControlImplementation as Native.ICheckBoxImplementation)?.SetChecked (value);
				}
				mvarChecked = value;
			}
		}

		public bool UseMnemonic { get; set; } = true;
		public CheckBoxDisplayStyle DisplayStyle { get; set; } = CheckBoxDisplayStyle.CheckBox;
	}
}
