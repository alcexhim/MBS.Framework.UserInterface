using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MBS.Framework.UserInterface
{
    public enum DpiAwareness
	{
		Default = 0,
		/// <summary>
		/// DPI unaware. This window does not scale for DPI changes and is always assumed to have a scale factor of 100% (96 DPI). It will be automatically
		/// scaled by the system on any other DPI setting.
		/// </summary>
		Unaware,
		/// <summary>
		/// System DPI aware. This window does not scale for DPI changes. It will query for the DPI once and use that value for the lifetime of the process. If
		/// the DPI changes, the process will not adjust to the new DPI value. It will be automatically scaled up or down by the system when the DPI changes
		/// from the system value.
		/// </summary>
		SystemAware,
		/// <summary>
		/// Per monitor DPI aware. This window checks for the DPI when it is created and adjusts the scale factor whenever the DPI changes. These processes are
		/// not automatically scaled by the system.
		/// </summary>
		PerMonitorAware,
		/// <summary>
		///		<para>
		///			Also known as Per Monitor v2. An advancement over the original per-monitor DPI awareness mode, which enables applications to access new
		///			DPI-related scaling behaviors on a per top-level window basis.
		///		</para>
		///		<para>
		///			Per Monitor v2 was made available in the Creators Update of Windows 10, and is not available on earlier versions of the operating system.
		///		</para>
		/// </summary>
		PerWindowAware,
		/// <summary>
		///		<para>
		///			DPI unaware with improved quality of GDI-based content. This mode behaves similarly to DPI_AWARENESS_CONTEXT_UNAWARE, but also enables the
		///			system to automatically improve the rendering quality of text and other GDI-based primitives when the window is displayed on a high-DPI
		///			monitor.
		///		</para>
		///		<para>
		///			<see cref="UnawareGDIScaled" /> was introduced in the October 2018 update of Windows 10 (also known as version 1809).
		///		</para>
		/// </summary>
		UnawareGDIScaled
	}
}
