using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using MBS.Framework.Collections.Generic;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Controls.Docking;
using UniversalWidgetToolkit.Dialogs;
using UniversalWidgetToolkit.DragDrop;
using UniversalWidgetToolkit.Drawing;

using UniversalWidgetToolkit.Input.Keyboard;
using UniversalWidgetToolkit.Input.Mouse;

using MBS.Framework.Drawing;

namespace UniversalWidgetToolkit.Engines.GTK
{
	public class GTKEngine : Engine
	{
		private int _exitCode = 0;
		private IntPtr mvarApplicationHandle = IntPtr.Zero;

		protected override bool WindowHasFocusInternal(Window window)
		{
			IntPtr hWindow = GetHandleForControl(window);
			return Internal.GTK.Methods.gtk_window_has_toplevel_focus(hWindow);
		}

		protected override Vector2D ClientToScreenCoordinatesInternal(Vector2D point)
		{
			return point;
		}

		protected override bool IsControlEnabledInternal(Control control)
		{
			IntPtr handle = GetHandleForControl(control);
			return Internal.GTK.Methods.gtk_widget_is_sensitive(handle);
		}
		protected override void SetControlEnabledInternal(Control control, bool value)
		{
			IntPtr handle = GetHandleForControl(control);
			Internal.GTK.Methods.gtk_widget_set_sensitive(handle, value);
		}

		private List<Window> _GetToplevelWindowsRetval = null;
		protected override Window[] GetToplevelWindowsInternal()
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
			Internal.GLib.Methods.g_list_free(hList);

			_GetToplevelWindowsRetval = null;
			return retval;
		}
		private void /*GFunc*/ _AddToList(IntPtr data, IntPtr user_data)
		{
			if (_GetToplevelWindowsRetval == null)
			{
				throw new InvalidOperationException("_AddToList called before initializing the list");
			}

			Control ctl = GetControlByHandle(data);
			Window window = (ctl as Window);

			if (window == null)
			{
				window = new Window();
				RegisterControlHandle(window, data);
			}

			_GetToplevelWindowsRetval.Add(window);
		}


		internal static uint GetAccelKeyForKeyboardKey(KeyboardKey key)
		{
			switch (key)
			{
				case KeyboardKey.A: return (uint)'A';
				case KeyboardKey.B: return (uint)'B';
				case KeyboardKey.C: return (uint)'C';
				case KeyboardKey.D: return (uint)'D';
				case KeyboardKey.E: return (uint)'E';
				case KeyboardKey.F: return (uint)'F';
				case KeyboardKey.G: return (uint)'G';
				case KeyboardKey.H: return (uint)'H';
				case KeyboardKey.I: return (uint)'I';
				case KeyboardKey.J: return (uint)'J';
				case KeyboardKey.K: return (uint)'K';
				case KeyboardKey.L: return (uint)'L';
				case KeyboardKey.M: return (uint)'M';
				case KeyboardKey.N: return (uint)'N';
				case KeyboardKey.O: return (uint)'O';
				case KeyboardKey.P: return (uint)'P';
				case KeyboardKey.Q: return (uint)'Q';
				case KeyboardKey.R: return (uint)'R';
				case KeyboardKey.S: return (uint)'S';
				case KeyboardKey.T: return (uint)'T';
				case KeyboardKey.U: return (uint)'U';
				case KeyboardKey.V: return (uint)'V';
				case KeyboardKey.W: return (uint)'W';
				case KeyboardKey.X: return (uint)'X';
				case KeyboardKey.Y: return (uint)'Y';
				case KeyboardKey.Z: return (uint)'Z';
			}
			return 0;
		}

		internal static Internal.GTK.Structures.GtkTargetEntry[] DragDropTargetToGtkTargetEntry(DragDropTarget[] targets)
		{
			List<Internal.GTK.Structures.GtkTargetEntry> list = new List<Internal.GTK.Structures.GtkTargetEntry>();
			foreach (DragDropTarget target in targets)
			{
				Internal.GTK.Constants.GtkTargetFlags flags = Internal.GTK.Constants.GtkTargetFlags.SameApp;
				if ((target.Flags & DragDropTargetFlags.OtherApplication) == DragDropTargetFlags.OtherApplication) flags |= Internal.GTK.Constants.GtkTargetFlags.OtherApp;
				if ((target.Flags & DragDropTargetFlags.OtherWidget) == DragDropTargetFlags.OtherWidget) flags |= Internal.GTK.Constants.GtkTargetFlags.OtherWidget;
				if ((target.Flags & DragDropTargetFlags.SameApplication) == DragDropTargetFlags.SameApplication) flags |= Internal.GTK.Constants.GtkTargetFlags.SameApp;
				if ((target.Flags & DragDropTargetFlags.SameWidget) == DragDropTargetFlags.SameWidget) flags |= Internal.GTK.Constants.GtkTargetFlags.SameWidget;

				list.Add(new Internal.GTK.Structures.GtkTargetEntry() { flags = flags, info = (uint)target.ID, target = target.Name });
			}
			return list.ToArray();
		}

		internal static Internal.GDK.Constants.GdkDragAction DragDropEffectToGdkDragAction(DragDropEffect actions)
		{
			Internal.GDK.Constants.GdkDragAction retval = Internal.GDK.Constants.GdkDragAction.Default;
			if ((actions & DragDropEffect.Copy) == DragDropEffect.Copy) retval |= Internal.GDK.Constants.GdkDragAction.Copy;
			if ((actions & DragDropEffect.Copy) == DragDropEffect.Link) retval |= Internal.GDK.Constants.GdkDragAction.Link;
			if ((actions & DragDropEffect.Move) == DragDropEffect.Copy) retval |= Internal.GDK.Constants.GdkDragAction.Move;

			// not sure what "scroll" means
			// if ((actions & DragDropEffect.Scroll) == DragDropEffect.Scroll) retval |= ??;
			return retval;
		}

