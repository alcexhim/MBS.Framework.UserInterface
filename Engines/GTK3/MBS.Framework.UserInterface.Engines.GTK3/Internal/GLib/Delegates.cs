//
//  Delegates.cs
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
namespace MBS.Framework.UserInterface.Engines.GTK3.Internal.GLib
{
	internal static class Delegates
	{
		public delegate int GCompareFunc(IntPtr a, IntPtr b);
		public delegate int GCompareDataFunc(IntPtr a, IntPtr b, IntPtr user_data);
		public delegate bool GEqualFunc(IntPtr a, IntPtr b);
		public delegate void GDestroyNotify(IntPtr data);
		public delegate void GFunc(IntPtr data, IntPtr user_data);
		public delegate uint GHashFunc(IntPtr key);
		public delegate void GHFunc(IntPtr key, IntPtr value, IntPtr user_data);
	}
}
