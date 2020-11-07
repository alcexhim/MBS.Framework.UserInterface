//
//  AppIndicator.cs
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

namespace MBS.Framework.UserInterface.Engines.GTK.InternalAPI.AppIndicator
{
	public class Indicator
	{
		private static class Methods
		{
			public const string LIBRARY_FILENAME = "appindicator";

			[DllImport(LIBRARY_FILENAME)]
			public static extern IntPtr app_indicator_new(string id, string icon_name, AppIndicatorCategory category);

			[DllImport(LIBRARY_FILENAME)]
			public static extern AppIndicatorStatus app_indicator_get_status(IntPtr /*AppIndicator*/ handle);
			[DllImport(LIBRARY_FILENAME)]
			public static extern void app_indicator_set_status(IntPtr /*AppIndicator*/ handle, AppIndicatorStatus value);

			[DllImport(LIBRARY_FILENAME)]
			public static extern IntPtr /*GtkMenu*/ app_indicator_get_menu(IntPtr /*AppIndicator*/ handle);
			[DllImport(LIBRARY_FILENAME)]
			public static extern void app_indicator_set_menu(IntPtr /*AppIndicator*/ handle, IntPtr /*GtkMenu*/ value);

			[DllImport(LIBRARY_FILENAME)]
			public static extern string app_indicator_get_label(IntPtr /*AppIndicator*/ handle);
			[DllImport(LIBRARY_FILENAME)]
			public static extern void app_indicator_set_label(IntPtr /*AppIndicator*/ handle, string label, string guide);

			[DllImport(LIBRARY_FILENAME)]
			public static extern string app_indicator_get_title(IntPtr /*AppIndicator*/ handle);
			[DllImport(LIBRARY_FILENAME)]
			public static extern void app_indicator_set_title(IntPtr /*AppIndicator*/ handle, string value);

			[DllImport(LIBRARY_FILENAME)]
			public static extern string app_indicator_get_icon(IntPtr /*AppIndicator*/ handle);
			[DllImport(LIBRARY_FILENAME)]
			public static extern void app_indicator_set_icon(IntPtr /*AppIndicator*/ handle, string value);

			[DllImport(LIBRARY_FILENAME)]
			public static extern string app_indicator_get_attention_icon(IntPtr /*AppIndicator*/ handle);
			[DllImport(LIBRARY_FILENAME)]
			public static extern void app_indicator_set_attention_icon(IntPtr /*AppIndicator*/ handle, string value);

		}
		private IntPtr Handle { get; set; } = IntPtr.Zero;

		public string IconName { get { return Methods.app_indicator_get_icon(Handle); } set { Methods.app_indicator_set_icon(Handle, value); } }
		public string AttentionIconName { get { return Methods.app_indicator_get_attention_icon(Handle); } set { Methods.app_indicator_set_attention_icon(Handle, value); } }

		public string Title { get { return Methods.app_indicator_get_title(Handle); } set { Methods.app_indicator_set_title(Handle, value); } }
		public string Label { get { return Methods.app_indicator_get_label(Handle); } set { Methods.app_indicator_set_label(Handle, value, value); } }

		public IntPtr HMenu { get { return Methods.app_indicator_get_menu(Handle); } set { Methods.app_indicator_set_menu(Handle, value); } }

		public AppIndicatorStatus Status { get { return Methods.app_indicator_get_status(Handle); } set { Methods.app_indicator_set_status(Handle, value); } }

		public Indicator(string name, string iconName, AppIndicatorCategory category)
		{
			Handle = Methods.app_indicator_new(name, iconName, category);
		}
	}
}
