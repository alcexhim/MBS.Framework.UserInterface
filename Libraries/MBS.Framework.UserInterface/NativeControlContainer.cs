//
//  NativeControlContainer.cs
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
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface
{
	public abstract class NativeControlContainer : IControlContainer
	{
		public abstract Control.ControlCollection Controls { get; }
		public abstract Layout Layout { get; set; }
		public abstract Rectangle Bounds { get; }
		public abstract bool IsCreated { get; }
		public abstract bool Visible { get; set; }
		public abstract ControlImplementation ControlImplementation { get; }
		public abstract IVirtualControlContainer Parent { get; }
		public abstract Padding Padding { get; set; }
		public abstract void BeginMoveDrag(MouseButtons buttons, double x, double y, DateTime now);

		public abstract Control[] GetAllControls();
	}
}
