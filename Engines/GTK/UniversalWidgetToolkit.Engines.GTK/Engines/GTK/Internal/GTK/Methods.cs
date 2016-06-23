using System;
using System.Runtime.InteropServices;

namespace UniversalWidgetToolkit.Engines.GTK.Internal.GTK
{
	internal static class Methods
	{
		public const string LIBRARY_FILENAME_V2 = "gtk-x11-2.0";
		public const string LIBRARY_FILENAME_V3 = "gtk-3";

		// using GTK3 seems to sacrifice theming support, whine whine
		public const string LIBRARY_FILENAME = LIBRARY_FILENAME_V3;

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_init_check(ref int argc, ref string[] args);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_main();
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_main_quit();

		#region Application
		[DllImport(LIBRARY_FILENAME_V3, EntryPoint="gtk_application_new")]
		public static extern IntPtr gtk_application_new_v3 (string application_id, Internal.GIO.Constants.GApplicationFlags flags);

		public static IntPtr gtk_application_new (string application_id, Internal.GIO.Constants.GApplicationFlags flags)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V2) {
				return IntPtr.Zero;
			} else {
				return gtk_application_new_v3 (application_id, flags);
			}
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_application_add_window (IntPtr /*GtkApplication*/ application, IntPtr /*GtkWindow*/ window);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GMenuModel*/ gtk_application_get_menubar (IntPtr /*GtkApplication*/ application);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_application_set_menubar (IntPtr /*GtkApplication*/ application, IntPtr /*GMenuModel*/ menubar);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GMenuModel*/ gtk_application_get_app_menu (IntPtr /*GtkApplication*/ application);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_application_set_app_menu (IntPtr /*GtkApplication*/ application, IntPtr /*GMenuModel*/ menu);
		#endregion

