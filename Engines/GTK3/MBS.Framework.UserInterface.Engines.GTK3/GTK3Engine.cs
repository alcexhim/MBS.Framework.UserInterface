using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Docking;
using MBS.Framework.UserInterface.Controls.FileBrowser;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Engines.GTK3.Drawing;
using MBS.Framework.UserInterface.Engines.GTK3.Printing;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Printing;

namespace MBS.Framework.UserInterface.Engines.GTK3
{
	public class GTK3Engine : Engine
	{
		protected override int Priority => (System.Environment.OSVersion.Platform == PlatformID.Unix ? 1 : -1);

		protected override CommandLineParser CreateCommandLineParser()
		{
			return new GTKCommandLineParser();
		}

		protected override Vector2D GetMouseCursorPositionInternal(MouseDevice mouse)
		{
			if (mouse == MouseDevice.Default)
			{
				IntPtr /*GdkWindow*/ window;
				IntPtr /*GdkDevice*/ mouse_device;

				IntPtr /*GdkDisplay*/ display = Internal.GDK.Methods.gdk_display_get_default();

				if (true) // (GTK_CHECK_VERSION(3, 20, 0))
				{
					IntPtr /*GdkSeat*/ seat = Internal.GDK.Methods.gdk_display_get_default_seat(display);
					mouse_device = Internal.GDK.Methods.gdk_seat_get_pointer(seat);
				}
				else
				{
					/*
					GdkDeviceManager* devman = gdk_display_get_device_manager(gdk_display_get_default());
					mouse_device = gdk_device_manager_get_client_pointer(devman);
					*/				
				}

				window = Internal.GDK.Methods.gdk_display_get_default_group(display);

				double x = 0.0, y = 0.0;
				Internal.GDK.Constants.GdkModifierType modifierState = Internal.GDK.Constants.GdkModifierType.None;
				Internal.GDK.Methods.gdk_window_get_device_position_double(window, mouse_device, ref x, ref y, ref modifierState);
				return new Vector2D(x, y);
			}
			throw new NotSupportedException();
		}

		public override TreeModelManager TreeModelManager { get; } = new GTK3TreeModelManager();

		private GTKSystemSettings _SystemSettings = new GTKSystemSettings();
		public override SystemSettings SystemSettings => _SystemSettings;

		private int _exitCode = 0;

		public IntPtr ApplicationHandle { get; private set; } = IntPtr.Zero;

		private Dictionary<Inhibitor, uint> _InhibitorIDs = new Dictionary<Inhibitor, uint>();
		protected override void RegisterInhibitorInternal(Inhibitor item)
		{
			if (_InhibitorIDs.ContainsKey(item))
			{
				// throw new InvalidOperationException("inhibitor already registered, please unregister it first");
				Console.Error.WriteLine("uwt: inhibitor: inhibitor already registered, please unregister it first");
				return;
			}

			Internal.GTK.Constants.GtkApplicationInhibitFlags flags = Internal.GTK.Constants.GtkApplicationInhibitFlags.None;
			if ((item.Type & InhibitorType.SystemIdle) == InhibitorType.SystemIdle)
			{
				flags |= Internal.GTK.Constants.GtkApplicationInhibitFlags.Idle;
			}
			if ((item.Type & InhibitorType.SystemLogout) == InhibitorType.SystemLogout)
			{
				flags |= Internal.GTK.Constants.GtkApplicationInhibitFlags.Logout;
			}
			if ((item.Type & InhibitorType.SystemSuspend) == InhibitorType.SystemSuspend)
			{
				flags |= Internal.GTK.Constants.GtkApplicationInhibitFlags.Suspend;
			}
			if ((item.Type & InhibitorType.SystemUserSwitch) == InhibitorType.SystemUserSwitch)
			{
				flags |= Internal.GTK.Constants.GtkApplicationInhibitFlags.Switch;
			}

			IntPtr hWnd = IntPtr.Zero;
			if (item.ParentWindow != null)
			{
				hWnd = (GetHandleForControl(item.ParentWindow) as GTKNativeControl).Handle;
			}
			_InhibitorIDs[item] = Internal.GTK.Methods.GtkApplication.gtk_application_inhibit(ApplicationHandle, hWnd, flags, item.Message);
		}
		protected override void UnregisterInhibitorInternal(Inhibitor item)
		{
			if (!_InhibitorIDs.ContainsKey(item))
			{
				Console.Error.WriteLine("uwt: inhibitor: no registered inhibitor found");
				return;
			}
			Internal.GTK.Methods.GtkApplication.gtk_application_uninhibit(ApplicationHandle, _InhibitorIDs[item]);
			_InhibitorIDs.Remove(item);
		}

		protected override Graphics CreateGraphicsInternal(Image image)
		{
			CairoImage ci = (image as CairoImage);
			if (ci == null)
				throw new NotSupportedException();

			IntPtr hCairoSurface = ci.Handle;
			IntPtr cr = Internal.Cairo.Methods.cairo_create(hCairoSurface);

			GTKGraphics graphics = new GTKGraphics(cr, new Rectangle(new Vector2D(0, 0), image.Size));
			return graphics;
		}
		// TODO: this should be migrated to the appropriate refactoring once we figure out what that is
		protected override Image CreateImage(int width, int height)
		{
			bool useCairo = true;
			if (!useCairo)
			{
				IntPtr hImage = Internal.GDK.Methods.gdk_pixbuf_new(Internal.GDK.Constants.GdkColorspace.RGB, true, 8, width, height);
				return new GDKPixbufImage(hImage);
			}
			else
			{
				IntPtr hImage = Internal.Cairo.Methods.cairo_image_surface_create(Internal.Cairo.Constants.CairoFormat.ARGB32, width, height);
				return new CairoImage(hImage);
			}
		}
		protected override Image LoadImage(StockType stockType, int size)
		{
			string stockTypeId = StockTypeToString(stockType);
			return LoadImageByName(stockTypeId, size);
		}
		protected override Image LoadImage(byte[] filedata, string type)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hLoader = CreateImageLoader(type);
			return LoadImage(hLoader, filedata, ref hError);
		}

		internal static Internal.GTK.Constants.GtkPositionType CardinalDirectionToGtkPositionType(CardinalDirection direction)
		{
			switch (direction)
			{
				case CardinalDirection.Bottom: return Internal.GTK.Constants.GtkPositionType.Bottom;
				case CardinalDirection.Left: return Internal.GTK.Constants.GtkPositionType.Left;
				case CardinalDirection.Right: return Internal.GTK.Constants.GtkPositionType.Right;
				case CardinalDirection.Top: return Internal.GTK.Constants.GtkPositionType.Top;
			}
			throw new ArgumentException("direction");
		}

