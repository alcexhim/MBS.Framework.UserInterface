//
//  CustomImplementation.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
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

namespace MBS.Framework.UserInterface
{
	public abstract class CustomImplementation : ControlImplementation
	{
		public CustomImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		protected override Dimension2D GetScrollBoundsInternal()
		{
			return Dimension2D.Empty;
		}
		protected override void SetScrollBoundsInternal(Dimension2D value)
		{
		}

		protected override Dimension2D GetControlSizeInternal()
		{
			return (Handle as CustomNativeControl).Handle.Size;
		}
		protected override void SetControlSizeInternal(Dimension2D value)
		{
			(Handle as CustomNativeControl).Handle.Size = value;
		}

		protected override void UpdateControlFontInternal(Font font)
		{
			(Handle as CustomNativeControl).Handle.Font = font;
		}
		protected override double GetAdjustmentValueInternal(Orientation orientation)
		{
			// FIXME: not implemented
			return 0.0;
		}
		protected override void SetAdjustmentValueInternal(Orientation orientation, double value)
		{
			// FIXME: not implemented
		}

		protected override void SetVerticalAlignmentInternal(VerticalAlignment value)
		{
			(Handle as CustomNativeControl).Handle.VerticalAlignment = value;
		}
		protected override VerticalAlignment GetVerticalAlignmentInternal()
		{
			return (Handle as CustomNativeControl).Handle.VerticalAlignment;
		}
		protected override void SetHorizontalAlignmentInternal(HorizontalAlignment value)
		{
			(Handle as CustomNativeControl).Handle.HorizontalAlignment = value;
		}
		protected override HorizontalAlignment GetHorizontalAlignmentInternal()
		{
			return (Handle as CustomNativeControl).Handle.HorizontalAlignment;
		}

		protected override bool SupportsEngineInternal(Type engineType)
		{
			return true;
		}
	}
}

