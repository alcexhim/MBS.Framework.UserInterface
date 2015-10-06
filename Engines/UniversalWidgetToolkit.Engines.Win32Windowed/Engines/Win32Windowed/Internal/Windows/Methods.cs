using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UniversalWidgetToolkit.Engines.Win32Windowed.Internal.Windows
{
	internal static class Methods
	{
		/// <summary>
		/// The CreateWindowEx function creates an overlapped, pop-up, or child window with an extended window style; otherwise, this function is identical to the CreateWindow function.
		/// </summary>
		/// <param name="dwExStyle">Specifies the extended window style of the window being created.</param>
		/// <param name="lpClassName">Pointer to a null-terminated string or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word must be zero. If lpClassName is a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, provided that the module that registers the class is also the module that creates the window. The class name can also be any of the predefined system class names.</param>
		/// <param name="lpWindowName">Pointer to a null-terminated string that specifies the window name. If the window style specifies a title bar, the window title pointed to by lpWindowName is displayed in the title bar. When using CreateWindow to create controls, such as buttons, check boxes, and static controls, use lpWindowName to specify the text of the control. When creating a static control with the SS_ICON style, use lpWindowName to specify the icon name or identifier. To specify an identifier, use the syntax "#num". </param>
		/// <param name="dwStyle">Specifies the style of the window being created. This parameter can be a combination of window styles, plus the control styles indicated in the Remarks section.</param>
		/// <param name="x">Specifies the initial horizontal position of the window. For an overlapped or pop-up window, the x parameter is the initial x-coordinate of the window's upper-left corner, in screen coordinates. For a child window, x is the x-coordinate of the upper-left corner of the window relative to the upper-left corner of the parent window's client area. If x is set to CW_USEDEFAULT, the system selects the default position for the window's upper-left corner and ignores the y parameter. CW_USEDEFAULT is valid only for overlapped windows; if it is specified for a pop-up or child window, the x and y parameters are set to zero.</param>
		/// <param name="y">Specifies the initial vertical position of the window. For an overlapped or pop-up window, the y parameter is the initial y-coordinate of the window's upper-left corner, in screen coordinates. For a child window, y is the initial y-coordinate of the upper-left corner of the child window relative to the upper-left corner of the parent window's client area. For a list box y is the initial y-coordinate of the upper-left corner of the list box's client area relative to the upper-left corner of the parent window's client area.
		/// <para>If an overlapped window is created with the WS_VISIBLE style bit set and the x parameter is set to CW_USEDEFAULT, then the y parameter determines how the window is shown. If the y parameter is CW_USEDEFAULT, then the window manager calls ShowWindow with the SW_SHOW flag after the window has been created. If the y parameter is some other value, then the window manager calls ShowWindow with that value as the nCmdShow parameter.</para></param>
		/// <param name="nWidth">Specifies the width, in device units, of the window. For overlapped windows, nWidth is the window's width, in screen coordinates, or CW_USEDEFAULT. If nWidth is CW_USEDEFAULT, the system selects a default width and height for the window; the default width extends from the initial x-coordinates to the right edge of the screen; the default height extends from the initial y-coordinate to the top of the icon area. CW_USEDEFAULT is valid only for overlapped windows; if CW_USEDEFAULT is specified for a pop-up or child window, the nWidth and nHeight parameter are set to zero.</param>
		/// <param name="nHeight">Specifies the height, in device units, of the window. For overlapped windows, nHeight is the window's height, in screen coordinates. If the nWidth parameter is set to CW_USEDEFAULT, the system ignores nHeight.</param> <param name="hWndParent">Handle to the parent or owner window of the window being created. To create a child window or an owned window, supply a valid window handle. This parameter is optional for pop-up windows.
		/// <para>Windows 2000/XP: To create a message-only window, supply HWND_MESSAGE or a handle to an existing message-only window.</para></param>
		/// <param name="hMenu">Handle to a menu, or specifies a child-window identifier, depending on the window style. For an overlapped or pop-up window, hMenu identifies the menu to be used with the window; it can be NULL if the class menu is to be used. For a child window, hMenu specifies the child-window identifier, an integer value used by a dialog box control to notify its parent about events. The application determines the child-window identifier; it must be unique for all child windows with the same parent window.</param>
		/// <param name="hInstance">Handle to the instance of the module to be associated with the window.</param> <param name="lpParam">Pointer to a value to be passed to the window through the CREATESTRUCT structure (lpCreateParams member) pointed to by the lParam param of the WM_CREATE message. This message is sent to the created window by this function before it returns.
		/// <para>If an application calls CreateWindow to create a MDI client window, lpParam should point to a CLIENTCREATESTRUCT structure. If an MDI client window calls CreateWindow to create an MDI child window, lpParam should point to a MDICREATESTRUCT structure. lpParam may be NULL if no additional data is needed.</para></param>
		/// <returns>If the function succeeds, the return value is a handle to the new window.
		/// <para>If the function fails, the return value is NULL. To get extended error information, call GetLastError.</para>
		/// <para>This function typically fails for one of the following reasons:</para>
		/// <list type="">
		/// <item>an invalid parameter value</item>
		/// <item>the system class was registered by a different module</item>
		/// <item>The WH_CBT hook is installed and returns a failure code</item>
		/// <item>if one of the controls in the dialog template is not registered, or its window window procedure fails WM_CREATE or WM_NCCREATE</item>
		/// </list></returns>

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CreateWindowEx
		(
			Constants.WindowStylesEx dwExStyle,
			string lpClassName,
			string lpWindowName,
			Constants.WindowStyles dwStyle,
			int x,
			int y,
			int nWidth,
			int nHeight,
			IntPtr hWndParent,
			IntPtr hMenu,
			IntPtr hInstance,
			IntPtr lpParam
		);

		/// <summary>
		/// Retrieves a message from the calling thread's message queue. The function dispatches incoming sent messages until a posted message is available for retrieval.
		/// </summary>
		/// <param name="lpMsg">A pointer to an <see cref="Structures.MSG"/> structure that receives message information from the thread's message queue.</param>
		/// <param name="hWnd">
		///	A handle to the window whose messages are to be retrieved. The window must belong to the current thread.
		///	
		/// If hWnd is NULL, GetMessage retrieves messages for any window that belongs to the current thread, and any messages
		/// on the current thread's message queue whose hwnd value is NULL (see the MSG structure). Therefore if hWnd is NULL,
		/// both window messages and thread messages are processed.
		/// 
		/// If hWnd is -1, GetMessage retrieves only messages on the current thread's message queue whose hwnd value is NULL,
		/// that is, thread messages as posted by <see cref="PostMessage" /> (when the hWnd parameter is NULL) or
		/// <see cref="PostThreadMessage" />.
		/// </param>
		/// <param name="wMsgFilterMin">
		/// The integer value of the lowest message value to be retrieved. Use WM_KEYFIRST (0x0100) to specify the first
		/// keyboard message or WM_MOUSEFIRST (0x0200) to specify the first mouse message.
		/// 
		/// Use WM_INPUT here and in wMsgFilterMax to specify only the WM_INPUT messages.
		/// 
		/// If wMsgFilterMin and wMsgFilterMax are both zero, GetMessage returns all available messages (that is, no range filtering is performed).
		/// </param>
		/// <param name="wMsgFilterMax">
		/// The integer value of the highest message value to be retrieved. Use WM_KEYLAST to specify the last keyboard message or WM_MOUSELAST to specify the last mouse message.
		/// 
		/// Use WM_INPUT here and in wMsgFilterMin to specify only the WM_INPUT messages.
		/// 
		/// If wMsgFilterMin and wMsgFilterMax are both zero, GetMessage returns all available messages (that is, no range filtering is performed).
		/// </param>
		/// <returns>
		/// If the function retrieves a message other than WM_QUIT, the return value is nonzero.
		/// 
		/// If the function retrieves the WM_QUIT message, the return value is zero.
		/// 
		/// If there is an error, the return value is -1. For example, the function fails if hWnd is an invalid window handle
		/// or lpMsg is an invalid pointer. To get extended error information, call GetLastError.
		/// </returns>
		[DllImport("user32.dll")]
		public static extern int GetMessage
		(
			[Out()] out Structures.MSG lpMsg,
			[In()] [Optional()] IntPtr hWnd,
			[In()] uint wMsgFilterMin,
			[In()] uint wMsgFilterMax
		);

		[DllImport("user32.dll")]
		public static extern IntPtr RegisterClassEx
		(
			[In()] Structures.WNDCLASSEX lpwcx
		);

		/// <summary>
		/// Dispatches a message to a window procedure. It is typically used to dispatch a message retrieved by the GetMessage function.
		/// </summary>
		/// <param name="lpMsg">A pointer to a structure that contains the message.</param>
		/// <returns>
		/// The return value specifies the value returned by the window procedure. Although its meaning depends on the message
		/// being dispatched, the return value generally is ignored.
		/// </returns>
		/// <remarks>
		/// The MSG structure must contain valid message values. If the lpmsg parameter points to a WM_TIMER message and the
		/// lParam parameter of the WM_TIMER message is not NULL, lParam points to a function that is called instead of the
		/// window procedure.
		/// 
		/// Note that the application is responsible for retrieving and dispatching input messages to the dialog box. Most
		/// applications use the main message loop for this. However, to permit the user to move to and to select controls by
		/// using the keyboard, the application must call IsDialogMessage. For more information, see Dialog Box Keyboard
		/// Interface.
		/// </remarks>
		[DllImport("user32.dll")]
		public static extern bool DispatchMessage
		(
			[In()] Structures.MSG lpMsg
		);
		/// <summary>
		/// Translates virtual-key messages into character messages. The character messages are posted to the calling thread's
		/// message queue, to be read the next time the thread calls the <see cref="GetMessage" /> or
		/// <see cref="PeekMessage" /> function.
		/// </summary>
		/// <param name="lpMsg">
		/// A pointer to an MSG structure that contains message information retrieved from the calling thread's message queue
		/// by using the <see cref="GetMessage" /> or <see cref="PeekMessage" /> function.
		/// </param>
		/// <returns>
		/// If the message is translated (that is, a character message is posted to the thread's message queue), the return
		/// value is nonzero.
		/// 
		/// If the message is WM_KEYDOWN, WM_KEYUP, WM_SYSKEYDOWN, or WM_SYSKEYUP, the return value is nonzero, regardless of
		/// the translation.
		/// 
		/// If the message is not translated (that is, a character message is not posted to the thread's message queue), the
		/// return value is zero.
		/// </returns>
		/// <remarks>
		/// The TranslateMessage function does not modify the message pointed to by the lpMsg parameter.
		/// 
		/// WM_KEYDOWN and WM_KEYUP combinations produce a WM_CHAR or WM_DEADCHAR message. WM_SYSKEYDOWN and WM_SYSKEYUP
		/// combinations produce a WM_SYSCHAR or WM_SYSDEADCHAR message.
		/// 
		/// TranslateMessage produces WM_CHAR messages only for keys that are mapped to ASCII characters by the keyboard
		/// driver.
		/// 
		/// If applications process virtual-key messages for some other purpose, they should not call TranslateMessage. For
		/// instance, an application should not call TranslateMessage if the TranslateAccelerator function returns a nonzero
		/// value. Note that the application is responsible for retrieving and dispatching input messages to the dialog box.
		/// Most applications use the main message loop for this. However, to permit the user to move to and to select
		/// controls by using the keyboard, the application must call IsDialogMessage. For more information, see Dialog Box
		/// Keyboard Interface.
		/// </remarks>
		[DllImport("user32.dll")]
		public static extern bool TranslateMessage
		(
			[In()] Structures.MSG lpMsg
		);

		[DllImport("user32.dll")]
		public static extern int DefWindowProc(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);
	}
}