		protected override Image LoadImage(string filename, string type = null)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hLoader = CreateImageLoader(type);
			byte[] buffer = System.IO.File.ReadAllBytes(filename);
			return LoadImage(hLoader, buffer, ref hError);
		}

		private Action<byte[], IntPtr> /*GdkPixbufDestroyNotify*/ _destroy_fn_d = null;
		private void _destroy_fn(byte[] pixels, IntPtr data)
		{
			Marshal.FreeHGlobal(data);
		}
		protected override Image LoadImage(byte[] filedata, int width, int height, int rowstride)
		{
			IntPtr hPinned = Marshal.AllocHGlobal(filedata.Length);
			Marshal.Copy(filedata, 0, hPinned, filedata.Length);

			IntPtr hImage = Internal.GDK.Methods.gdk_pixbuf_new_from_data(hPinned, Internal.GDK.Constants.GdkColorspace.RGB, true, 8, width, height, rowstride, _destroy_fn_d, hPinned);
			return new GDKPixbufImage(hImage);
		}

		protected override Image LoadImageByName(string name, int size)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hTheme = IntPtr.Zero;
			try
			{
				hTheme = Internal.GTK.Methods.GtkIconTheme.gtk_icon_theme_get_default();
			}
			catch (EntryPointNotFoundException ex)
			{
				return null;
			}
			IntPtr hPixbuf = Internal.GTK.Methods.GtkIconTheme.gtk_icon_theme_load_icon(hTheme, name, size, Internal.GTK.Constants.GtkIconLookupFlags.None, ref hError);
			return new GDKPixbufImage(hPixbuf);
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

		private static Dictionary<IntPtr, GDKPixbufImage> _PixbufsForHandle = new Dictionary<IntPtr, GDKPixbufImage>();
		private Image LoadImage(IntPtr hLoader, byte[] buffer, ref IntPtr hError)
		{
			Internal.GDK.Methods.gdk_pixbuf_loader_write(hLoader, buffer, buffer.Length, ref hError);
			IntPtr hPixbuf = Internal.GDK.Methods.gdk_pixbuf_loader_get_pixbuf(hLoader);
			Internal.GDK.Methods.gdk_pixbuf_loader_close(hLoader, ref hError);

			if (!_PixbufsForHandle.ContainsKey(hPixbuf))
			{
				_PixbufsForHandle[hPixbuf] = new GDKPixbufImage(hPixbuf);
			}
			return _PixbufsForHandle[hPixbuf];
		}

		private static Version _Version = null;
		public static Version Version
		{
			get
			{
				if (_Version == null)
				{
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

		protected override bool IsControlEnabledInternal(Control control)
		{
			GTKNativeControl hnc = (GetHandleForControl(control) as GTKNativeControl);
			if (hnc != null)
			{
				IntPtr handle = hnc.Handle;
				return Internal.GTK.Methods.GtkWidget.gtk_widget_is_sensitive(handle);
			}
			return true;
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

				list.Add(new Internal.GTK.Structures.GtkTargetEntry() { flags = flags, info = (uint)target.ID, target = GetDragDropTargetTypeName(target.Type) });
			}
			return list.ToArray();
		}

		private static string GetDragDropTargetTypeName(DragDropTargetType type)
		{
			if (type == DragDropTargetTypes.FileList)
			{
				return "text/uri-list";
			}
			throw new NotSupportedException();
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

		internal static MouseEventArgs GdkEventScrollToMouseEventArgs(Internal.GDK.Structures.GdkEventScroll e, IntPtr hEventArgs)
		{
			MouseButtons buttons = GdkModifierTypeToMouseButtons(e.state);
			KeyboardModifierKey modifierKeys = GdkModifierTypeToKeyboardModifierKey(e.state);

			double delta_x = e.delta_x, delta_y = e.delta_y;
			if (e.direction == Internal.GDK.Constants.GdkScrollDirection.Smooth)
			{
				bool ret = Internal.GDK.Methods.gdk_event_get_scroll_deltas(hEventArgs, ref delta_x, ref delta_y);
			}
			MouseEventArgs ee = new MouseEventArgs(e.x, e.y, buttons, modifierKeys, delta_x, delta_y);
			return ee;
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

		internal static Internal.GTK.Constants.GtkResponseType DialogResultToGtkResponseType(DialogResult value)
		{
			switch (value)
			{
				case DialogResult.OK: return Internal.GTK.Constants.GtkResponseType.OK;
				case DialogResult.Cancel: return Internal.GTK.Constants.GtkResponseType.Cancel;
				case DialogResult.Help: return Internal.GTK.Constants.GtkResponseType.Help;
				case DialogResult.No: return Internal.GTK.Constants.GtkResponseType.No;
				case DialogResult.None: return Internal.GTK.Constants.GtkResponseType.None;
				case DialogResult.Yes: return Internal.GTK.Constants.GtkResponseType.Yes;
			}
			return Internal.GTK.Constants.GtkResponseType.None;
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

			gc_Application_Startup = new Action<IntPtr, IntPtr>(Application_Startup);
			gc_MenuItem_Activated = new Action<IntPtr, IntPtr>(MenuItem_Activate);
			gc_Application_CommandLine = new Internal.GObject.Delegates.GApplicationCommandLineHandler(Application_CommandLine);
			gc_Application_QueryEnd = new Action<IntPtr, IntPtr>(Application_QueryEnd);

			Console.WriteLine("uwt-gtk: creating GtkApplication with unique name '{0}'", Application.Instance.UniqueName);
			ApplicationHandle = Internal.GTK.Methods.GtkApplication.gtk_application_new(Application.Instance.UniqueName, Internal.GIO.Constants.GApplicationFlags.HandlesCommandLine | Internal.GIO.Constants.GApplicationFlags.HandlesOpen);

			Internal.GLib.Structures.Value val = new Internal.GLib.Structures.Value(true);
			Internal.GObject.Methods.g_object_set_property(ApplicationHandle, "register-session", ref val);

			return check;
		}

		internal Internal.GDK.Structures.GdkRGBA ColorToGDKRGBA(Color color)
		{
			Internal.GDK.Structures.GdkRGBA rgba = new Internal.GDK.Structures.GdkRGBA();
			rgba.red = color.R;
			rgba.green = color.G;
			rgba.blue = color.B;
			rgba.alpha = color.A;
			return rgba;
		}

		protected override int StartInternal(Window waitForClose)
		{
			string[] argv = System.Environment.GetCommandLineArgs();
			int argc = argv.Length;

			if (ApplicationHandle != IntPtr.Zero)
			{
				Internal.GObject.Methods.g_signal_connect(ApplicationHandle, "activate", gc_Application_Activate, IntPtr.Zero);
				Internal.GObject.Methods.g_signal_connect(ApplicationHandle, "startup", gc_Application_Startup, IntPtr.Zero);
				Internal.GObject.Methods.g_signal_connect(ApplicationHandle, "command_line", gc_Application_CommandLine, IntPtr.Zero);
				Internal.GObject.Methods.g_signal_connect(ApplicationHandle, "open", gc_Application_Open, IntPtr.Zero);
				Internal.GObject.Methods.g_signal_connect(ApplicationHandle, "query_end", gc_Application_QueryEnd, IntPtr.Zero);
			}
			if (waitForClose != null)
			{
				waitForClose.Closed += WaitForClose_Closed;
			}

			if (ApplicationHandle != IntPtr.Zero)
			{
				// moved down here to allow for WaitForClose event
				_exitCode = Internal.GIO.Methods.g_application_run(ApplicationHandle, argc, argv);

				Internal.GObject.Methods.g_object_unref(ApplicationHandle);
			}
			return _exitCode;
		}

		private void WaitForClose_Closed(object sender, EventArgs e)
		{
			((UIApplication)Application.Instance).Stop();
		}

		protected override void StopInternal(int exitCode)
		{
			_exitCode = exitCode;
			Internal.GIO.Methods.g_application_quit(ApplicationHandle);
		}

		private Dictionary<Layout, IntPtr> handlesByLayout = new Dictionary<Layout, IntPtr>();

		private Dictionary<IntPtr, MenuItem> menuItemsByHandle = new Dictionary<IntPtr, MenuItem>();

		protected override void SetMenuItemVisibilityInternal(MenuItem item, bool visible)
		{
			IntPtr hMenuItem = (GetHandleForMenuItem(item) as GTKNativeControl).Handle;
			if (visible)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hMenuItem);
			}
			else
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hMenuItem);
			}
		}
		protected override void SetMenuItemEnabledInternal(MenuItem item, bool enabled)
		{
			IntPtr hMenuItem = (GetHandleForMenuItem(item) as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(hMenuItem, enabled);
		}

		protected override void SetToolbarItemVisibilityInternal(ToolbarItem item, bool visible)
		{
			IntPtr hToolbarItem = (GetHandleForToolbarItem(item) as GTKNativeControl).Handle;
			if (visible)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hToolbarItem);
			}
			else
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_hide(hToolbarItem);
			}
		}
		protected override void SetToolbarItemEnabledInternal(ToolbarItem item, bool enabled)
		{
			if (Internal.GTK.Methods.Gtk.gtk_get_major_version() < 4)
			{
				IntPtr hToolbarItem = (GetHandleForToolbarItem(item) as GTKNativeControl).Handle;
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(hToolbarItem, enabled);
			}
			else
			{
				// toolbar removed in GTK4, just use regular control
			}
		}

		private Action<IntPtr, IntPtr> gc_MenuItem_Activated = null;
		private Action<IntPtr, IntPtr> gc_Application_Activate = null;
		private Action<IntPtr, IntPtr> gc_Application_Startup = null;
		private Internal.GObject.Delegates.GApplicationCommandLineHandler gc_Application_CommandLine = null;
		private Action<IntPtr, IntPtr, int, string, IntPtr> gc_Application_Open = null;
		private Action<IntPtr, IntPtr> gc_Application_QueryEnd = null;

		protected bool _OpenFiles { get; private set; } = false;
		private void Application_Open(IntPtr application, IntPtr files, int n_files, string hint, IntPtr user_data)
		{
			Console.WriteLine("Application_OpenFiles");
			_OpenFiles = true;


			int argc = 0;
			// IntPtr hwpp = Internal.GIO.Methods.g_application_command_line_get_arguments(commandLine, ref argc);

			string[] arguments = new string[0]; // PtrToStringArray(hwpp, argc);

			ParseCommandLine(arguments, out ApplicationActivationType activationType);
			if (activationType == ApplicationActivationType.Unspecified)
			{
				if (_OpenFiles)
				{
					activationType = ApplicationActivationType.File;
				}
				else
				{
					activationType = ApplicationActivationType.CommandLineLaunch;
				}
			}
			ApplicationActivatedEventArgs e = new ApplicationActivatedEventArgs(_firstRun, activationType, Application.Instance.CommandLine);

			_firstRun = false;
			InvokeMethod(Application.Instance, "OnActivated", new object[] { e });
			// return e.ExitCode;
		}

		private void Application_Startup(IntPtr application, IntPtr user_data)
		{
			Console.WriteLine("Application_Startup");
			InvokeMethod(Application.Instance, "OnStartup", new object[] { EventArgs.Empty });
		}
		private void Application_QueryEnd(IntPtr application, IntPtr user_data)
		{
			Console.WriteLine("Application_QueryEnd");

			SessionEndingEventArgs ee = new SessionEndingEventArgs();
			InvokeMethod(Application.Instance, "OnSessionEnding", new object[] { ee });
			if (ee.PreventReason != null)
			{
				Internal.GTK.Methods.GtkApplication.gtk_application_inhibit(application, IntPtr.Zero, Internal.GTK.Constants.GtkApplicationInhibitFlags.Logout, ee.PreventReason);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct PtrToStringArrayStruct
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public IntPtr[] listOfStrings;
		}

		public static string[] PtrToStringArray(IntPtr hptr, int size)
		{
			// eww.. gross. thanks https://stackoverflow.com/questions/1323797/marshaling-pointer-to-an-array-of-strings
			PtrToStringArrayStruct hwss = (PtrToStringArrayStruct)Marshal.PtrToStructure(hptr, typeof(PtrToStringArrayStruct));
			string[] argv = new string[size];
			for (int i = 0; i < size; i++)
			{
				IntPtr hstr = hwss.listOfStrings[i];

				string p = Marshal.PtrToStringAuto(hstr);
				argv[i] = p;
			}
			return argv;
		}

		private static bool _firstRun = true;
		private int Application_CommandLine(IntPtr handle, IntPtr commandLine, IntPtr data)
		{
			int argc = 0;
			IntPtr hwpp = Internal.GIO.Methods.g_application_command_line_get_arguments(commandLine, ref argc);

			string[] arguments = PtrToStringArray(hwpp, argc);

			ParseCommandLine(arguments, out ApplicationActivationType activationType);
			if (activationType == ApplicationActivationType.Unspecified)
			{
				if (_OpenFiles)
				{
					activationType = ApplicationActivationType.File;
				}
				else
				{
					activationType = ApplicationActivationType.CommandLineLaunch;
				}
			}
			ApplicationActivatedEventArgs e = new ApplicationActivatedEventArgs(_firstRun, activationType, Application.Instance.CommandLine);

			_firstRun = false;
			InvokeMethod(Application.Instance, "OnActivated", new object[] { e });
			return e.ExitCode;
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

		public GTK3Engine()
		{
			InitializeStockIDs();
			InitializeEventHandlers();

			GtkPrintJob_status_changed_handler = new Action<IntPtr>(GtkPrintJob_status_changed);
			_destroy_fn_d = new Action<byte[], IntPtr>(_destroy_fn);

			_notifyActionCallback_d = new Action<IntPtr, string, IntPtr>(_notifyActionCallback);
		}

		private void InitializeStockIDs()
		{
			RegisterStockType(StockType.About, "gtk-about");
			RegisterStockType(StockType.Add, "gtk-add");
			RegisterStockType(StockType.Apply, "gtk-apply");
			RegisterStockType(StockType.Bold, "gtk-bold");
			RegisterStockType(StockType.Bookmarks, "user-bookmarks");
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
			RegisterStockType(StockType.DialogInfo, "dialog-information");
			RegisterStockType(StockType.DialogWarning, "dialog-warning");
			RegisterStockType(StockType.DialogError, "dialog-error");
			RegisterStockType(StockType.DialogQuestion, "dialog-question");
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
			RegisterStockType(StockType.Folder, "folder");
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
			RegisterStockType(StockType.Redo, "gtk-redo-ltr");
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
			RegisterStockType(StockType.Undo, "gtk-undo-ltr");
			RegisterStockType(StockType.Unindent, "gtk-unindent");
			RegisterStockType(StockType.Yes, "gtk-yes");
			RegisterStockType(StockType.Zoom100, "gtk-zoom-100");
			RegisterStockType(StockType.ZoomFit, "gtk-zoom-fit");
			RegisterStockType(StockType.ZoomIn, "gtk-zoom-in");
			RegisterStockType(StockType.ZoomOut, "gtk-zoom-out");
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

		internal static Internal.GTK.Constants.GtkSelectionMode SelectionModeToGtkSelectionMode(SelectionMode mode)
		{
			switch (mode)
			{
				case SelectionMode.None: return Internal.GTK.Constants.GtkSelectionMode.None;
				case SelectionMode.Single: return Internal.GTK.Constants.GtkSelectionMode.Single;
				case SelectionMode.Browse: return Internal.GTK.Constants.GtkSelectionMode.Browse;
				case SelectionMode.Multiple: return Internal.GTK.Constants.GtkSelectionMode.Multiple;
			}
			throw new InvalidOperationException();
		}
		internal static SelectionMode GtkSelectionModeToSelectionMode(Internal.GTK.Constants.GtkSelectionMode mode)
		{
			switch (mode)
			{
				case Internal.GTK.Constants.GtkSelectionMode.None: return SelectionMode.None;
				case Internal.GTK.Constants.GtkSelectionMode.Single: return SelectionMode.Single;
				case Internal.GTK.Constants.GtkSelectionMode.Browse: return SelectionMode.Browse;
				case Internal.GTK.Constants.GtkSelectionMode.Multiple: return SelectionMode.Multiple;
			}
			throw new InvalidOperationException();
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
				case 65293: key = KeyboardKey.Return; break;
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
				case 65365: key = KeyboardKey.PageUp; break;
				case 65366: key = KeyboardKey.PageDown; break;
				case 65383: key = KeyboardKey.Menu; break;
				case 269025062: key = KeyboardKey.BrowserBack; break;
				case 269025153: key = KeyboardKey.SelectMedia; break;
				case 65360: key = KeyboardKey.Home; break;
				case 65367: key = KeyboardKey.End; break;
				case 65379: key = KeyboardKey.Insert; break;
				case 65535: key = KeyboardKey.Delete; break;
				case 65307: key = KeyboardKey.Escape; break;
				case 65288: key = KeyboardKey.Back; break;

			// idk why shift+tab is handled as a different keycode
				case 65056: key = KeyboardKey.Tab; modifierKeys |= KeyboardModifierKey.Shift; break;
				case 65289: key = KeyboardKey.Tab; break;

				case 65407: key = KeyboardKey.NumLock; break;
				case 65421: key = KeyboardKey.Enter; break;
				case 65450: key = KeyboardKey.Multiply; break;
				case 65451: key = KeyboardKey.Add; break;
				case 65453: key = KeyboardKey.Subtract; break;
				case 65454: key = KeyboardKey.Decimal; break;
				case 65455: key = KeyboardKey.Divide; break;

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
				case 8: buttons = MouseButtons.XButton1; break;
				case 9: buttons = MouseButtons.XButton2; break;
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
			foreach (KeyValuePair<NativeControl, Control> kvp in controlsByHandle)
			{
				if (kvp.Key is GTKNativeControl)
				{
					if ((kvp.Key as GTKNativeControl).ContainsHandle(handle))
					{
						return kvp.Value;
					}
				}
			}
			return null;
		}

		protected override void UpdateControlLayoutInternal(Control control)
		{
			GTKNativeControl nc = (GetHandleForControl(control) as GTKNativeControl);
			Contract.Requires(nc != null);

			IntPtr hCtrl = nc.Handle;
			IControlContainer parent = (control.Parent as IControlContainer);
			if (parent != null && parent.Layout != null)
			{
				Constraints constraints = parent.Layout.GetControlConstraints(control);
				if (constraints != null)
				{
					if (parent.Layout is Layouts.BoxLayout)
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
							if (handlesByLayout.ContainsKey(parent.Layout))
							{
								hLayout = handlesByLayout[parent.Layout];
							}
							else
							{
								hLayout = Internal.GTK.Methods.GtkBox.gtk_box_new(Internal.GTK.Constants.GtkOrientation.Horizontal, true, 0);
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

				if (handle is CustomNativeControl)
				{
					Control ctl = (handle as CustomNativeControl).Handle;
					handle = CreateControlInternal(ctl);
				}

				IntPtr nativeHandle = (handle as GTKNativeControl).Handle;

				if (control.TooltipText != null)
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_tooltip_text(nativeHandle, control.TooltipText);
				}
			}
			return (handle as GTKNativeControl);
		}

		protected override void AfterHandleRegistered(Control control)
		{
			GTKNativeControl nc = (GetHandleForControl(control) as GTKNativeControl);
			UpdateControlLayout(control);
			UpdateControlProperties(control);

			if (control.Visible)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(nc.Handle);

				Control[] children = new Control[0];
				if (control is IVirtualControlContainer)
				{
					children = (control as IVirtualControlContainer).GetAllControls();
				}
				for (int i = 0; i < children.Length; i++)
				{
					if (!children[i].Visible)
					{
						GTKNativeControl nc1 = (children[i].ControlImplementation?.Handle as GTKNativeControl);
						if (nc1 != null)
							Internal.GTK.Methods.GtkWidget.gtk_widget_hide(nc1.Handle);
					}
					else
					{
						GTKNativeControl nc1 = (children[i].ControlImplementation?.Handle as GTKNativeControl);
						if (nc1 != null)
							Internal.GTK.Methods.GtkWidget.gtk_widget_show(nc1.Handle);
					}
				}
			}
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

			Type[] types = Reflection.GetAvailableTypes(new Type[] { typeof(GTKDialogImplementation) });
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsAbstract) continue;

				object[] atts = (types[i].GetCustomAttributes(typeof(ControlImplementationAttribute), false));
				if (atts.Length > 0)
				{
					ControlImplementationAttribute cia = (atts[0] as ControlImplementationAttribute);
					if (cia != null)
					{
						// yeah... that's a hack right ---------------------------------->there
						// it can be fixed, but we'd have to figure out the best way to implement CustomDialog vs. CommonDialog without
						// having the GenericDialogImplementation hijack the CommonDialog stuff if it comes up first in the list
						if (dialog.GetType().IsSubclassOf(cia.ControlType) || dialog.GetType() == cia.ControlType || (dialog.GetType().BaseType == typeof(Dialog) && cia.ControlType.BaseType == typeof(Dialog)))
						{
							GTKDialogImplementation di = (types[i].Assembly.CreateInstance(types[i].FullName, false, System.Reflection.BindingFlags.Default, null, new object[] { this, dialog }, System.Globalization.CultureInfo.CurrentCulture, null) as GTKDialogImplementation);
							GTKNativeControl nc = (di.CreateControl(dialog) as GTKNativeControl);
							RegisterControlHandle(dialog, nc);

							// hack:
							// InvokeMethod(dialog, "OnCreated", new object[] { EventArgs.Empty });

							DialogResult result1 = di.Run(parentHandle);
							return result1;
						}
					}
				}
			}
			return DialogResult.None;
		}

		private DialogResult GtkPrintOperationResultToDialogResult(Internal.GTK.Constants.GtkPrintOperationResult value)
		{
			if (value == Internal.GTK.Constants.GtkPrintOperationResult.Cancel) return DialogResult.Cancel;
			return DialogResult.OK;
		}

		#region Common Dialog
		public IntPtr CommonDialog_GetParentHandle(Dialog dlg)
		{
			if (dlg.Parent != null && IsControlCreated(dlg.Parent))
			{
				return (GetHandleForControl(dlg.Parent) as GTKNativeControl).Handle;
			}
			return IntPtr.Zero;
		}
		#endregion
		#region Print Dialog

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
				((UIApplication)Application.Instance).DoEvents();
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


		private Action<IntPtr> GtkPrintJob_status_changed_handler;
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

		private void RecursiveShowChildControls(Container container)
		{
			if (handlesByLayout.ContainsKey(container.Layout))
			{
				IntPtr hLayout = handlesByLayout[container.Layout];
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hLayout);
			}
			foreach (Control ctl in container.Controls)
			{
				if (ctl is Container)
				{
					if (handlesByLayout.ContainsKey((ctl as Container).Layout))
					{
						IntPtr hLayout = handlesByLayout[(ctl as Container).Layout];
						Internal.GTK.Methods.GtkWidget.gtk_widget_show(hLayout);
					}
					RecursiveShowChildControls(ctl as Container);
				}
				if (ctl.Visible)
				{
					IntPtr hCtl = (GetHandleForControl(ctl) as GTKNativeControl).Handle;
					Internal.GTK.Methods.GtkWidget.gtk_widget_show(hCtl);
				}
			}
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

				if (button.StockType != StockType.None)
				{
					control.ControlImplementation.SetControlText(control, StockTypeToString((StockType)button.StockType));
					Internal.GTK.Methods.GtkButton.gtk_button_set_use_stock(handle, true);
				}

				Internal.GTK.Methods.GtkButton.gtk_button_set_use_underline(handle, true);
				if (Internal.GTK.Methods.Gtk.gtk_get_major_version() < 4)
				{
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
			}

			Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(handle, control.Enabled);

			if (control.Size != null)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_size_request(handle, (int)control.Size.Width, (int)control.Size.Height);
			}
		}

		private static IntPtr hDefaultAccelGroup = IntPtr.Zero;
		internal IntPtr InitMenuItem(MenuItem menuItem, string accelPath = null)
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
						Internal.GTK.Methods.GtkAccelMap.gtk_accel_map_add_entry(accelPath, GTK3Engine.GetAccelKeyForKeyboardKey(cmi.Shortcut.Key), GTK3Engine.KeyboardModifierKeyToGdkModifierType(cmi.Shortcut.ModifierKeys));
					}
				}

				IntPtr hMenuFile = Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_new();
				Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_label(hMenuFile, cmi.Text);
				Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_use_underline(hMenuFile, true);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_sensitive(hMenuFile, cmi.Enabled);
				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hMenuFile);

				/*
				if (menuItem.HorizontalAlignment == MenuItemHorizontalAlignment.Right)
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_hexpand(hMenuFile, true);
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_halign(hMenuFile, Internal.GTK.Constants.GtkAlign.End);
				}
				*/

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

				// FIXME: calling gtk_widget_show (either directly, or from gtk_widget_show_all) on some cases freezes with stack overflow
				RegisterMenuItemHandle(menuItem, new GTKNativeControl(hMenuFile));
				return hMenuFile;
			}
			else if (menuItem is SeparatorMenuItem)
			{
				// IntPtr hMenuFile = Internal.GTK.Methods.Methods.gtk_separator_new (Internal.GTK.Constants.GtkOrientation.Horizontal);
				IntPtr hMenuFile = Internal.GTK.Methods.GtkSeparatorMenuItem.gtk_separator_menu_item_new();
				RegisterMenuItemHandle(menuItem, new GTKNativeControl(hMenuFile));
				return hMenuFile;
			}
			return IntPtr.Zero;
		}

		private MBS.Framework.Collections.Generic.HandleDictionary<Menu> _menuHandles = new Collections.Generic.HandleDictionary<Menu>();

		public IntPtr BuildMenu(Menu menu, string accelPath = null)
		{
			if (!_menuHandles.ContainsObject(menu))
			{
				IntPtr hMenu = Internal.GTK.Methods.GtkMenu.gtk_menu_new();
				if (menu.EnableTearoff)
				{
					try
					{
						IntPtr hMenuTearoff = Internal.GTK.Methods.GtkTearoffMenuItem.gtk_tearoff_menu_item_new();
						Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenu, hMenuTearoff);
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
					Internal.GTK.Methods.GtkMenu.gtk_menu_set_accel_group(hMenu, hDefaultAccelGroup);
				}

				foreach (MenuItem menuItem1 in menu.Items)
				{
					IntPtr hMenuItem = InitMenuItem(menuItem1, accelPath);
					Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenu, hMenuItem);
				}
				_menuHandles.Add(hMenu, menu);
			}
			return _menuHandles.GetHandle(menu);
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
				IntPtr hMenuItem = InitMenuItem(menuItem1, accelPath);
				Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenuFileMenu, hMenuItem);
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
					nii.hIndicator = new InternalAPI.AppIndicator.Indicator(nid.Name, nid.IconNameDefault, InternalAPI.AppIndicator.AppIndicatorCategory.ApplicationStatus);
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
							IntPtr hMenuItem = InitMenuItem(mi);
							Internal.GTK.Methods.GtkMenuShell.gtk_menu_shell_append(hMenu, hMenuItem);
						}
					}

					Internal.GTK.Methods.GtkWidget.gtk_widget_show_all(hMenu);

					nii.hIndicator.HMenu = hMenu;
				}

				if (nii.hMenuItemTitle != IntPtr.Zero)
				{
					Internal.GTK.Methods.GtkMenuItem.gtk_menu_item_set_label(nii.hMenuItemTitle, nid.Text);
				}

				nii.hIndicator.IconName = nid.IconNameDefault;
				nii.hIndicator.AttentionIconName = nid.IconNameAttention;
				switch (nid.Status)
				{
					case NotificationIconStatus.Hidden:
					{
						nii.hIndicator.Status = InternalAPI.AppIndicator.AppIndicatorStatus.Passive;
						break;
					}
					case NotificationIconStatus.Visible:
					{
						nii.hIndicator.Status = InternalAPI.AppIndicator.AppIndicatorStatus.Active;
						break;
					}
					case NotificationIconStatus.Attention:
					{
						nii.hIndicator.Status = InternalAPI.AppIndicator.AppIndicatorStatus.Attention;
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
			if (popup.Actions.Count > 0)
			{
				if (!_notify_action_items.ContainsKey(hNotification))
				{
					_notify_action_items[hNotification] = new Dictionary<string, CommandItem>();
				}
				foreach (CommandItem item in popup.Actions)
				{
					if (item is CommandReferenceCommandItem)
					{
						Command cmd = Application.Instance.FindCommand((item as CommandReferenceCommandItem).CommandID);
						if (cmd != null)
						{
							Internal.Notify.Methods.notify_notification_add_action(hNotification, cmd.ID, cmd.Title, _notifyActionCallback_d, IntPtr.Zero, IntPtr.Zero);
							_notify_action_items[hNotification][cmd.ID] = item;
						}
					}
					else if (item is ActionCommandItem)
					{
						ActionCommandItem aci = (item as ActionCommandItem);
						Internal.Notify.Methods.notify_notification_add_action(hNotification, aci.ID, aci.Title, _notifyActionCallback_d, IntPtr.Zero, IntPtr.Zero);
						_notify_action_items[hNotification][aci.ID] = aci;
					}
				}
			}
			Internal.Notify.Methods.notify_notification_show(hNotification, hError);
		}

		private Dictionary<IntPtr, Dictionary<string, CommandItem>> _notify_action_items = new Dictionary<IntPtr, Dictionary<string, CommandItem>>();

		private Action<IntPtr, string, IntPtr> _notifyActionCallback_d;
		private void _notifyActionCallback(IntPtr notification, string action, IntPtr user_data)
		{
			if (!_notify_action_items.ContainsKey(notification))
				return;

			if (!_notify_action_items[notification].ContainsKey(action))
				return;

			if (_notify_action_items[notification][action] is CommandReferenceCommandItem)
			{
				Application.Instance.ExecuteCommand((_notify_action_items[notification][action] as CommandReferenceCommandItem).CommandID);
			}
			else if (_notify_action_items[notification][action] is ActionCommandItem)
			{
				(_notify_action_items[notification][action] as ActionCommandItem).Execute();
			}
		}

		protected override void RepaintCustomControl(CustomControl control, int x, int y, int width, int height)
		{
			IntPtr handle = (GetHandleForControl(control) as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkWidget.gtk_widget_queue_draw_area(handle, x, y, width, height);
		}

		protected override void DoEventsInternal()
		{
			if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V4)
			{
			}
			else
			{
				while (Internal.GTK.Methods.Gtk.gtk_events_pending())
				{
					Internal.GTK.Methods.Gtk.gtk_main_iteration();
				}
			}
		}

		internal Internal.GTK.Constants.GtkFileChooserAction FileBrowserModeToGtkFileChooserAction(FileBrowserMode value)
		{
			switch (value)
			{
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
			throw new ArgumentException();
		}

		internal Internal.GTK.Constants.GtkPositionType RelativePositionToGtkPositionType(RelativePosition value)
		{
			switch (value)
			{
				case RelativePosition.Left: return Internal.GTK.Constants.GtkPositionType.Left;
				case RelativePosition.Right: return Internal.GTK.Constants.GtkPositionType.Right;
				case RelativePosition.Top: return Internal.GTK.Constants.GtkPositionType.Top;
				case RelativePosition.Bottom: return Internal.GTK.Constants.GtkPositionType.Bottom;
			}

			return Internal.GTK.Constants.GtkPositionType.Left;
		}
		internal RelativePosition GtkPositionTypeToRelativePosition(Internal.GTK.Constants.GtkPositionType value)
		{
			switch (value)
			{
				case Internal.GTK.Constants.GtkPositionType.Left: return RelativePosition.Left;
				case Internal.GTK.Constants.GtkPositionType.Right: return RelativePosition.Right;
				case Internal.GTK.Constants.GtkPositionType.Top: return RelativePosition.Top;
				case Internal.GTK.Constants.GtkPositionType.Bottom: return RelativePosition.Bottom;
			}
			return RelativePosition.Default;
		}

		internal static Internal.GDK.Constants.GdkGravity GravityToGdkGravity(Gravity value)
		{
			switch(value)
			{
				case Gravity.BottomCenter: return Internal.GDK.Constants.GdkGravity.South;
				case Gravity.BottomLeft: return Internal.GDK.Constants.GdkGravity.SouthWest;
				case Gravity.BottomRight: return Internal.GDK.Constants.GdkGravity.SouthEast;
				case Gravity.Center: return Internal.GDK.Constants.GdkGravity.Center;
				case Gravity.CenterLeft: return Internal.GDK.Constants.GdkGravity.West;
				case Gravity.CenterRight: return Internal.GDK.Constants.GdkGravity.East;
				case Gravity.Static: return Internal.GDK.Constants.GdkGravity.Static;
				case Gravity.TopCenter: return Internal.GDK.Constants.GdkGravity.North;
				case Gravity.TopLeft: return Internal.GDK.Constants.GdkGravity.NorthWest;
				case Gravity.TopRight: return Internal.GDK.Constants.GdkGravity.NorthEast;
			}
			throw new ArgumentOutOfRangeException();
		}

		protected override void ShowMenuPopupInternal(Menu menu)
		{
			IntPtr hMenu = BuildMenu(menu);
			Internal.GTK.Methods.GtkMenu.gtk_menu_popup_at_pointer(hMenu, IntPtr.Zero);
		}
		protected override void ShowMenuPopupInternal(Menu menu, Control widget, Gravity widgetAnchor, Gravity menuAnchor)
		{
			IntPtr hMenu = BuildMenu(menu);
			IntPtr hWidget = (widget.ControlImplementation.GetNativeImplementation().Handle as GTKNativeControl).Handle;
			Internal.GTK.Methods.GtkMenu.gtk_menu_popup_at_widget(hMenu, hWidget, GravityToGdkGravity(widgetAnchor), GravityToGdkGravity(menuAnchor), IntPtr.Zero);
			//Internal.GTK.Methods.GtkMenu.gtk_menu_popup_at_pointer(hMenu, IntPtr.Zero);
		}

		protected override void UpdateTreeModelInternal(TreeModel tm, TreeModelChangedEventArgs e)
		{
			IntPtr hTreeModel = ((GTKNativeTreeModel)TreeModelManager.GetHandleForTreeModel(tm)).Handle;
			if (hTreeModel == IntPtr.Zero)
			{
				// we do not have a treemodel handle yet
				return;
			}
			switch (e.Action)
			{
				case TreeModelChangedAction.Add:
				{
					NativeHandle iter = null;

					for (int i = 0; i < e.Rows.Count; i++)
					{
						TreeModelRow row = e.Rows[i];

						// as written we currently cannot do this...
						// int itemsCount = Internal.GTK.Methods.Methods.gtk_tree_store_
						if (e.ParentRow != null && (((UIApplication)Application.Instance).Engine.TreeModelManager as GTK3TreeModelManager).IsTreeModelRowRegistered(e.ParentRow))
						{
							// fixed 2019-07-16 16:44 by beckermj
							NativeHandle iterParent = TreeModelManager.GetHandleForTreeModelRow(e.ParentRow);
							TreeModelManager.InsertTreeModelRow(tm, row, out iter, 0, true);
						}
						else
						{
							// HACK: this is already done by the new CreateTreeModelRow code... calling it again results in two rows being added
							// but we should really make absolutely certain that it really doesn't need to be called anymore
							// RecursiveTreeStoreInsertRow(tm, row, hTreeModel, out iter, null, 0, true);
						}
					}
					break;
				}
				case TreeModelChangedAction.Remove:
				{
					foreach (TreeModelRow row in e.Rows)
					{
						Internal.GTK.Structures.GtkTreeIter iter = (TreeModelManager as GTK3TreeModelManager).GetHandleForTreeModelRow<Internal.GTK.Structures.GtkTreeIter>(row);
						Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_remove(hTreeModel, ref iter);
						// (Engine as GTKEngine).UnregisterGtkTreeIter(iter);
					}
					break;
				}
				case TreeModelChangedAction.Clear:
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_clear(hTreeModel);
					break;
				}
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
			new GTKCursorInfo("grabbing", Cursors.Grabbing),
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
			new GTKCursorInfo("zoom-out", Cursors.ZoomOut),

			new GTKCursorInfo("pencil", Cursors.Pencil),
			new GTKCursorInfo("eraser", Cursors.Eraser)
		};
		internal static void InitializeCursors(IntPtr /*GdkDisplay*/ display)
		{
			if (mvarCursorsInitialized) return;

			for (int i = 0; i < cursorInfo.Length; i++)
			{
				GTKCursorInfo info = cursorInfo[i];
				IntPtr hCursor = Internal.GDK.Methods.gdk_cursor_new_from_name(display, info.Name);
				info.Handle = hCursor;
				RegisterCursor(info.Name, info.UniversalCursor, hCursor);
				cursorInfo[i] = info; // structs are weird
			}
			mvarCursorsInitialized = true;
		}

		protected override void UpdateSystemColorsInternal()
		{
			Internal.GLib.Structures.Value val = new Internal.GLib.Structures.Value();

			IntPtr hctrl = Internal.GTK.Methods.GtkEntry.gtk_entry_new();
			IntPtr hCtxTextBox = Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(hctrl);
			IntPtr hCtxDefault = Internal.GTK.Methods.GtkStyleContext.gtk_style_context_new();

			Internal.GDK.Structures.GdkRGBA rgba = new Internal.GDK.Structures.GdkRGBA();
			Internal.GTK.Methods.GtkStyleContext.gtk_style_context_get_background_color(hCtxTextBox, Internal.GTK.Constants.GtkStateFlags.Normal, ref rgba);
			UpdateSystemColor(SystemColor.WindowBackground, Color.FromRGBADouble(rgba.red, rgba.green, rgba.blue, rgba.alpha));
			Internal.GTK.Methods.GtkStyleContext.gtk_style_context_get_color(hCtxTextBox, Internal.GTK.Constants.GtkStateFlags.Normal, ref rgba);
			UpdateSystemColor(SystemColor.WindowForeground, Color.FromRGBADouble(rgba.red, rgba.green, rgba.blue, rgba.alpha));

			Internal.GTK.Methods.GtkStyleContext.gtk_style_context_lookup_color(hCtxDefault, "theme_selected_bg_color", ref rgba);
			UpdateSystemColor(SystemColor.HighlightBackground, Color.FromRGBADouble(rgba.red, rgba.green, rgba.blue, rgba.alpha));
			Internal.GTK.Methods.GtkStyleContext.gtk_style_context_lookup_color(hCtxDefault, "theme_selected_fg_color", ref rgba);
			UpdateSystemColor(SystemColor.HighlightForeground, Color.FromRGBADouble(rgba.red, rgba.green, rgba.blue, rgba.alpha));
		}

		private IntPtr _gsOrgGnomeDesktopInterface = IntPtr.Zero;
		protected override void UpdateSystemFontsInternal()
		{
			if (_gsOrgGnomeDesktopInterface == IntPtr.Zero)
			{
				IntPtr /*GSettings*/ gs = Internal.GIO.Methods.g_settings_new("org.gnome.desktop.interface");
				_gsOrgGnomeDesktopInterface = gs;
			}
			if (_gsOrgGnomeDesktopInterface != IntPtr.Zero)
			{
				string fontName = Internal.GIO.Methods.g_settings_get_string(_gsOrgGnomeDesktopInterface, "font-name");
				UpdateSystemFont(SystemFont.DefaultFont, Font.Parse(fontName));
				UpdateSystemFont(SystemFont.MenuFont, Font.Parse(fontName));
			}
			UpdateSystemFont(SystemFont.Monospace, Font.FromFamily("Monospace", new Measurement(10, MeasurementUnit.Point)));
		}

		protected override bool ShowHelpInternal(HelpTopic topic)
		{
			// apparently, a System.ComponentModel.Win32Exception means "file not found".
			// In this case we could try khelpcenter, or something else, but there's just so many of them
			// that it's difficult to come up with an all-inclusive solution. Any suggestions?
			if (topic != null)
			{
				try
				{
					Process.Start("yelp", String.Format("help:{0}/{1}", Application.Instance.ShortName, topic.Name));
					return true;
				}
				catch (System.ComponentModel.Win32Exception ex)
				{
				}
			}
			else
			{
				try
				{
					Process.Start("yelp", String.Format("help:{0}", Application.Instance.ShortName));
					return true;
				}
				catch (System.ComponentModel.Win32Exception ex)
				{
				}
			}
			return false;
		}

		private Dictionary<Timer, TimerImplementation> _Timer_Implementations = new Dictionary<Timer, TimerImplementation>();
		protected override void Timer_StartInternal(Timer timer)
		{
			if (!_Timer_Implementations.ContainsKey(timer))
			{
				_Timer_Implementations[timer] = new GTKTimerImplementation(timer);
			}
			_Timer_Implementations[timer].Start();
		}
		protected override void Timer_StopInternal(Timer timer)
		{
		}

		protected override Screen GetDefaultScreenInternal()
		{
			IntPtr hScreenDefault = Internal.GDK.Methods.gdk_screen_get_default();
			return new GDKScreen(hScreenDefault);
		}

		private DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		protected override Process LaunchApplicationInternal(string path)
		{
			try
			{
				return base.LaunchApplicationInternal(path);
			}
			catch (Exception ex)
			{
				Process p = Process.Start(new ProcessStartInfo()
				{
					FileName = "xdg-open",
					Arguments = path,
					UseShellExecute = true,
					Verb = "open"
				});
				return p;
			}
		}

		protected override void PlaySystemSoundInternal(SystemSound sound)
		{
			// there is only one system sound on Linux
			IntPtr hDpy = Internal.GDK.Methods.gdk_display_get_default();
			Internal.GDK.Methods.gdk_display_beep(hDpy);
		}

		internal static Internal.GDK.Constants.GdkSeatCapabilities SeatCapabilitiesToGdkSeatCapabilities(SeatCapabilities caps)
		{
			Internal.GDK.Constants.GdkSeatCapabilities gdkCaps = Internal.GDK.Constants.GdkSeatCapabilities.None;
			if ((caps & SeatCapabilities.Keyboard) == SeatCapabilities.Keyboard) gdkCaps |= Internal.GDK.Constants.GdkSeatCapabilities.Keyboard;
			if ((caps & SeatCapabilities.Pointer) == SeatCapabilities.Pointer) gdkCaps |= Internal.GDK.Constants.GdkSeatCapabilities.Pointer;
			if ((caps & SeatCapabilities.TabletStylus) == SeatCapabilities.TabletStylus) gdkCaps |= Internal.GDK.Constants.GdkSeatCapabilities.TabletStylus;
			if ((caps & SeatCapabilities.Touch) == SeatCapabilities.Touch) gdkCaps |= Internal.GDK.Constants.GdkSeatCapabilities.Touch;
			return gdkCaps;
		}

		protected override void GrabSeatInternal(Seat seat, SeatCapabilities capabilities)
		{
			IntPtr hDpy = Internal.GDK.Methods.gdk_display_get_default();
			IntPtr hSeat = Internal.GDK.Methods.gdk_display_get_default_seat(hDpy);

			IntPtr hWidget = ((GTKNativeControl)seat.Owner.ControlImplementation.GetNativeImplementation().Handle).Handle;
			IntPtr hWindow = Internal.GTK.Methods.GtkWidget.gtk_widget_get_window(hWidget);

			Console.WriteLine("gtk3: grabbing seat: display {0}, seat {1}, widget {2}, window {3}", hDpy, hSeat, hWidget, hWindow);

			Internal.GDK.Constants.GdkGrabStatus status =
				Internal.GDK.Methods.gdk_seat_grab(hSeat,
				hWindow,
				SeatCapabilitiesToGdkSeatCapabilities(capabilities),
				false,
				IntPtr.Zero,
				IntPtr.Zero,
				IntPtr.Zero,
				IntPtr.Zero);

			Console.WriteLine("status is {0}", status);

		}
		protected override void ReleaseSeatInternal(Seat seat)
		{
			IntPtr hDpy = Internal.GDK.Methods.gdk_display_get_default();
			IntPtr hSeat = Internal.GDK.Methods.gdk_display_get_default_seat(hDpy);
			Internal.GDK.Methods.gdk_seat_ungrab(hSeat);

			Console.WriteLine("gtk3: releasing seat, display {0}, seat {1}", hDpy, hSeat);

		}
	}
}
