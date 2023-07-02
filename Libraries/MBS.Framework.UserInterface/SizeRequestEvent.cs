//
//  SizeRequestEvent.cs
//
//  Author:
//       beckermj <>
//
//  Copyright (c) 2023 ${CopyrightHolder}
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
namespace MBS.Framework.UserInterface
{
	public enum SizeRequestType
	{
		PreferredWidth,
		PreferredHeight,
		PreferredWidthForHeight,
		PreferredHeightForWidth
	}
	public class SizeRequestEventArgs : EventArgs
	{
		public SizeRequestType RequestType { get; }
		public int MinimumHeight { get; set; }
		public int NaturalHeight { get; set; }

		public bool Handled { get; set; } = false;

		public SizeRequestEventArgs(SizeRequestType requestType, int minimumHeight, int naturalHeight)
		{
			RequestType = requestType;
			MinimumHeight = minimumHeight;
			NaturalHeight = naturalHeight;
		}
	}
}
