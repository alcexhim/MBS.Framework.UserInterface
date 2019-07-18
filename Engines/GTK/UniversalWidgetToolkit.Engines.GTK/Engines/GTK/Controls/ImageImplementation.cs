//
//  ImageImplementation.cs
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
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.DragDrop;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Input.Keyboard;
using MBS.Framework.Drawing;

namespace UniversalWidgetToolkit.Engines.GTK
{
	[NativeImplementation(typeof(Image))]
	public class ImageImplementation : GTKNativeImplementation
	{
		public ImageImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Image ctl = (control as Image);
			IntPtr handle = Internal.GTK.Methods.gtk_image_new_from_icon_name(ctl.IconName);

			if (ctl.IconSize != Dimension2D.Empty)
				Internal.GTK.Methods.gtk_image_set_pixel_size(handle, (int) ctl.IconSize.Width);
			return new GTKNativeControl(handle);
		}
	}
}
