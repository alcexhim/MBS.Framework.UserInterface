//
//  CellEditingEvent.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2021 Mike Becker's Software
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
	public class CellEditingEventArgs : System.ComponentModel.CancelEventArgs
	{
		public TreeModelRow Row { get; } = null;
		public TreeModelColumn Column { get; } = null;
		public object OldValue { get; set; } = null;
		public object NewValue { get; set; } = null;

		public CellEditingEventArgs(TreeModelRow row, TreeModelColumn column, object oldValue, object newValue)
		{
			Row = row;
			Column = column;
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
