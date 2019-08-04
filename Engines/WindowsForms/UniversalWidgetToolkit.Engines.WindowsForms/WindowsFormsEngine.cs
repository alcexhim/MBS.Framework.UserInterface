using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit.Engines.WindowsForms
{
    public class WindowsFormsEngine : Engine
    {
		protected override void DestroyControlInternal (Control control)
		{
			throw new NotImplementedException ();
		}
		protected override NativeControl CreateControlInternal (Control control)
		{
			return base.CreateControlInternal (control);
		}

		protected override void TabContainer_ClearTabPagesInternal (TabContainer parent)
		{
			throw new NotImplementedException ();
		}
		protected override void TabContainer_RemoveTabPageInternal (TabContainer parent, TabPage tabPage)
		{
			throw new NotImplementedException ();
		}
		protected override void TabContainer_InsertTabPageInternal (TabContainer parent, int index, TabPage tabPage)
		{
			throw new NotImplementedException ();
		}

		protected override void UpdateControlLayoutInternal (Control control)
		{
			throw new NotImplementedException ();
		}
		protected override void UpdateControlPropertiesInternal (Control control, NativeControl handle)
		{
			throw new NotImplementedException ();
		}
		protected override void UpdateNotificationIconInternal (NotificationIcon nid, bool updateContextMenu)
		{
			throw new NotImplementedException ();
		}

		protected override bool WindowHasFocusInternal (Window window)
		{
			throw new NotImplementedException ();
		}

		protected override void InvalidateControlInternal (Control control, int x, int y, int width, int height)
		{
			throw new NotImplementedException ();
		}

		protected override DialogResult ShowDialogInternal (Dialog dialog, Window parent)
		{
			throw new NotImplementedException ();
		}

		protected override Window [] GetToplevelWindowsInternal ()
		{
			throw new NotImplementedException ();
		}

		protected override int StartInternal (Window waitForClose = null)
		{
			WindowsFormsNativeControl ncWaitForClose = (GetHandleForControl (waitForClose) as WindowsFormsNativeControl);
			System.Windows.Forms.Application.Run (ncWaitForClose.Handle as System.Windows.Forms.Form);
			return 0;
		}
		protected override void StopInternal (int exitCode)
		{
			throw new NotImplementedException ();
		}

		protected override void ShowNotificationPopupInternal (NotificationPopup popup)
		{
			throw new NotImplementedException ();
		}

		protected override void SetControlEnabledInternal (Control control, bool value)
		{
			throw new NotImplementedException ();
		}

		protected override Vector2D ClientToScreenCoordinatesInternal (Vector2D point)
		{
			throw new NotImplementedException ();
		}

		protected override void DoEventsInternal ()
		{
			System.Windows.Forms.Application.DoEvents ();
		}

		protected override Monitor [] GetMonitorsInternal ()
		{
			List<Monitor> list = new List<Monitor> ();
			foreach (System.Windows.Forms.Screen scr in System.Windows.Forms.Screen.AllScreens) {
				list.Add (new Monitor (scr.DeviceName, SDRectangleToUWTRectangle(scr.Bounds), SDRectangleToUWTRectangle(scr.WorkingArea)/*, scr.Primary*/));
			}
			return list.ToArray ();
		}

		private Rectangle SDRectangleToUWTRectangle (System.Drawing.Rectangle rect)
		{
			return new Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		protected override void RepaintCustomControl (CustomControl control, int x, int y, int width, int height)
		{
			throw new NotImplementedException ();
		}

		protected override bool IsControlDisposedInternal (Control ctl)
		{
			throw new NotImplementedException ();
		}
		protected override bool IsControlEnabledInternal (Control control)
		{
			throw new NotImplementedException ();
		}

		protected override bool InitializeInternal ()
		{
			System.Windows.Forms.Application.EnableVisualStyles ();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault (false);
			return true;
		}
	}
}