		#region Widget
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_widget_get_type ();

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_show (IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_show_all (IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_hide (IntPtr widget);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_queue_draw(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_queue_draw_are(IntPtr widget, int x, int y, int width, int height);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_size_request (IntPtr widget, int width, int height);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_get_size_request (IntPtr widget, out int width, out int height);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_sensitive(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_widget_is_sensitive(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_sensitive(IntPtr widget, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_destroy (IntPtr widget);
		#endregion

		#region Container
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_container_add(IntPtr container, IntPtr widget);
		#endregion

		#region Window
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_window_new (Constants.GtkWindowType windowType);

		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_window_get_title(IntPtr window);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_title(IntPtr window, string title);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_window_get_hide_titlebar_when_maximized (IntPtr window);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_hide_titlebar_when_maximized (IntPtr window, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_add_accel_group (IntPtr window, IntPtr accel_group);
		#endregion

		#region Box
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_hbox_new (bool homogenous, int spacing);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_vbox_new (bool homogenous, int spacing);

		[DllImport(LIBRARY_FILENAME_V3, EntryPoint="gtk_box_new")]
		private static extern IntPtr gtk_box_new_v3 (Constants.GtkOrientation orientation, int spacing);

		public static IntPtr gtk_box_new(Constants.GtkOrientation orientation, int spacing = 0)
		{
			return gtk_box_new (orientation, true, spacing);
		}
		public static IntPtr gtk_box_new(Constants.GtkOrientation orientation, bool homogenous = true, int spacing = 0, bool useV2BoxCreation = false)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V2 || useV2BoxCreation) {
				switch (orientation)
				{
					case Constants.GtkOrientation.Horizontal:
					{
						return gtk_hbox_new (homogenous, spacing);
					}
					case Constants.GtkOrientation.Vertical:
					{
						return gtk_vbox_new (homogenous, spacing);
					}
				}
			} else if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3) {
				return gtk_box_new_v3 (orientation, spacing);
			}
			return IntPtr.Zero;
		}
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_box_get_homogeneous (IntPtr box);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_set_homogeneous (IntPtr box, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_box_get_spacing (IntPtr box);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_set_spacing (IntPtr box, int value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_pack_start (IntPtr box, IntPtr child, bool expand, bool fill, int padding);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_pack_end (IntPtr box, IntPtr child, bool expand, bool fill, int padding);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_set_child_packing (IntPtr /*GtkBox*/ box, IntPtr /*GtkWidget*/ child, bool expand, bool fill, int padding, Constants.GtkPackType pack_type);
		#endregion

		#region Menu Shell
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_menu_shell_append(IntPtr shell, IntPtr child);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_menu_shell_insert(IntPtr shell, IntPtr child, int position);
		#endregion

		#region Menu Bar
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_menu_bar_new();
		#endregion

		#region Menu
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_menu_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_menu_get_title(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_set_title(IntPtr handle, string title);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkAccelGroup*/ gtk_menu_get_accel_group (IntPtr menu);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_set_accel_group (IntPtr menu, IntPtr /*GtkAccelGroup*/ accel_group);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_menu_get_accel_path (IntPtr menu);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_set_accel_path (IntPtr menu, string accel_path);
		#endregion

		#region Menu Item
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_menu_item_new();

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_menu_item_get_use_underline(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_item_set_use_underline(IntPtr handle, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_menu_item_get_label(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_item_set_label(IntPtr handle, string text);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_menu_item_get_submenu(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_item_set_submenu(IntPtr handle, IntPtr submenu);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_menu_item_get_right_justified (IntPtr menu_item);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_item_set_right_justified (IntPtr menu_item, bool value);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_menu_item_get_accel_path (IntPtr menu_item);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_item_set_accel_path (IntPtr menu_item, string accel_path);
		#endregion

		#region Separator
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_separator_new(Constants.GtkOrientation orientation);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_separator_menu_item_new ();
		#endregion

		#region Label
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_label_new(string text);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_label_new_with_mnemonic(string text);
		
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_label_set_justify (IntPtr /*GtkLabel*/ label, Constants.GtkJustification jtype);
		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkJustification gtk_label_get_justify(IntPtr /*GtkLabel*/ label);
		#endregion

		#region Button
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_button_new ();
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_button_get_label (IntPtr button);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_label (IntPtr button, string label);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_button_get_use_underline (IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_use_underline (IntPtr handle, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_button_get_use_stock (IntPtr button);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_use_stock (IntPtr button, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_button_get_focus_on_click (IntPtr button);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_focus_on_click (IntPtr button, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_relief (IntPtr button, Constants.GtkReliefStyle value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkReliefStyle gtk_button_get_relief (IntPtr button);
		#endregion

		#region Notebook
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_notebook_new ();
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_notebook_append_page (IntPtr hNotebook, IntPtr hChild, IntPtr hTabLabel);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_notebook_get_n_pages (IntPtr hNotebook);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_notebook_page_num (IntPtr hNotebook, IntPtr hChild);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_notebook_remove_page (IntPtr hNotebook, int page_num);
		#endregion

		#region Accel Map
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_accel_map_add_entry (string accel_path, uint accel_key, Internal.GDK.Constants.GdkModifierType accel_mods);
		#endregion

		#region Accel Group
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_accel_group_new ();
		#endregion

		#region File Chooser
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_file_chooser_get_filenames (IntPtr chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_file_chooser_get_select_multiple (IntPtr chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_select_multiple (IntPtr chooser, bool value);
		#endregion

		#region Dialog
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_dialog_run(IntPtr handle);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_dialog_add_button (IntPtr /*GtkDialog*/ dialog, string button_text, int response_id);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_dialog_add_button (IntPtr /*GtkDialog*/ dialog, string button_text, Constants.GtkResponseType response_id);

		#region Message Dialog
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_message_dialog_new(IntPtr parentHandle, Constants.GtkDialogFlags flags, Constants.GtkMessageType type, Constants.GtkButtonsType buttons, string content);
		#endregion

		#region File Chooser Dialog
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_file_chooser_dialog_new(string title, IntPtr parentHandle, Constants.GtkFileChooserAction action);
		#endregion

		#region Color Chooser Dialog
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_color_selection_dialog_new(string title);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parentHandle);

		public static IntPtr gtk_color_dialog_new(string title, IntPtr parentHandle, bool useLegacyFunctionality = false)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3 && !useLegacyFunctionality) {
				return gtk_color_chooser_dialog_new (title, parentHandle);
			} else {
				return gtk_color_selection_dialog_new (title);
			}
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_color_chooser_get_rgba (IntPtr /*GtkColorChooser*/ chooser, out Internal.GDK.Structures.GdkRGBA color);
		#endregion

		#region Font Dialog
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_chooser_dialog_new (string title, IntPtr parentHandle);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_selection_dialog_new (string title);

		public static IntPtr gtk_font_dialog_new(string title, IntPtr parentHandle, bool useLegacyFunctionality = false)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3 && !useLegacyFunctionality) {
				return gtk_font_chooser_dialog_new (title, parentHandle);
			} else {
				return gtk_font_selection_dialog_new (title);
			}
		}
		
		[DllImport(LIBRARY_FILENAME)]
		private static extern string gtk_font_chooser_get_font (IntPtr /*GtkFontSelectionDialog*/ fsd);

		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_selection_dialog_get_font_selection(IntPtr /*GtkFontSelectionDialog*/ fsd);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_selection_get_family (IntPtr /*GtkFontSelection*/ fsd);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_selection_get_face (IntPtr /*GtkFontSelection*/ fsd);

		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_chooser_get_font_family (IntPtr fsd);

		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_chooser_get_font_face (IntPtr fsd);

		[DllImport(LIBRARY_FILENAME)]
		private static extern int gtk_font_selection_get_size(IntPtr fsd);
		[DllImport(LIBRARY_FILENAME)]
		private static extern int gtk_font_chooser_get_font_size (IntPtr fsd);

		public static UniversalWidgetToolkit.Drawing.Font gtk_font_dialog_get_font(IntPtr handle, bool useLegacyFunctionality = false)
		{
			IntPtr hFontFamily = IntPtr.Zero;
			IntPtr hFontFace = IntPtr.Zero;
			int fontSize = 0;

			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3 && !useLegacyFunctionality) {
				hFontFamily = gtk_font_chooser_get_font_family (handle);
				hFontFace = gtk_font_chooser_get_font_face (handle);
				fontSize = gtk_font_chooser_get_font_size (handle);
			} else {
				IntPtr hsel = gtk_font_selection_dialog_get_font_selection (handle);
				hFontFamily = gtk_font_selection_get_family (hsel);
				hFontFace = gtk_font_selection_get_face (hsel);
				fontSize = gtk_font_selection_get_size (hsel);
			}

			string strFontFamily = Internal.Pango.Methods.pango_font_family_get_name (hFontFamily);
			string strFontFace = Internal.Pango.Methods.pango_font_face_get_face_name (hFontFace);

			bool isBold = false, isItalic = false;
			string[] strFontFaceParts = strFontFace.Split (new char[] { ' ' });
			foreach (string strFontFacePart in strFontFaceParts) {
				switch (strFontFacePart.ToLower ()) {
					case "bold":
					{
						isBold = true;
						break;
					}
					case "italic":
					{
						isItalic = true;
						break;
					}
				}
			}

			UniversalWidgetToolkit.Drawing.Font font = new UniversalWidgetToolkit.Drawing.Font ();
			if (isBold) {
				font.Weight = UniversalWidgetToolkit.Drawing.FontWeights.Bold;
			}
			font.Italic = isItalic;
			font.FamilyName = strFontFamily;
			font.FaceName = strFontFace;
			font.Size = ((double)fontSize / 1024);
			return font;
		}
		#endregion

		#region About Dialog
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_about_dialog_new ();
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_about_dialog_get_program_name (IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_about_dialog_set_program_name (IntPtr handle, string value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_about_dialog_get_version(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_about_dialog_set_version(IntPtr handle, string value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_about_dialog_get_copyright(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_about_dialog_set_copyright(IntPtr handle, string value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_about_dialog_get_comments(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_about_dialog_set_comments(IntPtr handle, string value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_about_dialog_get_license(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_about_dialog_set_license(IntPtr handle, string value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_about_dialog_set_license_type(IntPtr handle, Internal.GTK.Constants.GtkLicense value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_about_dialog_get_website(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_about_dialog_set_website(IntPtr handle, string value);
		#endregion

		#endregion
	}
}

