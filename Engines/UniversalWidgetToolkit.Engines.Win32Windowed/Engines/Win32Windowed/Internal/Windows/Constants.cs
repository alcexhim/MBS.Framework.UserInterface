using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit.Engines.Win32Windowed.Internal.Windows
{
	internal class Constants
	{
		[Flags]
		public enum WindowStyles : uint
		{
			WS_OVERLAPPED = 0x00000000,
			WS_POPUP = 0x80000000,
			WS_CHILD = 0x40000000,
			WS_MINIMIZE = 0x20000000,
			WS_VISIBLE = 0x10000000,
			WS_DISABLED = 0x08000000,
			WS_CLIPSIBLINGS = 0x04000000,
			WS_CLIPCHILDREN = 0x02000000,
			WS_MAXIMIZE = 0x01000000,
			WS_BORDER = 0x00800000,
			WS_DLGFRAME = 0x00400000,
			WS_VSCROLL = 0x00200000,
			WS_HSCROLL = 0x00100000,
			WS_SYSMENU = 0x00080000,
			WS_THICKFRAME = 0x00040000,
			WS_GROUP = 0x00020000,
			WS_TABSTOP = 0x00010000,

			WS_MINIMIZEBOX = 0x00020000,
			WS_MAXIMIZEBOX = 0x00010000,

			WS_CAPTION = WS_BORDER | WS_DLGFRAME,
			WS_TILED = WS_OVERLAPPED,
			WS_ICONIC = WS_MINIMIZE,
			WS_SIZEBOX = WS_THICKFRAME,
			WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

			WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
			WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
			WS_CHILDWINDOW = WS_CHILD
		}

		[Flags()]
		public enum WindowStylesEx : uint
		{
			None = 0x00000000,
			//Extended Window Styles

			DialogModalFrame = 0x00000001,
			NoParentNotify = 0x00000004,
			Topmost = 0x00000008,
			AcceptFiles = 0x00000010,
			Transparent = 0x00000020,

			//#if(WINVER >= 0x0400)

			MDIChild = 0x00000040,
			ToolWindow = 0x00000080,
			WindowEdge = 0x00000100,
			ClientEdge = 0x00000200,
			ContextHelp = 0x00000400,

			Right = 0x00001000,
			Left = 0x00000000,
			RightToLeftReading = 0x00002000,
			LeftScrollBar = 0x00004000,

			ControlParent = 0x00010000,
			StaticEdge = 0x00020000,
			AppWindow = 0x00040000,

			OverlappedWindow = (WindowEdge | ClientEdge),
			PaletteWindow = (WindowEdge | ToolWindow | Topmost),

			//#endif /* WINVER >= 0x0400 */

			//#if(WIN32WINNT >= 0x0500)

			Layered = 0x00080000,

			//#endif /* WIN32WINNT >= 0x0500 */

			//#if(WINVER >= 0x0500)

			NoInheritLayout = 0x00100000, // Disable inheritence of mirroring by children
			LayoutRightToLeft = 0x00400000, // Right to left mirroring

			//#endif /* WINVER >= 0x0500 */

			//#if(WIN32WINNT >= 0x0500)

			Composited = 0x02000000,
			NoActivate = 0x08000000

			//#endif /* WIN32WINNT >= 0x0500 */
		}
	}
}
