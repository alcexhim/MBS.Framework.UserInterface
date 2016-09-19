using System;
using System.Collections.Generic;
using System.ComponentModel;

using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Dialogs;

using UniversalWidgetToolkit.Input.Keyboard;

namespace UniversalWidgetToolkit.Engines.GTK
{
	public class GTKEngine : Engine
	{
		private int _exitCode = 0;
		private IntPtr mvarApplicationHandle = IntPtr.Zero;

		private uint GetAccelKeyForKeyboardKey(KeyboardKey key)
		{
			switch (key) {
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
		private Internal.GDK.Constants.GdkModifierType GetGdkModifierTypeForModifierKey(KeyboardModifierKey key)
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

		protected override bool InitializeInternal ()
		{
			string[] argv = System.Environment.GetCommandLineArgs ();
			int argc = argv.Length;
			
			bool check = Internal.GTK.Methods.gtk_init_check (ref argc, ref argv);
			if (!check)
				return check;

			string appname = System.Reflection.Assembly.GetEntryAssembly ().GetName ().Name;
			Internal.Notify.Methods.notify_init (appname);

			gc_Application_Activate = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback(Application_Activate);
			gc_Window_Activate = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback(Window_Activate);
			gc_Window_Closing = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback(Window_Closing);
			gc_Window_Closed = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback(Window_Closed);
			gc_Button_Clicked = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback (Button_Clicked);
			gc_MenuItem_Activated = new UniversalWidgetToolkit.Engines.GTK.Internal.GObject.Delegates.GCallback (MenuItem_Activate);

			IntPtr hApp = Internal.GTK.Methods.gtk_application_new ("net.alcetech.UniversalEditor", Internal.GIO.Constants.GApplicationFlags.None);
			// hApp = IntPtr.Zero;
			// IntPtr hApp = Internal.GIO.Methods.g_application_new ("net.alcetech.UniversalEditor", Internal.GIO.Constants.GApplicationFlags.None);
			if (hApp != IntPtr.Zero) {
				bool isRegistered = Internal.GIO.Methods.g_application_get_is_registered (hApp);

				Internal.GIO.Methods.g_application_register (hApp, IntPtr.Zero, IntPtr.Zero);

				isRegistered = Internal.GIO.Methods.g_application_get_is_registered (hApp);

				IntPtr simpleActionGroup = Internal.GIO.Methods.g_simple_action_group_new ();

				IntPtr hActionNew = Internal.GIO.Methods.g_simple_action_new ("new", Internal.GLib.Constants.GVariantType.Byte);

				Internal.GIO.Methods.g_action_map_add_action (hApp, hActionNew);
				
				IntPtr hMenu = Internal.GIO.Methods.g_menu_new ();

				IntPtr hMenuItemFile = Internal.GIO.Methods.g_menu_item_new ("_New Document", "app.new");

				Internal.GIO.Methods.g_menu_append_item (hMenu, hMenuItemFile);

				Internal.GTK.Methods.gtk_application_set_app_menu (hApp, hMenu);

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
		protected override int StartInternal (Window waitForClose)
		{
			if (mvarApplicationHandle != IntPtr.Zero) {
				string[] argv = System.Environment.GetCommandLineArgs ();
				int argc = argv.Length;

				Internal.GObject.Methods.g_signal_connect (mvarApplicationHandle, "activate", gc_Application_Activate, IntPtr.Zero);
				Internal.GIO.Methods.g_application_run (mvarApplicationHandle, argc, argv);
			}

			Internal.GTK.Methods.gtk_main ();

			return _exitCode;
		}
		protected override void StopInternal (int exitCode)
		{
			Internal.GTK.Methods.gtk_main_quit ();
			_exitCode = exitCode;
		}

		private Dictionary<IntPtr, Control> controlsByHandle = new Dictionary<IntPtr, Control>();
		private Dictionary<Control, IntPtr> handlesByControl = new Dictionary<Control, IntPtr>();

		private Dictionary<Layout, IntPtr> handlesByLayout = new Dictionary<Layout, IntPtr>();

		private Dictionary<IntPtr, MenuItem> menuItemsByHandle = new Dictionary<IntPtr, MenuItem>();

		private void RegisterControlHandle(Control control, IntPtr handle)
		{
			controlsByHandle [handle] = control;
			handlesByControl [control] = handle;
		}

		private Internal.GObject.Delegates.GCallback gc_MenuItem_Activated = null;
		private Internal.GObject.Delegates.GCallback gc_Application_Activate = null;
		private Internal.GObject.Delegates.GCallback gc_Window_Activate = null;
		private Internal.GObject.Delegates.GCallback gc_Window_Closing = null;
		private Internal.GObject.Delegates.GCallback gc_Window_Closed = null;
		private Internal.GObject.Delegates.GCallback gc_Button_Clicked = null;

		private void Application_Activate(IntPtr handle, IntPtr data)
		{
		}

		private void MenuItem_Activate(IntPtr handle, IntPtr data)
		{
			if (menuItemsByHandle.ContainsKey (handle)) {
				MenuItem mi = menuItemsByHandle [handle];
				if (mi is CommandMenuItem) {
					(mi as CommandMenuItem).OnClick (EventArgs.Empty);
				}
			}
		}

		private void Button_Clicked(IntPtr handle, IntPtr data)
		{
			Button button = (controlsByHandle[handle] as Button);
			if (button != null) {
				EventArgs e = new EventArgs ();
				button.OnClick (e);
			}
		}

		private void Window_Activate(IntPtr handle, IntPtr data)
		{
			Window window = (controlsByHandle[handle] as Window);
			if (window == null)
				return;

			window.OnActivate (EventArgs.Empty);
		}

		private void Window_Closing(IntPtr handle, IntPtr data)
		{
			Window window = (controlsByHandle[handle] as Window);
			if (window != null) {
				CancelEventArgs e = new CancelEventArgs ();
				window.OnClosing (e);
			}
		}

		private void Window_Closed(IntPtr handle, IntPtr data)
		{
			Window window = (controlsByHandle[handle] as Window);
			if (window != null) {
				window.OnClosed (EventArgs.Empty);
			}
		}

		private IntPtr CreateContainer(Container container)
		{
			IntPtr hContainer = IntPtr.Zero;

			if (container.Layout is Layouts.BoxLayout)
			{
				Layouts.BoxLayout box = (container.Layout as Layouts.BoxLayout);
				Internal.GTK.Constants.GtkOrientation orientation = Internal.GTK.Constants.GtkOrientation.Vertical;
				switch (box.Orientation) 
				{
					case Orientation.Horizontal:
					{
						orientation = Internal.GTK.Constants.GtkOrientation.Horizontal;
						break;
					}
					case Orientation.Vertical:
					{
						orientation = Internal.GTK.Constants.GtkOrientation.Vertical;
						break;
					}
				}
				hContainer = Internal.GTK.Methods.gtk_box_new (orientation, container.Controls.Count);
			}
			else if (container.Layout is Layouts.AbsoluteLayout)
			{
				Layouts.AbsoluteLayout abs = (container.Layout as Layouts.AbsoluteLayout);
				hContainer = Internal.GTK.Methods.gtk_fixed_new ();
			}
			else if (container.Layout is Layouts.GridLayout)
			{
				Layouts.GridLayout grid = (container.Layout as Layouts.GridLayout);
				hContainer = Internal.GTK.Methods.gtk_grid_new ();
				Internal.GTK.Methods.gtk_grid_set_row_spacing (hContainer, grid.RowSpacing);
				Internal.GTK.Methods.gtk_grid_set_column_spacing (hContainer, grid.ColumnSpacing);
			}

			if (hContainer != IntPtr.Zero)
			{
				handlesByLayout [container.Layout] = hContainer;

				foreach (Control ctl in container.Controls)
				{
					bool ret = CreateControl (ctl);
					if (!ret)
						continue;

					IntPtr ctlHandle = handlesByControl [ctl];
					
					if (container.Layout is Layouts.BoxLayout)
					{
						Layouts.BoxLayout box = (container.Layout as Layouts.BoxLayout);
						Internal.GTK.Methods.gtk_box_set_spacing (hContainer, box.Spacing);
						Internal.GTK.Methods.gtk_box_set_homogeneous (hContainer, box.Homogeneous);

						switch (box.Alignment)
						{
							case Alignment.Center:
							{
								Internal.GTK.Methods.gtk_box_pack_start (hContainer, ctlHandle, true, true, 0);
								break;
							}
							case Alignment.Far:
							{
								Internal.GTK.Methods.gtk_box_pack_end (hContainer, ctlHandle, true, true, 0);
								break;
							}
							case Alignment.Near:
							{
								Internal.GTK.Methods.gtk_box_pack_start (hContainer, ctlHandle, true, true, 0);
								break;
							}
						}
					}
					else if (container.Layout is Layouts.AbsoluteLayout)
					{
						Layouts.AbsoluteLayout.Constraints constraints = (Layouts.AbsoluteLayout.Constraints) container.Layout.GetControlConstraints (ctl);
						Internal.GTK.Methods.gtk_fixed_put (hContainer, ctlHandle, constraints.X, constraints.Y);
					}
					else if (container.Layout is Layouts.GridLayout)
					{
						Layouts.GridLayout.Constraints constraints = (Layouts.GridLayout.Constraints)container.Layout.GetControlConstraints (ctl);
						Internal.GTK.Methods.gtk_grid_attach (hContainer, ctlHandle, constraints.Column, constraints.Row, constraints.ColumnSpan, constraints.RowSpan);
					}
					else
					{
						Internal.GTK.Methods.gtk_container_add (hContainer, ctlHandle);
					}
				}
			}

			return hContainer;
		}

		private static Dictionary<StockType, string> mvarStockIDs = new Dictionary<StockType, string>();
		static GTKEngine()
		{
			mvarStockIDs.Add(StockType.About, "gtk-about");
			mvarStockIDs.Add(StockType.Add, "gtk-add");
			mvarStockIDs.Add(StockType.Apply, "gtk-apply");
			mvarStockIDs.Add(StockType.Bold, "gtk-bold");
			mvarStockIDs.Add(StockType.Cancel, "gtk-cancel");
			mvarStockIDs.Add(StockType.CapsLockWarning, "gtk-caps-lock-warning");
			mvarStockIDs.Add(StockType.CDROM, "gtk-cdrom");
			mvarStockIDs.Add(StockType.Clear, "gtk-clear");
			mvarStockIDs.Add(StockType.Close, "gtk-close");
			mvarStockIDs.Add(StockType.ColorPicker, "gtk-color-picker");
			mvarStockIDs.Add(StockType.Connect, "gtk-connect");
			mvarStockIDs.Add(StockType.Convert, "gtk-convert");
			mvarStockIDs.Add(StockType.Copy, "gtk-copy");
			mvarStockIDs.Add(StockType.Cut, "gtk-cut");
			mvarStockIDs.Add(StockType.Delete, "gtk-delete");
			mvarStockIDs.Add(StockType.DialogAuthentication, "gtk-dialog-authentication");
			mvarStockIDs.Add(StockType.DialogInfo, "gtk-dialog-info");
			mvarStockIDs.Add(StockType.DialogWarning, "gtk-dialog-warning");
			mvarStockIDs.Add(StockType.DialogError, "gtk-dialog-error");
			mvarStockIDs.Add(StockType.DialogQuestion, "gtk-dialog-question");
			mvarStockIDs.Add(StockType.Directory, "gtk-directory");
			mvarStockIDs.Add(StockType.Discard, "gtk-discard");
			mvarStockIDs.Add(StockType.Disconnect, "gtk-disconnect");
			mvarStockIDs.Add(StockType.DragAndDrop, "gtk-dnd");
			mvarStockIDs.Add(StockType.DragAndDropMultiple, "gtk-dnd-multiple");
			mvarStockIDs.Add(StockType.Edit, "gtk-edit");
			mvarStockIDs.Add(StockType.Execute, "gtk-execute");
			mvarStockIDs.Add(StockType.File, "gtk-file");
			mvarStockIDs.Add(StockType.Find, "gtk-find");
			mvarStockIDs.Add(StockType.FindAndReplace, "gtk-find-and-replace");
			mvarStockIDs.Add(StockType.Floppy, "gtk-floppy");
			mvarStockIDs.Add(StockType.Fullscreen, "gtk-fullscreen");
			mvarStockIDs.Add(StockType.GotoBottom, "gtk-goto-bottom");
			mvarStockIDs.Add(StockType.GotoFirst, "gtk-goto-first");
			mvarStockIDs.Add(StockType.GotoLast, "gtk-goto-last");
			mvarStockIDs.Add(StockType.GotoTop, "gtk-goto-top");
			mvarStockIDs.Add(StockType.GoBack, "gtk-go-back");
			mvarStockIDs.Add(StockType.GoDown, "gtk-go-down");
			mvarStockIDs.Add(StockType.GoForward, "gtk-go-forward");
			mvarStockIDs.Add(StockType.GoUp, "gtk-go-up");
			mvarStockIDs.Add(StockType.HardDisk, "gtk-harddisk");
			mvarStockIDs.Add(StockType.Help, "gtk-help");
			mvarStockIDs.Add(StockType.Home, "gtk-home");
			mvarStockIDs.Add(StockType.Index, "gtk-index");
			mvarStockIDs.Add(StockType.Indent, "gtk-indent");
			mvarStockIDs.Add(StockType.Info, "gtk-info");
			mvarStockIDs.Add(StockType.Italic, "gtk-italic");
			mvarStockIDs.Add(StockType.JumpTo, "gtk-jump-to");
			mvarStockIDs.Add(StockType.JustifyCenter, "gtk-justify-center");
			mvarStockIDs.Add(StockType.JustifyFill, "gtk-justify-fill");
			mvarStockIDs.Add(StockType.JustifyLeft, "gtk-justify-left");
			mvarStockIDs.Add(StockType.JustifyRight, "gtk-justify-right");
			mvarStockIDs.Add(StockType.LeaveFullscreen, "gtk-leave-fullscreen");
			mvarStockIDs.Add(StockType.MissingImage, "gtk-missing-image");
			mvarStockIDs.Add(StockType.MediaForward, "gtk-media-forward");
			mvarStockIDs.Add(StockType.MediaNext, "gtk-media-next");
			mvarStockIDs.Add(StockType.MediaPause, "gtk-media-pause");
			mvarStockIDs.Add(StockType.MediaPlay, "gtk-media-play");
			mvarStockIDs.Add(StockType.MediaPrevious, "gtk-media-previous");
			mvarStockIDs.Add(StockType.MediaRecord, "gtk-media-record");
			mvarStockIDs.Add(StockType.MediaRewind, "gtk-media-rewind");
			mvarStockIDs.Add(StockType.MediaStop, "gtk-media-stop");
			mvarStockIDs.Add(StockType.Network, "gtk-network");
			mvarStockIDs.Add(StockType.New, "gtk-new");
			mvarStockIDs.Add(StockType.No, "gtk-no");
			mvarStockIDs.Add(StockType.OK, "gtk-ok");
			mvarStockIDs.Add(StockType.Open, "gtk-open");
			mvarStockIDs.Add(StockType.OrientationPortrait, "gtk-orientation-portrait");
			mvarStockIDs.Add(StockType.OrientationLandscape, "gtk-orientation-landscape");
			mvarStockIDs.Add(StockType.OrientationReverseLandscape, "gtk-orientation-reverse-landscape");
			mvarStockIDs.Add(StockType.OrientationReversePortrait, "gtk-orientation-reverse-portrait");
			mvarStockIDs.Add(StockType.PageSetup, "gtk-page-setup");
			mvarStockIDs.Add(StockType.Paste, "gtk-paste");
			mvarStockIDs.Add(StockType.Preferences, "gtk-preferences");
			mvarStockIDs.Add(StockType.Print, "gtk-print");
			mvarStockIDs.Add(StockType.PrintError, "gtk-print-error");
			mvarStockIDs.Add(StockType.PrintPaused, "gtk-print-paused");
			mvarStockIDs.Add(StockType.PrintPreview, "gtk-print-preview");
			mvarStockIDs.Add(StockType.PrintReport, "gtk-print-report");
			mvarStockIDs.Add(StockType.PrintWarning, "gtk-print-warning");
			mvarStockIDs.Add(StockType.Properties, "gtk-properties");
			mvarStockIDs.Add(StockType.Quit, "gtk-quit");
			mvarStockIDs.Add(StockType.Redo, "gtk-redo");
			mvarStockIDs.Add(StockType.Refresh, "gtk-refresh");
			mvarStockIDs.Add(StockType.Remove, "gtk-remove");
			mvarStockIDs.Add(StockType.RevertToSaved, "gtk-revert-to-saved");
			mvarStockIDs.Add(StockType.Save, "gtk-save");
			mvarStockIDs.Add(StockType.SaveAs, "gtk-save-as");
			mvarStockIDs.Add(StockType.SelectAll, "gtk-select-all");
			mvarStockIDs.Add(StockType.SelectColor, "gtk-select-color");
			mvarStockIDs.Add(StockType.SelectFont, "gtk-select-font");
			mvarStockIDs.Add(StockType.SortAscending, "gtk-sort-ascending");
			mvarStockIDs.Add(StockType.SortDescending, "gtk-sort-descending");
			mvarStockIDs.Add(StockType.SpellCheck, "gtk-spell-check");
			mvarStockIDs.Add(StockType.Stop, "gtk-stop");
			mvarStockIDs.Add(StockType.Strikethrough, "gtk-strikethrough");
			mvarStockIDs.Add(StockType.Undelete, "gtk-undelete");
			mvarStockIDs.Add(StockType.Underline, "gtk-underline");
			mvarStockIDs.Add(StockType.Undo, "gtk-undo");
			mvarStockIDs.Add(StockType.Unindent, "gtk-unindent");
			mvarStockIDs.Add(StockType.Yes, "gtk-yes");
			mvarStockIDs.Add(StockType.Zoom100, "gtk-zoom-100");
			mvarStockIDs.Add(StockType.ZoomFit, "gtk-zoom-fit");
			mvarStockIDs.Add(StockType.ZoomIn, "gtk-zoom-in");
			mvarStockIDs.Add(StockType.ZoomOut, "gtk-zoom-out");
		}

		private IntPtr hDefaultAccelGroup = IntPtr.Zero;
		private void InitMenuItem(MenuItem menuItem, IntPtr hMenuShell, string accelPath = null)
		{
			if (menuItem is CommandMenuItem) {
				CommandMenuItem cmi = (menuItem as CommandMenuItem);
				if (accelPath != null) {
					accelPath += "/" + cmi.Name;
				}

				if (cmi.Shortcut != null) {
					Internal.GTK.Methods.gtk_accel_map_add_entry (accelPath, GetAccelKeyForKeyboardKey (cmi.Shortcut.Key), GetGdkModifierTypeForModifierKey (cmi.Shortcut.ModifierKeys));
				}

				IntPtr hMenuFile = Internal.GTK.Methods.gtk_menu_item_new ();
				Internal.GTK.Methods.gtk_menu_item_set_label (hMenuFile, cmi.Text);
				Internal.GTK.Methods.gtk_menu_item_set_use_underline (hMenuFile, true);

				if (menuItem.HorizontalAlignment == MenuItemHorizontalAlignment.Right) {
					Internal.GTK.Methods.gtk_menu_item_set_right_justified (hMenuFile, true);
				}

				if (cmi.Items.Count > 0) {
					IntPtr hMenuFileMenu = Internal.GTK.Methods.gtk_menu_new ();

					if (accelPath != null) {
						if (hDefaultAccelGroup == IntPtr.Zero) {
							hDefaultAccelGroup = Internal.GTK.Methods.gtk_accel_group_new ();
						}
						Internal.GTK.Methods.gtk_menu_set_accel_group (hMenuFileMenu, hDefaultAccelGroup);
					}

					foreach (MenuItem menuItem1 in cmi.Items) {
						InitMenuItem (menuItem1, hMenuFileMenu, accelPath);
					}

					Internal.GTK.Methods.gtk_menu_item_set_submenu (hMenuFile, hMenuFileMenu);
				}

				menuItemsByHandle[hMenuFile] = cmi;
				Internal.GObject.Methods.g_signal_connect (hMenuFile, "activate", gc_MenuItem_Activated, IntPtr.Zero);

				if (accelPath != null) {
					Internal.GTK.Methods.gtk_menu_item_set_accel_path (hMenuFile, accelPath);
				}

				Internal.GTK.Methods.gtk_menu_shell_append (hMenuShell, hMenuFile);
			}
			else if (menuItem is SeparatorMenuItem) {
				// IntPtr hMenuFile = Internal.GTK.Methods.gtk_separator_new (Internal.GTK.Constants.GtkOrientation.Horizontal);
				IntPtr hMenuFile = Internal.GTK.Methods.gtk_separator_menu_item_new ();
				Internal.GTK.Methods.gtk_menu_shell_append (hMenuShell, hMenuFile);
			}
		}

		private Dictionary<IntPtr, bool> textboxChanged = new Dictionary<IntPtr, bool> ();
		private void TextBox_Changed(IntPtr handle, IntPtr data)
		{
			if (!controlsByHandle.ContainsKey (handle))
				return;

			TextBox txt = controlsByHandle [handle] as TextBox;
			if (txt == null) return;
			textboxChanged [handle] = true;

			txt.OnChanged (EventArgs.Empty);
		}

		protected override bool CreateControlInternal (Control control)
		{
			IntPtr handle = IntPtr.Zero;
			if (control is Window) {
				handle = Internal.GTK.Methods.gtk_window_new (Internal.GTK.Constants.GtkWindowType.TopLevel);
	
				Window window = (control as Window);
				if (window.Bounds != Drawing.Rectangle.Empty)
				{
					Internal.GTK.Methods.gtk_widget_set_size_request (handle, (int)window.Bounds.Width, (int)window.Bounds.Height);
				}

				IntPtr hWindowContainer = Internal.GTK.Methods.gtk_vbox_new (false, 2);

				#region Menu Bar

				if (hDefaultAccelGroup == IntPtr.Zero) {
					hDefaultAccelGroup = Internal.GTK.Methods.gtk_accel_group_new ();
				}
				Internal.GTK.Methods.gtk_window_add_accel_group (handle, hDefaultAccelGroup);

				// create the menu bar
				IntPtr hMenuBar = Internal.GTK.Methods.gtk_menu_bar_new ();

				foreach (MenuItem menuItem in window.MenuBar.Items) {
					InitMenuItem (menuItem, hMenuBar, "<ApplicationFramework>");
				}

				Internal.GTK.Methods.gtk_box_pack_start (hWindowContainer, hMenuBar, false, true, 0);

				#endregion

				// rest of window child controls
				IntPtr hContainer = CreateContainer (window);
				
				if (hContainer != IntPtr.Zero)
				{
					Internal.GTK.Methods.gtk_box_pack_end (hWindowContainer, hContainer, true, true, 0);
				}

				Internal.GTK.Methods.gtk_container_add (handle, hWindowContainer);
				Internal.GTK.Methods.gtk_widget_show_all (hWindowContainer);

				Internal.GObject.Methods.g_signal_connect_after (handle, "show", gc_Window_Activate);

				Internal.GObject.Methods.g_signal_connect (handle, "destroy", gc_Window_Closing, new IntPtr(0xDEADBEEF));
				Internal.GObject.Methods.g_signal_connect_after (handle, "destroy", gc_Window_Closed, new IntPtr(0xDEADBEEF));

				if (mvarApplicationHandle != IntPtr.Zero) {
					// Internal.GTK.Methods.gtk_application_add_window (mvarApplicationHandle, handle);
				}
			}
			else if (control is Button) {
				handle = Internal.GTK.Methods.gtk_button_new ();

				// DON'T SET THIS... only Dialog buttons should get this by default
				// Internal.GTK.Methods.gtk_widget_set_can_default (handle, true);

				Internal.GObject.Methods.g_signal_connect (handle, "clicked", gc_Button_Clicked, new IntPtr(0xDEADBEEF));
			}
			else if (control is Label) {
				handle = Internal.GTK.Methods.gtk_label_new_with_mnemonic (control.Text);

				Label ctl = (control as Label);
				switch (ctl.HorizontalAlignment) {
					case HorizontalAlignment.Center: {
						Internal.GTK.Methods.gtk_widget_set_halign (handle, Internal.GTK.Constants.GtkAlign.Center);
						break;
					}
					case HorizontalAlignment.Justify: {
						Internal.GTK.Methods.gtk_widget_set_halign (handle, Internal.GTK.Constants.GtkAlign.Fill);
						break;
					}
					case HorizontalAlignment.Left: {
						Internal.GTK.Methods.gtk_widget_set_halign (handle, Internal.GTK.Constants.GtkAlign.Start);
						break;
					}
					case HorizontalAlignment.Right: {
						Internal.GTK.Methods.gtk_widget_set_halign (handle, Internal.GTK.Constants.GtkAlign.End);
						break;
					}
				}
				
				switch (ctl.VerticalAlignment) {
					case VerticalAlignment.Baseline: {
						Internal.GTK.Methods.gtk_widget_set_valign (handle, Internal.GTK.Constants.GtkAlign.Baseline);
						break;
					}
					case VerticalAlignment.Bottom: {
						Internal.GTK.Methods.gtk_widget_set_valign (handle, Internal.GTK.Constants.GtkAlign.End);
						break;
					}
					case VerticalAlignment.Middle: {
						Internal.GTK.Methods.gtk_widget_set_valign (handle, Internal.GTK.Constants.GtkAlign.Center);
						break;
					}
					case VerticalAlignment.Top: {
						Internal.GTK.Methods.gtk_widget_set_valign (handle, Internal.GTK.Constants.GtkAlign.Start);
						break;
					}
				}
			}
			else if (control is TextBox) {
				handle = Internal.GTK.Methods.gtk_entry_new ();
				Internal.GObject.Methods.g_signal_connect (handle, "changed", TextBox_Changed);

				TextBox ctl = (control as TextBox);
				string ctlText = ctl.Text;
				if (ctlText != null) {
					Internal.GTK.Methods.gtk_entry_set_text (handle, ctlText);
				}
				
				if (ctl.MaxLength > -1) {
					Internal.GTK.Methods.gtk_entry_set_max_length (handle, ctl.MaxLength);
				}
				if (ctl.WidthChars > -1) {
					Internal.GTK.Methods.gtk_entry_set_width_chars (handle, ctl.WidthChars);
				}
				Internal.GTK.Methods.gtk_entry_set_activates_default (handle, true);
				Internal.GTK.Methods.gtk_entry_set_visibility (handle, !ctl.UseSystemPasswordChar);

				Internal.GTK.Methods.gtk_editable_set_editable (handle, ctl.Editable);
			}
			else if (control is TabContainer) {
				handle = Internal.GTK.Methods.gtk_notebook_new ();

				TabContainer ctl = (control as TabContainer);

				foreach (TabPage tabPage in ctl.TabPages) {
					IntPtr hChild = Internal.GTK.Methods.gtk_label_new ("Test");
					IntPtr hTabLabel = Internal.GTK.Methods.gtk_label_new (tabPage.Text);

					Internal.GTK.Methods.gtk_notebook_append_page (handle, hChild, hTabLabel);
				}
			}
			else if (control is SplitContainer) {
				Internal.GTK.Constants.GtkOrientation orientation = Internal.GTK.Constants.GtkOrientation.Horizontal;

				SplitContainer ctl = (control as SplitContainer);
				switch (ctl.Orientation) {
					case Orientation.Horizontal: {
						orientation = Internal.GTK.Constants.GtkOrientation.Horizontal;
						break;
					}
					case Orientation.Vertical: {
						orientation = Internal.GTK.Constants.GtkOrientation.Vertical;
						break;
					}
				}
				handle = Internal.GTK.Methods.gtk_paned_new (orientation);

				foreach (Control ctl1 in ctl.Panel1.Controls) {
					if (!handlesByControl.ContainsKey (ctl1)) {
						CreateControl (ctl1);
					}
					if (!handlesByControl.ContainsKey (ctl1))
						continue;
					Internal.GTK.Methods.gtk_paned_pack1 (handle, handlesByControl [ctl1], true, true);
				}
				foreach (Control ctl1 in ctl.Panel2.Controls) {
					if (!handlesByControl.ContainsKey (ctl1)) {
						CreateControl (ctl1);
					}
					if (!handlesByControl.ContainsKey (ctl1))
						continue;
					Internal.GTK.Methods.gtk_paned_pack2 (handle, handlesByControl [ctl1], true, true);
				}
			}
			else if (control is Container) {
				handle = CreateContainer (control as Container);
			}

			if (handle != IntPtr.Zero) {
				if (control.Parent != null && control.Parent.Layout != null) {
					Constraints constraints = control.Parent.Layout.GetControlConstraints (control);
					if (constraints != null) {
						if (control.Parent.Layout is Layouts.BoxLayout) {
							Layouts.BoxLayout.Constraints cs = (constraints as Layouts.BoxLayout.Constraints);
							if (cs != null) {
								Internal.GTK.Constants.GtkPackType packType = Internal.GTK.Constants.GtkPackType.Start;
								switch (cs.PackType) {
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
								Internal.GTK.Methods.gtk_box_set_child_packing (handlesByLayout [control.Parent.Layout], handle, cs.Expand, cs.Fill, cs.Padding, packType);
							}
						}
					}
				}

				RegisterControlHandle (control, handle);
				UpdateControlProperties (control);
				return true;
			}
			return false;
		}
		protected override void SetControlVisibilityInternal (Control control, bool visible)
		{
			if (!handlesByControl.ContainsKey (control))
				CreateControl (control);

			if (!handlesByControl.ContainsKey (control))
				throw new NullReferenceException ("Control handle not found");

			IntPtr handle = handlesByControl [control];

			bool isgood = Internal.GObject.Methods.g_type_check_instance_is_a (handle, Internal.GTK.Methods.gtk_widget_get_type ());
			if (!isgood) {
				throw new ObjectDisposedException (control.GetType ().FullName);
			}

			if (visible) {
				Internal.GTK.Methods.gtk_widget_show (handle);
			} else {
				Internal.GTK.Methods.gtk_widget_hide (handle);
			}
		}

		protected override bool IsControlDisposedInternal (Control control)
		{
			if (!handlesByControl.ContainsKey (control))
				CreateControl (control);

			if (!handlesByControl.ContainsKey (control))
				throw new NullReferenceException ("Control handle not found");

			IntPtr handle = handlesByControl [control];

			bool isgood = Internal.GObject.Methods.g_type_check_instance_is_a (handle, Internal.GTK.Methods.gtk_widget_get_type ());
			return !isgood;
		}

		protected override bool IsControlCreatedInternal (Control control)
		{
			return handlesByControl.ContainsKey (control);
		}

		protected override Monitor[] GetMonitorsInternal ()
		{
			IntPtr defaultScreen = Internal.GDK.Methods.gdk_screen_get_default ();
			int monitorCount = Internal.GDK.Methods.gdk_screen_get_n_monitors (defaultScreen);

			Monitor[] monitors = new Monitor[monitorCount];
			return monitors;
		}
		protected override DialogResult ShowDialogInternal (Dialog dialog)
		{
			IntPtr parentHandle = IntPtr.Zero;
			if (dialog.Parent != null) {
				parentHandle = handlesByControl [dialog.Parent];
			}

			IntPtr handle = IntPtr.Zero;
			List<Button> buttons = new List<Button> ();

			if (dialog is Dialogs.MessageDialog) {
				Dialogs.MessageDialog dlg = (dialog as Dialogs.MessageDialog);

				Internal.GTK.Constants.GtkMessageType messageType = Internal.GTK.Constants.GtkMessageType.Other;
				switch (dlg.Icon)
				{
					case Dialogs.MessageDialogIcon.Error:
					{
						messageType = UniversalWidgetToolkit.Engines.GTK.Internal.GTK.Constants.GtkMessageType.Error;
						break;
					}
					case Dialogs.MessageDialogIcon.Information:
					{
						messageType = UniversalWidgetToolkit.Engines.GTK.Internal.GTK.Constants.GtkMessageType.Info;
						break;
					}
					case Dialogs.MessageDialogIcon.Warning:
					{
						messageType = UniversalWidgetToolkit.Engines.GTK.Internal.GTK.Constants.GtkMessageType.Warning;
						break;
					}
					case Dialogs.MessageDialogIcon.Question:
					{
						messageType = UniversalWidgetToolkit.Engines.GTK.Internal.GTK.Constants.GtkMessageType.Question;
						break;
					}
				}

				Internal.GTK.Constants.GtkButtonsType buttonsType = Internal.GTK.Constants.GtkButtonsType.None;
				switch (dlg.Buttons)
				{
					case Dialogs.MessageDialogButtons.AbortRetryIgnore:
					case Dialogs.MessageDialogButtons.CancelTryContinue:
					case Dialogs.MessageDialogButtons.RetryCancel:
					case Dialogs.MessageDialogButtons.YesNoCancel:
					{
						buttonsType = Internal.GTK.Constants.GtkButtonsType.None;
						break;
					}
					case Dialogs.MessageDialogButtons.OK:
					{
						buttonsType = Internal.GTK.Constants.GtkButtonsType.OK;
						break;
					}
					case Dialogs.MessageDialogButtons.OKCancel:
					{
						buttonsType = Internal.GTK.Constants.GtkButtonsType.OKCancel;
						break;
					}
					case Dialogs.MessageDialogButtons.YesNo:
					{
						buttonsType = Internal.GTK.Constants.GtkButtonsType.YesNo;
						break;
					}
				}

				handle = Internal.GTK.Methods.gtk_message_dialog_new (parentHandle, Internal.GTK.Constants.GtkDialogFlags.Modal, messageType, buttonsType, dlg.Content);

				switch (dlg.Buttons) {
					case Dialogs.MessageDialogButtons.AbortRetryIgnore:
					{
						buttons.Add (new Button ("_Abort", DialogResult.Abort));
						buttons.Add (new Button ("_Retry", DialogResult.Retry));
						buttons.Add (new Button ("_Ignore", DialogResult.Ignore));
						break;
					}
					case Dialogs.MessageDialogButtons.CancelTryContinue:
					{
						buttons.Add (new Button (ButtonStockType.Cancel, DialogResult.Abort));
						buttons.Add (new Button ("T_ry Again", DialogResult.Retry));
						buttons.Add (new Button ("C_ontinue", DialogResult.Ignore));
						break;
					}
					case Dialogs.MessageDialogButtons.RetryCancel:
					{
						buttons.Add (new Button ("_Retry", DialogResult.Retry));
						buttons.Add (new Button (ButtonStockType.Cancel, DialogResult.Ignore));
						break;
					}
					case Dialogs.MessageDialogButtons.YesNoCancel:
					{
						buttons.Add (new Button (ButtonStockType.Yes, DialogResult.Abort));
						buttons.Add (new Button (ButtonStockType.No, DialogResult.Retry));
						buttons.Add (new Button (ButtonStockType.Cancel, DialogResult.Ignore));
						break;
					}
				}
			}
			else if (dialog is FileDialog) {
				handle = FileDialog_Create (dialog as FileDialog);
			}
			else if (dialog is ColorDialog) {
				handle = ColorDialog_Create (dialog as ColorDialog);
			}
			else if (dialog is FontDialog) {
				handle = FontDialog_Create (dialog as FontDialog);
			}
			else if (dialog is AboutDialog) {
				handle = AboutDialog_Create (dialog as AboutDialog);
			}
			else {
				handle = Dialog_Create (dialog);
			}

			// Add any additional buttons to the end of the buttons list
			foreach (Button button in dialog.Buttons) {
				buttons.Add (button);
			}

			Dialog_AddButtons (handle, buttons);

			if (dialog.DefaultButton != null) {
				IntPtr hButtonDefault = handlesByControl [dialog.DefaultButton];
				Internal.GTK.Methods.gtk_widget_grab_default (hButtonDefault);
			}

			DialogResult result = DialogResult.None;

			if (handle != IntPtr.Zero) {
				while (true) {
					int nativeResult = Internal.GTK.Methods.gtk_dialog_run (handle);

					switch (nativeResult)
					{
						case (int)Internal.GTK.Constants.GtkResponseType.OK:
						case (int)Internal.GTK.Constants.GtkResponseType.Accept:
						{
							if (dialog is FileDialog) {
								FileDialog_Accept (dialog as FileDialog, handle);
							} else if (dialog is ColorDialog) {
								ColorDialog_Accept (dialog as ColorDialog, handle);
							} else if (dialog is FontDialog) {
								FontDialog_Accept (dialog as FontDialog, handle);
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

					if (nativeResult != 0) {
						break;
					}
				}
				Internal.GTK.Methods.gtk_widget_hide (handle);
			}
			return result;
		}
		protected override void InvalidateControlInternal (Control control)
		{
			if (!handlesByControl.ContainsKey (control))
				throw new NullReferenceException ("Control handle not found");

			IntPtr handle = handlesByControl [control];
			Internal.GTK.Methods.gtk_widget_queue_draw (handle);
		}

		protected override string GetControlTextInternal (Control control)
		{
			if (!handlesByControl.ContainsKey (control))
				return null;

			string value = null;
			IntPtr handle = handlesByControl [control];
			if (control is Window) {
				value = Internal.GTK.Methods.gtk_window_get_title (handle);
			} else if (control is Button) {
				value = Internal.GTK.Methods.gtk_button_get_label (handle);
			}
			else if (control is Label) {
				value = Internal.GTK.Methods.gtk_label_get_text (handle);
			} else if (control is TextBox) {
				/*
				if (textboxChanged.ContainsKey (handle)) {
					if (!textboxChanged [handle])
						return null;
				}
				*/
				textboxChanged [handle] = false;

				ushort textLength = Internal.GTK.Methods.gtk_entry_get_text_length (handle);
				if (textLength > 0) {
					value = Internal.GTK.Methods.gtk_entry_get_text (handle);
				} else {
					return String.Empty;
				}
			}

			if (value != null) {
				System.Text.StringBuilder sb = new System.Text.StringBuilder ();
				sb.Append (value);
				return sb.ToString ();
			}
			return null;
		}
		protected override void SetControlTextInternal (Control control, string text)
		{
			if (control is Window) {
				Internal.GTK.Methods.gtk_window_set_title (handlesByControl [control], control.Text);
			}
		}

		#region Common Dialog
		private IntPtr CommonDialog_GetParentHandle(CommonDialog dlg)
		{
			if (dlg.Parent != null && handlesByControl.ContainsKey (dlg.Parent)) {
				return handlesByControl [dlg.Parent];
			}
			return IntPtr.Zero;
		}
		#endregion
		#region File Dialog
		private IntPtr FileDialog_Create(FileDialog dlg)
		{
			string title = dlg.Title;

			Internal.GTK.Constants.GtkFileChooserAction fca = Internal.GTK.Constants.GtkFileChooserAction.Open;
			switch (dlg.Mode) {
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

			IntPtr handle = Internal.GTK.Methods.gtk_file_chooser_dialog_new (title, CommonDialog_GetParentHandle(dlg), fca);

			string accept_button = "gtk-save";
			string cancel_button = "gtk-cancel";

			switch (dlg.Mode) {
				case FileDialogMode.CreateFolder:
				case FileDialogMode.Save:
				{
					accept_button = mvarStockIDs [StockType.Save];
					break;
				}
				case FileDialogMode.SelectFolder:
				case FileDialogMode.Open:
				{
					accept_button = mvarStockIDs [StockType.Open];
					break;
				}
			}

			switch (System.Environment.OSVersion.Platform) {
				case PlatformID.MacOSX:
				case PlatformID.Unix:
				{
					// buttons go cancel, then accept
					Internal.GTK.Methods.gtk_dialog_add_button (handle, cancel_button, Internal.GTK.Constants.GtkResponseType.Cancel);
					Internal.GTK.Methods.gtk_dialog_add_button (handle, accept_button, Internal.GTK.Constants.GtkResponseType.Accept);
					break;
				}
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
				{
					// buttons go accept, then cancel
					Internal.GTK.Methods.gtk_dialog_add_button (handle, accept_button, Internal.GTK.Constants.GtkResponseType.Accept);
					Internal.GTK.Methods.gtk_dialog_add_button (handle, cancel_button, Internal.GTK.Constants.GtkResponseType.Cancel);
					break;
				}
			}
			
			Internal.GTK.Methods.gtk_file_chooser_set_select_multiple (handle, dlg.MultiSelect);
			return handle;
		}

		private void FileDialog_Accept(FileDialog dlg, IntPtr handle)
		{
			IntPtr gslist = Internal.GTK.Methods.gtk_file_chooser_get_filenames (handle);

			uint length = Internal.GLib.Methods.g_slist_length (gslist);
			dlg.SelectedFileNames.Clear ();
			for (uint i = 0; i < length; i++) {
				string fileName = Internal.GLib.Methods.g_slist_nth_data (gslist, i);
				dlg.SelectedFileNames.Add (fileName);
			}

			Internal.GLib.Methods.g_slist_free (gslist);
		}
		#endregion
		#region Color Dialog
		private IntPtr ColorDialog_Create(ColorDialog dlg)
		{
			string title = dlg.Title;
			if (title == null)
				title = "Select Color";

			IntPtr handle = Internal.GTK.Methods.gtk_color_dialog_new (title, CommonDialog_GetParentHandle(dlg), !dlg.AutoUpgradeEnabled);
			return handle;
		}
		private void ColorDialog_Accept(ColorDialog dlg, IntPtr handle)
		{
			Internal.GDK.Structures.GdkRGBA color = new Internal.GDK.Structures.GdkRGBA ();
			Internal.GTK.Methods.gtk_color_chooser_get_rgba (handle, out color);
			dlg.SelectedColor = UniversalWidgetToolkit.Drawing.Color.FromRGBADouble (color.red, color.green, color.blue, color.alpha);
		}
		#endregion
		#region Font Dialog
		private IntPtr FontDialog_Create(FontDialog dlg)
		{
			string title = dlg.Title;
			if (title == null)
				title = "Select Font";

			IntPtr handle = Internal.GTK.Methods.gtk_font_dialog_new (title, CommonDialog_GetParentHandle (dlg), !dlg.AutoUpgradeEnabled);
			return handle;
		}
		private void FontDialog_Accept(FontDialog dlg, IntPtr handle)
		{
			UniversalWidgetToolkit.Drawing.Font font = Internal.GTK.Methods.gtk_font_dialog_get_font (handle, !dlg.AutoUpgradeEnabled);
			dlg.SelectedFont = font;
		}
		#endregion
		#region About Dialog
		private IntPtr AboutDialog_Create(AboutDialog dlg)
		{
			IntPtr handle = Internal.GTK.Methods.gtk_about_dialog_new ();
			Internal.GTK.Methods.gtk_about_dialog_set_program_name (handle, dlg.ProgramName);
			if (dlg.Version != null) {
				Internal.GTK.Methods.gtk_about_dialog_set_version (handle, dlg.Version.ToString ());
			}
			Internal.GTK.Methods.gtk_about_dialog_set_copyright (handle, dlg.Copyright);
			Internal.GTK.Methods.gtk_about_dialog_set_comments (handle, dlg.Comments);
			if (dlg.LicenseText != null) {
				Internal.GTK.Methods.gtk_about_dialog_set_license (handle, dlg.LicenseText);
			}

			if (dlg.Website != null) {
				Internal.GTK.Methods.gtk_about_dialog_set_website (handle, dlg.Website);
			}

			if (Internal.GTK.Methods.LIBRARY_FILENAME == Internal.GTK.Methods.LIBRARY_FILENAME_V3) {
				if (dlg.LicenseType != LicenseType.Unknown) {
					switch (dlg.LicenseType) {
						case LicenseType.Artistic: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.Artistic);
							break;
						}
						case LicenseType.BSD: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.BSD);
							break;
						}
						case LicenseType.Custom: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.Custom);
							break;
						}
						case LicenseType.GPL20: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.GPL20);
							break;
						}
						case LicenseType.GPL30: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.GPL30);
							break;
						}
						case LicenseType.LGPL21: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.LGPL21);
							break;
						}
						case LicenseType.LGPL30: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.LGPL30);
							break;
						}
						case LicenseType.MITX11: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.MITX11);
							break;
						}
						case LicenseType.Unknown: {
							Internal.GTK.Methods.gtk_about_dialog_set_license_type (handle, Internal.GTK.Constants.GtkLicense.Unknown);
							break;
						}
					}
				}
			}
			return handle;
		}
		#endregion
		#region Generic Dialog
		private void Dialog_AddButton(IntPtr handle, Button button)
		{
			IntPtr buttonHandle = IntPtr.Zero;
			if (!handlesByControl.ContainsKey (button)) {
				CreateControl (button);
			}
			buttonHandle = handlesByControl [button];

			// Internal.GTK.Methods.gtk_dialog_add_button (handle, button.StockType == ButtonStockType.Connect ? "Connect" : "Cancel", button.ResponseValue);

			int nativeResponseValue = button.ResponseValue;
			switch (button.ResponseValue) {
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

			Internal.GTK.Methods.gtk_dialog_add_action_widget (handle, buttonHandle, nativeResponseValue);
			Internal.GTK.Methods.gtk_widget_set_can_default (buttonHandle, true);

			Internal.GTK.Methods.gtk_widget_show_all (buttonHandle);
		}
		private IntPtr Dialog_Create(Dialog dlg)
		{
			IntPtr handle = Internal.GTK.Methods.gtk_dialog_new ();
			Internal.GTK.Methods.gtk_window_set_title (handle, dlg.Title);

			IntPtr hDialogContent = Internal.GTK.Methods.gtk_dialog_get_content_area (handle);
			IntPtr hContainer = CreateContainer (dlg);

			Internal.GTK.Methods.gtk_container_add (hDialogContent, hContainer);
			Internal.GTK.Methods.gtk_widget_show_all (hDialogContent);

			return handle;
		}
		private void Dialog_AddButtons(IntPtr handle, List<Button> buttons)
		{
			switch (Environment.OSVersion.Platform) {
				case PlatformID.MacOSX:
				case PlatformID.Unix:
				{
					for (int i = buttons.Count - 1; i > -1; i--) {
						Dialog_AddButton (handle, buttons[i]);
					}
					break;
				}
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
				{
					for (int i = 0; i < buttons.Count; i++) {
						Dialog_AddButton (handle, buttons[i]);
					}
					break;
				}
			}
		}
		#endregion

		protected override void UpdateControlPropertiesInternal (Control control)
		{

			if (!handlesByControl.ContainsKey (control))
				return;

			IntPtr handle = handlesByControl [control];

			if (control is Window) {

				string text = control.Text;
				if (!String.IsNullOrEmpty (text)) {
					text = text.Replace ('&', '_');
				}

				Internal.GTK.Methods.gtk_window_set_title (handle, text);
			}
			else if (control is Button) {
				Button button = (control as Button);
				
				string text = control.Text;
				if (!String.IsNullOrEmpty (text)) {
					text = text.Replace ('&', '_');
				}

				if (!String.IsNullOrEmpty (text)) {
					Internal.GTK.Methods.gtk_button_set_label (handle, text);
				}

				if (button.StockType != ButtonStockType.None)
				{
					if (mvarStockIDs.ContainsKey((StockType)button.StockType)) {
						Internal.GTK.Methods.gtk_button_set_label (handle, mvarStockIDs[(StockType)button.StockType]);
						Internal.GTK.Methods.gtk_button_set_use_stock (handle, true);
					}
				}

				Internal.GTK.Methods.gtk_button_set_use_underline (handle, true);
				Internal.GTK.Methods.gtk_button_set_focus_on_click (handle, true);

				switch (button.BorderStyle) {
					case ButtonBorderStyle.None:
					{
						Internal.GTK.Methods.gtk_button_set_relief (handle, Internal.GTK.Constants.GtkReliefStyle.None);
						break;
					}
					case ButtonBorderStyle.Half:
					{
						Internal.GTK.Methods.gtk_button_set_relief (handle, Internal.GTK.Constants.GtkReliefStyle.Half);
						break;
					}
					case ButtonBorderStyle.Normal:
					{
						Internal.GTK.Methods.gtk_button_set_relief (handle, Internal.GTK.Constants.GtkReliefStyle.Normal);
						break;
					}
				}
			}
			else if (control is TextBox) {
				string text = null;
				if (text != null) {
					// Internal.GTK.Methods.gtk_entry_set_text (handle, text);
				}
			}
			else if (control is Label) {
				string text = null;
				if (text != null) {
					// Internal.GTK.Methods.gtk_label_set_text (handle, text);
				}
			}
		}

		protected override void TabContainer_ClearTabPagesInternal (TabContainer parent)
		{
			if (!handlesByControl.ContainsKey (parent))
				return;

			IntPtr handle = handlesByControl [parent];
			int pageCount = Internal.GTK.Methods.gtk_notebook_get_n_pages (handle);
			for (int i = 0; i < pageCount; i++) {
				Internal.GTK.Methods.gtk_notebook_remove_page (handle, i);
			}
		}
		protected override void TabContainer_InsertTabPageInternal (TabContainer parent, int index, TabPage tabPage)
		{
			if (!handlesByControl.ContainsKey (parent))
				return;

			IntPtr handle = handlesByControl [parent];
			IntPtr hTabLabel = Internal.GTK.Methods.gtk_label_new (tabPage.Text);
			IntPtr hChild = Internal.GTK.Methods.gtk_label_new ("Child control for " + tabPage.Text);

			Internal.GTK.Methods.gtk_notebook_append_page (handle, hChild, hTabLabel);
		}
		protected override void TabContainer_RemoveTabPageInternal (TabContainer parent, TabPage tabPage)
		{
			throw new NotImplementedException ();
		}

		private Dictionary<NotificationIcon, NotificationIconInfo> notificationIconInfo = new Dictionary<NotificationIcon, NotificationIconInfo> ();

		protected override void UpdateNotificationIconInternal (NotificationIcon nid, bool updateContextMenu)
		{
			try
			{
				NotificationIconInfo nii = new NotificationIconInfo();
				if (!notificationIconInfo.ContainsKey(nid)) {
					nii.hIndicator = Internal.AppIndicator.Methods.app_indicator_new(nid.Name, nid.IconNameDefault, Internal.AppIndicator.Constants.AppIndicatorCategory.ApplicationStatus);
					notificationIconInfo.Add(nid, nii);
					
					// Internal.AppIndicator.Methods.app_indicator_set_label(hIndicator, nid.Text, "I don't know what this is for");
					// Internal.AppIndicator.Methods.app_indicator_set_title(hIndicator, nid.Text);
				}
				else {
					nii = notificationIconInfo[nid];
				}

				if (updateContextMenu) {
					IntPtr hMenu = Internal.GTK.Methods.gtk_menu_new();

					IntPtr hMenuTitle = Internal.GTK.Methods.gtk_menu_item_new();
					Internal.GTK.Methods.gtk_widget_set_sensitive(hMenuTitle, false);
					Internal.GTK.Methods.gtk_menu_shell_append(hMenu, hMenuTitle);
					nii.hMenuItemTitle = hMenuTitle;

					IntPtr hMenuSeparator = Internal.GTK.Methods.gtk_separator_menu_item_new();
					Internal.GTK.Methods.gtk_menu_shell_append(hMenu, hMenuSeparator);

					if (nid.ContextMenu != null) {
						foreach (MenuItem mi in nid.ContextMenu.Items)
						{
							InitMenuItem(mi, hMenu);
						}
					}

					Internal.GTK.Methods.gtk_widget_show_all(hMenu);

					Internal.AppIndicator.Methods.app_indicator_set_menu(nii.hIndicator, hMenu);
				}

				if (nii.hMenuItemTitle != IntPtr.Zero) {
					Internal.GTK.Methods.gtk_menu_item_set_label(nii.hMenuItemTitle, nid.Text);
				}

				Internal.AppIndicator.Methods.app_indicator_set_attention_icon(nii.hIndicator, nid.IconNameAttention);
				switch (nid.Status) {
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
			catch {
			}
		}

		protected override void ShowNotificationPopupInternal (NotificationPopup popup)
		{
			IntPtr hError = IntPtr.Zero;
			IntPtr hNotification = Internal.Notify.Methods.notify_notification_new (popup.Summary, popup.Content, popup.IconName);
			Internal.Notify.Methods.notify_notification_show (hNotification, hError);
		}
	}
}