		internal static KeyboardModifierKey GdkModifierTypeToKeyboardModifierKey(Internal.GDK.Constants.GdkModifierType key)
		{
			KeyboardModifierKey modifierType = KeyboardModifierKey.None;
			if ((key & Internal.GDK.Constants.GdkModifierType.Alt) == Internal.GDK.Constants.GdkModifierType.Alt) modifierType |= KeyboardModifierKey.Alt;
			if ((key & Internal.GDK.Constants.GdkModifierType.Meta) == Internal.GDK.Constants.GdkModifierType.Meta) modifierType |= KeyboardModifierKey.Meta;
			if ((key & Internal.GDK.Constants.GdkModifierType.Control) == Internal.GDK.Constants.GdkModifierType.Control) modifierType |= KeyboardModifierKey.Control;
			if ((key & Internal.GDK.Constants.GdkModifierType.Hyper) == Internal.GDK.Constants.GdkModifierType.Hyper) modifierType |= KeyboardModifierKey.Hyper;
			if ((key & Internal.GDK.Constants.GdkModifierType.Shift) == Internal.GDK.Constants.GdkModifierType.Shift) modifierType |= KeyboardModifierKey.Shift;
			if ((key & Internal.GDK.Constants.GdkModifierType.Super) == Internal.GDK.Constants.GdkModifierType.Super) modifierType |= KeyboardModifierKey.Super;
			return modifierType;
		}
		internal static Internal.GDK.Constants.GdkModifierType KeyboardModifierKeyToGdkModifierType(KeyboardModifierKey key)
		{
			Internal.GDK.Constants.GdkModifierType modifierType = Internal.GDK.Constants.GdkModifierType.None;
			if ((key & KeyboardModifierKey.Alt) == KeyboardModifierKey.Alt) modifierType |= Internal.GDK.Constants.GdkModifierType.Alt;
			if ((key & KeyboardModifierKey.Meta) == KeyboardModifierKey.Meta) modifierType |= Internal.GDK.Constants.GdkModifierType.Meta;
			if ((key & KeyboardModifierKey.Control) == KeyboardModifierKey.Control) modifierType |= Internal.GDK.Constants.GdkModifierType.Control;
			if ((key & KeyboardModifierKey.Hyper) == KeyboardModifierKey.Hyper) modifierType |= Internal.GDK.Constants.GdkModifierType.Hyper;
			if ((key & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift) modifierType |= Internal.GDK.Constants.GdkModifierType.Shift;
			if ((key & KeyboardModifierKey.Super) == KeyboardModifierKey.Super) modifierType |= Internal.GDK.Constants.GdkModifierType.Super;
			return modifierType;
		}

		internal static MouseButtons GdkModifierTypeToMouseButtons(Internal.GDK.Constants.GdkModifierType modifierType)
		{
			MouseButtons button = MouseButtons.None;
			if ((modifierType & Internal.GDK.Constants.GdkModifierType.Button1) == Internal.GDK.Constants.GdkModifierType.Button1) button |= MouseButtons.Primary;
			if ((modifierType & Internal.GDK.Constants.GdkModifierType.Button3) == Internal.GDK.Constants.GdkModifierType.Button2) button |= MouseButtons.Secondary;
			if ((modifierType & Internal.GDK.Constants.GdkModifierType.Button2) == Internal.GDK.Constants.GdkModifierType.Button3) button |= MouseButtons.Wheel;
			if ((modifierType & Internal.GDK.Constants.GdkModifierType.Button4) == Internal.GDK.Constants.GdkModifierType.Button4) button |= MouseButtons.XButton1;
			if ((modifierType & Internal.GDK.Constants.GdkModifierType.Button5) == Internal.GDK.Constants.GdkModifierType.Button5) button |= MouseButtons.XButton2;
			return button;
		}
		internal static Internal.GDK.Constants.GdkModifierType MouseButtonsToGdkModifierType(MouseButtons buttons)
		{
			Internal.GDK.Constants.GdkModifierType button = Internal.GDK.Constants.GdkModifierType.None;
			if ((buttons & MouseButtons.Primary) == MouseButtons.Primary) button |= Internal.GDK.Constants.GdkModifierType.Button1;
			if ((buttons & MouseButtons.Secondary) == MouseButtons.Secondary) button |= Internal.GDK.Constants.GdkModifierType.Button3;
			if ((buttons & MouseButtons.Wheel) == MouseButtons.Wheel) button |= Internal.GDK.Constants.GdkModifierType.Button2;
			if ((buttons & MouseButtons.XButton1) == MouseButtons.XButton1) button |= Internal.GDK.Constants.GdkModifierType.Button4;
			if ((buttons & MouseButtons.XButton2) == MouseButtons.XButton2) button |= Internal.GDK.Constants.GdkModifierType.Button5;
			return button;
		}


		protected override bool InitializeInternal()
		{
			string[] argv = System.Environment.GetCommandLineArgs();
			int argc = argv.Length;

			bool check = Internal.GTK.Methods.gtk_init_check(ref argc, ref argv);
			if (!check)
				return check;

			string appname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
			Internal.Notify.Methods.notify_init(appname);

			gc_Application_Activate = new Internal.GObject.Delegates.GCallback(Application_Activate);
			gc_MenuItem_Activated = new Internal.GObject.Delegates.GCallback(MenuItem_Activate);

			IntPtr hApp = Internal.GTK.Methods.gtk_application_new("net.alcetech.UniversalEditor", Internal.GIO.Constants.GApplicationFlags.None);
			// hApp = IntPtr.Zero;
			// IntPtr hApp = Internal.GIO.Methods.g_application_new ("net.alcetech.UniversalEditor", Internal.GIO.Constants.GApplicationFlags.None);
			if (hApp != IntPtr.Zero)
			{
				bool isRegistered = Internal.GIO.Methods.g_application_get_is_registered(hApp);

				Internal.GIO.Methods.g_application_register(hApp, IntPtr.Zero, IntPtr.Zero);

				isRegistered = Internal.GIO.Methods.g_application_get_is_registered(hApp);

				IntPtr simpleActionGroup = Internal.GIO.Methods.g_simple_action_group_new();

				IntPtr hActionNew = Internal.GIO.Methods.g_simple_action_new("new", Internal.GLib.Constants.GVariantType.Byte);

				Internal.GIO.Methods.g_action_map_add_action(hApp, hActionNew);

				IntPtr hMenu = Internal.GIO.Methods.g_menu_new();

				IntPtr hMenuItemFile = Internal.GIO.Methods.g_menu_item_new("_New Document", "app.new");

				Internal.GIO.Methods.g_menu_append_item(hMenu, hMenuItemFile);

				Internal.GTK.Methods.gtk_application_set_app_menu(hApp, hMenu);

				mvarApplicationHandle = hApp;
			}

			// TODO: fix this, it doesn't work (crashes with SIGSEGV)
			// Internal.GIO.Methods.g_action_map_add_action (hApp, actionFile);

			/*
			IntPtr hMenu = Internal.GIO.Methods.g_menu_new ();s
			IntPtr hMenuItem = Internal.GIO.Methods.g_menu_item_new ("File", "app.file");
			Internal.GIO.Methods.g_menu_append_item (hMenu, hMenuItem);

			Internal.GTK.Methods.gtk_application_set_menubar (hApp, hMenu);
			*/

			return check;
		}
		protected override int StartInternal(Window waitForClose)
		{
			if (mvarApplicationHandle != IntPtr.Zero)
			{
				string[] argv = System.Environment.GetCommandLineArgs();
				int argc = argv.Length;
				
				Internal.GObject.Methods.g_signal_connect(mvarApplicationHandle, "activate", gc_Application_Activate, IntPtr.Zero);
				Internal.GIO.Methods.g_application_run(mvarApplicationHandle, argc, argv);
			}

			Internal.GTK.Methods.gtk_main();

			return _exitCode;
		}
		protected override void StopInternal(int exitCode)
		{
			Internal.GTK.Methods.gtk_main_quit();
			_exitCode = exitCode;
		}

		private Dictionary<Layout, IntPtr> handlesByLayout = new Dictionary<Layout, IntPtr>();

		private Dictionary<IntPtr, MenuItem> menuItemsByHandle = new Dictionary<IntPtr, MenuItem>();

		private Internal.GObject.Delegates.GCallback gc_MenuItem_Activated = null;
		private Internal.GObject.Delegates.GCallback gc_Application_Activate = null;

		private void Application_Activate(IntPtr handle, IntPtr data)
		{
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

		public GTKEngine()
		{
			InitializeStockIDs();
			InitializeEventHandlers();
		}

		private void InitializeStockIDs()
		{
			RegisterStockType(StockType.About, "gtk-about");
			RegisterStockType(StockType.Add, "gtk-add");
			RegisterStockType(StockType.Apply, "gtk-apply");
			RegisterStockType(StockType.Bold, "gtk-bold");
			RegisterStockType(StockType.Cancel, "gtk-cancel");
			RegisterStockType(StockType.CapsLockWarning, "gtk-caps-lock-warning");
			RegisterStockType(StockType.CDROM, "gtk-cdrom");
			RegisterStockType(StockType.Clear, "gtk-clear");
			RegisterStockType(StockType.Close, "gtk-close");
			RegisterStockType(StockType.ColorPicker, "gtk-color-picker");
			RegisterStockType(StockType.Connect, "gtk-connect");
			RegisterStockType(StockType.Convert, "gtk-convert");
			RegisterStockType(StockType.Copy, "gtk-copy");
			RegisterStockType(StockType.Cut, "gtk-cut");
			RegisterStockType(StockType.Delete, "gtk-delete");
			RegisterStockType(StockType.DialogAuthentication, "gtk-dialog-authentication");
			RegisterStockType(StockType.DialogInfo, "gtk-dialog-info");
			RegisterStockType(StockType.DialogWarning, "gtk-dialog-warning");
			RegisterStockType(StockType.DialogError, "gtk-dialog-error");
			RegisterStockType(StockType.DialogQuestion, "gtk-dialog-question");
			RegisterStockType(StockType.Directory, "gtk-directory");
			RegisterStockType(StockType.Discard, "gtk-discard");
			RegisterStockType(StockType.Disconnect, "gtk-disconnect");
			RegisterStockType(StockType.DragAndDrop, "gtk-dnd");
			RegisterStockType(StockType.DragAndDropMultiple, "gtk-dnd-multiple");
			RegisterStockType(StockType.Edit, "gtk-edit");
			RegisterStockType(StockType.Execute, "gtk-execute");
			RegisterStockType(StockType.File, "gtk-file");
			RegisterStockType(StockType.Find, "gtk-find");
			RegisterStockType(StockType.FindAndReplace, "gtk-find-and-replace");
			RegisterStockType(StockType.Floppy, "gtk-floppy");
			RegisterStockType(StockType.Fullscreen, "gtk-fullscreen");
			RegisterStockType(StockType.GotoBottom, "gtk-goto-bottom");
			RegisterStockType(StockType.GotoFirst, "gtk-goto-first");
			RegisterStockType(StockType.GotoLast, "gtk-goto-last");
			RegisterStockType(StockType.GotoTop, "gtk-goto-top");
			RegisterStockType(StockType.GoBack, "gtk-go-back");
			RegisterStockType(StockType.GoDown, "gtk-go-down");
			RegisterStockType(StockType.GoForward, "gtk-go-forward");
			RegisterStockType(StockType.GoUp, "gtk-go-up");
			RegisterStockType(StockType.HardDisk, "gtk-harddisk");
			RegisterStockType(StockType.Help, "gtk-help");
			RegisterStockType(StockType.Home, "gtk-home");
			RegisterStockType(StockType.Index, "gtk-index");
			RegisterStockType(StockType.Indent, "gtk-indent");
			RegisterStockType(StockType.Info, "gtk-info");
			RegisterStockType(StockType.Italic, "gtk-italic");
			RegisterStockType(StockType.JumpTo, "gtk-jump-to");
			RegisterStockType(StockType.JustifyCenter, "gtk-justify-center");
			RegisterStockType(StockType.JustifyFill, "gtk-justify-fill");
			RegisterStockType(StockType.JustifyLeft, "gtk-justify-left");
			RegisterStockType(StockType.JustifyRight, "gtk-justify-right");
			RegisterStockType(StockType.LeaveFullscreen, "gtk-leave-fullscreen");
			RegisterStockType(StockType.MissingImage, "gtk-missing-image");
			RegisterStockType(StockType.MediaForward, "gtk-media-forward");
			RegisterStockType(StockType.MediaNext, "gtk-media-next");
			RegisterStockType(StockType.MediaPause, "gtk-media-pause");
			RegisterStockType(StockType.MediaPlay, "gtk-media-play");
			RegisterStockType(StockType.MediaPrevious, "gtk-media-previous");
			RegisterStockType(StockType.MediaRecord, "gtk-media-record");
			RegisterStockType(StockType.MediaRewind, "gtk-media-rewind");
			RegisterStockType(StockType.MediaStop, "gtk-media-stop");
			RegisterStockType(StockType.Network, "gtk-network");
			RegisterStockType(StockType.New, "gtk-new");
			RegisterStockType(StockType.No, "gtk-no");
			RegisterStockType(StockType.OK, "gtk-ok");
			RegisterStockType(StockType.Open, "gtk-open");
			RegisterStockType(StockType.OrientationPortrait, "gtk-orientation-portrait");
			RegisterStockType(StockType.OrientationLandscape, "gtk-orientation-landscape");
			RegisterStockType(StockType.OrientationReverseLandscape, "gtk-orientation-reverse-landscape");
			RegisterStockType(StockType.OrientationReversePortrait, "gtk-orientation-reverse-portrait");
			RegisterStockType(StockType.PageSetup, "gtk-page-setup");
			RegisterStockType(StockType.Paste, "gtk-paste");
			RegisterStockType(StockType.Preferences, "gtk-preferences");
			RegisterStockType(StockType.Print, "gtk-print");
			RegisterStockType(StockType.PrintError, "gtk-print-error");
			RegisterStockType(StockType.PrintPaused, "gtk-print-paused");
			RegisterStockType(StockType.PrintPreview, "gtk-print-preview");
			RegisterStockType(StockType.PrintReport, "gtk-print-report");
			RegisterStockType(StockType.PrintWarning, "gtk-print-warning");
			RegisterStockType(StockType.Properties, "gtk-properties");
			RegisterStockType(StockType.Quit, "gtk-quit");
			RegisterStockType(StockType.Redo, "gtk-redo");
			RegisterStockType(StockType.Refresh, "gtk-refresh");
			RegisterStockType(StockType.Remove, "gtk-remove");
			RegisterStockType(StockType.RevertToSaved, "gtk-revert-to-saved");
			RegisterStockType(StockType.Save, "gtk-save");
			RegisterStockType(StockType.SaveAs, "gtk-save-as");
			RegisterStockType(StockType.SelectAll, "gtk-select-all");
			RegisterStockType(StockType.SelectColor, "gtk-select-color");
			RegisterStockType(StockType.SelectFont, "gtk-select-font");
			RegisterStockType(StockType.SortAscending, "gtk-sort-ascending");
			RegisterStockType(StockType.SortDescending, "gtk-sort-descending");
			RegisterStockType(StockType.SpellCheck, "gtk-spell-check");
			RegisterStockType(StockType.Stop, "gtk-stop");
			RegisterStockType(StockType.Strikethrough, "gtk-strikethrough");
			RegisterStockType(StockType.Undelete, "gtk-undelete");
			RegisterStockType(StockType.Underline, "gtk-underline");
			RegisterStockType(StockType.Undo, "gtk-undo");
			RegisterStockType(StockType.Unindent, "gtk-unindent");
			RegisterStockType(StockType.Yes, "gtk-yes");
			RegisterStockType(StockType.Zoom100, "gtk-zoom-100");
			RegisterStockType(StockType.ZoomFit, "gtk-zoom-fit");
			RegisterStockType(StockType.ZoomIn, "gtk-zoom-in");
			RegisterStockType(StockType.ZoomOut, "gtk-zoom-out");
		}

		protected override void DestroyControlInternal(Control control)
		{
			IntPtr handle = GetHandleForControl(control);
			if (control is Dialog)
			{
				// this way is recommended per GTK3.0 docs:
				// "destroying the dialog during gtk_dialog_run() is a very bad idea, because your post-run code won't know whether the dialog was destroyed or not"
				Internal.GTK.Methods.gtk_dialog_response(handle, Internal.GTK.Constants.GtkResponseType.None);
				return;
			}
			Internal.GTK.Methods.gtk_widget_destroy(handle);
		}

		private bool gc_key_press_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			// we cannot pass this param explicitly
			// MUST USE INTPTR THEN PTRTOSTRUCTURE!
			Internal.GDK.Structures.GdkEventKey e = (Internal.GDK.Structures.GdkEventKey)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventKey));

