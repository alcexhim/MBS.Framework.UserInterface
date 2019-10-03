using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using MBS.Framework.Collections.Generic;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Docking;
using MBS.Framework.UserInterface.Dialogs;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Drawing;

using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;

using MBS.Framework.Drawing;
using System.Runtime.InteropServices;
using MBS.Framework.UserInterface.Controls.FileBrowser;
using MBS.Framework.UserInterface.Printing;
using MBS.Framework.UserInterface.Engines.GTK.Printing;
using MBS.Framework.UserInterface.Engines.GTK.Drawing;
using MBS.Framework.UserInterface.Engines.GTK.Internal.GTK;

namespace MBS.Framework.UserInterface.Engines.GTK
{
	public class GTKEngine : Engine
	{
		protected override int Priority => (System.Environment.OSVersion.Platform == PlatformID.Unix ? 1 : -1);

		private int _exitCode = 0;
		private IntPtr mvarApplicationHandle = IntPtr.Zero;

		// TODO: this should be migrated to the appropriate refactoring once we figure out what that is
		protected override Image LoadImage(byte[] filedata, string type)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hLoader = CreateImageLoader(type);
			return LoadImage(hLoader, filedata, ref hError);
		}
		protected override Image LoadImage(string filename, string type = null)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hLoader = CreateImageLoader(type);
			byte[] buffer = System.IO.File.ReadAllBytes(filename);
			return LoadImage(hLoader, buffer, ref hError);
		}
		protected override Image LoadImageByName(string name, int size)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hTheme = Internal.GTK.Methods.GtkIconTheme.gtk_icon_theme_get_default();
			IntPtr hPixbuf = Internal.GTK.Methods.GtkIconTheme.gtk_icon_theme_load_icon(hTheme, name, size, Constants.GtkIconLookupFlags.None, ref hError);
			return new GTKNativeImage(hPixbuf);
		}

		private IntPtr CreateImageLoader(string type = null)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hLoader = IntPtr.Zero;
			if (type != null)
			{
				hLoader = Internal.GDK.Methods.gdk_pixbuf_loader_new_with_type(type, ref hError);
			}
			else
			{
				hLoader = Internal.GDK.Methods.gdk_pixbuf_loader_new();
			}
			return hLoader;
		}

		private Image LoadImage(IntPtr hLoader, byte[] buffer, ref IntPtr hError)
		{
			Internal.GDK.Methods.gdk_pixbuf_loader_write(hLoader, buffer, buffer.Length, ref hError);
			Internal.GDK.Methods.gdk_pixbuf_loader_close(hLoader);

			IntPtr hPixbuf = Internal.GDK.Methods.gdk_pixbuf_loader_get_pixbuf(hLoader);
			Internal.GObject.Methods.g_object_unref(hLoader);

			return new GTKNativeImage(hPixbuf);
		}

		private static Version _Version = null;
		public static Version Version
		{
			get
			{
				if (_Version == null) {
					uint major = Internal.GTK.Methods.Gtk.gtk_get_major_version();
					uint minor = Internal.GTK.Methods.Gtk.gtk_get_minor_version();
					uint micro = Internal.GTK.Methods.Gtk.gtk_get_micro_version();
					_Version = new Version((int)major, (int)minor, (int)micro);
				}
				return _Version;
			}
		}

		protected override bool WindowHasFocusInternal(Window window)
		{
			IntPtr hWindow = (GetHandleForControl(window) as GTKNativeControl).Handle;
			return Internal.GTK.Methods.GtkWindow.gtk_window_has_toplevel_focus(hWindow);
		}

		protected override Vector2D ClientToScreenCoordinatesInternal(Vector2D point)
		{
			return point;
		}

		protected override bool IsControlEnabledInternal(Control control)
		{
			IntPtr handle = (GetHandleForControl(control) as GTKNativeControl).Handle;
			return Internal.GTK.Methods.GtkWidget.gtk_widget_is_sensitive(handle);
		}
		protected override void SetControlEnabledInternal(Control control, bool value)
		{
			IntPtr handle = (GetHandleForControl(control) as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(handle, value);
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
			IntPtr hList = Internal.GTK.Methods.GtkWindow.gtk_window_list_toplevels();
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
				RegisterControlHandle(window, new GTKNativeControl(data));
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

		internal static DialogResult GtkResponseTypeToDialogResult(Internal.GTK.Constants.GtkResponseType value)
		{
			DialogResult result = DialogResult.None;
			switch (value)
			{
				case Internal.GTK.Constants.GtkResponseType.OK:
				case Internal.GTK.Constants.GtkResponseType.Accept:
				{
					result = DialogResult.OK;
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.Apply:
				{
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.Cancel:
				{
					result = DialogResult.Cancel;
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.Close:
				{
					result = DialogResult.Cancel;
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.DeleteEvent:
				{
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.Help:
				{
					result = DialogResult.Help;
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.No:
				{
					result = DialogResult.No;
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.None:
				{
					result = DialogResult.None;
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.Reject:
				{
					result = DialogResult.Cancel;
					break;
				}
				case Internal.GTK.Constants.GtkResponseType.Yes:
				{
					result = DialogResult.Yes;
					break;
				}
			}
			return result;
		}

		protected override bool InitializeInternal()
		{
			string[] argv = System.Environment.GetCommandLineArgs();
			int argc = argv.Length;

			bool check = Internal.GTK.Methods.Gtk.gtk_init_check(ref argc, ref argv);
			if (!check)
				return check;

			string appname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
			Internal.Notify.Methods.notify_init(appname);

			gc_Application_Activate = new Internal.GObject.Delegates.GCallback(Application_Activate);
			gc_Application_Startup = new Internal.GObject.Delegates.GCallback(Application_Startup);
			gc_MenuItem_Activated = new Internal.GObject.Delegates.GCallback(MenuItem_Activate);
			gc_Application_CommandLine = new Internal.GObject.Delegates.GApplicationCommandLineHandler(Application_CommandLine);

			IntPtr hApp = Internal.GTK.Methods.GtkApplication.gtk_application_new(Application.UniqueName, Internal.GIO.Constants.GApplicationFlags.HandlesCommandLine | Internal.GIO.Constants.GApplicationFlags.HandlesOpen);

			if (hApp == IntPtr.Zero)
			{
				Console.WriteLine("error initting app {0}", Application.UniqueName);
			}
			if (hApp != IntPtr.Zero)
			{
				bool isRegistered = Internal.GIO.Methods.g_application_get_is_registered(hApp);
				Console.WriteLine("GtkApplication is registered? {0}", isRegistered);

				if (!isRegistered)
				{
					Console.WriteLine("no, we are registering it now");
					Internal.GIO.Methods.g_application_register(hApp, IntPtr.Zero, IntPtr.Zero);
				}

				isRegistered = Internal.GIO.Methods.g_application_get_is_registered(hApp);
				Console.WriteLine("GtkApplication is registered? How about now? {0}", isRegistered);
				/*
				IntPtr simpleActionGroup = Internal.GIO.Methods.g_simple_action_group_new();

				IntPtr hActionNew = Internal.GIO.Methods.g_simple_action_new("new", Internal.GLib.Constants.GVariantType.Byte);

				Internal.GIO.Methods.g_action_map_add_action(hApp, hActionNew);

				IntPtr hMenu = Internal.GIO.Methods.g_menu_new();

				IntPtr hMenuItemFile = Internal.GIO.Methods.g_menu_item_new("_New Document", "app.new");

				Internal.GIO.Methods.g_menu_append_item(hMenu, hMenuItemFile);

				Internal.GTK.Methods.Methods.gtk_application_set_app_menu(hApp, hMenu);
				*/
				mvarApplicationHandle = hApp;
			}

			// TODO: fix this, it doesn't work (crashes with SIGSEGV)
			// Internal.GIO.Methods.g_action_map_add_action (hApp, actionFile);

			/*
			IntPtr hMenu = Internal.GIO.Methods.g_menu_new ();s
			IntPtr hMenuItem = Internal.GIO.Methods.g_menu_item_new ("File", "app.file");
			Internal.GIO.Methods.g_menu_append_item (hMenu, hMenuItem);

			Internal.GTK.Methods.Methods.gtk_application_set_menubar (hApp, hMenu);
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
				Internal.GObject.Methods.g_signal_connect(mvarApplicationHandle, "startup", gc_Application_Startup, IntPtr.Zero);
				Internal.GObject.Methods.g_signal_connect(mvarApplicationHandle, "command_line", gc_Application_CommandLine, IntPtr.Zero);
				Internal.GIO.Methods.g_application_run(mvarApplicationHandle, argc, argv);
			}

			if (waitForClose != null)
			{
				waitForClose.Closed += WaitForClose_Closed;
			}

			Internal.GTK.Methods.Gtk.gtk_main();

			return _exitCode;
		}

		private void WaitForClose_Closed(object sender, EventArgs e)
		{
			Application.Stop();
		}

		protected override void StopInternal(int exitCode)
		{
			Internal.GTK.Methods.Gtk.gtk_main_quit();
			_exitCode = exitCode;
		}

		private Dictionary<Layout, IntPtr> handlesByLayout = new Dictionary<Layout, IntPtr>();

		private Dictionary<IntPtr, MenuItem> menuItemsByHandle = new Dictionary<IntPtr, MenuItem>();

		private Internal.GObject.Delegates.GCallback gc_MenuItem_Activated = null;
		private Internal.GObject.Delegates.GCallback gc_Application_Activate = null;
		private Internal.GObject.Delegates.GCallback gc_Application_Startup = null;
		private Internal.GObject.Delegates.GApplicationCommandLineHandler gc_Application_CommandLine = null;

		private void Application_Activate(IntPtr handle, IntPtr data)
		{
		}
		private void Application_Startup(IntPtr application, IntPtr user_data)
		{
			Console.WriteLine("Application_Startup");
			InvokeStaticMethod(typeof(Application), "OnStartup");
		}
		private int Application_CommandLine(IntPtr handle, IntPtr commandLine, IntPtr data)
		{
			return 0;
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

			GtkPrintJob_status_changed_handler = new Internal.GObject.Delegates.GCallbackV1I(GtkPrintJob_status_changed);
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
			IntPtr handle = (GetHandleForControl(control) as GTKNativeControl).Handle;
			if (control is Dialog)
			{
				// this way is recommended per GTK3.0 docs:
				// "destroying the dialog during gtk_dialog_run() is a very bad idea, because your post-run code won't know whether the dialog was destroyed or not"
				Internal.GTK.Methods.GtkDialog.gtk_dialog_response(handle, Internal.GTK.Constants.GtkResponseType.None);
				return;
			}
			Internal.GTK.Methods.GtkWidget.gtk_widget_destroy(handle);
		}

		internal static KeyEventArgs GdkEventKeyToKeyEventArgs(Internal.GDK.Structures.GdkEventKey e)
		{
			uint keyCode = e.keyval;
			uint keyData = e.hardware_keycode;

			KeyEventArgs ee = new KeyEventArgs();
			KeyboardModifierKey modifierKeys = KeyboardModifierKey.None;
			ee.Key = GdkKeyCodeToKeyboardKey(e.keyval, e.hardware_keycode, out modifierKeys);
			ee.ModifierKeys = GdkModifierTypeToKeyboardModifierKey(e.state);
			return ee;
		}


		internal static KeyboardKey GdkKeyCodeToKeyboardKey(uint keyval, uint keycode, out KeyboardModifierKey modifierKeys)
		{
			KeyboardKey key = KeyboardKey.None;
			modifierKeys = KeyboardModifierKey.None;

			switch (keyval)
			{
				case 33: key = KeyboardKey.D1; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 64: key = KeyboardKey.D2; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 35: key = KeyboardKey.D3; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 36: key = KeyboardKey.D4; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 37: key = KeyboardKey.D5; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 94: key = KeyboardKey.D6; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 38: key = KeyboardKey.D7; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 42: key = KeyboardKey.D8; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 40: key = KeyboardKey.D9; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 41: key = KeyboardKey.D0; modifierKeys |= KeyboardModifierKey.Shift; break;

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
				case 65360: key = KeyboardKey.Home; break;
				case 65367: key = KeyboardKey.End; break;
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

		internal static MouseEventArgs GdkEventButtonToMouseEventArgs(Internal.GDK.Structures.GdkEventButton e)
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

		internal static MouseEventArgs TranslateMouseEventArgs(MouseEventArgs ee, IntPtr widget)
		{
			// translate the (window) mouse coordinates for a GtkWidget into (widget) relative coordinates
			int outX = 0, outY = 0;
			IntPtr hChildList = Internal.GTK.Methods.GtkContainer.gtk_container_get_children(widget);
			IntPtr hChild = Internal.GLib.Methods.g_list_nth_data(hChildList, 0);
			Internal.GTK.Methods.GtkWidget.gtk_widget_translate_coordinates(widget, hChild, (int)ee.X, (int)ee.Y, ref outX, ref outY);
			return new MouseEventArgs(outX, outY, ee.Buttons, ee.ModifierKeys);
		}

		internal static MouseEventArgs GdkEventMotionToMouseEventArgs(Internal.GDK.Structures.GdkEventMotion e)
		{
			MouseButtons buttons = GdkModifierTypeToMouseButtons(e.state);
			KeyboardModifierKey modifierKeys = GdkModifierTypeToKeyboardModifierKey(e.state);
			MouseEventArgs ee = new MouseEventArgs(e.x, e.y, buttons, modifierKeys);
			return ee;
		}

		public Control GetControlByHandle(IntPtr handle)
		{
			foreach (KeyValuePair<NativeControl, Control> kvp in controlsByHandle) {
				if (kvp.Key is GTKNativeControl) {
					if ((kvp.Key as GTKNativeControl).ContainsHandle(handle)) {
						return kvp.Value;
					}
				}
			}
			return null;
		}

		protected override void UpdateControlLayoutInternal(Control control)
		{
			IntPtr hCtrl = (GetHandleForControl(control) as GTKNativeControl).Handle;
			if (control.Parent != null && control.Parent.Layout != null) {
				Constraints constraints = control.Parent.Layout.GetControlConstraints(control);
				if (constraints != null) {
					if (control.Parent.Layout is Layouts.BoxLayout) {
						Layouts.BoxLayout.Constraints cs = (constraints as Layouts.BoxLayout.Constraints);
						if (cs != null) {
							Internal.GTK.Constants.GtkPackType packType = Internal.GTK.Constants.GtkPackType.Start;
							switch (cs.PackType) {
							case Layouts.BoxLayout.PackType.Start: {
									packType = Internal.GTK.Constants.GtkPackType.Start;
									break;
								}
							case Layouts.BoxLayout.PackType.End: {
									packType = Internal.GTK.Constants.GtkPackType.End;
									break;
								}
							}

							IntPtr hLayout = IntPtr.Zero;
							if (handlesByLayout.ContainsKey(control.Parent.Layout)) {
								hLayout = handlesByLayout[control.Parent.Layout];
							} else {
								hLayout = Internal.GTK.Methods.GtkBox.gtk_hbox_new(true, 0);
							}

							int padding = (cs.Padding == 0 ? control.Padding.All : cs.Padding);
							Internal.GTK.Methods.GtkBox.gtk_box_set_child_packing(hLayout, hCtrl, cs.Expand, cs.Fill, padding, packType);
						}
					}
				}
			}

			control.ControlImplementation?.UpdateControlLayout();
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

				if (handle is CustomNativeControl) {
					Control ctl = (handle as CustomNativeControl).Handle;
					handle = CreateControlInternal(ctl);
				}

				IntPtr nativeHandle = (handle as GTKNativeControl).Handle;

				if (control.TooltipText != null)
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_tooltip_text(nativeHandle, control.TooltipText);
				}

				RegisterControlHandle(control, handle as GTKNativeControl);

				UpdateControlLayout(control);
				UpdateControlProperties(control);

				if (control.Visible) {
					Internal.GTK.Methods.GtkWidget.gtk_widget_show_all((handle as GTKNativeControl).Handle);
				}
			}

			return (handle as GTKNativeControl);
		}

		protected override bool IsControlDisposedInternal(Control control)
		{
			if (!IsControlCreated(control))
				return true;

			IntPtr handle = (GetHandleForControl(control) as GTKNativeControl).Handle;

			bool isgood = Internal.GObject.Methods.g_type_check_instance_is_a(handle, Internal.GTK.Methods.GtkWidget.gtk_widget_get_type());
			return !isgood;
		}

		protected override Monitor[] GetMonitorsInternal()
		{
			IntPtr defaultScreen = Internal.GDK.Methods.gdk_screen_get_default();
			int monitorCount = Internal.GDK.Methods.gdk_screen_get_n_monitors(defaultScreen);

			Monitor[] monitors = new Monitor[monitorCount];
			return monitors;
		}

		private Func<IntPtr, IntPtr, bool> gc_delete_event_handler = null;
		private bool gc_delete_event(IntPtr /*GtkWidget*/ widget, IntPtr /*GdkEventKey*/ evt)
		{
			// blatently stolen from GTKNativeImplementation
			// we need to build more GTKNativeImplementation-based dialog impls to avoid code bloat

			// destroy all handles associated with widget
			Control ctl = GetControlByHandle(widget);
			UnregisterControlHandle(ctl);
			return false;
		}

		// hack hack hack until we base everything off of GTKNativeImplementation
		private void InitializeEventHandlers()
		{
			// eww
			gc_delete_event_handler = new Func<IntPtr, IntPtr, bool>(gc_delete_event);
		}

		protected override DialogResult ShowDialogInternal(Dialog dialog, Window parent)
		{
			InvokeMethod(dialog, "OnCreating", EventArgs.Empty);

			IntPtr parentHandle = IntPtr.Zero;
			if (parent == null)
			{
				if (dialog.Parent != null)
				{
					parentHandle = (GetHandleForControl(dialog.Parent) as GTKNativeControl).Handle;
				}
			}
			else
			{
				parentHandle = (GetHandleForControl(parent) as GTKNativeControl).Handle;
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
				handle = Internal.GTK.Methods.GtkMessageDialog.gtk_message_dialog_new(parentHandle, Internal.GTK.Constants.GtkDialogFlags.Modal, messageType, buttonsType, dlg.Content);

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
				GTKNativeControl nc = ((new Dialogs.PrintDialogImplementation(this, dialog).CreateControl(dialog)) as GTKNativeControl);
				handle = nc.Handle;

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

			Internal.GTK.Methods.GtkWindow.gtk_window_set_decorated(handle, dialog.Decorated);
			Internal.GTK.Methods.GtkWindow.gtk_window_set_default_size(handle, (int)dialog.Size.Width, (int)dialog.Size.Height);
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_size_request(handle, (int)dialog.MinimumSize.Width, (int)dialog.MinimumSize.Height);

			Internal.GObject.Methods.g_signal_connect(handle, "delete_event", gc_delete_event_handler);

			// Add any additional buttons to the end of the buttons list
			foreach (Button button in dialog.Buttons)
			{
				buttons.Add(button);
			}

			IntPtr[] hButtons = Dialog_AddButtons(handle, buttons);

			if (dialog.DefaultButton != null)
			{
				IntPtr hButtonDefault = (GetHandleForControl(dialog.DefaultButton) as GTKNativeControl).Handle;
				Internal.GTK.Methods.GtkWidget.gtk_widget_grab_default(hButtonDefault);
			}

			// if (dialog.AutoUpgradeEnabled) {
			Internal.GLib.Structures.Value val = new MBS.Framework.UserInterface.Engines.GTK.Internal.GLib.Structures.Value(1);
			// }

			DialogResult result = DialogResult.None;

			InvokeMethod(dialog, "OnCreated", EventArgs.Empty);

			if (handle != IntPtr.Zero)
			{
				int nativeResult = Internal.GTK.Methods.GtkDialog.gtk_dialog_run(handle);

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
					if (result == DialogResult.None)
						result = dialog.DialogResult;
				}

				/*
				for (int i = 0; i < hButtons.Length; i++)
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_destroy(hButtons[i]);
				}
				*/
				Internal.GTK.Methods.GtkWidget.gtk_widget_destroy(handle);
			}
			return result;
		}

		private IntPtr AppChooserDialog_Create(AppChooserDialog dialog)
		{
			IntPtr handle = Internal.GTK.Methods.GtkAppChooserDialog.gtk_app_chooser_dialog_new_for_content_type(CommonDialog_GetParentHandle(dialog), Internal.GTK.Constants.GtkDialogFlags.Modal, dialog.ContentType);
			return handle;
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

			IntPtr handle = (GetHandleForControl(control) as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_queue_draw_area(handle, x, y, width, height);
		}

		#region Common Dialog
		private IntPtr CommonDialog_GetParentHandle(CommonDialog dlg)
		{
			if (dlg.Parent != null && IsControlCreated(dlg.Parent))
			{
				return (GetHandleForControl(dlg.Parent) as GTKNativeControl).Handle;
			}
			return IntPtr.Zero;
		}
		#endregion
		#region File Dialog
		private IntPtr FileDialog_Create(FileDialog dlg)
		{
			string title = dlg.Text;

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

			IntPtr handle = Internal.GTK.Methods.GtkFileChooserDialog.gtk_file_chooser_dialog_new(title, CommonDialog_GetParentHandle(dlg), fca);

			// set up the file filters
			foreach (FileDialogFileNameFilter filter in dlg.FileNameFilters)
			{
				Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_add_filter(handle, CreateGTKFileChooserFilter(filter));
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
					// gnome3 : no longer display explicitcancel button in UI
					Internal.GTK.Methods.GtkDialog.gtk_dialog_add_button(handle, cancel_button, Internal.GTK.Constants.GtkResponseType.Cancel);
					Internal.GTK.Methods.GtkDialog.gtk_dialog_add_button(handle, accept_button, Internal.GTK.Constants.GtkResponseType.Accept);
					break;
				}
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
				{
					// buttons go accept, then cancel
					Internal.GTK.Methods.GtkDialog.gtk_dialog_add_button(handle, accept_button, Internal.GTK.Constants.GtkResponseType.Accept);
					Internal.GTK.Methods.GtkDialog.gtk_dialog_add_button(handle, cancel_button, Internal.GTK.Constants.GtkResponseType.Cancel);
					break;
				}
			}

			Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_set_select_multiple(handle, dlg.MultiSelect);
			Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_set_do_overwrite_confirmation(handle, dlg.ConfirmOverwrite);
			if (dlg.SelectedFileNames.Count > 0)
			{
				if (System.IO.File.Exists(dlg.SelectedFileNames[0]) && dlg.HighlightExistingFile)
				{
					Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_set_filename(handle, dlg.SelectedFileNames[0]);
				}
				else
				{
					Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_set_current_name(handle, dlg.SelectedFileNames[0]);
				}
			}
			return handle;
		}

		internal static IntPtr CreateGTKFileChooserFilter(FileDialogFileNameFilter filter)
		{
			IntPtr hFileFilter = Internal.GTK.Methods.GtkFileFilter.gtk_file_filter_new();
			Internal.GTK.Methods.GtkFileFilter.gtk_file_filter_set_name(hFileFilter, filter.Title);
			string[] patterns = filter.Filter.Split(new char[] { ';' });
			foreach (string pattern in patterns)
			{
				Internal.GTK.Methods.GtkFileFilter.gtk_file_filter_add_pattern(hFileFilter, pattern);
			}
			return hFileFilter;
		}

		private void FileDialog_Accept(FileDialog dlg, IntPtr handle)
		{
			string[] fileNames = Internal.GTK.Methods.GtkFileChooser.gtk_file_chooser_get_filenames_managed (handle);
			foreach (string fileName in fileNames) {
				dlg.SelectedFileNames.Add (fileName);
			}
		}
		#endregion
		#region Color Dialog
		private IntPtr ColorDialog_Create(ColorDialog dlg)
		{
			string title = dlg.Text;
			if (title == null)
				title = "Select Color";

			IntPtr handle = Internal.GTK.Methods.GtkColorDialog.gtk_color_dialog_new(title, CommonDialog_GetParentHandle(dlg), !dlg.AutoUpgradeEnabled);
			return handle;
		}
		private void ColorDialog_Accept(ColorDialog dlg, IntPtr handle)
		{
			Internal.GDK.Structures.GdkRGBA color = new Internal.GDK.Structures.GdkRGBA();
			Internal.GTK.Methods.GtkColorDialog.gtk_color_chooser_get_rgba(handle, out color);
			dlg.SelectedColor = Color.FromRGBADouble(color.red, color.green, color.blue, color.alpha);
		}
		#endregion
		#region Font Dialog
		private IntPtr FontDialog_Create(FontDialog dlg)
		{
			string title = dlg.Text;
			if (title == null)
				title = "Select Font";

			IntPtr handle = Internal.GTK.Methods.GtkFontChooserDialog.gtk_font_dialog_new(title, CommonDialog_GetParentHandle(dlg), !dlg.AutoUpgradeEnabled);
			return handle;
		}
		private void FontDialog_Accept(FontDialog dlg, IntPtr handle)
		{
			MBS.Framework.UserInterface.Drawing.Font font = Internal.GTK.Methods.GtkFontChooserDialog.gtk_font_dialog_get_font(handle, !dlg.AutoUpgradeEnabled);
			dlg.SelectedFont = font;
		}
		#endregion
		#region About Dialog
		private IntPtr AboutDialog_Create(AboutDialog dlg)
		{
			IntPtr handle = Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_new();
			Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_program_name(handle, dlg.ProgramName);
			if (dlg.Version != null)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_version(handle, dlg.Version.ToString());
			}
			Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_copyright(handle, dlg.Copyright);
			Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_comments(handle, dlg.Comments);
			if (dlg.LicenseText != null)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license(handle, dlg.LicenseText);
			}

			if (dlg.Website != null)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_website(handle, dlg.Website);
			}

			if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V3)
			{
				if (dlg.LicenseType != LicenseType.Unknown)
				{
					switch (dlg.LicenseType)
					{
						case LicenseType.Artistic:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Artistic);
								break;
							}
						case LicenseType.BSD:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.BSD);
								break;
							}
						case LicenseType.Custom:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Custom);
								break;
							}
						case LicenseType.GPL20:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.GPL20);
								break;
							}
						case LicenseType.GPL30:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.GPL30);
								break;
							}
						case LicenseType.LGPL21:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.LGPL21);
								break;
							}
						case LicenseType.LGPL30:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.LGPL30);
								break;
							}
						case LicenseType.MITX11:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.MITX11);
								break;
							}
						case LicenseType.Unknown:
							{
								Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Unknown);
								break;
							}
					}
				}
			}
			return handle;
		}
		#endregion
		#region Print Dialog

		private DialogResult PrintDialog_Run(IntPtr parent, IntPtr handle)
		{
			IntPtr error = IntPtr.Zero;
			Internal.GTK.Constants.GtkResponseType gtkResult = (Internal.GTK.Constants.GtkResponseType) Internal.GTK.Methods.GtkDialog.gtk_dialog_run(handle);
			Internal.GTK.Methods.GtkWidget.gtk_widget_destroy(handle);
			return GtkResponseTypeToDialogResult(gtkResult);
		}

		private Dictionary<Printer, IntPtr> _PrinterToHandle = new Dictionary<Printer, IntPtr>();
		private Dictionary<IntPtr, Printer> _HandleToPrinter = new Dictionary<IntPtr, Printer>();
		private IntPtr PrinterToHandle(Printer printer)
		{
			if (_PrinterToHandle.ContainsKey(printer))
				return _PrinterToHandle[printer];
			return (printer as GTKPrinter).Handle;
		}
		private Printer HandleToPrinter(IntPtr handle)
		{
			if (_HandleToPrinter.ContainsKey(handle))
				return _HandleToPrinter[handle];
			return null;
		}
		private void RegisterPrinter(Printer printer, IntPtr handle)
		{
			_PrinterToHandle[printer] = handle;
			_HandleToPrinter[handle] = printer;
		}

		List<Printer> listPrinters = null;
		protected override Printer[] GetPrintersInternal()
		{
			if (listPrinters != null)
				throw new InvalidOperationException("still enumerating printers from the last call to GetPrinters");

			listPrinters = new List<Printer>();
			Internal.GTK.Methods.Gtk.gtk_enumerate_printers(_GetPrintersInternal, IntPtr.Zero, new Action<IntPtr>(p_DestroyNotify), true);
			return listPrinters.ToArray();
		}
		private void p_DestroyNotify(IntPtr data)
		{
		}

		/// <summary>
		/// The type of function passed to gtk_enumerate_printers().
		/// </summary>
		/// <returns><c>true</c> to stop the enumeration, <c>false</c> otherwise.</returns>
		/// <param name="printer">Note that you need to ref @printer, if you want to keep a reference to it after the function has returned.</param>
		/// <param name="data">user data passed to gtk_enumerate_printers</param>
		private bool _GetPrintersInternal(IntPtr /*GtkPrinter*/ handle, IntPtr data)
		{
			GTKPrinter printer = new GTKPrinter(handle);
			listPrinters.Add(printer);
			return false;
		}

		protected override void PrintInternal(PrintJob job)
		{
			Contract.Requires(job != null);

			IntPtr hPrinter = PrinterToHandle(job.Printer);
			IntPtr hSettings = Internal.GTK.Methods.GtkPrintSettings.gtk_print_settings_new();
			IntPtr hPageSetup = Internal.GTK.Methods.GtkPageSetup.gtk_page_setup_new();

			IntPtr hJob = Internal.GTK.Methods.GtkPrintJob.gtk_print_job_new(job.Title, hPrinter, hSettings, hPageSetup);
			Internal.GObject.Methods.g_signal_connect(hJob, "status_changed", GtkPrintJob_status_changed_handler);


			Internal.GTK.Delegates.GtkPrintJobCompleteFunc hCallbackComplete = new Internal.GTK.Delegates.GtkPrintJobCompleteFunc(GtkPrintJob_Complete);

			IntPtr hError = IntPtr.Zero;
			IntPtr hCairoSurface = Internal.GTK.Methods.GtkPrintJob.gtk_print_job_get_surface(hJob, ref hError);

			IntPtr cr = Internal.Cairo.Methods.cairo_create(hCairoSurface);
			GTKGraphics graphics = new GTKGraphics(cr);

			InvokeMethod(job, "OnDrawPage", new PrintEventArgs(graphics));

			Internal.Cairo.Methods.cairo_show_page(cr);

			// automatically called by cairo_destroy
			Internal.Cairo.Methods.cairo_surface_finish(hCairoSurface);

			Internal.GTK.Methods.GtkPrintJob.gtk_print_job_send(hJob, hCallbackComplete, IntPtr.Zero, new Internal.GObject.Delegates.GDestroyNotify(GtkPrintJob_Destroy));

			printing = true;
			while (printing)
			{
				System.Threading.Thread.Sleep(500);
				Application.DoEvents();
			}

			Internal.Cairo.Methods.cairo_destroy(cr);
			Internal.Cairo.Methods.cairo_surface_destroy(hCairoSurface);

			// clean up
			// Internal.GLib.Methods.g_main_loop_unref(loop);
			Internal.GObject.Methods.g_object_unref(hSettings);
			Internal.GObject.Methods.g_object_unref(hPageSetup);
			Internal.GObject.Methods.g_object_unref(hPrinter);
		}

		private bool printing = false;


		private Internal.GObject.Delegates.GCallbackV1I GtkPrintJob_status_changed_handler;
		/// <summary>
		/// Emitted after the user has finished changing print settings in the dialog, before the actual rendering starts.
		/// A typical use for ::begin-print is to use the parameters from the GtkPrintContext and paginate the document
		/// accordingly, and then set the number of pages with gtk_print_operation_set_n_pages().
		/// </summary>
		/// <param name="operation">Operation.</param>
		private void GtkPrintJob_status_changed(IntPtr /*GtkPrintOperation*/ handle)
		{
			Internal.GTK.Constants.GtkPrintStatus status = Internal.GTK.Methods.GtkPrintJob.gtk_print_job_get_status(handle);
			printing = true;
			if (status == Internal.GTK.Constants.GtkPrintStatus.Aborted || status == Internal.GTK.Constants.GtkPrintStatus.Finished)
			{
				printing = false;
			}
		}
		private void GtkPrintJob_Complete(IntPtr print_job, IntPtr user_data, ref Internal.GLib.Structures.GError error)
		{
		}
		private void GtkPrintJob_Destroy(IntPtr data)
		{
		}


		#endregion
		#region Generic Dialog
		private IntPtr Dialog_AddButton(IntPtr handle, Button button)
		{
			/*
			if (button.ResponseValue == (int)DialogResult.Cancel)
				return IntPtr.Zero;
			*/

			IntPtr buttonHandle = IntPtr.Zero;
			if (!IsControlCreated(button)) CreateControl(button);

			buttonHandle = (GetHandleForControl(button) as GTKNativeControl).Handle;

			if (button.ResponseValue == (int)DialogResult.OK) {
				if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
				{
				}
				else
				{
					IntPtr hStyleCtx = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(buttonHandle);
					Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class(hStyleCtx, "suggested-action");
				}
			}

			// Internal.GTK.Methods.GtkDialog.gtk_dialog_add_button (handle, button.StockType == ButtonStockType.Connect ? "Connect" : "Cancel", button.ResponseValue);

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

			Internal.GTK.Methods.GtkDialog.gtk_dialog_add_action_widget(handle, buttonHandle, nativeResponseValue);
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_can_default(buttonHandle, true);

			// UpdateControlProperties(button, buttonHandle);
			// above updatecontrolprops call must be called after buttonHandle is realized

			Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(buttonHandle);
			return buttonHandle;
		}
		private IntPtr Dialog_Create(Dialog dlg, IntPtr hParent)
		{
			IntPtr handle = Internal.GObject.Methods.g_object_new (Internal.GTK.Methods.GtkDialog.gtk_dialog_get_type (), "transient-for", hParent, "use-header-bar", 1, IntPtr.Zero);
			// IntPtr handle = Internal.GTK.Methods.GtkDialog.gtk_dialog_new_with_buttons(dlg.Text, hParent, Internal.GTK.Constants.GtkDialogFlags.Modal, null);

			IntPtr hText = Marshal.StringToHGlobalAuto (dlg.Text);
			Internal.GTK.Methods.GtkWindow.gtk_window_set_title(handle, hText);

			IntPtr hDialogContent = Internal.GTK.Methods.GtkDialog.gtk_dialog_get_content_area(handle);

			NativeControl hContainer = (new Controls.ContainerImplementation(this, dlg)).CreateControl(dlg);
			// NativeControl hContainer = CreateContainer (dlg);

			Internal.GTK.Methods.GtkBox.gtk_box_pack_start(hDialogContent, (hContainer as GTKNativeControl).Handle, true, true, 0);
			Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(hDialogContent);

			RegisterControlHandle(dlg, new GTKNativeControl(handle));
			return handle;
		}

		private void RecursiveShowChildControls(Container container)
		{
			if (handlesByLayout.ContainsKey(container.Layout)) {
				IntPtr hLayout = handlesByLayout [container.Layout];
				Internal.GTK.Methods.GtkWidget.gtk_widget_show (hLayout);
			}
			foreach (Control ctl in container.Controls) {
				if (ctl is Container) {
					if (handlesByLayout.ContainsKey((ctl as Container).Layout)) {
						IntPtr hLayout = handlesByLayout [(ctl as Container).Layout];
						Internal.GTK.Methods.GtkWidget.gtk_widget_show (hLayout);
					}
					RecursiveShowChildControls (ctl as Container);
				}
				if (ctl.Visible) {
					IntPtr hCtl = (GetHandleForControl(ctl) as GTKNativeControl).Handle;
					Internal.GTK.Methods.GtkWidget.gtk_widget_show (hCtl);
				}
			}
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
						if (hButton == IntPtr.Zero)
							continue;
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

		protected override void UpdateControlPropertiesInternal(Control control, NativeControl native)
		{
			IntPtr handle = (native as GTKNativeControl).Handle;
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
					// Internal.GTK.Methods.GtkButton.gtk_button_set_label(handle, text);
				}

				if (button.StockType != ButtonStockType.None)
				{
					control.ControlImplementation.SetControlText (control, StockTypeToString ((StockType)button.StockType));
					Internal.GTK.Methods.GtkButton.gtk_button_set_use_stock(handle, true);
				}

				Internal.GTK.Methods.GtkButton.gtk_button_set_use_underline(handle, true);
				Internal.GTK.Methods.GtkButton.gtk_button_set_focus_on_click(handle, true);

				switch (button.BorderStyle)
				{
					case ButtonBorderStyle.None:
					{
						Internal.GTK.Methods.GtkButton.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.None);
						break;
					}
					case ButtonBorderStyle.Half:
					{
						Internal.GTK.Methods.GtkButton.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.Half);
						break;
					}
					case ButtonBorderStyle.Normal:
					{
						Internal.GTK.Methods.GtkButton.gtk_button_set_relief(handle, Internal.GTK.Constants.GtkReliefStyle.Normal);
						break;
					}
				}
			}

			Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(handle, control.Enabled);
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_size_request(handle, (int)control.Size.Width, (int)control.Size.Height);
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
						Internal.GTK.Methods.GtkAccelMap.gtk_accel_map_add_entry(accelPath, GTKEngine.GetAccelKeyForKeyboardKey(cmi.Shortcut.Key), GTKEngine.KeyboardModifierKeyToGdkModifierType(cmi.Shortcut.ModifierKeys));
					}
				}

				IntPtr hMenuFile = Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_new();
				Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_label(hMenuFile, cmi.Text);
				Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_use_underline(hMenuFile, true);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(hMenuFile, cmi.Enabled);

				if (menuItem.HorizontalAlignment == MenuItemHorizontalAlignment.Right)
				{
					Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_right_justified(hMenuFile, true);
				}

				if (cmi.Items.Count > 0)
				{
					IntPtr hMenuFileMenu = BuildMenu(cmi, hMenuFile, accelPath);
				}

				menuItemsByHandle[hMenuFile] = cmi;

				Internal.GObject.Methods.g_signal_connect(hMenuFile, "activate", gc_MenuItem_Activated, IntPtr.Zero);

				if (accelPath != null)
				{
					Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_accel_path(hMenuFile, accelPath);
				}

				Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenuShell, hMenuFile);
			}
			else if (menuItem is SeparatorMenuItem)
			{
				// IntPtr hMenuFile = Internal.GTK.Methods.Methods.gtk_separator_new (Internal.GTK.Constants.GtkOrientation.Horizontal);
				IntPtr hMenuFile = Internal.GTK.Methods.GtkSeparatorMenuItem.gtk_separator_menu_item_new();
				Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenuShell, hMenuFile);
			}
		}

		public IntPtr BuildMenu(Menu menu, string accelPath = null)
		{
			IntPtr hMenuFileMenu = Internal.GTK.Methods.GtkMenu.gtk_menu_new();
			if (menu.EnableTearoff)
			{
				try
				{
					IntPtr hMenuTearoff = Internal.GTK.Methods.GtkTearoffMenuItem.gtk_tearoff_menu_item_new();
					Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenuFileMenu, hMenuTearoff);
				}
				catch (EntryPointNotFoundException ex)
				{
					Console.WriteLine("uwt: gtk: GtkTearoffMenuItem has finally been deprecated. You need to implement it yourself now!");

					// this functionality is deprecated, so just in case it finally gets removed...
					// however, some people like it, so UWT will support it indefinitely ;)
					// if it does eventually get removed, we should be able to replicate this feature natively in UWT anyway
				}
			}

			if (accelPath != null)
			{
				if (hDefaultAccelGroup == IntPtr.Zero)
				{
					hDefaultAccelGroup = Internal.GTK.Methods.GtkAccelGroup.gtk_accel_group_new();
				}
				Internal.GTK.Methods.GtkMenu.gtk_menu_set_accel_group(hMenuFileMenu, hDefaultAccelGroup);
			}

			foreach (MenuItem menuItem1 in menu.Items)
			{
				InitMenuItem(menuItem1, hMenuFileMenu, accelPath);
			}
			return hMenuFileMenu;
		}
		public IntPtr BuildMenu(CommandMenuItem cmi, IntPtr hMenuFile, string accelPath = null)
		{
			IntPtr hMenuFileMenu = Internal.GTK.Methods.GtkMenu.gtk_menu_new();
			if (cmi.EnableTearoff)
			{
				try
				{
					IntPtr hMenuTearoff = Internal.GTK.Methods.GtkTearoffMenuItem.gtk_tearoff_menu_item_new();
					Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenuFileMenu, hMenuTearoff);
				}
				catch (EntryPointNotFoundException ex)
				{
					Console.WriteLine("uwt: gtk: GtkTearoffMenuItem has finally been deprecated. You need to implement it yourself now!");

					// this functionality is deprecated, so just in case it finally gets removed...
					// however, some people like it, so UWT will support it indefinitely ;)
					// if it does eventually get removed, we should be able to replicate this feature natively in UWT anyway
				}
			}

			if (accelPath != null)
			{
				if (hDefaultAccelGroup == IntPtr.Zero)
				{
					hDefaultAccelGroup = Internal.GTK.Methods.GtkAccelGroup.gtk_accel_group_new();
				}
				Internal.GTK.Methods.GtkMenu.gtk_menu_set_accel_group(hMenuFileMenu, hDefaultAccelGroup);
			}

			foreach (MenuItem menuItem1 in cmi.Items)
			{
				InitMenuItem(menuItem1, hMenuFileMenu, accelPath);
			}

			Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_submenu(hMenuFile, hMenuFileMenu);
			return hMenuFileMenu;
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
					IntPtr hMenu = Internal.GTK.Methods.GtkMenu.gtk_menu_new();

					IntPtr hMenuTitle = Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_new();
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(hMenuTitle, false);
					Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenu, hMenuTitle);
					nii.hMenuItemTitle = hMenuTitle;

					IntPtr hMenuSeparator = Internal.GTK.Methods.GtkSeparatorMenuItem.gtk_separator_menu_item_new();
					Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenu, hMenuSeparator);

					if (nid.ContextMenu != null)
					{
						foreach (MenuItem mi in nid.ContextMenu.Items)
						{
							InitMenuItem(mi, hMenu);
						}
					}

					Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(hMenu);

					Internal.AppIndicator.Methods.app_indicator_set_menu(nii.hIndicator, hMenu);
				}

				if (nii.hMenuItemTitle != IntPtr.Zero)
				{
					Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_label(nii.hMenuItemTitle, nid.Text);
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
			IntPtr handle = (GetHandleForControl(control) as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_queue_draw_area(handle, x, y, width, height);
		}

		protected override void DoEventsInternal()
		{
			while (Internal.GTK.Methods.Gtk.gtk_events_pending())
			{
				Internal.GTK.Methods.Gtk.gtk_main_iteration();
			}
		}

		internal Internal.GTK.Constants.GtkFileChooserAction FileBrowserModeToGtkFileChooserAction(FileBrowserMode value)
		{
			switch (value) {
				case FileBrowserMode.Open:
				{
					return Internal.GTK.Constants.GtkFileChooserAction.Open;
				}
				case FileBrowserMode.Save:
				{
					return Internal.GTK.Constants.GtkFileChooserAction.Save;
				}
				case FileBrowserMode.CreateFolder:
				{
					return Internal.GTK.Constants.GtkFileChooserAction.CreateFolder;
				}
				case FileBrowserMode.SelectFolder:
				{
					return Internal.GTK.Constants.GtkFileChooserAction.SelectFolder;
				}
			}
			throw new ArgumentException ();
		}

		internal Internal.GTK.Constants.GtkPositionType RelativePositionToGtkPositionType(RelativePosition value)
		{
			switch (value) {
			case RelativePosition.Left: return Internal.GTK.Constants.GtkPositionType.Left;
			case RelativePosition.Right: return Internal.GTK.Constants.GtkPositionType.Right;
			case RelativePosition.Top: return Internal.GTK.Constants.GtkPositionType.Top;
			case RelativePosition.Bottom: return Internal.GTK.Constants.GtkPositionType.Bottom;
			}

			return Internal.GTK.Constants.GtkPositionType.Left;
		}
		internal RelativePosition GtkPositionTypeToRelativePosition(Internal.GTK.Constants.GtkPositionType value)
		{
			switch (value) {
			case Internal.GTK.Constants.GtkPositionType.Left: return RelativePosition.Left;
			case Internal.GTK.Constants.GtkPositionType.Right: return RelativePosition.Right;
			case Internal.GTK.Constants.GtkPositionType.Top: return RelativePosition.Top;
			case Internal.GTK.Constants.GtkPositionType.Bottom: return RelativePosition.Bottom;
			}
			return RelativePosition.Default;
		}

		protected override NativeTreeModel CreateTreeModelInternal(TreeModel model)
		{
			List<IntPtr> listColumnTypes = new List<IntPtr>();
			if (model != null)
			{
				foreach (TreeModelColumn c in model.Columns)
				{
					IntPtr ptr = Internal.GLib.Constants.GType.FromType(c.DataType);
					if (ptr == IntPtr.Zero) continue;

					listColumnTypes.Add(ptr);
				}
			}

			if (listColumnTypes.Count <= 0)
			{
				Console.WriteLine("uwt ERROR: you did not specify any columns for the ListView!!!");
				listColumnTypes.Add(Internal.GLib.Constants.GType.FromType(typeof(string)));
			}

			IntPtr[] columnTypes = listColumnTypes.ToArray();
			IntPtr hTreeStore = Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_newv(columnTypes.Length, columnTypes);

			DefaultTreeModel dtm = (model as DefaultTreeModel);
			if (dtm != null)
			{
				Internal.GTK.Structures.GtkTreeIter hIter = new Internal.GTK.Structures.GtkTreeIter();
				foreach (TreeModelRow row in dtm.Rows)
				{
					RecursiveTreeStoreInsertRow(dtm, row, hTreeStore, out hIter, null, dtm.Rows.Count - 1);
				}
			}

			return new GTKNativeTreeModel(hTreeStore);
		}

		private Dictionary<TreeModelRow, Internal.GTK.Structures.GtkTreeIter> _GtkTreeIterForTreeModelRow = new Dictionary<TreeModelRow, Internal.GTK.Structures.GtkTreeIter>();
		private Dictionary<Internal.GTK.Structures.GtkTreeIter, TreeModelRow> _TreeModelRowForGtkTreeIter = new Dictionary<Internal.GTK.Structures.GtkTreeIter, TreeModelRow>();
		internal void RegisterGtkTreeIter(TreeModelRow row, Internal.GTK.Structures.GtkTreeIter hIter)
		{
			_GtkTreeIterForTreeModelRow[row] = hIter;
			_TreeModelRowForGtkTreeIter[hIter] = row;
		}
		internal void UnregisterGtkTreeIter(Structures.GtkTreeIter iter)
		{
			if (_TreeModelRowForGtkTreeIter.ContainsKey(iter))
			{
				_GtkTreeIterForTreeModelRow.Remove(_TreeModelRowForGtkTreeIter[iter]);
			}
			else
			{
				Console.WriteLine("attempted to unregister invalid GtkTreeIter for TreeModel");
			}
			_TreeModelRowForGtkTreeIter.Remove(iter);
		}
		internal TreeModelRow GetTreeModelRowForGtkTreeIter(Internal.GTK.Structures.GtkTreeIter hIter)
		{
			if (_TreeModelRowForGtkTreeIter.ContainsKey(hIter))
				return _TreeModelRowForGtkTreeIter[hIter];
			return null;
		}
		internal Internal.GTK.Structures.GtkTreeIter GetGtkTreeIterForTreeModelRow(TreeModelRow row)
		{
			return _GtkTreeIterForTreeModelRow[row];
		}
		internal bool IsTreeModelRowRegistered(TreeModelRow row)
		{
			return _GtkTreeIterForTreeModelRow.ContainsKey(row);
		}

		private void RecursiveTreeStoreInsertRow(TreeModel tm, TreeModelRow row, IntPtr hTreeStore, out Internal.GTK.Structures.GtkTreeIter hIter, Internal.GTK.Structures.GtkTreeIter? parent, int position, bool append = false)
		{
			if (parent == null)
			{
				if (append)
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_append(hTreeStore, out hIter, IntPtr.Zero);
				}
				else
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_insert(hTreeStore, out hIter, IntPtr.Zero, position);
				}
			}
			else
			{
				Internal.GTK.Structures.GtkTreeIter hIterParent = parent.Value;
				if (append)
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_append(hTreeStore, out hIter, ref hIterParent);
				}
				else
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_insert(hTreeStore, out hIter, ref hIterParent, position);
				}
			}

			RegisterGtkTreeIter(row, hIter);

			foreach (TreeModelRowColumn rc in row.RowColumns)
			{
				// since "Marshalling of type object is not implemented"
				// (mono/metadata/marshal.c:6507) we have to do it ourselves


				Internal.GLib.Structures.Value val = Internal.GLib.Structures.Value.FromObject(rc.Value);

				// Internal.GTK.Methods.Methods.gtk_tree_store_insert(hTreeStore, out hIter, IntPtr.Zero, 0);
				Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_set_value(hTreeStore, ref hIter, tm.Columns.IndexOf(rc.Column), ref val);

				// this can only be good, right...?
				// val.Dispose();

				// I thought this caused "malloc() : smallbin doubly linked list corrupted" error, but apparently it doesn't...?
				// back to square one...
			}

			foreach (TreeModelRow row2 in row.Rows)
			{
				Internal.GTK.Structures.GtkTreeIter hIter2 = new Internal.GTK.Structures.GtkTreeIter();
				RecursiveTreeStoreInsertRow(tm, row2, hTreeStore, out hIter2, hIter, row.Rows.Count - 1);
			}
		}

		private Clipboard _DefaultClipboard = null;
		protected override Clipboard GetDefaultClipboardInternal()
		{
			if (_DefaultClipboard == null)
			{
				IntPtr hDisplay = Internal.GDK.Methods.gdk_display_get_default();
				IntPtr hClipboard = Internal.GTK.Methods.GtkClipboard.gtk_clipboard_get_default(hDisplay);
				_DefaultClipboard = new GTKClipboard(hClipboard);
			}
			return _DefaultClipboard;
		}



		public static IntPtr DragUriListFromArray(string[] uris)
		{
			IntPtr /*GList*/ uri_list = IntPtr.Zero;

			if (uris == null)
				return uri_list;

			for (int i = 0; i < uris.Length; i++)
			{
				IntPtr hStr = Marshal.StringToHGlobalUni(uris[i]);
				uri_list = Internal.GLib.Methods.g_list_prepend(uri_list, hStr);
			}

			return Internal.GLib.Methods.g_list_reverse(uri_list);
		}


		private static System.Collections.Generic.Dictionary<string, IntPtr> _CursorHandlesByName = new System.Collections.Generic.Dictionary<string, IntPtr>();
		private static System.Collections.Generic.Dictionary<string, Cursor> _CursorsByName = new Dictionary<string, Cursor>();
		private static System.Collections.Generic.Dictionary<Cursor, IntPtr> _CursorHandlesByCursor = new Dictionary<Cursor, IntPtr>();

		private static void RegisterCursor(string name, Cursor cursor, IntPtr handle)
		{
			_CursorHandlesByCursor[cursor] = handle;
			_CursorHandlesByName[name] = handle;
			_CursorsByName[name] = cursor;
		}

		internal static IntPtr /*GdkCursor*/ GetCursorByName(string name)
		{
			if (_CursorHandlesByName.ContainsKey(name))
				return _CursorHandlesByName[name];
			return IntPtr.Zero;
		}

		internal static Cursor GetCursorByHandle(IntPtr /*GdkCursor*/ handle)
		{
			foreach (GTKCursorInfo info in cursorInfo)
			{
				if (handle == info.Handle)
					return info.UniversalCursor;
			}
			return null;
		}
		internal static IntPtr /*GdkCursor*/ GetHandleForCursor(Cursor cursor)
		{
			foreach (GTKCursorInfo info in cursorInfo)
			{
				if (cursor == info.UniversalCursor)
					return info.Handle;
			}
			return IntPtr.Zero;
		}

		private static bool mvarCursorsInitialized = false;
		private static GTKCursorInfo[] cursorInfo = new GTKCursorInfo[]
		{
			new GTKCursorInfo("default", Cursors.Default),
			new GTKCursorInfo("help", Cursors.Help),
			new GTKCursorInfo("pointer", Cursors.Pointer),
			new GTKCursorInfo("context-menu", Cursors.ContextMenu),
			new GTKCursorInfo("progress", Cursors.Progress),
			new GTKCursorInfo("wait", Cursors.Wait),
			new GTKCursorInfo("cell", Cursors.Cell),
			new GTKCursorInfo("crosshair", Cursors.Crosshair),
			new GTKCursorInfo("text", Cursors.Text),
			new GTKCursorInfo("vertical-text", Cursors.VerticalText),
			new GTKCursorInfo("alias", Cursors.Alias),
			new GTKCursorInfo("copy", Cursors.Copy),
			new GTKCursorInfo("no-drop", Cursors.NoDrop),
			new GTKCursorInfo("move", Cursors.Move),
			new GTKCursorInfo("not-allowed", Cursors.NotAllowed),
			new GTKCursorInfo("grab", Cursors.Grab),
			new GTKCursorInfo( "grabbing", Cursors.Grabbing),
			new GTKCursorInfo("all-scroll", Cursors.AllScroll),
			new GTKCursorInfo("col-resize", Cursors.ResizeColumn),
			new GTKCursorInfo("row-resize", Cursors.ResizeRow),
			new GTKCursorInfo("n-resize", Cursors.ResizeN),
			new GTKCursorInfo("e-resize", Cursors.ResizeE),
			new GTKCursorInfo("s-resize", Cursors.ResizeS),
			new GTKCursorInfo("w-resize", Cursors.ResizeW),
			new GTKCursorInfo("ne-resize", Cursors.ResizeNE),
			new GTKCursorInfo("nw-resize", Cursors.ResizeNW),
			new GTKCursorInfo("sw-resize", Cursors.ResizeSW),
			new GTKCursorInfo( "se-resize", Cursors.ResizeSE),
			new GTKCursorInfo("ew-resize", Cursors.ResizeEW),
			new GTKCursorInfo("ns-resize", Cursors.ResizeNS),
			new GTKCursorInfo("nesw-resize", Cursors.ResizeNESW),
			new GTKCursorInfo("nwse-resize", Cursors.ResizeNWSE),
			new GTKCursorInfo("zoom-in", Cursors.ZoomIn),
			new GTKCursorInfo("zoom-out", Cursors.ZoomOut)
		};
		internal static void InitializeCursors(IntPtr /*GdkDisplay*/ display)
		{
			if (mvarCursorsInitialized) return;

			for (int i = 0;  i < cursorInfo.Length;  i++)
			{
				GTKCursorInfo info = cursorInfo[i];
				IntPtr hCursor = Internal.GDK.Methods.gdk_cursor_new_from_name(display, info.Name);
				Console.WriteLine("setting cursor {0} for {1} to {2}", info.Name, info.UniversalCursor, hCursor);
				info.Handle = hCursor;
				RegisterCursor(info.Name, info.UniversalCursor, hCursor);
				cursorInfo[i] = info; // structs are weird
			}
			mvarCursorsInitialized = true;
		}
	}
}

