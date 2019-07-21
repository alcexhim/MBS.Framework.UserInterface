using System;
using System.Collections.Generic;
using System.ComponentModel;

using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Native;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[NativeImplementation(typeof(Window))]
	public class WindowImplementation : ContainerImplementation, IWindowNativeImplementation
	{
		private Dictionary<IntPtr, MenuItem> menuItemsByHandle = new Dictionary<IntPtr, MenuItem>();

		private Internal.GObject.Delegates.GCallback gc_MenuItem_Activated = null;

		private Internal.GObject.Delegates.GCallback gc_Window_Activate = null;
		private Internal.GObject.Delegates.GCallback gc_Window_Closing = null;
		private Internal.GObject.Delegates.GCallback gc_Window_Closed = null;

		public string GetIconName()
		{
			return Internal.GTK.Methods.gtk_window_get_icon_name ((Handle as GTKNativeControl).Handle);
		}
		public void SetIconName(string value)
		{
			Internal.GTK.Methods.gtk_window_set_icon_name ((Handle as GTKNativeControl).Handle, value);
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
			IntPtr hList = Internal.GTK.Methods.gtk_window_list_toplevels();
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
			gc_MenuItem_Activated = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback(MenuItem_Activate);

			gc_Window_Activate = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback(Window_Activate);
			gc_Window_Closing = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback(Window_Closing);
			gc_Window_Closed = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback(Window_Closed);
		}
		
		private void Window_Activate(IntPtr handle, IntPtr data)
		{
			Window window = (Application.Engine.GetControlByHandle(handle) as Window);
			if (window == null)
				return;

			InvokeMethod(window, "OnActivate", EventArgs.Empty);
		}

		private void Window_Closing(IntPtr handle, IntPtr data)
		{
			Window window = (Application.Engine.GetControlByHandle(handle) as Window);
			if (window != null)
			{
				CancelEventArgs e = new CancelEventArgs();
				InvokeMethod(window, "OnClosing", e);
			}
		}

		private void Window_Closed(IntPtr handle, IntPtr data)
		{
			Window window = (Application.Engine.GetControlByHandle(handle) as Window);
			if (window != null)
			{
				InvokeMethod(window, "OnClosed", EventArgs.Empty);
			}
		}

		private static IntPtr hDefaultAccelGroup = IntPtr.Zero;

		private void InitMenuItem(MenuItem menuItem, IntPtr hMenuShell, string accelPath = null)
		{
			if (menuItem is CommandMenuItem)
			{
				CommandMenuItem cmi = (menuItem as CommandMenuItem);
				if (accelPath != null)
				{

					string cmiName = cmi.Name;
					if (String.IsNullOrEmpty(cmiName))
					{
						cmiName = cmi.Text;
					}

					// clear out the possible mnemonic definitions
					cmiName = cmiName.Replace("_", String.Empty);

					accelPath += "/" + cmiName;
					if (cmi.Shortcut != null)
					{
						Internal.GTK.Methods.gtk_accel_map_add_entry(accelPath, GTKEngine.GetAccelKeyForKeyboardKey(cmi.Shortcut.Key), GTKEngine.KeyboardModifierKeyToGdkModifierType(cmi.Shortcut.ModifierKeys));
					}
				}

				IntPtr hMenuFile = Internal.GTK.Methods.gtk_menu_item_new();
				Internal.GTK.Methods.gtk_menu_item_set_label(hMenuFile, cmi.Text);
				Internal.GTK.Methods.gtk_menu_item_set_use_underline(hMenuFile, true);

				if (menuItem.HorizontalAlignment == MenuItemHorizontalAlignment.Right)
				{
					Internal.GTK.Methods.gtk_menu_item_set_right_justified(hMenuFile, true);
				}

				if (cmi.Items.Count > 0)
				{
					IntPtr hMenuFileMenu = Internal.GTK.Methods.gtk_menu_new();

					if (accelPath != null)
					{
						if (hDefaultAccelGroup == IntPtr.Zero)
						{
							hDefaultAccelGroup = Internal.GTK.Methods.gtk_accel_group_new();
						}
						Internal.GTK.Methods.gtk_menu_set_accel_group(hMenuFileMenu, hDefaultAccelGroup);
					}

					foreach (MenuItem menuItem1 in cmi.Items)
					{
						InitMenuItem(menuItem1, hMenuFileMenu, accelPath);
					}

					Internal.GTK.Methods.gtk_menu_item_set_submenu(hMenuFile, hMenuFileMenu);
				}

				menuItemsByHandle[hMenuFile] = cmi;
				Internal.GObject.Methods.g_signal_connect(hMenuFile, "activate", gc_MenuItem_Activated, IntPtr.Zero);

				if (accelPath != null)
				{
					Internal.GTK.Methods.gtk_menu_item_set_accel_path(hMenuFile, accelPath);
				}

				Internal.GTK.Methods.gtk_menu_shell_append(hMenuShell, hMenuFile);
			}
			else if (menuItem is SeparatorMenuItem)
			{
				// IntPtr hMenuFile = Internal.GTK.Methods.gtk_separator_new (Internal.GTK.Constants.GtkOrientation.Horizontal);
				IntPtr hMenuFile = Internal.GTK.Methods.gtk_separator_menu_item_new();
				Internal.GTK.Methods.gtk_menu_shell_append(hMenuShell, hMenuFile);
			}
		}

		protected override string GetControlTextInternal(Control control)
		{
			return Internal.GTK.Methods.gtk_window_get_title(Engine.GetHandleForControl(control));
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			Internal.GTK.Methods.gtk_window_set_title(Engine.GetHandleForControl(control), control.Text);
		}

		[System.Diagnostics.DebuggerNonUserCode()]
		protected override NativeControl CreateControlInternal(Control control)
		{
			Window window = (control as Window);
			if (window == null) throw new InvalidOperationException();

			IntPtr handle = Internal.GTK.Methods.gtk_window_new(Internal.GTK.Constants.GtkWindowType.TopLevel);
			GTKNativeControl ncContainer = (base.CreateControlInternal(control) as GTKNativeControl);
			IntPtr hContainer = ncContainer.Handle;

			if (window.Bounds != Rectangle.Empty)
			{
				Internal.GTK.Methods.gtk_widget_set_size_request(handle, (int)window.Bounds.Width, (int)window.Bounds.Height);
			}

			IntPtr hWindowContainer = Internal.GTK.Methods.gtk_vbox_new(false, 2);

			#region Menu Bar

			if (hDefaultAccelGroup == IntPtr.Zero)
			{
				hDefaultAccelGroup = Internal.GTK.Methods.gtk_accel_group_new();
			}
			Internal.GTK.Methods.gtk_window_add_accel_group(handle, hDefaultAccelGroup);

			// create the menu bar
			IntPtr hMenuBar = Internal.GTK.Methods.gtk_menu_bar_new();

			foreach (MenuItem menuItem in window.MenuBar.Items)
			{
				InitMenuItem(menuItem, hMenuBar, "<ApplicationFramework>");
			}

			Internal.GTK.Methods.gtk_box_pack_start(hWindowContainer, hMenuBar, false, true, 0);

			#endregion

			if (hContainer != IntPtr.Zero)
			{
				Internal.GTK.Methods.gtk_box_pack_end(hWindowContainer, hContainer, true, true, 0);
			}

			Internal.GTK.Methods.gtk_container_add(handle, hWindowContainer);

			Internal.GObject.Methods.g_signal_connect_after(handle, "show", gc_Window_Activate);

			Internal.GObject.Methods.g_signal_connect(handle, "destroy", gc_Window_Closing, new IntPtr(0xDEADBEEF));
			Internal.GObject.Methods.g_signal_connect_after(handle, "destroy", gc_Window_Closed, new IntPtr(0xDEADBEEF));

			Internal.GTK.Methods.gtk_window_set_default_size(handle, (int)window.Size.Width, (int)window.Size.Height);
			Internal.GTK.Methods.gtk_window_set_decorated(handle, window.Decorated);
			Internal.GTK.Methods.gtk_window_set_focus_on_map(handle, true);
			IntPtr hHeaderBar = Internal.GTK.Methods.gtk_header_bar_new();
			Internal.GTK.Methods.gtk_header_bar_set_title(hHeaderBar, window.Text);
			Internal.GTK.Methods.gtk_window_set_titlebar(handle, hHeaderBar);
			Internal.GTK.Methods.gtk_window_set_icon_name(handle, window.IconName);

			return new GTKNativeControl(handle);
		}
	}
}
