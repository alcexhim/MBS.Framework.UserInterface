//
//  Image.cs
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
namespace MBS.Framework.UserInterface.Drawing
{
	public abstract class Image
	{
		public int Width { get; protected set; } = 0;
		public int Height { get; protected set; } = 0;

		public static Image FromStock(StockType stockType, int size)
		{
			return ((UIApplication)Application.Instance).Engine.LoadImage(stockType, size);
		}
		public static Image FromName(string name, int size)
		{
			Image image = ((UIApplication)Application.Instance).Engine.LoadImageByName(name, size);
			return image;
		}
		public static Image FromBytes(byte[] data, string type)
		{
			Image image = ((UIApplication)Application.Instance).Engine.LoadImage(data, type);
			return image;
		}
		public static Image FromBytes(byte[] data, int width, int height, int rowstride)
		{
			Image image = ((UIApplication)Application.Instance).Engine.LoadImage(data, width, height, rowstride);
			return image;
		}

		public static Image Create(int width, int height)
		{
			Image image = ((UIApplication)Application.Instance).Engine.CreateImage(width, height);
			return image;
		}

		public static Image FromFile(string filename, string type = null)
		{
			if (type == null)
			{
				type = System.IO.Path.GetExtension(filename).ToLower().Substring(1);
			}
			Image image = ((UIApplication)Application.Instance).Engine.LoadImage(filename, type);
			return image;
		}
	}
}
