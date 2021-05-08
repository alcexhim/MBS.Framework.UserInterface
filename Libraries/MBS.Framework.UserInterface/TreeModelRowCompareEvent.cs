//
//  TreeModelRowCompareEvent.cs
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
	public class TreeModelRowCompareEventArgs : EventArgs
	{
		public TreeModelRow Left { get; private set; } = null;
		public TreeModelRow Right { get; private set; } = null;
		public int ColumnIndex { get; private set; } = 0;

		public int Value { get; set; } = 0;
		public bool Handled { get; set; } = false;

		public TreeModelRowCompareEventArgs(TreeModelRow left, TreeModelRow right, int columnIndex)
		{
			Left = left;
			Right = right;
			ColumnIndex = columnIndex;
		}
	}
	public delegate void TreeModelRowCompareEventHandler(object sender, TreeModelRowCompareEventArgs e);
}
