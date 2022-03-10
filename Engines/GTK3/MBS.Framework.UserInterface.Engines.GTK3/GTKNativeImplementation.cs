//
//  GTKNativeImplementation.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019
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
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.ListView;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Engines.GTK3.Internal.GDK;
using System.Runtime.InteropServices;

namespace MBS.Framework.UserInterface.Engines.GTK3
{
	public abstract class GTKNativeImplementation : NativeImplementation
	{
		public GTKNativeImplementation(Engine engine, Control control) : base(engine, control)
		{
			InitializeEventHandlers();
		}

		protected override bool IsControlVisibleInternal()
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			return Internal.GTK.Methods.GtkWidget.gtk_widget_is_visible(handle);
		}
		protected override void SetControlVisibilityInternal (bool visible)
		{
			IntPtr handle = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
			if (visible) {
				Internal.GTK.Methods.GtkWidget.gtk_widget_show (handle); // should 'all' be here? guess it doesn't matter
			} else {
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide (handle);
			}
		}

		protected override bool SupportsEngineInternal(Type engineType)
		{
			return (engineType == typeof(GTK3Engine));
		}

		protected override void InvalidateInternal(int x, int y, int width, int height)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_queue_draw_area(handle, x, y, width, height);
		}

		protected override void DestroyInternal()
		{
			if (Control is Dialog)
			{
				IntPtr handle = (Handle as GTKNativeControl).GetNamedHandle("dialog");
				// this way is recommended per GTK3.0 docs:
				// "destroying the dialog during gtk_dialog_run() is a very bad idea, because your post-run code won't know whether the dialog was destroyed or not"
				Internal.GTK.Methods.GtkDialog.gtk_dialog_response(handle, GTK3Engine.DialogResultToGtkResponseType((Control as Dialog).DialogResult));
			}
			else
			{
				IntPtr handle = (Handle as GTKNativeControl).Handle;
				if (Internal.GTK.Methods.Gtk.gtk_get_major_version() < 4)
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_destroy(handle);
				}
				else
				{
					// FIXME: ecannot destroy widget in GTK4???
				}
			}
		}

		internal virtual void RegisterDragSourceGTK(IntPtr handle, Internal.GDK.Constants.GdkModifierType modifiers, Internal.GTK.Structures.GtkTargetEntry[] targets, Internal.GDK.Constants.GdkDragAction actions)
		{
			Internal.GTK.Methods.GtkDragSource.gtk_drag_source_set(handle, modifiers, targets, targets.Length, actions);
		}
		internal virtual void RegisterDropTargetGTK(IntPtr handle, Internal.GDK.Constants.GdkModifierType modifiers, Internal.GTK.Structures.GtkTargetEntry[] targets, Internal.GDK.Constants.GdkDragAction actions)
		{
			if (Internal.GTK.Methods.Gtk.gtk_get_major_version() < 4)
			{
				Internal.GTK.Methods.GtkDragDest.gtk_drag_dest_set(handle, Internal.GTK.Constants.GtkDestDefaults.All, targets, targets.Length, actions);
			}
			else
			{
				Console.Error.WriteLine("gtk_drag_dest_set  -- removed in GTK4!");
			}
		}
		protected override void RegisterDropTargetInternal(Control control, DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			Internal.GDK.Constants.GdkModifierType modifiers = GTK3Engine.KeyboardModifierKeyToGdkModifierType(modifierKeys) | GTK3Engine.MouseButtonsToGdkModifierType(buttons);

			IntPtr handle = (Engine.GetHandleForControl(control) as GTKNativeControl).Handle;
			if (handle == IntPtr.Zero) return;

			RegisterDropTargetGTK(handle, modifiers, GTK3Engine.DragDropTargetToGtkTargetEntry(targets), GTK3Engine.DragDropEffectToGdkDragAction(actions));
		}
		protected override void RegisterDragSourceInternal(Control control, DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			Internal.GDK.Constants.GdkModifierType modifiers = GTK3Engine.KeyboardModifierKeyToGdkModifierType(modifierKeys) | GTK3Engine.MouseButtonsToGdkModifierType(buttons);

			IntPtr handle = (Engine.GetHandleForControl(control) as GTKNativeControl).Handle;
			if (handle == IntPtr.Zero) return;

			RegisterDragSourceGTK(handle, modifiers, GTK3Engine.DragDropTargetToGtkTargetEntry(targets), GTK3Engine.DragDropEffectToGdkDragAction(actions));
		}

		protected override void SetMarginInternal(Padding value)
		{
			if (value != Padding.Empty)
			{
				IntPtr handle = (Handle as GTKNativeControl).Handle;
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_top(handle, value.Top);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_bottom(handle, value.Bottom);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_start(handle, value.Left);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_end(handle, value.Right);
			}
		}


		protected override void OnCreated (EventArgs e)
		{
			base.OnCreated (e);

			IntPtr handle = (Handle as GTKNativeControl).Handle;

			if (Control.Size != Dimension2D.Empty && Control.Size != null)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_size_request(handle, (int)Control.Size.Width, (int)Control.Size.Height);
			}

			if (Control.Margin != Padding.Empty)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_top(handle, Control.Margin.Top);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_bottom(handle, Control.Margin.Bottom);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_start(handle, Control.Margin.Left);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_margin_end(handle, Control.Margin.Right);
			}

			SetupCommonEvents (FindEventHandlingHandle ((Handle as GTKNativeControl), Control));

			IntPtr hCtrl = handle;
			if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
			{
			}
			else
			{
				IntPtr hStyleContext = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context (hCtrl);
				foreach (ControlStyleClass cls in Control.Style.Classes) {
					Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class (hStyleContext, cls.Value);
				}
			}

			if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
			{
			}
			else
			{
				switch (Control.HorizontalAlignment)
				{
					case HorizontalAlignment.Center:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.Center);
						break;
					}
					case HorizontalAlignment.Default:
					case HorizontalAlignment.Justify:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.Fill);
						break;
					}
					case HorizontalAlignment.Left:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.Start);
						break;
					}
					case HorizontalAlignment.Right:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.End);
						break;
					}
				}

				switch (Control.VerticalAlignment)
				{
					case VerticalAlignment.Baseline:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.Baseline);
						break;
					}
					case VerticalAlignment.Bottom:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.End);
						break;
					}
					case VerticalAlignment.Default:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.Fill);
						break;
					}
					case VerticalAlignment.Middle:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.Center);
						break;
					}
					case VerticalAlignment.Top:
					{
						Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.Start);
						break;
					}
				}
			}
		}

		protected override void SetFocusInternal ()
		{
			IntPtr hCtrl = (Handle as GTKNativeControl).EventHandle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_grab_focus (hCtrl);
		}

		protected override Rectangle GetControlBoundsInternal()
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GDK.Structures.GdkRectangle alloc = new Internal.GDK.Structures.GdkRectangle();
			Internal.GTK.Methods.GtkWidget.gtk_widget_get_allocation(handle, ref alloc);
			return new Rectangle(alloc.x, alloc.y, alloc.width, alloc.height);
		}

		protected override Dimension2D GetControlSizeInternal()
		{
			if (!Engine.IsControlCreated(Control))
			{
				return Dimension2D.Empty;
			}

			Internal.GDK.Structures.GdkRectangle hAlloc = new Internal.GDK.Structures.GdkRectangle();
			Internal.GTK.Methods.GtkWidget.gtk_widget_get_allocation((Handle as GTKNativeControl).Handle, ref hAlloc);

			return new Dimension2D(hAlloc.width, hAlloc.height);
		}

		protected override void SetControlSizeInternal(Dimension2D value)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_size_request(handle, (int)value.Width, (int)value.Height);
		}


		#region Event Handlers
		// converting these into standalone fields solved a HUGE (and esoteric) crash in handling keyboard events...
		private Internal.GObject.Delegates.GCallbackV1I gc_show_handler = null;
		private Internal.GObject.Delegates.GCallback gc_realize_handler = null;
		private Internal.GObject.Delegates.GCallback gc_unrealize_handler = null;
		private Internal.GObject.Delegates.GCallback gc_map_handler = null;
		private Internal.GObject.Delegates.GCallbackV3I gc_map_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_button_press_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_button_release_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_motion_notify_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_focus_in_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_focus_out_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_key_press_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_key_release_event_handler = null;

		private Internal.GTK.Delegates.GtkDragEvent gc_drag_begin_handler = null;
		private Internal.GTK.Delegates.GtkDragEvent gc_drag_data_delete_handler = null;
		private Internal.GTK.Delegates.GtkDragDataGetEvent gc_drag_data_get_handler = null;

		private Func<IntPtr, IntPtr, bool> gc_destroy_event_handler = null;
		private bool gc_destroy_event(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkEventKey*/ evt)
		{
			// destroy all handles associated with widget
			Control ctl = (Engine as GTK3Engine).GetControlByHandle(widget);
			Engine.UnregisterControlHandle(ctl);
			return false;
		}
		private Func<IntPtr, IntPtr, bool> gc_delete_event_handler = null;
		private bool gc_delete_event(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkEventKey*/ evt)
		{
			// destroy all handles associated with widget
			Control ctl = (Engine as GTK3Engine).GetControlByHandle(widget);
			// Engine.UnregisterControlHandle(ctl);
			// moved to WindowImplementation
			return false;
		}

		private void InitializeEventHandlers()
		{
			gc_realize_handler = new Internal.GObject.Delegates.GCallback(gc_realize);
			gc_unrealize_handler = new Internal.GObject.Delegates.GCallback(gc_unrealize);
			gc_map_handler = new Internal.GObject.Delegates.GCallback(gc_map);
			gc_map_event_handler = new Internal.GObject.Delegates.GCallbackV3I(gc_map_event);
			gc_show_handler = new Internal.GObject.Delegates.GCallbackV1I(gc_show);
			gc_button_press_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_button_press_event);
			gc_button_release_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_button_release_event);
			gc_motion_notify_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_motion_notify_event);
			gc_focus_in_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_focus_in_event);
			gc_focus_out_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_focus_out_event);
			gc_key_press_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_key_press_event);
			gc_key_release_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_key_release_event);
			gc_drag_begin_handler = new Internal.GTK.Delegates.GtkDragEvent(gc_drag_begin);
			gc_drag_data_delete_handler = new Internal.GTK.Delegates.GtkDragEvent(gc_drag_data_delete);
			gc_drag_data_get_handler = new Internal.GTK.Delegates.GtkDragDataGetEvent(gc_drag_data_get);
			gc_destroy_event_handler = new Func<IntPtr, IntPtr, bool>(gc_destroy_event);
			gc_delete_event_handler = new Func<IntPtr, IntPtr, bool>(gc_delete_event);
			gc_configure_event_handler = new Func<IntPtr, IntPtr, IntPtr, bool>(gc_configure_event);
		}

		private Func<IntPtr, IntPtr, IntPtr, bool> gc_configure_event_handler = null;
		private bool gc_configure_event(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkEvent*/ evt, IntPtr user_data)
		{
			// we cannot pass this param explicitly
			// MUST USE INTPTR THEN PTRTOSTRUCTURE!
			Internal.GDK.Structures.GdkEventConfigure e = (Internal.GDK.Structures.GdkEventConfigure)System.Runtime.InteropServices.Marshal.PtrToStructure(evt, typeof(Internal.GDK.Structures.GdkEventConfigure));

			Control ctl = (Engine as GTK3Engine).GetControlByHandle(widget);
			if (ctl != null)
			{
				if (ctl.ControlImplementation != null)
				{
					if ((int)ctl.Size.Width != e.width || (int)ctl.Size.Height != e.height)
					{
						Dimension2D oldSize = ctl.Size;
						Dimension2D newSize = new Framework.Drawing.Dimension2D(e.width, e.height);
						ResizingEventArgs ee = new ResizingEventArgs(oldSize, newSize);
						InvokeMethod(ctl.ControlImplementation, "OnResizing", new object[] { ee });

						if (ee.Cancel)
							return true;

						MBS.Framework.Reflection.SetField(ctl, "mvarSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy, newSize);

						ResizedEventArgs ee1 = new ResizedEventArgs(oldSize, newSize);
						InvokeMethod(ctl.ControlImplementation, "OnResized", new object[] { ee1 });
					}
				}
			}
			return false;
		}


		/// <summary>
		/// Connects the native GTK signals for the base GtkWidget class to the control with the given handle.
		/// </summary>
		/// <param name="nativeHandle">The handle of the control for which to connect signals</param>
		private void SetupCommonEvents(IntPtr nativeHandle)
		{
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "motion_notify_event", gc_motion_notify_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "button_press_event", gc_button_press_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "button_release_event", gc_button_release_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "focus_in_event", gc_focus_in_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "focus_out_event", gc_focus_out_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "key_press_event", gc_key_press_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "key_release_event", gc_key_release_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "realize", gc_realize_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "unrealize", gc_unrealize_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "show", gc_show_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "map", gc_map_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "map_event", gc_map_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "destroy_event", gc_destroy_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "delete_event", gc_delete_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "configure_event", gc_configure_event_handler);

			Internal.GObject.Methods.g_signal_connect_after(nativeHandle, "drag_begin", gc_drag_begin_handler);
			Internal.GObject.Methods.g_signal_connect_after(nativeHandle, "drag_data_delete", gc_drag_data_delete_handler);
			Internal.GObject.Methods.g_signal_connect_after(nativeHandle, "drag_data_get", gc_drag_data_get_handler);
		}

		private IntPtr GetScrolledWindowChild(IntPtr hScrolledWindow)
		{
			IntPtr hList = Internal.GTK.Methods.GtkContainer.gtk_container_get_children(hScrolledWindow);
			IntPtr hTreeView = Internal.GLib.Methods.g_list_nth_data(hList, 0);
			return hTreeView;
		}

		/// <summary>
		/// Returns the actual control handle (for event signaling) if a control is e.g. surrounded by GtkScrolledWindow
		/// </summary>
		private IntPtr FindEventHandlingHandle(GTKNativeControl fakeHandle, Control ctl)
		{
			if (ctl is ListViewControl)
			{
				return GetScrolledWindowChild(fakeHandle.Handle);
			}
			else
			{
				IntPtr hEventBox = fakeHandle.GetNamedHandle("EventBox");
				if (hEventBox != IntPtr.Zero)
					return hEventBox;
			}
			return fakeHandle.EventHandle;
		}

		private void gc_show(IntPtr /*GtkWidget*/ widget)
		{
			if (this == null) return;

			OnShown(EventArgs.Empty);
		}

		private void gc_drag_begin(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkDragContext*/ context, IntPtr user_data)
		{
			if (this == null) return;

			DragEventArgs e = new DragEventArgs();
			OnDragBegin (e);
		}
		private void gc_drag_data_delete(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkDragContext*/ context, IntPtr user_data)
		{
			if (this == null) return;

			EventArgs e = new EventArgs();
			OnDragDataDelete (e);
		}
		private void gc_drag_data_get(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkDragContext*/ context, IntPtr /*GtkSelectionData*/ data, uint info, uint time, IntPtr user_data)
		{
			if (this == null) return;

			DragDropDataRequestEventArgs e = new DragDropDataRequestEventArgs(null);
			OnDragDropDataRequest (e);
			if (e.Cancel) return;

			if (e.Data is string)
			{
				Internal.GTK.Methods.GtkSelection.gtk_selection_data_set_text(data, ((string)e.Data), ((string)e.Data).Length);
			}
			else if (e.Data is byte[])
			{
				Internal.GTK.Methods.GtkSelection.gtk_selection_data_set(data, IntPtr.Zero, 8, ((byte[])e.Data), ((byte[])e.Data).Length);
			}
			else if (e.Data is string[])
			{
				string[] array = (string[])e.Data;
				if (array.Length > 0)
				{
					if (array[0].Contains("://"))
					{
						Internal.GTK.Methods.GtkSelection.gtk_selection_data_set_uris(data, array);
					}
				}
			}
			else if (e.Data == null)
			{
			}
		}

		private void gc_realize(IntPtr /*GtkWidget*/ widget, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_realize: ControlImplementation is NULL");
				return;
			}

			// Internal.GTK.Methods.GtkWindow.gtk_window_set_type_hint(widget, WindowTypeHintToGdkWindowTypeHint(Control.TypeHint));

			OnRealize(EventArgs.Empty);
		}

		private static Constants.GdkWindowTypeHint WindowTypeHintToGdkWindowTypeHint(WindowTypeHint typeHint)
		{
			switch (typeHint)
			{
				case WindowTypeHint.Combo: return Constants.GdkWindowTypeHint.Combo;
				case WindowTypeHint.Desktop: return Constants.GdkWindowTypeHint.Desktop;
				case WindowTypeHint.Dialog: return Constants.GdkWindowTypeHint.Dialog;
				case WindowTypeHint.Dnd: return Constants.GdkWindowTypeHint.Dnd;
				case WindowTypeHint.Dock: return Constants.GdkWindowTypeHint.Dock;
				case WindowTypeHint.DropdownMenu: return Constants.GdkWindowTypeHint.DropdownMenu;
				case WindowTypeHint.Menu: return Constants.GdkWindowTypeHint.Menu;
				case WindowTypeHint.Normal: return Constants.GdkWindowTypeHint.Normal;
				case WindowTypeHint.Notification: return Constants.GdkWindowTypeHint.Notification;
				case WindowTypeHint.PopupMenu: return Constants.GdkWindowTypeHint.PopupMenu;
				case WindowTypeHint.SplashScreen: return Constants.GdkWindowTypeHint.SplashScreen;
				case WindowTypeHint.Toolbar: return Constants.GdkWindowTypeHint.Toolbar;
				case WindowTypeHint.Tooltip: return Constants.GdkWindowTypeHint.Tooltip;
				case WindowTypeHint.Utility: return Constants.GdkWindowTypeHint.Utility;
			}
			return Constants.GdkWindowTypeHint.Normal;
		}

		private void gc_unrealize(IntPtr /*GtkWidget*/ widget, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_unrealize: ControlImplementation is NULL");
				return;
			}

			OnUnrealize(EventArgs.Empty);
		}
		private void gc_map(IntPtr /*GtkWidget*/ widget, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_map: ControlImplementation is NULL");
				return;
			}

			OnMapping(EventArgs.Empty);
		}




		protected static void SetWmClass(string name, string @class, IntPtr handle)
		{
			var a = new Internal.X11.Structures.XClassHint
			{
				res_name = Marshal.StringToCoTaskMemAnsi(name),
				res_class = Marshal.StringToCoTaskMemAnsi(@class)
			};
			IntPtr classHints = Marshal.AllocCoTaskMem(Marshal.SizeOf(a));
			Marshal.StructureToPtr(a, classHints, true);

			IntPtr hWndGdk = Internal.GTK.Methods.GtkWidget.gtk_widget_get_window(handle);
			IntPtr hDpyGdk = Internal.GDK.Methods.gdk_window_get_display(hWndGdk);
			IntPtr hDpyX11 = Internal.GDK.Methods.gdk_x11_display_get_xdisplay(hDpyGdk);
			IntPtr hWndX11 = Internal.GDK.Methods.gdk_x11_window_get_xid(hWndGdk);
			if (hWndX11 != IntPtr.Zero)
			{
				Internal.X11.Methods.XSetClassHint(hDpyX11, hWndX11, classHints);
			}
			else
			{
				Console.WriteLine("uwt: gtk: SetWmClass FAILED - not running under X11");
			}

			Marshal.FreeCoTaskMem(a.res_name);
			Marshal.FreeCoTaskMem(a.res_class);

			Marshal.FreeCoTaskMem(classHints);
		}

		private void gc_map_event(IntPtr /*GtkWidget*/ widget, IntPtr evt, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_map_event: ControlImplementation is NULL");
				return;
			}

			if (Control.WindowName != null || Control.WindowClass != null)
			{
				string windowName = Control.WindowName;
				string windowClass = Control.WindowClass;

				if (windowName == null)
				{
					windowName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
				}
				else if (windowClass == null)
				{
					windowClass = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
				}
				SetWmClass(windowName, windowClass, widget);
			}

			OnMapped(EventArgs.Empty);
		}

		private bool gc_focus_in_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_focus_in_event: ControlImplementation is NULL");
				return false;
			}

			EventArgs ee = EventArgs.Empty;
			foreach (EventFilter eh in Application.Instance.EventFilters)
			{
				if ((eh.EventType & EventFilterType.GotFocus) == EventFilterType.GotFocus)
				{
					if (eh.Process(ref ee, EventFilterType.GotFocus))
						return true;
				}
			}
			OnGotFocus(ee);
			return false;
		}

		private bool gc_focus_out_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_focus_out_event: ControlImplementation is NULL");
				return false;
			}

			EventArgs ee = EventArgs.Empty;
			foreach (EventFilter eh in Application.Instance.EventFilters)
			{
				if ((eh.EventType & EventFilterType.LostFocus) == EventFilterType.LostFocus)
				{
					if (eh.Process(ref ee, EventFilterType.LostFocus))
						return true;
				}
			}
			OnLostFocus (ee);
			return false;
		}

		private bool gc_key_press_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_key_press_event: ControlImplementation is NULL");
				return false;
			}

			// we cannot pass this param explicitly
			// MUST USE INTPTR THEN PTRTOSTRUCTURE!
			Internal.GDK.Structures.GdkEventKey e = (Internal.GDK.Structures.GdkEventKey)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventKey));

			KeyEventArgs ee = GTK3Engine.GdkEventKeyToKeyEventArgs(e);
			foreach (EventFilter eh in Application.Instance.EventFilters)
			{
				if (eh is EventFilter<KeyEventArgs> && (eh.EventType & EventFilterType.KeyDown) == EventFilterType.KeyDown)
				{
					if (((EventFilter<KeyEventArgs>)eh).Process(ref ee, EventFilterType.KeyDown))
						return true;
				}
			}
			OnKeyDown(ee);
			return ee.Cancel;
		}
		private bool gc_key_release_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_key_release_event: ControlImplementation is NULL");
				return false;
			}

			// we cannot pass this param explicitly
			// MUST USE INTPTR THEN PTRTOSTRUCTURE!
			Internal.GDK.Structures.GdkEventKey e = (Internal.GDK.Structures.GdkEventKey)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventKey));

			KeyEventArgs ee = GTK3Engine.GdkEventKeyToKeyEventArgs(e);
			foreach (EventFilter eh in Application.Instance.EventFilters)
			{
				if (eh is EventFilter<KeyEventArgs> && (eh.EventType & EventFilterType.KeyUp) == EventFilterType.KeyUp)
				{
					if (((EventFilter<KeyEventArgs>)eh).Process(ref ee, EventFilterType.KeyUp))
						return true;
				}
			}
			
			OnKeyUp(ee);
			return ee.Cancel;
		}

		protected override void InitializeControlPropertiesInternal(NativeControl native)
		{
			IntPtr handle = (native as GTKNativeControl).Handle;

			Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(handle, Control.Enabled);
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_tooltip_text(handle, Control.TooltipText);
		}

		protected override void UpdateControlLayoutInternal()
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;

			if (Control.MinimumSize != Dimension2D.Empty) {
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_size_request (handle, (int)Control.MinimumSize.Width, (int)Control.MinimumSize.Height);
			}

			try{
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_focus_on_click (handle, Control.FocusOnClick);
			}
			catch (EntryPointNotFoundException ex) {
				// we must be using an old version of Gtk
			}
		}

		protected override IVirtualControlContainer GetParentControlInternal()
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			IntPtr hParent = Internal.GTK.Methods.GtkWidget.gtk_widget_get_parent(handle);
			if (hParent == IntPtr.Zero)
				return null;

			Control ctl = (Engine as GTK3Engine).GetControlByHandle(hParent);
			if (ctl != null)
			{
				if (ctl is IVirtualControlContainer)
					return (ctl as IVirtualControlContainer);
				return null;
			}
			else
			{
				IntPtr hTestParent = hParent;
				while (hTestParent != IntPtr.Zero)
				{
					hTestParent = Internal.GTK.Methods.GtkWidget.gtk_widget_get_parent(hTestParent);
					ctl = (Engine as GTK3Engine).GetControlByHandle(hTestParent);
					if (ctl is IVirtualControlContainer)
						return (ctl as IVirtualControlContainer);
				}
			}
			return new GTKNativeControlContainer(hParent);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="GTKNativeImplementation" /> uses context menu handling provided by the subclass.
		/// </summary>
		/// <value><c>true</c> if context menu handling should be provided by the subclass; otherwise, <c>false</c>.</value>
		protected virtual bool SubclassHandlesContextMenu { get; } = false;

		private MouseButtons _mousedown_buttons = MouseButtons.None;
		private bool _mouse_double_click = false;
		private bool gc_button_press_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_button_press_event: ControlImplementation is NULL");
				return false;
			}

			Internal.GDK.Structures.GdkEventButton e = (Internal.GDK.Structures.GdkEventButton)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventButton));
			MouseEventArgs ee = GTK3Engine.GdkEventButtonToMouseEventArgs(e);
			if (e.type == MBS.Framework.UserInterface.Engines.GTK3.Internal.GDK.Constants.GdkEventType.DoubleButtonPress) {
				_mouse_double_click = true;
				return false;
			}
			else
			{
				_mouse_double_click = false;
			}

			_mousedown_buttons = ee.Buttons;

			foreach (EventFilter eh in Application.Instance.EventFilters)
			{
				if (eh is EventFilter<MouseEventArgs> && (eh.EventType & EventFilterType.MouseDown) == EventFilterType.MouseDown)
				{
					if (((EventFilter<MouseEventArgs>)eh).Process(ref ee, EventFilterType.MouseDown))
						return true;
				}
			}
			if (!_mouse_double_click)
			{
				OnMouseDown(ee);
			}

			if (ee.Handled) return true;

			if (ee.Buttons == MouseButtons.Secondary && !SubclassHandlesContextMenu)
			{
				// default implementation - display a context menu if we have one set
				// moved this up here to give us a chance to add a context menu if we don't have one associated yet
				OnBeforeContextMenu(ee);

				if (Control.ContextMenu != null)
				{
					IntPtr hMenu = (Engine as GTK3Engine).BuildMenu(Control.ContextMenu);

					foreach (MenuItem mi in Control.ContextMenu.Items)
					{
						RecursiveApplyMenuItemVisibility(mi);
					}
					Internal.GTK.Methods.GtkWidget.gtk_widget_show(hMenu);
					Internal.GTK.Methods.GtkMenu.gtk_menu_popup_at_pointer(hMenu, IntPtr.Zero);

					OnAfterContextMenu(ee);
				}
			}

			return false;
		}

		private void RecursiveApplyMenuItemVisibility(MenuItem mi)
		{
			if (mi == null)
				return;

			IntPtr hMi = ((Engine as GTK3Engine).GetHandleForMenuItem(mi) as GTKNativeControl).Handle;
			if (mi.Visible)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hMi);
			}
			else
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hMi);
			}

			if (mi is CommandMenuItem)
			{
				foreach (MenuItem mi1 in (mi as CommandMenuItem).Items)
				{
					RecursiveApplyMenuItemVisibility(mi1);
				}
			}
		}

		private bool gc_motion_notify_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_motion_notify_event: ControlImplementation is NULL");
				return false;
			}

			Internal.GDK.Structures.GdkEventMotion e = (Internal.GDK.Structures.GdkEventMotion)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventMotion));

			MouseEventArgs ee = GTK3Engine.GdkEventMotionToMouseEventArgs(e);
			ee = new MouseEventArgs(ee.X, ee.Y, _mousedown_buttons, ee.ModifierKeys);

			foreach (EventFilter eh in Application.Instance.EventFilters)
			{
				if (eh is EventFilter<MouseEventArgs> && (eh.EventType & EventFilterType.MouseMove) == EventFilterType.MouseMove)
				{
					if (((EventFilter<MouseEventArgs>)eh).Process(ref ee, EventFilterType.MouseMove))
						return true;
				}
			}
			OnMouseMove(ee);

			if (ee.Handled) return true;

			// TRUE to stop other handlers from being invoked for the event. FALSE to propagate the event further.
			return false;
		}
		private bool gc_button_release_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			if (this == null)
			{
				Console.WriteLine("uwt: gtk: ERROR: gc_button_release_event: ControlImplementation is NULL");
				return false;
			}

			_mousedown_buttons = MouseButtons.None;
			Internal.GDK.Structures.GdkEventButton e = (Internal.GDK.Structures.GdkEventButton)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventButton));
			MouseEventArgs ee = GTK3Engine.GdkEventButtonToMouseEventArgs(e);

			foreach (EventFilter eh in Application.Instance.EventFilters)
			{
				if (eh is EventFilter<MouseEventArgs> && (eh.EventType & EventFilterType.MouseUp) == EventFilterType.MouseUp)
				{
					if (((EventFilter<MouseEventArgs>)eh).Process(ref ee, EventFilterType.MouseUp))
						return true;
				}
			}
			OnMouseUp(ee);

			if (ee.Buttons == MouseButtons.Primary) {
				if (_mouse_double_click) {
					OnMouseDoubleClick (ee);
					_mouse_double_click = false;
				} else {
					OnClick (ee);
				}
			}

			if (ee.Handled) return true;
			return false;
		}


		#endregion

		protected override string GetTooltipTextInternal()
		{
			return string.Empty;
			return Internal.GTK.Methods.GtkWidget.gtk_widget_get_tooltip_text((Handle as GTKNativeControl).Handle);
		}
		protected override void SetTooltipTextInternal(string value)
		{
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_tooltip_text((Handle as GTKNativeControl).Handle, value);
		}

		protected override Cursor GetCursorInternal()
		{
			IntPtr hGdkWindow = Internal.GTK.Methods.GtkWidget.gtk_widget_get_window((Handle as GTKNativeControl).Handle);
			IntPtr hGdkDisplay = Internal.GDK.Methods.gdk_window_get_display(hGdkWindow);
			IntPtr hCursor = Internal.GDK.Methods.gdk_window_get_cursor(hGdkWindow);

			GTK3Engine.InitializeCursors(hGdkDisplay);

			Cursor c = GTK3Engine.GetCursorByHandle(hCursor);
			if (c == null) return Cursors.Default;

			return c;
		}
		protected override void SetCursorInternal(Cursor value)
		{
			IntPtr hGdkWindow = Internal.GTK.Methods.GtkWidget.gtk_widget_get_window((Handle as GTKNativeControl).Handle);
			IntPtr hGdkDisplay = Internal.GDK.Methods.gdk_window_get_display(hGdkWindow);

			GTK3Engine.InitializeCursors(hGdkDisplay);

			IntPtr hCursor = GTK3Engine.GetHandleForCursor(value);
			Internal.GDK.Methods.gdk_window_set_cursor(hGdkWindow, hCursor);
		}

		protected override bool HasFocusInternal()
		{
			GTKNativeControl nc = (Handle as GTKNativeControl);
			bool hasFocus = Internal.GTK.Methods.GtkWidget.gtk_widget_has_focus(nc.Handle);
			bool isFocus = Internal.GTK.Methods.GtkWidget.gtk_widget_is_focus(nc.Handle);
			for (int i = 0; i < nc.AdditionalHandles.Length; i++)
			{
				bool hasFocus2 = Internal.GTK.Methods.GtkWidget.gtk_widget_has_focus(nc.AdditionalHandles[i]);
				bool isFocus2 = Internal.GTK.Methods.GtkWidget.gtk_widget_is_focus(nc.AdditionalHandles[i]);
				hasFocus = hasFocus || hasFocus2;
				isFocus = isFocus || isFocus2;
			}
			return hasFocus || isFocus;
		}

		protected override double GetAdjustmentValueInternal(Orientation orientation)
		{
			// FIXME: only implemented for certain controls
			return 0.0;
		}
		protected override void SetAdjustmentValueInternal(Orientation orientation, double value)
		{
			// FIXME: only implemented for certain controls
		}
		protected override Dimension2D GetScrollBoundsInternal()
		{
			// FIXME: only implemented for certain controls
			return Dimension2D.Empty;
		}
		protected override void SetScrollBoundsInternal(Dimension2D value)
		{
			// FIXME: only implemented for certain controls
		}

		protected override AdjustmentScrollType GetAdjustmentScrollTypeInternal(Orientation orientation)
		{
			IntPtr hScrolledWindow = (Handle as GTKNativeControl).GetNamedHandle("ScrolledWindow");
			if (hScrolledWindow == IntPtr.Zero)
				return AdjustmentScrollType.Never;

			Internal.GTK.Constants.GtkPolicyType policyH = Internal.GTK.Constants.GtkPolicyType.Never, policyV = Internal.GTK.Constants.GtkPolicyType.Never;
			Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_get_policy(hScrolledWindow, ref policyH, ref policyV);
			switch (orientation)
			{
				case Orientation.Horizontal:
				{
					switch (policyH)
					{
						case Internal.GTK.Constants.GtkPolicyType.Always: return AdjustmentScrollType.Always;
						case Internal.GTK.Constants.GtkPolicyType.Automatic: return AdjustmentScrollType.Automatic;
						case Internal.GTK.Constants.GtkPolicyType.External: return AdjustmentScrollType.External;
						case Internal.GTK.Constants.GtkPolicyType.Never: return AdjustmentScrollType.Never;
					}
					throw new ArgumentOutOfRangeException(nameof(policyH));
				}
				case Orientation.Vertical:
				{
					switch (policyV)
					{
						case Internal.GTK.Constants.GtkPolicyType.Always: return AdjustmentScrollType.Always;
						case Internal.GTK.Constants.GtkPolicyType.Automatic: return AdjustmentScrollType.Automatic;
						case Internal.GTK.Constants.GtkPolicyType.External: return AdjustmentScrollType.External;
						case Internal.GTK.Constants.GtkPolicyType.Never: return AdjustmentScrollType.Never;
					}
					throw new ArgumentOutOfRangeException(nameof(policyV));
				}
			}
			throw new ArgumentOutOfRangeException(nameof(orientation));
		}
		protected override void SetAdjustmentScrollTypeInternal(Orientation orientation, AdjustmentScrollType value)
		{
			IntPtr hScrolledWindow = (Handle as GTKNativeControl).GetNamedHandle("ScrolledWindow");
			if (hScrolledWindow == IntPtr.Zero)
				return;

			Internal.GTK.Constants.GtkPolicyType policyH = Internal.GTK.Constants.GtkPolicyType.Never, policyV = Internal.GTK.Constants.GtkPolicyType.Never;
			Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_get_policy(hScrolledWindow, ref policyH, ref policyV);
			switch (orientation)
			{
				case Orientation.Horizontal:
				{
					switch (value)
					{
						case AdjustmentScrollType.Always: policyH = Internal.GTK.Constants.GtkPolicyType.Always; break;
						case AdjustmentScrollType.Automatic: policyH = Internal.GTK.Constants.GtkPolicyType.Automatic; break;
						case AdjustmentScrollType.External: policyH = Internal.GTK.Constants.GtkPolicyType.External; break;
						case AdjustmentScrollType.Never: policyH = Internal.GTK.Constants.GtkPolicyType.Never; break;
					}
					Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_set_policy(hScrolledWindow, policyH, policyV);
					break;
				}
				case Orientation.Vertical:
				{
					switch (value)
					{
						case AdjustmentScrollType.Always: policyV = Internal.GTK.Constants.GtkPolicyType.Always; break;
						case AdjustmentScrollType.Automatic: policyV = Internal.GTK.Constants.GtkPolicyType.Automatic; break;
						case AdjustmentScrollType.External: policyV = Internal.GTK.Constants.GtkPolicyType.External; break;
						case AdjustmentScrollType.Never: policyV = Internal.GTK.Constants.GtkPolicyType.Never; break;
					}
					Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_set_policy(hScrolledWindow, policyH, policyV);
					break;
				}
			}
		}

		protected override void UpdateControlFontInternal(Font font)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			IntPtr hPangoContext = Internal.GTK.Methods.GtkWidget.gtk_widget_get_pango_context(handle);
			IntPtr hFontDesc = Internal.Pango.Methods.pango_context_get_font_description(hPangoContext);
			if (font.FamilyName != null)
				Internal.Pango.Methods.pango_font_description_set_family(hFontDesc, font.FamilyName);
			if (font.Size != null)
				Internal.Pango.Methods.pango_font_description_set_size(hFontDesc, (int)(font.Size * Internal.Pango.Constants.PangoScale));
			if (font.Weight != null)
				Internal.Pango.Methods.pango_font_description_set_weight(hFontDesc, (int)font.Weight);
		}

		protected override HorizontalAlignment GetHorizontalAlignmentInternal()
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Constants.GtkAlign align = Internal.GTK.Methods.GtkWidget.gtk_widget_get_valign(handle);
			switch (align)
			{
				case Internal.GTK.Constants.GtkAlign.Center: return HorizontalAlignment.Center;
				case Internal.GTK.Constants.GtkAlign.End: return HorizontalAlignment.Right;
				case Internal.GTK.Constants.GtkAlign.Start: return HorizontalAlignment.Left;
			}
			return HorizontalAlignment.Default;
		}
		protected override void SetHorizontalAlignmentInternal(HorizontalAlignment value)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			switch (value)
			{
				case HorizontalAlignment.Center:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.Center);
					break;
				}
				case HorizontalAlignment.Default:
				case HorizontalAlignment.Justify:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.Fill);
					break;
				}
				case HorizontalAlignment.Left:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.Start);
					break;
				}
				case HorizontalAlignment.Right:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(handle, Internal.GTK.Constants.GtkAlign.End);
					break;
				}
			}
		}
		protected override VerticalAlignment GetVerticalAlignmentInternal()
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Constants.GtkAlign align = Internal.GTK.Methods.GtkWidget.gtk_widget_get_valign(handle);
			switch (align)
			{
				case Internal.GTK.Constants.GtkAlign.Baseline: return VerticalAlignment.Baseline;
				case Internal.GTK.Constants.GtkAlign.Center: return VerticalAlignment.Middle;
				case Internal.GTK.Constants.GtkAlign.End: return VerticalAlignment.Bottom;
				case Internal.GTK.Constants.GtkAlign.Start: return VerticalAlignment.Top;
			}
			return VerticalAlignment.Default;
		}
		protected override void SetVerticalAlignmentInternal(VerticalAlignment value)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			switch (value)
			{
				case VerticalAlignment.Baseline:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.Baseline);
					break;
				}
				case VerticalAlignment.Bottom:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.End);
					break;
				}
				case VerticalAlignment.Default:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.Fill);
					break;
				}
				case VerticalAlignment.Middle:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.Center);
					break;
				}
				case VerticalAlignment.Top:
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_valign(handle, Internal.GTK.Constants.GtkAlign.Start);
					break;
				}
			}
		}

		protected override Vector2D GetLocationInternal()
		{
			return new Vector2D(0, 0);
		}
		protected override void SetLocationInternal(Vector2D location)
		{
			// FIXME: not implemented
			if (Control is Window)
			{
				// HACK: fix this when we figure out why our ControlImplementation is a ContainerImplementation instead of a WindowImplementation
				Internal.GTK.Methods.GtkWindow.gtk_window_move((Handle as GTKNativeControl).Handle, (int)location.X, (int)location.Y);
			}
		}

	}
}
