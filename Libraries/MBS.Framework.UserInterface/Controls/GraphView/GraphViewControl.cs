//
//  GraphViewControl.cs
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
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface.Controls.GraphView
{
	/// <summary>
	/// Provides a UWT implementation of a graph node rendering system.
	/// </summary>
	public class GraphViewControl : CustomControl, IGraphNodeContainer
	{
		private DragManager dm = new DragManager();
		public GraphNode.GraphNodeCollection Nodes { get; }

		public GraphViewControl()
		{
			dm.Register(this);
			dm.BeforeControlPaint += dm_BeforeControlPaint;

			Nodes = new GraphNode.GraphNodeCollection(null);
		}

		/*
		private void dm_HitTest(object sender, DragManagerHitTestEventArgs e)
		{

		}
		*/

		private void dm_BeforeControlPaint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Colors.Gray);

			Font font = Font.FromFamily("Sans", 14);

			Rectangle nodePos = new Rectangle(54, 68, 256, 32);
			foreach (GraphNode node in Nodes)
			{
				TextMeasurement textSize = e.Graphics.MeasureText(node.Title, font);
				nodePos.Width = textSize.Size.Width + 8;

				e.Graphics.FillRectangle(new SolidBrush(Colors.White), nodePos);
				e.Graphics.DrawRectangle(new Pen(Colors.Black), nodePos);

				e.Graphics.DrawText(node.Title, font, nodePos, new SolidBrush(Colors.Black), HorizontalAlignment.Center, VerticalAlignment.Middle);
				nodePos = nodePos.Translate(nodePos.Right + 32, 0);
			}
		}
	}
}
