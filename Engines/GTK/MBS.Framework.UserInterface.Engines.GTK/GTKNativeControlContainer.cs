//
//  GTKNativeControlContainer.cs
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

namespace MBS.Framework.UserInterface.Engines.GTK
{
	public class GTKNativeControlContainer : NativeControlContainer
	{
		IntPtr _Handle = IntPtr.Zero;
		public GTKNativeControlContainer(IntPtr handle)
		{
			_Handle = handle;
		}

		public override Control.ControlCollection Controls => throw new NotImplementedException();

		public override Layout Layout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override Rectangle Bounds => throw new NotImplementedException();

		public override bool IsCreated => throw new NotImplementedException();

		public override bool Visible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override ControlImplementation ControlImplementation => throw new NotImplementedException();

		public override IControlContainer Parent => throw new NotImplementedException();

		public override Padding Padding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override Control[] GetAllControls()
		{
			throw new NotImplementedException();
		}
	}
}
