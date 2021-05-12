//
//  ListViewColumnCellRenderer.cs
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
using System.Collections.Generic;

namespace MBS.Framework.UserInterface
{
	public abstract class CellRenderer
	{
		public class CellRendererCollection
			: System.Collections.ObjectModel.Collection<CellRenderer>
		{

		}
		public CellRenderer(IEnumerable<CellRendererColumn> columns)
		{
			foreach (CellRendererColumn column in columns)
			{
				Columns.Add(column);
			}
		}

		public CellRendererColumn.CellRendererColumnCollection Columns { get; } = new CellRendererColumn.CellRendererColumnCollection();
		public bool Expand { get; set; }
	}

	public class CellRendererText
		: CellRenderer
	{
		public CellRendererText(IEnumerable<CellRendererColumn> columns)
			: base(columns)
		{
		}

		public CellRendererText(TreeModelColumn textColumn)
			: base(new CellRendererColumn[] { new CellRendererColumn(CellRendererProperty.Text, textColumn) } )
		{

		}
	}
	public class CellRendererChoice
		: CellRenderer
	{
		public CellRendererChoice(IEnumerable<CellRendererColumn> columns)
			: base(columns)
		{
		}

		public CellRendererChoice(TreeModelColumn textColumn)
			: base(new CellRendererColumn[] { new CellRendererColumn(CellRendererProperty.Text, textColumn) })
		{

		}
		public DefaultTreeModel Model { get; }
	}
	public class CellRendererToggle
		: CellRenderer
	{
		public CellRendererToggle(IEnumerable<CellRendererColumn> columns)
			: base(columns)
		{
		}

		public CellRendererToggle(TreeModelColumn textColumn)
			: base(new CellRendererColumn[] { new CellRendererColumn(CellRendererProperty.Text, textColumn) })
		{

		}
	}
	public class CellRendererImage
		: CellRenderer
	{
		public CellRendererImage(IEnumerable<CellRendererColumn> columns)
			: base(columns)
		{
		}

		public CellRendererImage(TreeModelColumn imageColumn)
			: base(new CellRendererColumn[] { new CellRendererColumn(CellRendererProperty.Image, imageColumn) })
		{

		}
	}
}
