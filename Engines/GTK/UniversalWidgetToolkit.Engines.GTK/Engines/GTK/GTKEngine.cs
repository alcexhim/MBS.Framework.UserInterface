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

			Console.WriteLine ("gtk-engine: launching main loop");
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
				handlesByLayout [container.Layout] = hContainer;
			}

			if (hContainer != IntPtr.Zero)
			{
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
								Internal.GTK.Methods.gtk_container_add (hContainer, ctlHandle);
								break;
							}
							case Alignment.Far:
							{
								Internal.GTK.Methods.gtk_box_pack_end (hContainer, ctlHandle, false, false, 0);
								break;
							}
							case Alignment.Near:
							{
								Internal.GTK.Methods.gtk_box_pack_start (hContainer, ctlHandle, false, false, 0);
								break;
							}
						}
					}
					else
					{
						Internal.GTK.Methods.gtk_container_add (hContainer, ctlHandle);
					}
				}
			}

			return hContainer;
		}

		private static Dictionary<StockType, string> mvarStockButtonIDs = new Dictionary<StockType, string>();
		static GTKEngine()
		{
			mvarStockButtonIDs.Add(StockType.About, "gtk-about");
			mvarStockButtonIDs.Add(StockType.Add, "gtk-add");
			mvarStockButtonIDs.Add(StockType.Apply, "gtk-apply");
			mvarStockButtonIDs.Add(StockType.Bold, "gtk-bold");
			mvarStockButtonIDs.Add(StockType.Cancel, "gtk-cancel");
			mvarStockButtonIDs.Add(StockType.CapsLockWarning, "gtk-caps-lock-warning");
			mvarStockButtonIDs.Add(StockType.CDROM, "gtk-cdrom");
			mvarStockButtonIDs.Add(StockType.Clear, "gtk-clear");
			mvarStockButtonIDs.Add(StockType.Close, "gtk-close");
			mvarStockButtonIDs.Add(StockType.ColorPicker, "gtk-color-picker");
			mvarStockButtonIDs.Add(StockType.Connect, "gtk-connect");
			mvarStockButtonIDs.Add(StockType.Convert, "gtk-convert");
			mvarStockButtonIDs.Add(StockType.Copy, "gtk-copy");
			mvarStockButtonIDs.Add(StockType.Cut, "gtk-cut");
			mvarStockButtonIDs.Add(StockType.Delete, "gtk-delete");
			mvarStockButtonIDs.Add(StockType.DialogAuthentication, "gtk-dialog-authentication");
			mvarStockButtonIDs.Add(StockType.DialogInfo, "gtk-dialog-info");
			mvarStockButtonIDs.Add(StockType.DialogWarning, "gtk-dialog-warning");
			mvarStockButtonIDs.Add(StockType.DialogError, "gtk-dialog-error");
			mvarStockButtonIDs.Add(StockType.DialogQuestion, "gtk-dialog-question");
			mvarStockButtonIDs.Add(StockType.Directory, "gtk-directory");
			mvarStockButtonIDs.Add(StockType.Discard, "gtk-discard");
			mvarStockButtonIDs.Add(StockType.Disconnect, "gtk-disconnect");
			mvarStockButtonIDs.Add(StockType.DragAndDrop, "gtk-dnd");
			mvarStockButtonIDs.Add(StockType.DragAndDropMultiple, "gtk-dnd-multiple");
			mvarStockButtonIDs.Add(StockType.Edit, "gtk-edit");
			mvarStockButtonIDs.Add(StockType.Execute, "gtk-execute");
			mvarStockButtonIDs.Add(StockType.File, "gtk-file");
			mvarStockButtonIDs.Add(StockType.Find, "gtk-find");
			mvarStockButtonIDs.Add(StockType.FindAndReplace, "gtk-find-and-replace");
			mvarStockButtonIDs.Add(StockType.Floppy, "gtk-floppy");
			mvarStockButtonIDs.Add(StockType.Fullscreen, "gtk-fullscreen");
			mvarStockButtonIDs.Add(StockType.GotoBottom, "gtk-goto-bottom");
			mvarStockButtonIDs.Add(StockType.GotoFirst, "gtk-goto-first");
			mvarStockButtonIDs.Add(StockType.GotoLast, "gtk-goto-last");
			mvarStockButtonIDs.Add(StockType.GotoTop, "gtk-goto-top");
			mvarStockButtonIDs.Add(StockType.GoBack, "gtk-go-back");
			mvarStockButtonIDs.Add(StockType.GoDown, "gtk-go-down");
			mvarStockButtonIDs.Add(StockType.GoForward, "gtk-go-forward");
			mvarStockButtonIDs.Add(StockType.GoUp, "gtk-go-up");
			mvarStockButtonIDs.Add(StockType.HardDisk, "gtk-harddisk");
			mvarStockButtonIDs.Add(StockType.Help, "gtk-help");
			mvarStockButtonIDs.Add(StockType.Home, "gtk-home");
			mvarStockButtonIDs.Add(StockType.Index, "gtk-index");
			mvarStockButtonIDs.Add(StockType.Indent, "gtk-indent");
			mvarStockButtonIDs.Add(StockType.Info, "gtk-info");
			mvarStockButtonIDs.Add(StockType.Italic, "gtk-italic");
			mvarStockButtonIDs.Add(StockType.JumpTo, "gtk-jump-to");
			mvarStockButtonIDs.Add(StockType.JustifyCenter, "gtk-justify-center");
			mvarStockButtonIDs.Add(StockType.JustifyFill, "gtk-justify-fill");
			mvarStockButtonIDs.Add(StockType.JustifyLeft, "gtk-justify-left");
			mvarStockButtonIDs.Add(StockType.JustifyRight, "gtk-justify-right");
			mvarStockButtonIDs.Add(StockType.LeaveFullscreen, "gtk-leave-fullscreen");
			mvarStockButtonIDs.Add(StockType.MissingImage, "gtk-missing-image");
			mvarStockButtonIDs.Add(StockType.MediaForward, "gtk-media-forward");
			mvarStockButtonIDs.Add(StockType.MediaNext, "gtk-media-next");
			mvarStockButtonIDs.Add(StockType.MediaPause, "gtk-media-pause");
			mvarStockButtonIDs.Add(StockType.MediaPlay, "gtk-media-play");
			mvarStockButtonIDs.Add(StockType.MediaPrevious, "gtk-media-previous");
			mvarStockButtonIDs.Add(StockType.MediaRecord, "gtk-media-record");
			mvarStockButtonIDs.Add(StockType.MediaRewind, "gtk-media-rewind");
			mvarStockButtonIDs.Add(StockType.MediaStop, "gtk-media-stop");
			mvarStockButtonIDs.Add(StockType.Network, "gtk-network");
			mvarStockButtonIDs.Add(StockType.New, "gtk-new");
			mvarStockButtonIDs.Add(StockType.No, "gtk-no");
			mvarStockButtonIDs.Add(StockType.OK, "gtk-ok");
			mvarStockButtonIDs.Add(StockType.Open, "gtk-open");
			mvarStockButtonIDs.Add(StockType.OrientationPortrait, "gtk-orientation-portrait");
			mvarStockButtonIDs.Add(StockType.OrientationLandscape, "gtk-orientation-landscape");
			mvarStockButtonIDs.Add(StockType.OrientationReverseLandscape, "gtk-orientation-reverse-landscape");
			mvarStockButtonIDs.Add(StockType.OrientationReversePortrait, "gtk-orientation-reverse-portrait");
			mvarStockButtonIDs.Add(StockType.PageSetup, "gtk-page-setup");
			mvarStockButtonIDs.Add(StockType.Paste, "gtk-paste");
			mvarStockButtonIDs.Add(StockType.Preferences, "gtk-preferences");
			mvarStockButtonIDs.Add(StockType.Print, "gtk-print");
			mvarStockButtonIDs.Add(StockType.PrintError, "gtk-print-error");
			mvarStockButtonIDs.Add(StockType.PrintPaused, "gtk-print-paused");
			mvarStockButtonIDs.Add(StockType.PrintPreview, "gtk-print-preview");
			mvarStockButtonIDs.Add(StockType.PrintReport, "gtk-print-report");
			mvarStockButtonIDs.Add(StockType.PrintWarning, "gtk-print-warning");
			mvarStockButtonIDs.Add(StockType.Properties, "gtk-properties");
			mvarStockButtonIDs.Add(StockType.Quit, "gtk-quit");
			mvarStockButtonIDs.Add(StockType.Redo, "gtk-redo");
			mvarStockButtonIDs.Add(StockType.Refresh, "gtk-refresh");
			mvarStockButtonIDs.Add(StockType.Remove, "gtk-remove");
			mvarStockButtonIDs.Add(StockType.RevertToSaved, "gtk-revert-to-saved");
			mvarStockButtonIDs.Add(StockType.Save, "gtk-save");
			mvarStockButtonIDs.Add(StockType.SaveAs, "gtk-save-as");
			mvarStockButtonIDs.Add(StockType.SelectAll, "gtk-select-all");
			mvarStockButtonIDs.Add(StockType.SelectColor, "gtk-select-color");
			mvarStockButtonIDs.Add(StockType.SelectFont, "gtk-select-font");
			mvarStockButtonIDs.Add(StockType.SortAscending, "gtk-sort-ascending");
			mvarStockButtonIDs.Add(StockType.SortDescending, "gtk-sort-descending");
			mvarStockButtonIDs.Add(StockType.SpellCheck, "gtk-spell-check");
			mvarStockButtonIDs.Add(StockType.Stop, "gtk-stop");
			mvarStockButtonIDs.Add(StockType.Strikethrough, "gtk-strikethrough");
			mvarStockButtonIDs.Add(StockType.Undelete, "gtk-undelete");
			mvarStockButtonIDs.Add(StockType.Underline, "gtk-underline");
			mvarStockButtonIDs.Add(StockType.Undo, "gtk-undo");
			mvarStockButtonIDs.Add(StockType.Unindent, "gtk-unindent");
			mvarStockButtonIDs.Add(StockType.Yes, "gtk-yes");
			mvarStockButtonIDs.Add(StockType.Zoom100, "gtk-zoom-100");
			mvarStockButtonIDs.Add(StockType.ZoomFit, "gtk-zoom-fit");
			mvarStockButtonIDs.Add(StockType.ZoomIn, "gtk-zoom-in");
			mvarStockButtonIDs.Add(StockType.ZoomOut, "gtk-zoom-out");
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

				Internal.GObject.Methods.g_signal_connect (handle, "clicked", gc_Button_Clicked, new IntPtr(0xDEADBEEF));
			}
			else if (control is Label) {
				handle = Internal.GTK.Methods.gtk_label_new_with_mnemonic (control.Text);

				Internal.GTK.Constants.GtkJustification justify = UniversalWidgetToolkit.Engines.GTK.Internal.GTK.Constants.GtkJustification.Left;
				switch ((control as Label).HorizontalAlignment) {
					case HorizontalAlignment.Center:
					{
						justify = Internal.GTK.Constants.GtkJustification.Center;
						break;
					}
					case HorizontalAlignment.Justify:
					{
						justify = Internal.GTK.Constants.GtkJustification.Fill;
						break;
					}
					case HorizontalAlignment.Left:
					{
						justify = Internal.GTK.Constants.GtkJustification.Left;
						break;
					}
					case HorizontalAlignment.Right:
					{
						justify = Internal.GTK.Constants.GtkJustification.Right;
						break;
					}
				}

				Internal.GTK.Methods.gtk_label_set_justify (handle, justify);
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
		protected override CommonDialogResult ShowDialogInternal (CommonDialog dialog)
		{
			IntPtr parentHandle = IntPtr.Zero;
			if (dialog.Parent != null) {
				parentHandle = handlesByControl [dialog.Parent];
			}

			IntPtr handle = IntPtr.Zero;
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

			CommonDialogResult result = CommonDialogResult.None;

			if (handle != IntPtr.Zero) {
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
						result = CommonDialogResult.OK;
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.Apply:
					{
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.Cancel:
					{
						result = CommonDialogResult.Cancel;
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.Close:
					{
						result = CommonDialogResult.Cancel;
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.DeleteEvent:
					{
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.Help:
					{
						result = CommonDialogResult.Help;
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.No:
					{
						result = CommonDialogResult.No;
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.None:
					{
						result = CommonDialogResult.None;
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.Reject:
					{
						result = CommonDialogResult.Cancel;
						break;
					}
					case (int)Internal.GTK.Constants.GtkResponseType.Yes:
					{
						result = CommonDialogResult.Yes;
						break;
					}
				}

				Internal.GTK.Methods.gtk_widget_destroy (handle);
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
			if (control is Window) {
				return Internal.GTK.Methods.gtk_window_get_title (handlesByControl [control]);
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
					accept_button = mvarStockButtonIDs [StockType.Save];
					break;
				}
				case FileDialogMode.SelectFolder:
				case FileDialogMode.Open:
				{
					accept_button = mvarStockButtonIDs [StockType.Open];
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

		protected override void UpdateControlPropertiesInternal (Control control)
		{

			if (!handlesByControl.ContainsKey (control))
				return;

			string text = control.Text;
			if (!String.IsNullOrEmpty (text)) {
				text = text.Replace ('&', '_');
			}

			IntPtr handle = handlesByControl [control];

			if (control is Window) {
				Internal.GTK.Methods.gtk_window_set_title (handle, text);
			}
			else if (control is Button) {
				Button button = (control as Button);

				if (!String.IsNullOrEmpty (text)) {
					Internal.GTK.Methods.gtk_button_set_label (handle, text);
				}

				if (button.StockType != StockType.None)
				{
					if (mvarStockButtonIDs.ContainsKey(button.StockType)) {
						Internal.GTK.Methods.gtk_button_set_label (handle, mvarStockButtonIDs[button.StockType]);
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

		private Dictionary<NotificationIcon, IntPtr> handlesByNotificationIcon = new Dictionary<NotificationIcon, IntPtr> ();

		protected override void UpdateNotificationIconInternal (NotificationIcon nid, bool updateContextMenu)
		{
			try
			{
				IntPtr hIndicator = IntPtr.Zero;
				if (!handlesByNotificationIcon.ContainsKey(nid)) {
					hIndicator = Internal.AppIndicator.Methods.app_indicator_new(nid.Name, nid.IconNameDefault, Internal.AppIndicator.Constants.AppIndicatorCategory.ApplicationStatus);
					handlesByNotificationIcon.Add(nid, hIndicator);
					
					// Internal.AppIndicator.Methods.app_indicator_set_label(hIndicator, nid.Text, "I don't know what this is for");
					// Internal.AppIndicator.Methods.app_indicator_set_title(hIndicator, nid.Text);
				}
				else {
					hIndicator = handlesByNotificationIcon[nid];
				}

				if (updateContextMenu) {
					IntPtr hMenu = Internal.GTK.Methods.gtk_menu_new();

					IntPtr hMenuTitle = Internal.GTK.Methods.gtk_menu_item_new();
					Internal.GTK.Methods.gtk_widget_set_sensitive(hMenuTitle, false);
					Internal.GTK.Methods.gtk_menu_item_set_label(hMenuTitle, nid.Text);
					Internal.GTK.Methods.gtk_menu_shell_append(hMenu, hMenuTitle);

					IntPtr hMenuSeparator = Internal.GTK.Methods.gtk_separator_menu_item_new();
					Internal.GTK.Methods.gtk_menu_shell_append(hMenu, hMenuSeparator);

					if (nid.ContextMenu != null) {
						foreach (MenuItem mi in nid.ContextMenu.Items)
						{
							InitMenuItem(mi, hMenu);
						}
					}

					Internal.GTK.Methods.gtk_widget_show_all(hMenu);

					Internal.AppIndicator.Methods.app_indicator_set_menu(hIndicator, hMenu);
				}

				Internal.AppIndicator.Methods.app_indicator_set_attention_icon(hIndicator, nid.IconNameAttention);
				switch (nid.Status) {
					case NotificationIconStatus.Hidden:
					{
						Internal.AppIndicator.Methods.app_indicator_set_status(hIndicator, Internal.AppIndicator.Constants.AppIndicatorStatus.Passive);
						break;
					}
					case NotificationIconStatus.Visible:
					{
						Internal.AppIndicator.Methods.app_indicator_set_status(hIndicator, Internal.AppIndicator.Constants.AppIndicatorStatus.Active);
						break;
					}
					case NotificationIconStatus.Attention:
					{
						Internal.AppIndicator.Methods.app_indicator_set_status(hIndicator, Internal.AppIndicator.Constants.AppIndicatorStatus.Attention);
						break;
					}
				}
			}
			catch {
			}
		}
	}
}

