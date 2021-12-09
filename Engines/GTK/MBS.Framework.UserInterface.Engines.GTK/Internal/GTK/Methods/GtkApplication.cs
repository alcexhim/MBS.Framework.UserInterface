//
//  GtkApplication.cs
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
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK.Internal.GTK.Methods
{
	internal class GtkApplication
	{
		[DllImport(Gtk.LIBRARY_FILENAME, EntryPoint = "gtk_application_new")]
		private static extern IntPtr gtk_application_new_v3(string application_id, Internal.GIO.Constants.GApplicationFlags flags);

		public static IntPtr gtk_application_new(string application_id, Internal.GIO.Constants.GApplicationFlags flags)
		{
			if (Gtk.LIBRARY_FILENAME == Gtk.LIBRARY_FILENAME_V2)
			{
				return IntPtr.Zero;
			}
			else
			{
				return gtk_application_new_v3(application_id, flags);
			}
		}

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_application_add_window(IntPtr /*GtkApplication*/ application, IntPtr /*GtkWindow*/ window);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GMenuModel*/ gtk_application_get_menubar(IntPtr /*GtkApplication*/ application);
		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_application_set_menubar(IntPtr /*GtkApplication*/ application, IntPtr /*GMenuModel*/ menubar);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr /*GMenuModel*/ gtk_application_get_app_menu(IntPtr /*GtkApplication*/ application);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_application_set_app_menu(IntPtr /*GtkApplication*/ application, IntPtr /*GMenuModel*/ menu);

		/// <summary>
		/// 	<para>
		/// 		Inform the session manager that certain types of actions should be inhibited. This is not guaranteed to work on
		/// 		all platforms and for all types of actions.
		/// 	</para>
		///		<para>
		///			Applications should invoke this method when they begin an operation that should not be interrupted, such as creating
		/// 		a CD or DVD. The types of actions that may be blocked are specified by the flags parameter. When the application
		/// 		completes the operation it should call gtk_application_uninhibit() to remove the inhibitor. Note that an application
		/// 		can have multiple inhibitors, and all of them must be individually removed. Inhibitors are also cleared when the
		/// 		application exits.
		/// 	</para>
		/// 	<para>
		/// 		Applications should not expect that they will always be able to block the action. In most cases, users will be given
		/// 		the option to force the action to take place.
		/// 	</para>
		/// 	<para>
		/// 		Reasons should be short and to the point.
		/// 	</para>
		/// 	<para>
		/// 		If window is given, the session manager may point the user to this window to find out more about why the action is
		/// 		inhibited.
		/// 	</para>
		/// </summary>
		/// <returns>
		/// A non-zero cookie that is used to uniquely identify this request. It should be used as an argument to
		/// gtk_application_uninhibit() in order to remove the request. If the platform does not support inhibiting or the request failed
		/// for some reason, 0 is returned.
		/// </returns>
		/// <param name="application">the GtkApplication</param>
		/// <param name="window">a GtkWindow, or NULL</param>
		/// <param name="flags">what types of actions should be inhibited</param>
		/// <param name="reason">a short, human-readable string that explains why these operations are inhibited</param>
		public static uint gtk_application_inhibit(IntPtr /*GtkApplication*/ application, IntPtr /*GtkWindow*/ window, Constants.GtkApplicationInhibitFlags flags, string reason)
		{
			IntPtr hReason = IntPtr.Zero;
			if (reason != null)
			{
				hReason = Marshal.StringToHGlobalAuto(reason);
			}
			if (window == IntPtr.Zero)
			{
				window = gtk_application_get_active_window(application);
			}
			return _gtk_application_inhibit(application, window, flags, hReason);
		}

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern IntPtr gtk_application_get_active_window(IntPtr /*GtkApplication*/ application);

		[DllImport(Gtk.LIBRARY_FILENAME, EntryPoint = "gtk_application_inhibit", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint _gtk_application_inhibit(IntPtr /*GtkApplication*/ application, IntPtr /*GtkWindow*/ window, Constants.GtkApplicationInhibitFlags flags, IntPtr reason);

		[DllImport(Gtk.LIBRARY_FILENAME)]
		public static extern void gtk_application_uninhibit(IntPtr /*GtkApplication*/ application, uint cookie);
	}
}
