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
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface
{
	public abstract class CustomImplementation : ControlImplementation
	{
		protected override Vector2D ClientToScreenCoordinatesInternal(Vector2D point)
		{
			throw new NotImplementedException();
		}

		public CustomImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		protected override void InvalidateInternal(int x, int y, int width, int height)
		{
			(Handle as CustomNativeControl).Handle.Invalidate(x, y, width, height);
		}

		protected override void DestroyInternal()
		{
			(Handle as CustomNativeControl).Handle.Destroy();
		}

		protected override void BeginMoveDragInternal(MouseButtons button, double x, double y, DateTime timestamp)
		{
			(Handle as CustomNativeControl).Handle.BeginMoveDrag(button, x, y, timestamp);
		}

		protected override Vector2D GetLocationInternal()
		{
			return (Handle as CustomNativeControl).Handle.Location;
		}
		protected override void SetLocationInternal(Vector2D location)
		{
			(Handle as CustomNativeControl).Handle.Location = location;
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

		protected override void UpdateControlFontInternal(NativeControl handle, Font font)
		{
			(handle as CustomNativeControl).Handle.Font = font;
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
		protected override AdjustmentScrollType GetAdjustmentScrollTypeInternal(Orientation orientation)
		{
			// FIXME: not implemented
			return AdjustmentScrollType.Never;
		}
		protected override void SetAdjustmentScrollTypeInternal(Orientation orientation, AdjustmentScrollType value)
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

		protected override IVirtualControlContainer GetParentControlInternal()
		{
			return (Handle as CustomNativeControl).Handle.Parent;
		}

		protected override void SetMarginInternal(Padding value)
		{
			// FIXME: not implemented
		}

		protected override void InitializeControlPropertiesInternal(NativeControl handle)
		{
			// intentionally not implemented
		}
	}
}
