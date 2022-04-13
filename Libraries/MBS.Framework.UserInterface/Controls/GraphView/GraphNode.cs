//
//  GraphNode.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2022 Mike Becker's Software
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
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Controls.GraphView
{
	public class GraphNode : IGraphNodeContainer
	{
		public class GraphNodeCollection
			: System.Collections.ObjectModel.Collection<GraphNode>
		{
			private IGraphNodeContainer _parent = null;
			public GraphNodeCollection(IGraphNodeContainer parent)
			{
				_parent = parent;
			}
		}

		public GraphNode(string title = null, Image image = null)
		{
			ParentNode = null;
			Nodes = new GraphNodeCollection(this);

			Title = title;
			Image = image;
		}

		public GraphNode ParentNode { get; private set; } = null;
		public GraphNode.GraphNodeCollection Nodes { get; }

		public string Title { get; set; } = null;
		public Image Image { get; set; } = null;
	}
}
