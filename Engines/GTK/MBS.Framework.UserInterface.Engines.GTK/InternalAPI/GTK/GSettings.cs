//
//  GSettings.cs
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
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK.InternalAPI.GTK
{
	public class GSettings
	{
		[DllImport(Internal.GTK.Methods.Gtk.LIBRARY_FILENAME)]
		private static extern IntPtr /*GSettings*/ g_settings_new(string schema_id);
		[DllImport(Internal.GTK.Methods.Gtk.LIBRARY_FILENAME)]
		private static extern int g_settings_get_int(IntPtr /*GSettings*/ settings, string key);
		[DllImport(Internal.GTK.Methods.Gtk.LIBRARY_FILENAME)]
		private static extern bool g_settings_is_writable(IntPtr /*GSettings*/ settings, string key);
		[DllImport(Internal.GTK.Methods.Gtk.LIBRARY_FILENAME)]
		private static extern void g_settings_reset(IntPtr /*GSettings*/ settings, string key);

		private IntPtr Handle { get; set; } = IntPtr.Zero;

		public GSettings(string schema_id)
		{
			Handle = g_settings_new(schema_id);
		}

		/// <summary>
		/// Creates a new <see cref="GSettings" /> object with the relocatable schema specified by schema_id and a given path.
		/// You only need to do this if you want to directly create a settings object with a schema that doesn't have a specified path of its own. That's quite rare.
		/// It is a programmer error to call this function for a schema that has an explicitly specified path.
		/// It is a programmer error if path is not a valid path. A valid path begins and ends with '/' and does not contain two consecutive '/' characters.
		/// </summary>
		/// <param name="schema_id">Schema identifier.</param>
		/// <param name="path">Path.</param>
		public GSettings(string schema_id, string path)
		{
		}

		/// <summary>
		/// Finds out if a key can be written or not.
		/// </summary>
		/// <returns><c>true</c>, if key can be written, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		public bool IsWritable(string key)
		{
			return g_settings_is_writable(Handle, key);
		}

		public int GetInt32(string key)
		{
			int value = g_settings_get_int(Handle, key);
			return value;
		}

		/// <summary>
		/// Resets key to its default value.
		/// This call resets the key, as much as possible, to its default value. That might be the value specified in the schema or the one set by the administrator.
		/// </summary>
		/// <param name="key">The name of the key whose value should be reset.</param>
		public void Reset(string key)
		{
			g_settings_reset(Handle, key);
		}
	}
}
