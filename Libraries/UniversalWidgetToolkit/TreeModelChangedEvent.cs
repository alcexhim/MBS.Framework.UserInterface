﻿//
//  TreeModelChangedEvent.cs
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
using System.Collections;
using System.Collections.Specialized;

namespace UniversalWidgetToolkit
{
	public enum TreeModelChangedAction
	{
		Add,
		Modify,
		Remove,
		Clear
	}
	public delegate void TreeModelChangedEventHandler(object sender, TreeModelChangedEventArgs e);
	public class TreeModelChangedEventArgs : EventArgs
	{
		public TreeModelChangedAction Action { get; private set; }
		public TreeModelRow ParentRow { get; private set; } = null;

		/// <summary>
		/// The rows that have been added, modified, or removed.
		/// </summary>
		/// <value>The rows.</value>
		public TreeModelRow.TreeModelRowCollection Rows { get; } = new TreeModelRow.TreeModelRowCollection();

		public TreeModelChangedEventArgs(TreeModelChangedAction action, TreeModelRow[] items = null, TreeModelRow parentRow = null)
		{
			Action = action;
			if (items != null)
			{
				foreach (TreeModelRow item in items)
				{
					Rows.Add(item);
				}
			}
			ParentRow = parentRow;
		}
	}
}