			Control ctl = GetControlByHandle(widget);
			if (ctl == null) return false;
			if (!ctl.IsCreated) return false;
			
			KeyEventArgs ee = GdkEventKeyToKeyEventArgs(e);
			ctl.OnKeyDown(ee);
			return ee.Cancel;
		}
		private bool gc_key_release_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			// we cannot pass this param explicitly
			// MUST USE INTPTR THEN PTRTOSTRUCTURE!
			Internal.GDK.Structures.GdkEventKey e = (Internal.GDK.Structures.GdkEventKey)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventKey));

			Control ctl = GetControlByHandle(widget);
			if (ctl == null) return false;
			if (!ctl.IsCreated) return false;

			KeyEventArgs ee = GdkEventKeyToKeyEventArgs(e);
			ctl.OnKeyUp(ee);
			return ee.Cancel;
		}

		private static KeyEventArgs GdkEventKeyToKeyEventArgs(Internal.GDK.Structures.GdkEventKey e)
		{
			uint keyCode = e.keyval;
			uint keyData = e.hardware_keycode;

			KeyEventArgs ee = new KeyEventArgs();
			KeyboardModifierKey modifierKeys = KeyboardModifierKey.None;
			ee.Key = GdkKeyCodeToKeyboardKey(e.keyval, e.hardware_keycode, out modifierKeys);
			ee.ModifierKeys = GdkModifierTypeToKeyboardModifierKey(e.state);
			return ee;
		}


		private static KeyboardKey GdkKeyCodeToKeyboardKey(uint keyval, uint keycode, out KeyboardModifierKey modifierKeys)
		{
			KeyboardKey key = KeyboardKey.None;
			modifierKeys = KeyboardModifierKey.None;

			switch (keyval)
			{
				case 45: key = KeyboardKey.Minus; break;
				case 95: key = KeyboardKey.Minus; modifierKeys |= KeyboardModifierKey.Shift; break;

				case 61: key = KeyboardKey.Plus; break;
				case 43: key = KeyboardKey.Plus; modifierKeys |= KeyboardModifierKey.Shift; break;

				case 96: key = KeyboardKey.Tilde; break;
				case 126: key = KeyboardKey.Tilde; modifierKeys |= KeyboardModifierKey.Shift; break;

				case 44: key = KeyboardKey.Comma; break;
				case 60: key = KeyboardKey.Comma; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 46: key = KeyboardKey.Period; break;
				case 62: key = KeyboardKey.Period; modifierKeys |= KeyboardModifierKey.Shift; break;

				case 32: key = KeyboardKey.Space; break;
				case 47: key = KeyboardKey.Question; break;
				case 91: key = KeyboardKey.OpenBrackets; break;
				case 123: key = KeyboardKey.OpenBrackets; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 93: key = KeyboardKey.CloseBrackets; break;
				case 125: key = KeyboardKey.CloseBrackets; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 59: key = KeyboardKey.Semicolon; break;
				case 58: key = KeyboardKey.Semicolon; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 39: key = KeyboardKey.Quotes; break;
				case 34: key = KeyboardKey.Quotes; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 92: key = KeyboardKey.Backslash; break;
				case 124: key = KeyboardKey.Pipe; break;
				case 63: key = KeyboardKey.Question; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 65293: key = KeyboardKey.Enter; break;
				case 65505: key = KeyboardKey.LShiftKey; break;
				case 65506: key = KeyboardKey.RShiftKey; break;
				case 65507: key = KeyboardKey.LControlKey; break;
				case 65513: key = KeyboardKey.LMenu; break;
				case 65508: key = KeyboardKey.RControlKey; break;
				case 65511: key = KeyboardKey.LMenu; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 65512: key = KeyboardKey.RMenu; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 65514: key = KeyboardKey.RMenu; break;
				case 65515: key = KeyboardKey.LWin; break;
				case 65516: key = KeyboardKey.RWin; break; // assumed
				case 65361: key = KeyboardKey.ArrowLeft; break;
				case 65362: key = KeyboardKey.ArrowUp; break;
				case 65363: key = KeyboardKey.ArrowRight; break;
				case 65364: key = KeyboardKey.ArrowDown; break;
				case 65383: key = KeyboardKey.Menu; break;
				case 269025062: key = KeyboardKey.BrowserBack; break;
				case 269025153: key = KeyboardKey.SelectMedia; break;
				case 65379: key = KeyboardKey.Insert; break;
				case 65535: key = KeyboardKey.Delete; break;
				case 65307: key = KeyboardKey.Escape; break;
				case 65288: key = KeyboardKey.Back; break;
				case 65289: key = KeyboardKey.Tab; break;
				case 65509: key = KeyboardKey.CapsLock; break;
				case 269025048: key = KeyboardKey.BrowserHome; break;
				default:
				{
					if (keyval >= 48 && keyval <= 57)
					{
							key = (KeyboardKey)((uint)KeyboardKey.D0 + (keyval - 48));
					}
					else if (keyval >= 65 && keyval <= 90)
					{
						key = (KeyboardKey)((uint)KeyboardKey.A + (keyval - 65));
						modifierKeys |= KeyboardModifierKey.Shift;
					}
					else if (keyval >= 97 && keyval <= 122)
					{
						key = (KeyboardKey)((uint)KeyboardKey.A + (keyval - 97));
					}
					else if (keyval >= 65470 && keyval <= 65482)
					{
						key = (KeyboardKey)((uint)KeyboardKey.F1 + (keyval - 65470));
					}
					break;
				}
			}

			if (key == KeyboardKey.None) Console.WriteLine("GdkKeyCodeToKeyboardKey not handled for keyval: " + keyval.ToString() + "; keycode: " + keycode.ToString());
			return key;
		}

		private MouseEventArgs GdkEventButtonToMouseEventArgs(Internal.GDK.Structures.GdkEventButton e)
		{
			MouseButtons buttons = MouseButtons.None;
			switch (e.button)
			{
				case 1: buttons = MouseButtons.Primary; break;
				case 2: buttons = MouseButtons.Wheel; break;
				case 3: buttons = MouseButtons.Secondary; break;
				case 4: buttons = MouseButtons.XButton1; break;
				case 5: buttons = MouseButtons.XButton2; break;
			}
			KeyboardModifierKey modifierKeys = GdkModifierTypeToKeyboardModifierKey(e.state);
			MouseEventArgs ee = new MouseEventArgs(e.x, e.y, buttons, modifierKeys);
			return ee;
		}
		private MouseEventArgs GdkEventMotionToMouseEventArgs(Internal.GDK.Structures.GdkEventMotion e)
		{
			MouseButtons buttons = GdkModifierTypeToMouseButtons(e.state);
			KeyboardModifierKey modifierKeys = GdkModifierTypeToKeyboardModifierKey(e.state);
			MouseEventArgs ee = new MouseEventArgs(e.x, e.y, buttons, modifierKeys);
			return ee;
		}

		private MouseButtons _mousedown_buttons = MouseButtons.None;
		private bool gc_button_press_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			Internal.GDK.Structures.GdkEventButton e = (Internal.GDK.Structures.GdkEventButton)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventButton));
			MouseEventArgs ee = GdkEventButtonToMouseEventArgs(e);
			
			Control ctl = GetControlByHandle(widget);
			if (ctl != null)
			{
				_mousedown_buttons = ee.Buttons;
				ctl.OnMouseDown(ee);
				if (ee.Handled) return true;
			}
			return false;
		}
		private bool gc_motion_notify_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			Internal.GDK.Structures.GdkEventMotion e = (Internal.GDK.Structures.GdkEventMotion)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventMotion));

			Control ctl = GetControlByHandle(widget);
			MouseEventArgs ee = GdkEventMotionToMouseEventArgs(e);
			ee = new MouseEventArgs(ee.X, ee.Y, _mousedown_buttons, ee.ModifierKeys);
			if (ctl != null)
			{
				ctl.OnMouseMove(ee);
			}
			else
			{
				Console.Error.WriteLine("uwt: gtk: motion_notify_event called on empty control");
			}

			if (ee.Handled) return true;

			// TRUE to stop other handlers from being invoked for the event. FALSE to propagate the event further.
			return false;
		}
		private bool gc_button_release_event(IntPtr /*GtkWidget*/ widget, IntPtr hEventArgs, IntPtr user_data)
		{
			_mousedown_buttons = MouseButtons.None;
			Internal.GDK.Structures.GdkEventButton e = (Internal.GDK.Structures.GdkEventButton)System.Runtime.InteropServices.Marshal.PtrToStructure(hEventArgs, typeof(Internal.GDK.Structures.GdkEventButton));
			MouseEventArgs ee = GdkEventButtonToMouseEventArgs(e);
			
			Control ctl = GetControlByHandle(widget);
			if (ctl != null)
			{
				ctl.OnMouseUp(ee);
				
				if (ee.Buttons == MouseButtons.Primary)
					ctl.OnClick(EventArgs.Empty);

				if (ee.Handled) return true;
			}
			return false;
		}

		// converting these into standalone fields solved a HUGE (and esoteric) crash in handling keyboard events...
		private Internal.GObject.Delegates.GCallback gc_realize_handler = null;
		private Internal.GObject.Delegates.GCallback gc_unrealize_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_button_press_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_button_release_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_motion_notify_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_key_press_event_handler = null;
		private Internal.GTK.Delegates.GtkWidgetEvent gc_key_release_event_handler = null;

		private Internal.GTK.Delegates.GtkDragEvent gc_drag_begin_handler = null;
		private Internal.GTK.Delegates.GtkDragEvent gc_drag_data_delete_handler = null;
		private Internal.GTK.Delegates.GtkDragDataGetEvent gc_drag_data_get_handler = null;
		private void InitializeEventHandlers()
		{
			gc_realize_handler = new Internal.GObject.Delegates.GCallback(gc_realize);
			gc_unrealize_handler = new Internal.GObject.Delegates.GCallback(gc_unrealize);
			gc_button_press_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_button_press_event);
			gc_button_release_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_button_release_event);
			gc_motion_notify_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_motion_notify_event);
			gc_key_press_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_key_press_event);
			gc_key_release_event_handler = new Internal.GTK.Delegates.GtkWidgetEvent(gc_key_release_event);
			gc_drag_begin_handler = new Internal.GTK.Delegates.GtkDragEvent(gc_drag_begin);
			gc_drag_data_delete_handler = new Internal.GTK.Delegates.GtkDragEvent(gc_drag_data_delete);
			gc_drag_data_get_handler = new Internal.GTK.Delegates.GtkDragDataGetEvent(gc_drag_data_get);
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
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "key_press_event", gc_key_press_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "key_release_event", gc_key_release_event_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "realize", gc_realize_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "unrealize", gc_unrealize_handler);

			Internal.GObject.Methods.g_signal_connect(nativeHandle, "drag_begin", gc_drag_begin_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "drag_data_delete", gc_drag_data_delete_handler);
			Internal.GObject.Methods.g_signal_connect(nativeHandle, "drag_data_get", gc_drag_data_get_handler);
		}

		private void gc_drag_begin(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkDragContext*/ context, IntPtr user_data)
		{
			Console.WriteLine("gc_drag_begin");
		}
		private void gc_drag_data_delete(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkDragContext*/ context, IntPtr user_data)
		{
			Console.WriteLine("gc_drag_data_delete");
		}
		private void gc_drag_data_get(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkDragContext*/ context, IntPtr /*GtkSelectionData*/ data, uint info, uint time, IntPtr user_data)
		{
			Console.WriteLine("gc_drag_data_get");
		}

		private void InvokeMethod(object obj, string meth, params object[] parms)
		{
			Type t = obj.GetType();
			System.Reflection.MethodInfo mi = t.GetMethod(meth, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			if (mi != null)
			{
				mi.Invoke(obj, parms);
			}
			else
			{
				Console.WriteLine("Engine::InvokeMethod: not found '" + meth + "' on '" + t.FullName + "'");
			}
		}

		private void gc_realize(IntPtr /*GtkWidget*/ widget, IntPtr user_data)
		{
			Control ctl = GetControlByHandle(widget);
			if (ctl == null) return;

			if (ctl.NativeImplementation != null)
			{
				InvokeMethod(ctl.NativeImplementation, "OnRealize", EventArgs.Empty);
			}
		}
		private void gc_unrealize(IntPtr /*GtkWidget*/ widget, IntPtr user_data)
		{
			Control ctl = GetControlByHandle(widget);
			if (ctl == null) return;
			
			if (ctl.NativeImplementation != null)
			{
				InvokeMethod(ctl.NativeImplementation, "OnUnrealize", EventArgs.Empty);
			}
		}
		
		private IntPtr GetScrolledWindowChild(IntPtr hScrolledWindow)
		{
			IntPtr hList = Internal.GTK.Methods.gtk_container_get_children(hScrolledWindow);
			IntPtr hTreeView = Internal.GLib.Methods.g_list_nth_data(hList, 0);
			Console.WriteLine("returning {0} child handle for scrolled window {1}", hTreeView, hScrolledWindow);
			return hTreeView;
		}

		/// <summary>
		/// Returns the actual control handle (for event signaling) if a control is e.g. surrounded by GtkScrolledWindow
		/// </summary>
		private IntPtr FindRealHandle(IntPtr fakeHandle, Control ctl)
		{
			if (ctl is ListView)
			{
				return GetScrolledWindowChild(fakeHandle);
			}
			return fakeHandle;
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			Contract.Assert(control != null);
			NativeControl handle = base.CreateControlInternal(control);

			if (handle == null)
			{
				if (control is Container)
				{
					// Containers are special... for now
					// handle = CreateContainer(control as Container);
					handle = (new Controls.ContainerImplementation(this, control as Container)).CreateControl(control);
				}
				else
				{
					throw new NotImplementedException("NativeImplementation not found for control type: " + control.GetType().FullName);
				}
			}

			if (handle != null)
			{
				IntPtr nativeHandle = (handle as GTKNativeControl).Handle;
				Console.WriteLine("setting events on {0} \"{1}\" [{2}]", handle, control.Name, control.GetType().FullName);
				SetupCommonEvents(FindRealHandle(nativeHandle, control));
				
				if (control.Parent != null && control.Parent.Layout != null)
				{
					Constraints constraints = control.Parent.Layout.GetControlConstraints(control);
					if (constraints != null)
					{
						if (control.Parent.Layout is Layouts.BoxLayout)
						{
							Layouts.BoxLayout.Constraints cs = (constraints as Layouts.BoxLayout.Constraints);
							if (cs != null)
							{
								Internal.GTK.Constants.GtkPackType packType = Internal.GTK.Constants.GtkPackType.Start;
								switch (cs.PackType)
								{
									case Layouts.BoxLayout.PackType.Start:
									{
										packType = Internal.GTK.Constants.GtkPackType.Start;
										break;
									}
									case Layouts.BoxLayout.PackType.End:
									{
										packType = Internal.GTK.Constants.GtkPackType.End;
										break;
									}
								}

								IntPtr hLayout = IntPtr.Zero;
								if (handlesByLayout.ContainsKey(control.Parent.Layout))
								{
									hLayout = handlesByLayout[control.Parent.Layout];
								}
								else
								{
									hLayout = Internal.GTK.Methods.gtk_hbox_new(true, 0);
								}
								int padding = (cs.Padding == 0 ? control.Padding.All : cs.Padding);
								Internal.GTK.Methods.gtk_box_set_child_packing(hLayout, (handle as GTKNativeControl).Handle, cs.Expand, cs.Fill, padding, packType);
							}
						}
					}
				}

				RegisterControlHandle(control, (handle as GTKNativeControl).Handle);
				UpdateControlProperties(control);

				if (control.MinimumSize != Dimension2D.Empty)
					Internal.GTK.Methods.gtk_widget_set_size_request((handle as GTKNativeControl).Handle, (int)control.MinimumSize.Width, (int)control.MinimumSize.Height);
				
				Internal.GTK.Methods.gtk_widget_show_all((handle as GTKNativeControl).Handle);
			}
			return handle;
		}
		protected override void SetControlVisibilityInternal(Control control, bool visible)
		{
			if (IsControlDisposed(control)) CreateControl(control);
			if (IsControlDisposed(control))
				throw new ObjectDisposedException(control.GetType().FullName);

			IntPtr handle = GetHandleForControl(control);

			if (visible)
			{
				Internal.GTK.Methods.gtk_widget_show(handle);
			}
			else
			{
				Internal.GTK.Methods.gtk_widget_hide(handle);
			}
		}

		protected override bool IsControlDisposedInternal(Control control)
		{
			if (!IsControlCreated(control))
				return true;

			IntPtr handle = GetHandleForControl(control);

			bool isgood = Internal.GObject.Methods.g_type_check_instance_is_a(handle, Internal.GTK.Methods.gtk_widget_get_type());
			return !isgood;
		}

		protected override Monitor[] GetMonitorsInternal()
		{
			IntPtr defaultScreen = Internal.GDK.Methods.gdk_screen_get_default();
			int monitorCount = Internal.GDK.Methods.gdk_screen_get_n_monitors(defaultScreen);

			Monitor[] monitors = new Monitor[monitorCount];
			return monitors;
		}

		[DebuggerNonUserCode()]
		protected override DialogResult ShowDialogInternal(Dialog dialog, Window parent)
		{
			dialog.OnCreating(EventArgs.Empty);

			IntPtr parentHandle = IntPtr.Zero;
			if (parent == null)
			{
				if (dialog.Parent != null)
				{
					parentHandle = GetHandleForControl(dialog.Parent);
				}
			}
			else
			{
				parentHandle = GetHandleForControl(parent);
			}

			IntPtr handle = IntPtr.Zero;
			List<Button> buttons = new List<Button>();

			if (dialog is MessageDialog)
			{
				MessageDialog dlg = (dialog as MessageDialog);

				Internal.GTK.Constants.GtkMessageType messageType = Internal.GTK.Constants.GtkMessageType.Other;
				switch (dlg.Icon)
				{
					case MessageDialogIcon.Error:
					{
						messageType = Internal.GTK.Constants.GtkMessageType.Error;
						break;
					}
					case MessageDialogIcon.Information:
					{
						messageType = Internal.GTK.Constants.GtkMessageType.Info;
						break;
					}
					case MessageDialogIcon.Warning:
					{
						messageType = Internal.GTK.Constants.GtkMessageType.Warning;
						break;
					}
					case MessageDialogIcon.Question:
					{
						messageType = Internal.GTK.Constants.GtkMessageType.Question;
						break;
					}
				}

				Internal.GTK.Constants.GtkButtonsType buttonsType = Internal.GTK.Constants.GtkButtonsType.None;
				switch (dlg.Buttons)
				{
					case MessageDialogButtons.AbortRetryIgnore:
					case MessageDialogButtons.CancelTryContinue:
					case MessageDialogButtons.RetryCancel:
					case MessageDialogButtons.YesNoCancel:
					{
						buttonsType = Internal.GTK.Constants.GtkButtonsType.None;
						break;
					}
					case MessageDialogButtons.OK:
					{
						buttonsType = Internal.GTK.Constants.GtkButtonsType.OK;
						break;
					}
					case MessageDialogButtons.OKCancel:
					{
						buttonsType = Internal.GTK.Constants.GtkButtonsType.OKCancel;
						break;
					}
					case MessageDialogButtons.YesNo:
					{
						buttonsType = Internal.GTK.Constants.GtkButtonsType.YesNo;
						break;
					}
				}
				handle = Internal.GTK.Methods.gtk_message_dialog_new(parentHandle, Internal.GTK.Constants.GtkDialogFlags.Modal, messageType, buttonsType, dlg.Content);

				switch (dlg.Buttons)
				{
					case MessageDialogButtons.AbortRetryIgnore:
					{
						buttons.Add(new Button("_Abort", DialogResult.Abort));
						buttons.Add(new Button("_Retry", DialogResult.Retry));
						buttons.Add(new Button("_Ignore", DialogResult.Ignore));
						break;
					}
					case MessageDialogButtons.CancelTryContinue:
					{
						buttons.Add(new Button(ButtonStockType.Cancel, DialogResult.Abort));
						buttons.Add(new Button("T_ry Again", DialogResult.Retry));
						buttons.Add(new Button("C_ontinue", DialogResult.Ignore));
						break;
					}
					case MessageDialogButtons.RetryCancel:
					{
						buttons.Add(new Button("_Retry", DialogResult.Retry));
						buttons.Add(new Button(ButtonStockType.Cancel, DialogResult.Ignore));
						break;
					}
					case MessageDialogButtons.YesNoCancel:
					{
						buttons.Add(new Button(ButtonStockType.Yes, DialogResult.Abort));
						buttons.Add(new Button(ButtonStockType.No, DialogResult.Retry));
						buttons.Add(new Button(ButtonStockType.Cancel, DialogResult.Ignore));
						break;
					}
				}
			}
			else if (dialog is FileDialog)
			{
				handle = FileDialog_Create(dialog as FileDialog);
			}
			else if (dialog is ColorDialog)
			{
				handle = ColorDialog_Create(dialog as ColorDialog);
			}
			else if (dialog is FontDialog)
			{
				handle = FontDialog_Create(dialog as FontDialog);
			}
			else if (dialog is AboutDialog)
			{
				handle = AboutDialog_Create(dialog as AboutDialog);
			}
			else if (dialog is PrintDialog)
			{
				handle = PrintDialog_Create(dialog as PrintDialog);

				DialogResult result1 = PrintDialog_Run(parentHandle, handle);
				return result1;
			}
			else if (dialog is AppChooserDialog)
			{
				handle = AppChooserDialog_Create(dialog as AppChooserDialog);
			}
			else
			{
				handle = Dialog_Create(dialog, parentHandle);
			}

			Internal.GTK.Methods.gtk_window_set_decorated(handle, dialog.Decorated);

			// Add any additional buttons to the end of the buttons list
			foreach (Button button in dialog.Buttons)
			{
				buttons.Add(button);
			}

			IntPtr[] hButtons = Dialog_AddButtons(handle, buttons);

			if (dialog.DefaultButton != null)
			{
				IntPtr hButtonDefault = GetHandleForControl(dialog.DefaultButton);
				Internal.GTK.Methods.gtk_widget_grab_default(hButtonDefault);
			}

			DialogResult result = DialogResult.None;

			dialog.OnCreated(EventArgs.Empty);

			if (handle != IntPtr.Zero)
			{
				while (true)
				{
					int nativeResult = Internal.GTK.Methods.gtk_dialog_run(handle);

					switch (nativeResult)
					{
						case (int)Internal.GTK.Constants.GtkResponseType.OK:
						case (int)Internal.GTK.Constants.GtkResponseType.Accept:
						{
							if (dialog is FileDialog)
							{
								FileDialog_Accept(dialog as FileDialog, handle);
							}
							else if (dialog is ColorDialog)
							{
								ColorDialog_Accept(dialog as ColorDialog, handle);
							}
							else if (dialog is FontDialog)
							{
								FontDialog_Accept(dialog as FontDialog, handle);
							}
							result = DialogResult.OK;
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.Apply:
						{
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.Cancel:
						{
							result = DialogResult.Cancel;
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.Close:
						{
							result = DialogResult.Cancel;
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.DeleteEvent:
						{
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.Help:
						{
							result = DialogResult.Help;
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.No:
						{
							result = DialogResult.No;
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.None:
						{
							result = DialogResult.None;
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.Reject:
						{
							result = DialogResult.Cancel;
							break;
						}
						case (int)Internal.GTK.Constants.GtkResponseType.Yes:
						{
							result = DialogResult.Yes;
							break;
						}
					}

					// FIXME: this results in corruption; better to cancel dialog close event?
					if ((nativeResult != 0 && nativeResult != -1) || result == DialogResult.None)
					{
						break;
					}
				}
				Console.WriteLine("dialog has " + hButtons.Length.ToString() + " buttons");
				for (int i = 0; i < hButtons.Length; i++)
				{
					Console.WriteLine("uwt: gtk: destroying dialog button at handle " + hButtons[i].ToString());
					Internal.GTK.Methods.gtk_widget_destroy(hButtons[i]);
				}
				Console.WriteLine("uwt: gtk: gtk_widget_destroy");
				Internal.GTK.Methods.gtk_widget_destroy(handle);
			}
			return result;
		}

		private IntPtr AppChooserDialog_Create(AppChooserDialog dialog)
		{
			IntPtr handle = Internal.GTK.Methods.gtk_app_chooser_dialog_new_for_content_type(CommonDialog_GetParentHandle(dialog), Internal.GTK.Constants.GtkDialogFlags.Modal, dialog.ContentType);
			return handle;
		}

		private DialogResult PrintDialog_Run(IntPtr parent, IntPtr handle)
		{
			IntPtr error = IntPtr.Zero;
			Internal.GTK.Constants.GtkPrintOperationResult gtkResult = Internal.GTK.Methods.gtk_print_operation_run(handle, Internal.GTK.Constants.GtkPrintOperationAction.PrintDialog, parent, error);
			return GtkPrintOperationResultToDialogResult(gtkResult);
		}

		private DialogResult GtkPrintOperationResultToDialogResult(Internal.GTK.Constants.GtkPrintOperationResult value)
		{
			if (value == Internal.GTK.Constants.GtkPrintOperationResult.Cancel) return DialogResult.Cancel;
			return DialogResult.OK;
		}

		protected override void InvalidateControlInternal(Control control, int x, int y, int width, int height)
		{
			if (!IsControlCreated(control))
				throw new NullReferenceException("Control handle not found");

			IntPtr handle = GetHandleForControl(control);
			Internal.GTK.Methods.gtk_widget_queue_draw_area(handle, x, y, width, height);
		}

		#region Common Dialog
		private IntPtr CommonDialog_GetParentHandle(CommonDialog dlg)
		{
			if (dlg.Parent != null && IsControlCreated(dlg.Parent))
			{
				return GetHandleForControl(dlg.Parent);
			}
			return IntPtr.Zero;
		}
		#endregion
		#region File Dialog
		private IntPtr FileDialog_Create(FileDialog dlg)
		{
			string title = dlg.Title;

			Internal.GTK.Constants.GtkFileChooserAction fca = Internal.GTK.Constants.GtkFileChooserAction.Open;
			switch (dlg.Mode)
			{
				case FileDialogMode.CreateFolder:
				{
					fca = Internal.GTK.Constants.GtkFileChooserAction.CreateFolder;
					if (title == null)
						title = "Create Folder";
					break;
				}
				case FileDialogMode.Open:
				{
					fca = Internal.GTK.Constants.GtkFileChooserAction.Open;
					if (title == null)
						title = "Open";
					break;
				}
				case FileDialogMode.Save:
				{
					fca = Internal.GTK.Constants.GtkFileChooserAction.Save;
					if (title == null)
						title = "Save";
					break;
				}
				case FileDialogMode.SelectFolder:
				{
					fca = Internal.GTK.Constants.GtkFileChooserAction.SelectFolder;
					if (title == null)
						title = "Select Folder";
					break;
				}
			}

			IntPtr handle = Internal.GTK.Methods.gtk_file_chooser_dialog_new(title, CommonDialog_GetParentHandle(dlg), fca);

			// set up the file filters
			foreach (FileDialogFileNameFilter filter in dlg.FileNameFilters)
			{
				Internal.GTK.Methods.gtk_file_chooser_add_filter(handle, CreateGTKFileChooserFilter(filter));
			}

			string accept_button = "gtk-save";
			string cancel_button = "gtk-cancel";

			switch (dlg.Mode)
			{
				case FileDialogMode.CreateFolder:
				case FileDialogMode.Save:
				{
					accept_button = StockTypeToString(StockType.Save);
					break;
				}
				case FileDialogMode.SelectFolder:
				case FileDialogMode.Open:
				{
					accept_button = StockTypeToString(StockType.Open);
					break;
				}
			}

			switch (System.Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
				{
					// buttons go cancel, then accept
					Internal.GTK.Methods.gtk_dialog_add_button(handle, cancel_button, Internal.GTK.Constants.GtkResponseType.Cancel);
					Internal.GTK.Methods.gtk_dialog_add_button(handle, accept_button, Internal.GTK.Constants.GtkResponseType.Accept);
					break;
				}
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
				{
					// buttons go accept, then cancel
					Internal.GTK.Methods.gtk_dialog_add_button(handle, accept_button, Internal.GTK.Constants.GtkResponseType.Accept);
					Internal.GTK.Methods.gtk_dialog_add_button(handle, cancel_button, Internal.GTK.Constants.GtkResponseType.Cancel);
					break;
				}
			}

			Internal.GTK.Methods.gtk_file_chooser_set_select_multiple(handle, dlg.MultiSelect);
			return handle;
		}

		private IntPtr CreateGTKFileChooserFilter(FileDialogFileNameFilter filter)
		{
			IntPtr hFileFilter = Internal.GTK.Methods.gtk_file_filter_new();
			Internal.GTK.Methods.gtk_file_filter_set_name(hFileFilter, filter.Title);
			string[] patterns = filter.Filter.Split(new char[] { ';' });
			foreach (string pattern in patterns)
			{
				Internal.GTK.Methods.gtk_file_filter_add_pattern(hFileFilter, pattern);
			}
			return hFileFilter;
		}

		private void FileDialog_Accept(FileDialog dlg, IntPtr handle)
		{
			IntPtr gslist = Internal.GTK.Methods.gtk_file_chooser_get_filenames(handle);

			uint length = Internal.GLib.Methods.g_slist_length(gslist);
			dlg.SelectedFileNames.Clear();
			for (uint i = 0; i < length; i++)
			{
				// WE MUST DO THIS IN ORDER TO MANUALLY FREE THE MEMORY AT THE END
				IntPtr hFileNameStr = Internal.GLib.Methods.g_slist_nth_data(gslist, i);
				
				// This is now a managed pointer to a managed string. We're all good, so...
				string fileName = System.Runtime.InteropServices.Marshal.PtrToStringAuto(hFileNameStr);
				dlg.SelectedFileNames.Add(fileName);
				
				// DESTROY THE UNMANAGED POINTER NOW THAT WE'RE DONE!!!
				Internal.GLib.Methods.g_free(hFileNameStr);

				// this fixes a bug in Universal Editor where the FileDialog could only be opened a few times before crashing the application...
				// but DOES NOT fix the bug in Concertroid :(
			}

			Internal.GLib.Methods.g_slist_free(gslist);
		}
		#endregion
		#region Color Dialog
		private IntPtr ColorDialog_Create(ColorDialog dlg)
		{
			string title = dlg.Title;
			if (title == null)
				title = "Select Color";

			IntPtr handle = Internal.GTK.Methods.gtk_color_dialog_new(title, CommonDialog_GetParentHandle(dlg), !dlg.AutoUpgradeEnabled);
			return handle;
		}
		private void ColorDialog_Accept(ColorDialog dlg, IntPtr handle)
		{
			Internal.GDK.Structures.GdkRGBA color = new Internal.GDK.Structures.GdkRGBA();
			Internal.GTK.Methods.gtk_color_chooser_get_rgba(handle, out color);
			dlg.SelectedColor = Color.FromRGBADouble(color.red, color.green, color.blue, color.alpha);
		}
		#endregion
		#region Font Dialog
		private IntPtr FontDialog_Create(FontDialog dlg)
		{
			string title = dlg.Title;
			if (title == null)
				title = "Select Font";

			IntPtr handle = Internal.GTK.Methods.gtk_font_dialog_new(title, CommonDialog_GetParentHandle(dlg), !dlg.AutoUpgradeEnabled);
			return handle;
		}
		private void FontDialog_Accept(FontDialog dlg, IntPtr handle)
		{
			UniversalWidgetToolkit.Drawing.Font font = Internal.GTK.Methods.gtk_font_dialog_get_font(handle, !dlg.AutoUpgradeEnabled);
			dlg.SelectedFont = font;
		}
		#endregion
		#region About Dialog
		private IntPtr AboutDialog_Create(AboutDialog dlg)
		{
			IntPtr handle = Internal.GTK.Methods.gtk_about_dialog_new();
			Internal.GTK.Methods.gtk_about_dialog_set_program_name(handle, dlg.ProgramName);
			if (dlg.Version != null)
			{
				Internal.GTK.Methods.gtk_about_dialog_set_version(handle, dlg.Version.ToString());
			}
			Internal.GTK.Methods.gtk_about_dialog_set_copyright(handle, dlg.Copyright);
			Internal.GTK.Methods.gtk_about_dialog_set_comments(handle, dlg.Comments);
			if (dlg.LicenseText != null)
			{
				Internal.GTK.Methods.gtk_about_dialog_set_license(handle, dlg.LicenseText);
			}

			if (dlg.Website != null)
			{
				Internal.GTK.Methods.gtk_about_dialog_set_website(handle, dlg.Website);
			}

			if (Internal.GTK.Methods.LIBRARY_FILENAME == Internal.GTK.Methods.LIBRARY_FILENAME_V3)
			{
				if (dlg.LicenseType != LicenseType.Unknown)
				{
					switch (dlg.LicenseType)
					{
						case LicenseType.Artistic:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Artistic);
								break;
							}
						case LicenseType.BSD:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.BSD);
								break;
							}
						case LicenseType.Custom:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Custom);
								break;
							}
						case LicenseType.GPL20:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.GPL20);
								break;
							}
						case LicenseType.GPL30:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.GPL30);
								break;
							}
						case LicenseType.LGPL21:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.LGPL21);
								break;
							}
						case LicenseType.LGPL30:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.LGPL30);
								break;
							}
						case LicenseType.MITX11:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.MITX11);
								break;
							}
						case LicenseType.Unknown:
							{
								Internal.GTK.Methods.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Unknown);
								break;
							}
					}
				}
			}
			return handle;
		}
		#endregion
		#region Print Dialog
		private IntPtr PrintDialog_Create(PrintDialog dlg)
		{
			IntPtr handle = Internal.GTK.Methods.gtk_print_operation_new();
			return handle;
		}
		#endregion
		#region Generic Dialog
		private IntPtr Dialog_AddButton(IntPtr handle, Button button)
		{
			IntPtr buttonHandle = IntPtr.Zero;
			if (!IsControlCreated(button)) CreateControl(button);

			buttonHandle = GetHandleForControl(button);

			// Internal.GTK.Methods.gtk_dialog_add_button (handle, button.StockType == ButtonStockType.Connect ? "Connect" : "Cancel", button.ResponseValue);

			int nativeResponseValue = button.ResponseValue;
			switch (button.ResponseValue)
			{
				case (int)DialogResult.Cancel:
				{
					nativeResponseValue = (int)Internal.GTK.Constants.GtkResponseType.Cancel;
					break;
				}
				case (int)DialogResult.No:
				{
					nativeResponseValue = (int)Internal.GTK.Constants.GtkResponseType.No;
					break;
				}
				case (int)DialogResult.OK:
				{
					nativeResponseValue = (int)Internal.GTK.Constants.GtkResponseType.OK;
					break;
				}
				case (int)DialogResult.Yes:
				{
					nativeResponseValue = (int)Internal.GTK.Constants.GtkResponseType.Yes;
					break;
				}
			}

			Internal.GTK.Methods.gtk_dialog_add_action_widget(handle, buttonHandle, nativeResponseValue);
			Internal.GTK.Methods.gtk_widget_set_can_default(buttonHandle, true);

			// UpdateControlProperties(button, buttonHandle);
			// above updatecontrolprops call must be called after buttonHandle is realized

			Internal.GTK.Methods.gtk_widget_show_all(buttonHandle);
			return buttonHandle;
		}
		private IntPtr Dialog_Create(Dialog dlg, IntPtr hParent)
		{
			IntPtr handle = Internal.GTK.Methods.gtk_dialog_new_with_buttons(dlg.Title, hParent, Internal.GTK.Constants.GtkDialogFlags.Modal, null);
			Internal.GTK.Methods.gtk_window_set_title(handle, dlg.Title);

			IntPtr hDialogContent = Internal.GTK.Methods.gtk_dialog_get_content_area(handle);

			NativeControl hContainer = (new Controls.ContainerImplementation(this, dlg)).CreateControl(dlg);
			// NativeControl hContainer = CreateContainer (dlg);

			Internal.GTK.Methods.gtk_box_pack_start(hDialogContent, (hContainer as GTKNativeControl).Handle, true, true, 0);
			Internal.GTK.Methods.gtk_widget_show_all(hDialogContent);

			RegisterControlHandle(dlg, handle);
			return handle;
		}
		private IntPtr[] Dialog_AddButtons(IntPtr handle, List<Button> buttons)
		{
			List<IntPtr> list = new List<IntPtr>();
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
				{
					for (int i = buttons.Count - 1; i > -1; i--)
					{
						IntPtr hButton = Dialog_AddButton(handle, buttons[i]);
						list.Add(hButton);
					}
					break;
				}
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
				{
					for (int i = 0; i < buttons.Count; i++)
					{
						IntPtr hButton = Dialog_AddButton(handle, buttons[i]);
						list.Add(hButton);
					}
					break;
				}
			}
			return list.ToArray();
		}
		#endregion

		protected override void UpdateControlPropertiesInternal(Control control, IntPtr handle)
		{
			if (control is Button)
			{
				Button button = (control as Button);

				string text = control.Text;
				if (!String.IsNullOrEmpty(text))
				{
					text = text.Replace('&', '_');
				}

				if (!String.IsNullOrEmpty(text))
				{
					// Internal.GTK.Methods.gtk_button_set_label(handle, text);
				}

				if (button.StockType != ButtonStockType.None)
				{
					Internal.GTK.Methods.gtk_button_set_label(handle, StockTypeToString((StockType)button.StockType));
					Internal.GTK.Methods.gtk_button_set_use_stock(handle, true);
				}

				Internal.GTK.Methods.gtk_button_set_use_underline(handle, true);
				Internal.GTK.Methods.gtk_button_set_focus_on_click(handle, true);

				switch (button.BorderStyle)
				{
					case ButtonBorderStyle.None:
					{
						Internal.GTK.Methods.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.None);
						break;
					}
					case ButtonBorderStyle.Half:
					{
						Internal.GTK.Methods.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.Half);
						break;
					}
					case ButtonBorderStyle.Normal:
					{
						Internal.GTK.Methods.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.Normal);
						break;
					}
				}
			}
			Internal.GTK.Methods.gtk_widget_set_sensitive(handle, control.Enabled);
			Internal.GTK.Methods.gtk_widget_set_size_request(handle, (int)control.Size.Width, (int)control.Size.Height);
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


		protected override void TabContainer_ClearTabPagesInternal(TabContainer parent)
		{
			if (!IsControlCreated(parent))
				return;

			IntPtr handle = GetHandleForControl(parent);
			int pageCount = Internal.GTK.Methods.gtk_notebook_get_n_pages(handle);
			for (int i = 0; i < pageCount; i++)
			{
				Internal.GTK.Methods.gtk_notebook_remove_page(handle, i);
			}
		}
		protected override void TabContainer_InsertTabPageInternal(TabContainer parent, int index, TabPage tabPage)
		{
			if (!IsControlCreated(parent))
				return;

			IntPtr handle = GetHandleForControl(parent);
			Controls.TabContainerImplementation.NotebookAppendPage(this, parent, handle, tabPage, index);
		}
		protected override void TabContainer_RemoveTabPageInternal(TabContainer parent, TabPage tabPage)
		{
			throw new NotImplementedException();
		}

		private Dictionary<NotificationIcon, NotificationIconInfo> notificationIconInfo = new Dictionary<NotificationIcon, NotificationIconInfo>();

		protected override void UpdateNotificationIconInternal(NotificationIcon nid, bool updateContextMenu)
		{
			try
			{
				NotificationIconInfo nii = new NotificationIconInfo();
				if (!notificationIconInfo.ContainsKey(nid))
				{
					nii.hIndicator = Internal.AppIndicator.Methods.app_indicator_new(nid.Name, nid.IconNameDefault, Internal.AppIndicator.Constants.AppIndicatorCategory.ApplicationStatus);
					notificationIconInfo.Add(nid, nii);

					// Internal.AppIndicator.Methods.app_indicator_set_label(hIndicator, nid.Text, "I don't know what this is for");
					// Internal.AppIndicator.Methods.app_indicator_set_title(hIndicator, nid.Text);
				}
				else
				{
					nii = notificationIconInfo[nid];
				}

				if (updateContextMenu)
				{
					IntPtr hMenu = Internal.GTK.Methods.gtk_menu_new();

					IntPtr hMenuTitle = Internal.GTK.Methods.gtk_menu_item_new();
					Internal.GTK.Methods.gtk_widget_set_sensitive(hMenuTitle, false);
					Internal.GTK.Methods.gtk_menu_shell_append(hMenu, hMenuTitle);
					nii.hMenuItemTitle = hMenuTitle;

					IntPtr hMenuSeparator = Internal.GTK.Methods.gtk_separator_menu_item_new();
					Internal.GTK.Methods.gtk_menu_shell_append(hMenu, hMenuSeparator);

					if (nid.ContextMenu != null)
					{
						foreach (MenuItem mi in nid.ContextMenu.Items)
						{
							InitMenuItem(mi, hMenu);
						}
					}

					Internal.GTK.Methods.gtk_widget_show_all(hMenu);

					Internal.AppIndicator.Methods.app_indicator_set_menu(nii.hIndicator, hMenu);
				}

				if (nii.hMenuItemTitle != IntPtr.Zero)
				{
					Internal.GTK.Methods.gtk_menu_item_set_label(nii.hMenuItemTitle, nid.Text);
				}

				Internal.AppIndicator.Methods.app_indicator_set_attention_icon(nii.hIndicator, nid.IconNameAttention);
				switch (nid.Status)
				{
					case NotificationIconStatus.Hidden:
						{
							Internal.AppIndicator.Methods.app_indicator_set_status(nii.hIndicator, Internal.AppIndicator.Constants.AppIndicatorStatus.Passive);
							break;
						}
					case NotificationIconStatus.Visible:
						{
							Internal.AppIndicator.Methods.app_indicator_set_status(nii.hIndicator, Internal.AppIndicator.Constants.AppIndicatorStatus.Active);
							break;
						}
					case NotificationIconStatus.Attention:
						{
							Internal.AppIndicator.Methods.app_indicator_set_status(nii.hIndicator, Internal.AppIndicator.Constants.AppIndicatorStatus.Attention);
							break;
						}
				}
			}
			catch
			{
			}
		}

		private Internal.GDL.Constants.GdlDockItemBehavior UwtDockItemBehaviorToGtkDockItemBehavior(DockingItemBehavior value)
		{
			Internal.GDL.Constants.GdlDockItemBehavior retval = Internal.GDL.Constants.GdlDockItemBehavior.BEH_NORMAL;
			if ((value & DockingItemBehavior.Normal) == DockingItemBehavior.Normal) retval |= Internal.GDL.Constants.GdlDockItemBehavior.BEH_NORMAL;
			return retval;
		}

		protected override void ShowNotificationPopupInternal(NotificationPopup popup)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hNotification = Internal.Notify.Methods.notify_notification_new(popup.Summary, popup.Content, popup.IconName);
			Internal.Notify.Methods.notify_notification_show(hNotification, hError);
		}

		protected override void RepaintCustomControl(CustomControl control, int x, int y, int width, int height)
		{
			IntPtr handle = GetHandleForControl(control);
			Internal.GTK.Methods.gtk_widget_queue_draw_area(handle, x, y, width, height);
		}
	}
}

