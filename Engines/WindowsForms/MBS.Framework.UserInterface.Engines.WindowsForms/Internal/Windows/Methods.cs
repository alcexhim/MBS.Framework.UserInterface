//
//  Methods.cs
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

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Internal.Windows
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME_USER32 = "user32";
		public const string LIBRARY_FILENAME_SHELL32 = "shell32";

		[DllImport(LIBRARY_FILENAME_USER32)]
		public static extern int MessageBox(IntPtr /*HWND*/ hWnd, string lpText, string lpCaption, uint uType);

		[DllImport(LIBRARY_FILENAME_USER32)]
		public static extern bool EnumWindows(Delegates.EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport(LIBRARY_FILENAME_USER32)]
		public static extern bool IsWindowVisible(IntPtr /*HWND*/ hWnd);

		[DllImport(LIBRARY_FILENAME_USER32)]
		public static extern IntPtr GetActiveWindow();
		[DllImport(LIBRARY_FILENAME_USER32)]
		public static extern bool ShowWindow(IntPtr /*HWND*/ hWnd, Constants.ShowWindowCommand nCmdShow);

		[DllImport(LIBRARY_FILENAME_USER32)]
		private static extern IntPtr GetWindowLongPtrW(IntPtr /*HWND*/ hWnd, Constants.WindowLong nIndex);
		[DllImport(LIBRARY_FILENAME_USER32)]
		private static extern int GetWindowLong(IntPtr /*HWND*/ hWnd, Constants.WindowLong nIndex);

		public static IntPtr GetWindowLongPtr(IntPtr /*HWND*/ hWnd, Constants.WindowLong nIndex)
		{
			if (Environment.Is64BitProcess)
			{
				return GetWindowLongPtrW(hWnd, nIndex);
			}
			int val = GetWindowLong(hWnd, nIndex);
			return new IntPtr(val);
		}


		// TaskDialog
		[DllImport("comctl32.dll", CharSet = CharSet.Unicode)]
		public static extern int TaskDialog(IntPtr hwndParent, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, int dwCommonButtons, IntPtr pszIcon, out int pnButton);
		[DllImport("comctl32.dll", CharSet = CharSet.Unicode)]
		public static extern uint /*HRESULT*/ TaskDialogIndirect(ref Structures.TASKDIALOGCONFIG pTaskConfig, out int pnButton, out int pnRadioButton, [MarshalAs(UnmanagedType.Bool)] out bool pfVerificationFlagChecked);

		/// <summary>
		/// Sets the process-default DPI awareness to system-DPI awareness. This is equivalent to calling <see cref="SetProcessDpiAwarenessContext"/> with a
		/// <see cref="Constants.DpiAwarenessContext" /> value of <see cref="Constants.DpiAwarenessContext.SystemAware" />.
		/// </summary>
		/// <returns></returns>
		[DllImport(LIBRARY_FILENAME_USER32)]
		public static extern bool SetProcessDPIAware();

		/// <summary>
		/// Sets the current process to a specified dots per inch (dpi) awareness context. The DPI awareness contexts are from the
		/// <see cref="Constants.DpiAwarenessContext" /> value.
		/// </summary>
		/// <param name="value">A <see cref="Constants.DpiAwarenessContext" /> handle to set.</param>
		/// <returns></returns>
		[DllImport(LIBRARY_FILENAME_USER32)]
		public static extern bool SetProcessDpiAwarenessContext(Constants.DpiAwarenessContext value);

		/// <summary>
		/// Specifies a unique application-defined Application User Model ID (AppUserModelID) that identifies the current process to the taskbar. This identifier
		/// allows an application to group its associated processes and windows under a single taskbar button.
		/// </summary>
		/// <param name="AppID">Pointer to the AppUserModelID to assign to the current process.</param>
		/// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
		[DllImport(LIBRARY_FILENAME_SHELL32)]
		public static extern int /*HRESULT*/ SetCurrentProcessExplicitAppUserModelID(string /*PCWSTR*/ AppID);
	}
}
