using System;
using System.Collections.Generic;
using System.ComponentModel;

using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Native;
using System.Runtime.InteropServices;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(Window))]
	public class WindowImplementation : ContainerImplementation, IWindowNativeImplementation
	{
		private Dictionary<IntPtr, MenuItem> menuItemsByHandle = new Dictionary<IntPtr, MenuItem>();

		private Internal.GObject.Delegates.GCallback gc_MenuItem_Activated = null;

		private Internal.GObject.Delegates.GCallback gc_Window_Activate = null;
		private Func<IntPtr, IntPtr, bool> gc_Window_Closing = null;
		private Internal.GObject.Delegates.GCallback gc_Window_Closed = null;

		public string GetIconName()
		{
			if (Handle == null) return String.Empty;
			return Internal.GTK.Methods.GtkWindow.gtk_window_get_icon_name ((Handle as GTKNativeControl).Handle);
		}
		public void SetIconName(string value)
		{
			if (Handle == null) return;
			Internal.GTK.Methods.GtkWindow.gtk_window_set_icon_name ((Handle as GTKNativeControl).Handle, value);
		}

		private string _DocumentFileName = null;
		public string GetDocumentFileName()
		{
			IntPtr handle = (Handle as GTKNativeControl).GetNamedHandle("HeaderBar");
			if (handle != IntPtr.Zero)
				return Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_get_subtitle(handle);
			return _DocumentFileName;
		}
		public void SetDocumentFileName(string value)
		{
			IntPtr handle = (Handle as GTKNativeControl).GetNamedHandle("HeaderBar");
			if (handle != IntPtr.Zero)
				Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_set_subtitle(handle, value);
			_DocumentFileName = value;
		}

		public bool GetStatusBarVisible()
		{
			return true;

		}
		public void SetStatusBarVisible(bool value)
		{
			if (Handle == null) return;

			IntPtr hStatusBar = (Handle as GTKNativeControl).GetNamedHandle("StatusBar");
			if (hStatusBar != IntPtr.Zero)
			{
				if (value)
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_show(hStatusBar);
				}
				else
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hStatusBar);
				}
			}
		}


		private List<Window> _GetToplevelWindowsRetval = null;
		public Window[] GetToplevelWindows()
		{
			if (_GetToplevelWindowsRetval != null)
			{
				// should not happen
				throw new InvalidOperationException();
			}

			_GetToplevelWindowsRetval = new List<Window>();
			IntPtr hList = Internal.GTK.Methods.GtkWindow.gtk_window_list_toplevels();
			Internal.GLib.Methods.g_list_foreach(hList, _AddToList, IntPtr.Zero);

			Window[] retval = _GetToplevelWindowsRetval.ToArray();
			_GetToplevelWindowsRetval = null;
			return retval;
		}
		private void /*GFunc*/ _AddToList(IntPtr data, IntPtr user_data)
		{
			if( _GetToplevelWindowsRetval == null)
			{
				throw new InvalidOperationException("_AddToList called before initializing the list");
			}

			Window window = new Window();
			_GetToplevelWindowsRetval.Add(window);
		}

		private void MenuItem_Activate(IntPtr handle, IntPtr data)
		{
			if (menuItemsByHandle.ContainsKey(handle))
			{
				MenuItem mi = menuItemsByHandle[handle];
				if (mi is CommandMenuItem)
				{
					(mi as CommandMenuItem).OnClick(EventArgs.Empty);
				}
			}
		}

		public WindowImplementation(Engine engine, Window window) : base(engine, window)
		{
			gc_MenuItem_Activated = new MBS.Framework.UserInterface.Engines.GTK.Internal.GObject.Delegates.GCallback(MenuItem_Activate);

			gc_Window_Activate = new MBS.Framework.UserInterface.Engines.GTK.Internal.GObject.Delegates.GCallback(Window_Activate);
			gc_Window_Closing = new Func<IntPtr, IntPtr, bool>(Window_Closing);
			gc_Window_Closed = new MBS.Framework.UserInterface.Engines.GTK.Internal.GObject.Delegates.GCallback(Window_Closed);
		}
		
		private void Window_Activate(IntPtr handle, IntPtr data)
		{
			Window window = (((UIApplication)Application.Instance).Engine as GTKEngine).GetControlByHandle(handle) as Window;
			if (window == null)
				return;

			InvokeMethod(window, "OnActivate", EventArgs.Empty);
		}

		private bool Window_Closing(IntPtr handle, IntPtr data)
		{
			Window window = (((UIApplication)Application.Instance).Engine as GTKEngine).GetControlByHandle(handle) as Window;
			if (window != null)
			{
				WindowClosingEventArgs e = new WindowClosingEventArgs(WindowCloseReason.UserClosing);
				InvokeMethod(window, "OnClosing", e);
				if (e.Cancel)
					return true;
			}
			return false;
		}

		private void Window_Closed(IntPtr handle, IntPtr data)
		{
			Window window = (((UIApplication)Application.Instance).Engine as GTKEngine).GetControlByHandle(handle) as Window;
			if (window != null)
			{
				InvokeMethod(window, "OnClosed", EventArgs.Empty);
				Engine.UnregisterControlHandle(window);
			}
		}

		private static IntPtr hDefaultAccelGroup = IntPtr.Zero;

		private void InitMenuItem(MenuItem menuItem, IntPtr hMenuShell, string accelPath = null)
		{
			IntPtr hMenuFile = (Engine as GTKEngine).InitMenuItem(menuItem, accelPath);
			if (hMenuFile != IntPtr.Zero)
				Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenuShell, hMenuFile);
		}

		protected override string GetControlTextInternal(Control control)
		{
			IntPtr hTitle = Internal.GTK.Methods.GtkWindow.gtk_window_get_title((Engine.GetHandleForControl(control) as GTKNativeControl).Handle);
			return Marshal.PtrToStringAuto (hTitle);
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			IntPtr hTitle = Marshal.StringToHGlobalAuto (text);
			Internal.GTK.Methods.GtkWindow.gtk_window_set_title((Engine.GetHandleForControl(control) as GTKNativeControl).Handle, hTitle);
		}

		protected override void SetControlBoundsInternal(Rectangle bounds)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Window window = (Control as Window);
			Internal.GTK.Methods.GtkWindow.gtk_window_move(handle, (int)bounds.X, (int)bounds.Y);
		}

		protected internal virtual void OnClosed(EventArgs e)
		{
			InvokeMethod((Control as Window), "OnClosed", e);
		}

		private Func<IntPtr, IntPtr, IntPtr, bool> window_state_event_d;
		private bool window_state_event(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkEvent*/ evt, IntPtr user_data)
		{
			Internal.GDK.Structures.GdkEventWindowState s_evt =
				(Internal.GDK.Structures.GdkEventWindowState)Marshal.PtrToStructure(evt, typeof(Internal.GDK.Structures.GdkEventWindowState));

			return true;
		}

		// [System.Diagnostics.DebuggerNonUserCode()]
		protected override NativeControl CreateControlInternal(Control control)
		{
			Window window = (control as Window);
			if (window == null) throw new InvalidOperationException();

			IntPtr handle = Internal.GTK.Methods.GtkApplicationWindow.gtk_application_window_new((((UIApplication)Application.Instance).Engine as GTKEngine).ApplicationHandle);

			window_state_event_d = new Func<IntPtr, IntPtr, IntPtr, bool>(window_state_event);
			Internal.GObject.Methods.g_signal_connect(handle, "window-state-event", window_state_event_d);

			GTKNativeControl ncContainer = (base.CreateControlInternal(control) as GTKNativeControl);
			IntPtr hContainer = ncContainer.Handle;

			if (window.Bounds != Rectangle.Empty)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_size_request(handle, (int)window.Bounds.Width, (int)window.Bounds.Height);
			}

			IntPtr hWindowContainer = Internal.GTK.Methods.GtkBox.gtk_vbox_new(false, 2);

			#region Menu Bar
			IntPtr hMenuBar = Internal.GTK.Methods.GtkMenuBar.gtk_menu_bar_new();

			if (window.MenuBar.Items.Count > 0)
			{
				if (hDefaultAccelGroup == IntPtr.Zero)
				{
					hDefaultAccelGroup = Internal.GTK.Methods.GtkAccelGroup.gtk_accel_group_new();
				}
				Internal.GTK.Methods.GtkWindow.gtk_window_add_accel_group(handle, hDefaultAccelGroup);

				// create the menu bar
				switch (window.CommandDisplayMode)
				{
					case CommandDisplayMode.CommandBar:
					case CommandDisplayMode.Both:
					{
						foreach (MenuItem menuItem in window.MenuBar.Items)
						{
							InitMenuItem(menuItem, hMenuBar, "<ApplicationFramework>");
						}
						break;
					}
				}
			}

			if (window.MenuBar.Items.Count > 0 && (window.CommandDisplayMode == CommandDisplayMode.CommandBar || window.CommandDisplayMode == CommandDisplayMode.Both))
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hMenuBar);
			}
			else
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hMenuBar);
			}
			Internal.GTK.Methods.GtkBox.gtk_box_pack_start(hWindowContainer, hMenuBar, false, true, 0);
			#endregion

			if (control is MainWindow)
			{
				#region Toolbars
				if ((control as MainWindow).CommandDisplayMode == CommandDisplayMode.CommandBar || (control as MainWindow).CommandDisplayMode == CommandDisplayMode.Both)
				{
					for (int i = 0; i < ((UIApplication)Application.Instance).CommandBars.Count; i++)
					{
						Toolbar tb = window.LoadCommandBar(((UIApplication)Application.Instance).CommandBars[i]);
						Engine.CreateControl(tb);

						IntPtr hToolbar = (Engine.GetHandleForControl(tb) as GTKNativeControl).Handle;
						bool showGrip = false; // tb.GripStyle == GripStyle.Visible;

						if (showGrip)
						{
							IntPtr hGripBox = Internal.GTK.Methods.GtkHandleBox.gtk_handle_box_new();
							Internal.GTK.Methods.GtkContainer.gtk_container_add(hGripBox, hToolbar);

							Internal.GTK.Methods.GtkBox.gtk_box_pack_start(hWindowContainer, hGripBox, false, true, 0);

							Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(hGripBox);
						}
						else
						{
							Internal.GTK.Methods.GtkBox.gtk_box_pack_start(hWindowContainer, hToolbar, false, true, 0);
						}
					}
				}
				#endregion
			}

			IntPtr hStatusBar = IntPtr.Zero;
			IntPtr hHeaderBar = IntPtr.Zero;

			if (hContainer != IntPtr.Zero)
			{
				Internal.GTK.Methods.GtkBox.gtk_box_pack_start(hWindowContainer, hContainer, true, true, 0);
			}

			Internal.GTK.Methods.GtkContainer.gtk_container_add(handle, hWindowContainer);

			Internal.GObject.Methods.g_signal_connect_after(handle, "show", gc_Window_Activate);

			Internal.GObject.Methods.g_signal_connect(handle, "delete_event", gc_Window_Closing, new IntPtr(0xDEADBEEF));
			Internal.GObject.Methods.g_signal_connect_after(handle, "destroy", gc_Window_Closed, new IntPtr(0xDEADBEEF));

			Internal.GTK.Methods.GtkWindow.gtk_window_set_default_size(handle, (int)window.Size.Width, (int)window.Size.Height);
			Internal.GTK.Methods.GtkWindow.gtk_window_set_decorated(handle, window.Decorated);
			Internal.GTK.Methods.GtkWindow.gtk_window_set_focus_on_map(handle, true);
			Internal.GTK.Methods.GtkWindow.gtk_window_set_icon_name(handle, window.IconName);
			Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_set_subtitle(handle, window.DocumentFileName);

			if (window.Decorated)
			{
				if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
				{
				}
				else
				{
					hHeaderBar = Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_new();
					Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_set_title(hHeaderBar, window.Text);
					Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_set_show_close_button(hHeaderBar, true);
					// Internal.GTK.Methods.GtkWindow.gtk_window_set_titlebar(handle, hHeaderBar);

					IntPtr hBBox = Internal.GTK.Methods.GtkBox.gtk_box_new(Internal.GTK.Constants.GtkOrientation.Horizontal);
					Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class(Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(hBBox), "linked");

					for (int i = 0; i < window.TitleBarButtons.Count; i++)
					{
						Button button = window.TitleBarButtons[i];
						Engine.CreateControl(button);

						IntPtr hButton = (Engine.GetHandleForControl(button) as GTKNativeControl).Handle;

						/*
						if (button.HorizontalAlignment == HorizontalAlignment.Left || button.HorizontalAlignment == HorizontalAlignment.Default)
						{
							Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_pack_start(hHeaderBar, hButton);
						}
						else if (button.HorizontalAlignment == HorizontalAlignment.Right)
						{
							Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_pack_end(hHeaderBar, hButton);
						}
						*/
						Internal.GTK.Methods.GtkBox.gtk_box_pack_start(hBBox, hButton, false, false, 0);
					}

					Internal.GTK.Methods.GtkHeaderBar.gtk_header_bar_pack_start(hHeaderBar, hBBox);
				}

				hStatusBar = Internal.GTK.Methods.GtkStatusBar.gtk_statusbar_new();
				Internal.GTK.Methods.GtkBox.gtk_box_pack_end(hWindowContainer, hStatusBar, false, true, 0);
			}
			switch (window.StartPosition)
			{
				case WindowStartPosition.Center:
				{
					Internal.GTK.Methods.GtkWindow.gtk_window_set_position(handle, Internal.GTK.Constants.GtkWindowPosition.Center);
					break;
				}
				case WindowStartPosition.Manual:
				{
					Internal.GTK.Methods.GtkWindow.gtk_window_set_position(handle, Internal.GTK.Constants.GtkWindowPosition.None);
					Internal.GTK.Methods.GtkWindow.gtk_window_move(handle, (int)window.Location.X, (int)window.Location.Y);
					break;
				}
			}
			/*
			if (window.CommandDisplayMode == CommandDisplayMode.CommandBar || window.CommandDisplayMode == CommandDisplayMode.Both)
			{
				foreach (CommandBar cb in ((UIApplication)Application.Instance).CommandBars)
				{
					window.Controls.Add(window.LoadCommandBar(cb));
				}
			}
			*/
			// HACK: required for Universal Editor splash screen to work
			// Internal.GTK.Methods.GtkWidget.gtk_widget_show_now(handle);
			// Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(handle);
			// Application.DoEvents();

			return new GTKNativeControl(handle, new KeyValuePair<string, IntPtr>[]
			{
				new KeyValuePair<string, IntPtr>("MenuBar", hMenuBar),
				new KeyValuePair<string, IntPtr>("StatusBar", hStatusBar),
				new KeyValuePair<string, IntPtr>("HeaderBar", hHeaderBar)
			});
		}

		protected override Dimension2D GetControlSizeInternal()
		{
			if (_Size != null)
			{
				// size has been set by code and window has not yet been resized
				return _Size;
			}
			int w = 0, h = 0;
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWindow.gtk_window_get_size(handle, ref w, ref h);
			return new Dimension2D(w, h);
		}

		private Dimension2D _Size = null;
		protected override void SetControlSizeInternal(Dimension2D value)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWindow.gtk_window_resize(handle, (int)value.Width, (int)value.Height);
			_Size = value;
		}

		protected override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWindow.gtk_window_set_default_size(handle, (int)Control.Size.Width, (int)Control.Size.Height);
		}

		protected override void OnResized(ResizedEventArgs e)
		{
			base.OnResized(e);
			_Size = null;
		}

		protected override void OnShown(EventArgs e)
		{
			if (!(Control as Window).StatusBar.Visible)
			{
				if (Handle != null)
				{
					IntPtr hStatusbar = (Handle as GTKNativeControl).GetNamedHandle("StatusBar");
					if (hStatusbar != IntPtr.Zero)
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hStatusbar);
					}
				}
			}

			base.OnShown(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			MouseEventArgs ee = GTKEngine.TranslateMouseEventArgs(e, (Handle as GTKNativeControl).Handle);
			base.OnMouseDown(ee);
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			MouseEventArgs ee = GTKEngine.TranslateMouseEventArgs(e, (Handle as GTKNativeControl).Handle);
			base.OnMouseUp(ee);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			MouseEventArgs ee = GTKEngine.TranslateMouseEventArgs(e, (Handle as GTKNativeControl).Handle);
			base.OnMouseMove(ee);
		}

		public void InsertMenuItem(int index, MenuItem item)
		{
			IntPtr hMenuBar = (Handle as GTKNativeControl).GetNamedHandle("MenuBar");
			IntPtr hMenuChild = (Engine as GTKEngine).InitMenuItem(item);
			Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_insert(hMenuBar, hMenuChild, index);
			Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(hMenuChild);
		}
		public void ClearMenuItems()
		{
			IntPtr hMenuBar = (Handle as GTKNativeControl).GetNamedHandle("MenuBar");
		}
		public void RemoveMenuItem(MenuItem item)
		{
			IntPtr hMenuBar = (Handle as GTKNativeControl).GetNamedHandle("MenuBar");
			IntPtr hMenuItem = ((Engine as GTKEngine).GetHandleForMenuItem(item) as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_destroy(hMenuItem);
		}

		private bool _FullScreen = false;
		public bool IsFullScreen()
		{
			return _FullScreen;
		}
		public void SetFullScreen(bool value)
		{
			if (value)
			{
				Internal.GTK.Methods.GtkWindow.gtk_window_fullscreen((Handle as GTKNativeControl).Handle);
			}
			else
			{
				Internal.GTK.Methods.GtkWindow.gtk_window_unfullscreen((Handle as GTKNativeControl).Handle);
			}
			_FullScreen = value;
		}

		protected override Vector2D GetLocationInternal()
		{
			return base.GetLocationInternal();
		}
		protected override void SetLocationInternal(Vector2D location)
		{
			base.SetLocationInternal(location);
		}
	}
}
