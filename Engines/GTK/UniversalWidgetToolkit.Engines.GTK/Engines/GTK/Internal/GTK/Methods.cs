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
		public static extern void gtk_widget_set_hexpand(IntPtr /*GtkWidget*/ widget, bool expand);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_vexpand(IntPtr /*GtkWidget*/ widget, bool expand);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_widget_translate_coordinates(IntPtr /*GtkWidget*/ src_widget, IntPtr /*GtkWidget*/ dest_widget, int src_x, int src_y, ref int dest_x, ref int dest_y);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_init_check(ref int argc, ref string[] args);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_main();

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_main_quit();

		#region Application
		[DllImport(LIBRARY_FILENAME_V3, EntryPoint = "gtk_application_new")]
		public static extern IntPtr gtk_application_new_v3(string application_id, Internal.GIO.Constants.GApplicationFlags flags);

		public static IntPtr gtk_application_new(string application_id, Internal.GIO.Constants.GApplicationFlags flags)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V2)
			{
				return IntPtr.Zero;
			}
			else
			{
				return gtk_application_new_v3(application_id, flags);
			}
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_application_add_window(IntPtr /*GtkApplication*/ application, IntPtr /*GtkWindow*/ window);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_toolbar_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_toolbar_set_show_arrow(IntPtr /*GtkToolbar*/ toolbar, bool show_arrow);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_toolbar_insert(IntPtr /*GtkToolbar*/ toolbar, IntPtr /*GtkToolItem*/ item, int pos);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_tool_button_new(IntPtr /*GtkWidget*/ icon_widget, string label);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_tool_button_get_label(IntPtr /*GtkToolButton*/ button);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tool_button_set_label(IntPtr /*GtkToolButton*/ button, string label);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_toggle_tool_button_new();

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GMenuModel*/ gtk_application_get_menubar(IntPtr /*GtkApplication*/ application);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_application_set_menubar(IntPtr /*GtkApplication*/ application, IntPtr /*GMenuModel*/ menubar);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GMenuModel*/ gtk_application_get_app_menu(IntPtr /*GtkApplication*/ application);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_application_set_app_menu(IntPtr /*GtkApplication*/ application, IntPtr /*GMenuModel*/ menu);

		#endregion

		#region Widget
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_widget_get_type();

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_show(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_show_all(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_hide(IntPtr widget);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_queue_draw(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_queue_draw_area(IntPtr widget, int x, int y, int width, int height);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_size_request(IntPtr widget, int width, int height);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_get_size_request(IntPtr widget, out int width, out int height);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_sensitive(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_widget_is_sensitive(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_sensitive(IntPtr widget, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_can_focus(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_can_focus(IntPtr widget, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_can_default(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_can_default(IntPtr widget, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_widget_get_receives_default(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_receives_default(IntPtr widget, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_grab_default(IntPtr widget);

		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkAlign gtk_widget_set_halign(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_halign(IntPtr widget, Constants.GtkAlign value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkAlign gtk_widget_set_valign(IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_set_valign(IntPtr widget, Constants.GtkAlign value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_widget_destroy(IntPtr widget);
		#endregion

		#region Editable
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_editable_get_editable(IntPtr /*GtkEntry*/ entry);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_editable_set_editable(IntPtr /*GtkEntry*/ entry, bool value);

		#region Entry
		[DllImport(LIBRARY_FILENAME)]
		public static extern GType gtk_entry_get_type();

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_entry_new();

		[DllImport(LIBRARY_FILENAME)]
		public static extern ushort gtk_entry_get_text_length(IntPtr /*GtkEntry*/ entry);

		[DllImport(LIBRARY_FILENAME, EntryPoint = "gtk_entry_get_text")]
		private static extern IntPtr gtk_entry_get_text_ptr(IntPtr /*GtkEntry*/ entry);
		public static string gtk_entry_get_text(IntPtr /*GtkEntry*/ entry)
		{
			IntPtr ptr = gtk_entry_get_text_ptr(entry);
			string val = Marshal.PtrToStringAuto(ptr);
			return val;
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_entry_set_text(IntPtr /*GtkEntry*/ entry, string value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_entry_get_visibility(IntPtr /*GtkEntry*/ entry);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_entry_set_visibility(IntPtr /*GtkEntry*/ entry, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_entry_get_max_length(IntPtr /*GtkEntry*/ entry);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_entry_set_max_length(IntPtr /*GtkEntry*/ entry, int value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_entry_get_width_chars(IntPtr /*GtkEntry*/ entry);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_decorated(IntPtr /*GtkWindow*/ handle, bool decorated);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_window_has_toplevel_focus(IntPtr /*GtkWindow*/ window);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GList*/ gtk_window_list_toplevels();

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_entry_set_width_chars(IntPtr /*GtkEntry*/ entry, int value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_entry_get_activates_default(IntPtr /*GtkEntry*/ entry);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_entry_set_activates_default(IntPtr /*GtkEntry*/ entry, bool value);
		#endregion

		#region Text Tag Table
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_text_tag_table_new();
		#endregion
		#region Text Buffer
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_text_buffer_new(IntPtr /*GtkTextTagTable*/ table);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_text_buffer_set_text(IntPtr /*GtkTextBuffer*/ buffer, string text, int len);
		#endregion

		#region Text View
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_text_view_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_text_view_get_buffer(IntPtr /*GtkTextView*/ text_view);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_text_view_set_buffer(IntPtr /*GtkTextView*/ text_view, IntPtr /*GtkTextBuffer*/ buffer);
		#endregion

		#region Cell Renderer

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_cell_renderer_text_new();
		#endregion


		#region Tree View
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_tree_view_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_tree_view_new_with_model(IntPtr /*GtkTreeModel*/ model);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_view_set_headers_visible(IntPtr /*GtkTreeView*/ tree_view, bool headers_visible);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_view_set_headers_clickable(IntPtr /*GtkTreeView*/ tree_view, bool setting);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkTreeSelection*/ gtk_tree_view_get_selection(IntPtr /*GtkTreeView*/ tree_view);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_view_insert_column_with_attributes(IntPtr handle, int position, string title, IntPtr /*GtkCellRenderer*/ cell, string attributeName, int columnIndexForAttributeValue, IntPtr setThisToZero);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkTreeModel*/ gtk_tree_view_get_model(IntPtr /*GtkTreeView*/ tree_view);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_view_set_model(IntPtr /*GtkTreeView*/ tree_view, IntPtr /*GtkTreeModel*/ model);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_tree_model_get_iter(IntPtr /*GtkTreeModel*/ tree_model, ref Structures.GtkTreeIter iter, IntPtr /*GtkTreePath*/ path);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_tree_model_get_iter_first(IntPtr /*GtkTreeModel*/ tree_model, ref Structures.GtkTreeIter iter);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_tree_model_iter_next(IntPtr /*GtkTreeModel*/ tree_model, ref Structures.GtkTreeIter iter);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_tree_model_iter_previous(IntPtr /*GtkTreeModel*/ tree_model, ref Structures.GtkTreeIter iter);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_view_enable_model_drag_source(IntPtr /*GtkWidget*/ widget, GDK.Constants.GdkModifierType start_button_mask, Structures.GtkTargetEntry[] targets, int n_targets, GDK.Constants.GdkDragAction actions);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_view_enable_model_drag_dest(IntPtr /*GtkWidget*/ widget, Structures.GtkTargetEntry[] targets, int n_targets, GDK.Constants.GdkDragAction actions);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_tree_store_newv(int columns, IntPtr[] columnTypes);

		/// <summary>
		/// Removes all rows from the specified GtkTreeStore.
		/// </summary>
		/// <param name="tree_store">The GtkTreeStore to clear.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_store_clear(IntPtr /*GtkTreeStore*/ tree_store);

		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_tree_store_insert(IntPtr /*GtkTreeStore*/ tree_store, out Structures.GtkTreeIter /*GtkTreeIter*/ iter, ref Structures.GtkTreeIter parent, int position);
		[DllImport(LIBRARY_FILENAME, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_tree_store_insert(IntPtr /*GtkTreeStore*/ tree_store, out Structures.GtkTreeIter /*GtkTreeIter*/ iter, IntPtr parent, int position);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_store_insert_with_valuesv(IntPtr /*GtkTreeStore*/ tree_store, out Structures.GtkTreeIter /*GtkTreeIter*/ iter, ref Structures.GtkTreeIter parent, int position, ref int columns, object[] values, int n_values);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_store_append(IntPtr /*GtkTreeStore*/ tree_store, out Structures.GtkTreeIter iter, ref Structures.GtkTreeIter parent);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_store_append(IntPtr /*GtkTreeStore*/ tree_store, out Structures.GtkTreeIter iter, IntPtr parent);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_store_set_value(IntPtr /*GtkTreeStore*/ tree_store, ref Structures.GtkTreeIter /*GtkTreeIter*/ iter, int column, ref GLib.Structures.Value value);

		#endregion
		#region Icon View
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_icon_view_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkTreeModel*/ gtk_icon_view_get_model(IntPtr /*GtkIconView*/ icon_view);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_icon_view_set_model(IntPtr /*GtkIconView*/ icon_view, IntPtr /*GtkTreeModel*/ model);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GList*/ gtk_icon_view_get_selected_items(IntPtr /*GtkIconView*/ icon_view);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_icon_view_set_text_column(IntPtr /*GtkIconView*/ icon_view, int column);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_icon_view_get_text_column(IntPtr /*GtkIconView*/ icon_view);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_icon_view_set_pixbuf_column(IntPtr /*GtkIconView*/ icon_view, int column);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_icon_view_get_pixbuf_column(IntPtr /*GtkIconView*/ icon_view);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_icon_view_set_item_width(IntPtr /*GtkIconView*/ icon_view, int column);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_icon_view_get_item_width(IntPtr /*GtkIconView*/ icon_view);[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkSelectionMode gtk_icon_view_get_selection_mode(IntPtr /*GtkIconView*/ icon_view);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_icon_view_set_selection_mode(IntPtr /*GtkIconView*/ icon_view, Constants.GtkSelectionMode type);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_icon_view_enable_model_drag_source(IntPtr /*GtkWidget*/ widget, GDK.Constants.GdkModifierType start_button_mask, Structures.GtkTargetEntry[] targets, int n_targets, GDK.Constants.GdkDragAction actions);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_icon_view_enable_model_drag_dest(IntPtr /*GtkWidget*/ widget, Structures.GtkTargetEntry[] targets, int n_targets, GDK.Constants.GdkDragAction actions);
		#endregion

		#region Tree Selection
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_tree_selection_get_selected(IntPtr /*GtkTreeSelection*/ selection, ref IntPtr /*GtkTreeModel*/ model, Structures.GtkTreeIter[] iter);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GList*/ gtk_tree_selection_get_selected_rows(IntPtr /*GtkTreeSelection*/ selection, ref IntPtr /*GtkTreeModel*/ model);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_tree_selection_count_selected_rows(IntPtr /*GtkTreeSelection*/ selection);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_selection_select_all(IntPtr /*GtkTreeSelection*/ selection);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_selection_unselect_all(IntPtr /*GtkTreeSelection*/ selection);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_tree_selection_set_mode(IntPtr /*GtkTreeSelection*/ selection, Constants.GtkSelectionMode type);
		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkSelectionMode gtk_tree_selection_get_mode(IntPtr /*GtkTreeSelection*/ selection);
		#endregion
		#region Tree Path
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_tree_path_to_string(IntPtr /*GtkTreePath*/ path);
		#endregion
		#endregion

		#region Container
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_container_add(IntPtr container, IntPtr widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_container_get_focus_child(IntPtr container);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_container_get_children(IntPtr /*GtkContainer*/ container);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_container_set_focus_child(IntPtr container, IntPtr widget);
		#endregion

		#region Window
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_window_new(Constants.GtkWindowType windowType);

		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_window_get_title(IntPtr window);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_title(IntPtr window, string title);

		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_window_get_icon_name(IntPtr window);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_icon_name(IntPtr window, string icon_name);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_focus_on_map(IntPtr window, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_window_get_hide_titlebar_when_maximized(IntPtr window);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_hide_titlebar_when_maximized(IntPtr window, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_add_accel_group(IntPtr window, IntPtr accel_group);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_default_size(IntPtr /*GtkWindow*/ window, int width, int height);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_get_default_size(IntPtr /*GtkWindow*/ window, out int width, out int height);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_resize(IntPtr /*GtkWindow*/ window, int width, int height);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_move(IntPtr /*GtkWindow*/ window, int x, int y);

		/// <summary>
		/// Gtks the window set titlebar.
		/// </summary>
		/// <param name="window">Window.</param>
		/// <param name="titlebar">Titlebar.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_window_set_titlebar(IntPtr /*GtkWindow*/ window, IntPtr /*GtkWidget*/ titlebar);
		#endregion

		#region Header Bar
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_header_bar_new();

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_header_bar_set_title(IntPtr /*GtkHeaderBar*/ bar, string title);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_header_bar_set_subtitle(IntPtr /*GtkHeaderBar*/ bar, string subtitle);

		#endregion

		#region Box
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_hbox_new(bool homogenous, int spacing);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_vbox_new(bool homogenous, int spacing);

		[DllImport(LIBRARY_FILENAME_V3, EntryPoint = "gtk_box_new")]
		private static extern IntPtr gtk_box_new_v3(Constants.GtkOrientation orientation, int spacing);

		public static IntPtr gtk_box_new(Constants.GtkOrientation orientation, int spacing = 0)
		{
			return gtk_box_new(orientation, true, spacing);
		}
		public static IntPtr gtk_box_new(Constants.GtkOrientation orientation, bool homogenous = true, int spacing = 0, bool useGtk2API = false)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V2 || useGtk2API)
			{
				switch (orientation)
				{
					case Constants.GtkOrientation.Horizontal:
						{
							return gtk_hbox_new(homogenous, spacing);
						}
					case Constants.GtkOrientation.Vertical:
						{
							return gtk_vbox_new(homogenous, spacing);
						}
				}
			}
			else if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3)
			{
				return gtk_box_new_v3(orientation, spacing);
			}
			return IntPtr.Zero;
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_box_get_homogeneous(IntPtr box);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_set_homogeneous(IntPtr box, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_box_get_spacing(IntPtr box);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_set_spacing(IntPtr box, int value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_pack_start(IntPtr box, IntPtr child, bool expand, bool fill, int padding);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_pack_end(IntPtr box, IntPtr child, bool expand, bool fill, int padding);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_box_set_child_packing(IntPtr /*GtkBox*/ box, IntPtr /*GtkWidget*/ child, bool expand, bool fill, int padding, Constants.GtkPackType pack_type);
		#endregion

		#region Fixed
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_fixed_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_fixed_put(IntPtr /*GtkFixed*/ _fixed, IntPtr /*GtkWidget*/ widget, int x, int y);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_fixed_move(IntPtr /*GtkFixed*/ _fixed, IntPtr /*GtkWidget*/ widget, int x, int y);
		#endregion

		#region Grid
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_grid_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint gtk_grid_get_row_spacing(IntPtr /*GtkGrid*/ grid);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_grid_set_row_spacing(IntPtr /*GtkGrid*/ grid, uint spacing);
		[DllImport(LIBRARY_FILENAME)]
		public static extern uint gtk_grid_get_column_spacing(IntPtr /*GtkGrid*/ grid);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_grid_set_column_spacing(IntPtr /*GtkGrid*/ grid, uint spacing);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_grid_attach(IntPtr /*GtkGrid*/ grid, IntPtr /*GtkWidget*/ widget, int left, int top, int width, int height);
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
		public static extern IntPtr /*GtkAccelGroup*/ gtk_menu_get_accel_group(IntPtr menu);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_set_accel_group(IntPtr menu, IntPtr /*GtkAccelGroup*/ accel_group);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_menu_get_accel_path(IntPtr menu);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_set_accel_path(IntPtr menu, string accel_path);
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
		public static extern bool gtk_menu_item_get_right_justified(IntPtr menu_item);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_item_set_right_justified(IntPtr menu_item, bool value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_menu_item_get_accel_path(IntPtr menu_item);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_menu_item_set_accel_path(IntPtr menu_item, string accel_path);
		#endregion

		#region Separator
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_separator_new(Constants.GtkOrientation orientation);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_separator_menu_item_new();
		#endregion

		#region Label
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_label_new(string text);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_label_new_with_mnemonic(string text);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_label_set_line_wrap(IntPtr /*GtkLabel*/ label, bool wrap);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*PangoAttrList*/ gtk_label_get_attributes(IntPtr /*GtkLabel*/ label);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_label_set_attributes(IntPtr /*GtkLabel*/ label, IntPtr /*PangoAttrList*/ attrs);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_label_get_text(IntPtr /*GtkLabel*/ label);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_label_set_text(IntPtr /*GtkLabel*/ label, IntPtr value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_label_get_label(IntPtr /*GtkLabel*/ label);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_label_set_label(IntPtr /*GtkLabel*/ label, IntPtr value);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_label_set_justify(IntPtr /*GtkLabel*/ label, Constants.GtkJustification jtype);
		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkJustification gtk_label_get_justify(IntPtr /*GtkLabel*/ label);
		#endregion

		#region Button
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_button_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_button_get_label(IntPtr button);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_label(IntPtr button, string label);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_button_get_always_show_image(IntPtr button);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_always_show_image(IntPtr button, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_button_get_use_underline(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_use_underline(IntPtr handle, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_button_get_use_stock(IntPtr button);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_use_stock(IntPtr button, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_button_get_focus_on_click(IntPtr button);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_focus_on_click(IntPtr button, bool value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_button_set_relief(IntPtr button, Constants.GtkReliefStyle value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkReliefStyle gtk_button_get_relief(IntPtr button);
		#endregion

		#region Notebook
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_notebook_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_notebook_append_page(IntPtr hNotebook, IntPtr hChild, IntPtr hTabLabel);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_notebook_get_n_pages(IntPtr hNotebook);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_notebook_page_num(IntPtr hNotebook, IntPtr hChild);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_notebook_remove_page(IntPtr hNotebook, int page_num);
		#endregion

		#region Paned
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_hpaned_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_vpaned_new();

		[DllImport(LIBRARY_FILENAME_V3, EntryPoint = "gtk_paned_new")]
		private static extern IntPtr gtk_paned_new_v3(Constants.GtkOrientation orientation);

		public static IntPtr gtk_paned_new(Constants.GtkOrientation orientation, bool useGtk2API = false)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V2 || useGtk2API)
			{
				switch (orientation)
				{
					case Constants.GtkOrientation.Horizontal:
						{
							return gtk_hpaned_new();
						}
					case Constants.GtkOrientation.Vertical:
						{
							return gtk_vpaned_new();
						}
				}
			}
			else if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3)
			{
				return gtk_paned_new_v3(orientation);
			}
			return IntPtr.Zero;
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_paned_add1(IntPtr hPaned, IntPtr hChild);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_paned_add2(IntPtr hPaned, IntPtr hChild);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_paned_pack1(IntPtr hPaned, IntPtr hChild, bool resize, bool shrink);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_paned_pack2(IntPtr hPaned, IntPtr hChild, bool resize, bool shrink);
		#endregion

		#region Accel Map
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_accel_map_add_entry(string accel_path, uint accel_key, Internal.GDK.Constants.GdkModifierType accel_mods);
		#endregion

		#region Accel Group
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_accel_group_new();
		#endregion

		#region File Chooser
		// Preview widget
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_preview_widget(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkWidget*/ preview_widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_file_chooser_get_preview_widget(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_preview_widget_active(IntPtr /*GtkFileChooser*/ chooser, bool active);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_file_chooser_get_preview_widget_active(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_use_preview_label(IntPtr /*GtkFileChooser*/ chooser, bool use_label);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_file_chooser_get_use_preview_label(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_file_chooser_get_preview_filename(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_file_chooser_get_preview_uri(IntPtr /*GtkFileChooser*/ chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GFile*/ gtk_file_chooser_get_preview_file(IntPtr /*GtkFileChooser*/ chooser);

		// Extra widget
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_extra_widget(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkWidget*/ extra_widget);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_file_chooser_get_extra_widget(IntPtr /*GtkFileChooser*/ chooser);

		// List of user selectable filters
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_add_filter(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkFileFilter*/ filter);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_remove_filter(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkFileFilter*/ filter);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GSList*/ gtk_file_chooser_list_filters(IntPtr /*GtkFileChooser*/ chooser);

		// Current filter
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_filter(IntPtr /*GtkFileChooser*/ chooser, IntPtr /*GtkFileFilter*/ filter);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkFileFilter*/ gtk_file_chooser_get_filter(IntPtr /*GtkFileChooser*/ chooser);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_file_chooser_get_filenames(IntPtr chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_file_chooser_get_select_multiple(IntPtr chooser);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_chooser_set_select_multiple(IntPtr chooser, bool value);
		#endregion

		#region File Filter
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkFileFilter*/ gtk_file_filter_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_filter_set_name(IntPtr /*GtkFileFilter*/ filter, string name);
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_file_filter_get_name(IntPtr /*GtkFileFilter*/ filter);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_filter_add_mime_type(IntPtr /*GtkFileFilter*/ filter, string mime_type);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_filter_add_pattern(IntPtr /*GtkFileFilter*/ filter, string pattern);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_filter_add_pixbuf_formats(IntPtr /*GtkFileFilter*/ filter);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_file_filter_add_custom(IntPtr /*GtkFileFilter*/ filter, Constants.GtkFileFilterFlags needed, Delegates.GtkFileFilterFunc func, IntPtr data, GLib.Delegates.GDestroyNotify notify);

		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkFileFilterFlags gtk_file_filter_get_needed(IntPtr /*GtkFileFilter*/ filter);
		[DllImport(LIBRARY_FILENAME)]
		public static extern bool gtk_file_filter_filter(IntPtr /*GtkFileFilter*/ filter, ref Structures.GtkFileFilterInfo filter_info);
		#endregion

		#region Print Operation
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_print_operation_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern Constants.GtkPrintOperationResult gtk_print_operation_run(IntPtr /*GtkPrintOperation*/ op, Constants.GtkPrintOperationAction action, IntPtr /*GtkWindow*/ parent, IntPtr /*GError***/ error);
		#endregion

		#region Dialog
		[DllImport(LIBRARY_FILENAME), Obsolete("Use gtk_dialog_new_with_buttons to properly set parent window")]
		public static extern IntPtr gtk_dialog_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_dialog_new_with_buttons(string title, IntPtr hParent, Constants.GtkDialogFlags flags, string first_button_text);
		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_dialog_run(IntPtr handle);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_dialog_response(IntPtr /*GtkDialog*/ dialog, Constants.GtkResponseType response_id);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_dialog_response(IntPtr /*GtkDialog*/ dialog, int response_id);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_dialog_get_content_area(IntPtr /*GtkDialog*/ dialog);

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_dialog_add_button(IntPtr /*GtkDialog*/ dialog, string button_text, int response_id);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_dialog_add_button(IntPtr /*GtkDialog*/ dialog, string button_text, Constants.GtkResponseType response_id);

		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_dialog_add_action_widget(IntPtr /*GtkDialog*/ dialog, IntPtr /*GtkWidget*/ child, int response_id);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_dialog_add_action_widget(IntPtr /*GtkDialog*/ dialog, IntPtr /*GtkWidget*/ child, Constants.GtkResponseType response_id);

		[DllImport(LIBRARY_FILENAME)]
		public static extern int gtk_dialog_get_default_response(IntPtr /*GtkDialog*/ dialog);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_dialog_set_default_response(IntPtr /*GtkDialog*/ dialog, int value);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_dialog_set_default_response(IntPtr /*GtkDialog*/ dialog, Constants.GtkResponseType value);

		#region Message Dialog
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_message_dialog_new(IntPtr parentHandle, Constants.GtkDialogFlags flags, Constants.GtkMessageType type, Constants.GtkButtonsType buttons, string content);
		#endregion

		#region File Chooser Dialog
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_file_chooser_dialog_new(string title, IntPtr parentHandle, Constants.GtkFileChooserAction action, string first_button_text = null, string first_button_response = null);
		#endregion

		#region Color Chooser Dialog
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_color_selection_dialog_new(string title);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parentHandle);

		public static IntPtr gtk_color_dialog_new(string title, IntPtr parentHandle, bool useLegacyFunctionality = false)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3 && !useLegacyFunctionality)
			{
				return gtk_color_chooser_dialog_new(title, parentHandle);
			}
			else
			{
				return gtk_color_selection_dialog_new(title);
			}
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_color_chooser_get_rgb(IntPtr /*GtkColorChooser*/ chooser, out Internal.GDK.Structures.GdkRGBA color);

		// only in gtk-3
		[DllImport(LIBRARY_FILENAME, EntryPoint = "gtk_color_chooser_get_rgba")]
		private static extern IntPtr gtk_color_chooser_get_rgba_internal(IntPtr /*GtkColorChooser*/ chooser, out Internal.GDK.Structures.GdkRGBA color);

		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_color_selection_dialog_get_color_selection(IntPtr /*GtkColorSelectionDialog*/ chooser);

		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_color_selection_get_current_color(IntPtr /*GtkColorSelection*/ colorsel, out Internal.GDK.Structures.GdkColor color);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_color_selection_get_previous_color(IntPtr /*GtkColorSelection*/ colorsel, out Internal.GDK.Structures.GdkColor color);

		public static IntPtr gtk_color_chooser_get_rgba(IntPtr /*GtkColorChooser*/ chooser, out Internal.GDK.Structures.GdkRGBA color)
		{
			IntPtr retval = IntPtr.Zero;

			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3)
			{
				retval = gtk_color_chooser_get_rgba_internal(chooser, out color);
				return retval;
			}
			else if (LIBRARY_FILENAME == LIBRARY_FILENAME_V2)
			{
				IntPtr colorsel = gtk_color_selection_dialog_get_color_selection(chooser);
				Internal.GDK.Structures.GdkColor color1 = new Internal.GDK.Structures.GdkColor();
				gtk_color_selection_get_current_color(colorsel, out color1);

				// this seems weird. is this correct? dividing TWICE???
				Internal.GDK.Structures.GdkRGBA color2 = new Internal.GDK.Structures.GdkRGBA();
				color2.red = ((double)color1.red / 255) / 255;
				color2.green = ((double)color1.green / 255) / 255;
				color2.blue = ((double)color1.blue / 255) / 255;
				color2.alpha = 1.0;
				color = color2;
				return IntPtr.Zero;
			}

			Internal.GDK.Structures.GdkRGBA empty = new Internal.GDK.Structures.GdkRGBA();
			color = empty;
			return IntPtr.Zero;
		}

		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_app_chooser_dialog_new_for_content_type(IntPtr /*GtkWindow*/ parent, Constants.GtkDialogFlags modal, string contentType);
		#endregion

		#region Font Dialog
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parentHandle);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_selection_dialog_new(string title);

		public static IntPtr gtk_font_dialog_new(string title, IntPtr parentHandle, bool useLegacyFunctionality = false)
		{
			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3 && !useLegacyFunctionality)
			{
				return gtk_font_chooser_dialog_new(title, parentHandle);
			}
			else
			{
				return gtk_font_selection_dialog_new(title);
			}
		}

		[DllImport(LIBRARY_FILENAME)]
		private static extern string gtk_font_chooser_get_font(IntPtr /*GtkFontSelectionDialog*/ fsd);

		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_selection_dialog_get_font_selection(IntPtr /*GtkFontSelectionDialog*/ fsd);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_selection_get_family(IntPtr /*GtkFontSelection*/ fsd);
		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_selection_get_face(IntPtr /*GtkFontSelection*/ fsd);

		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_chooser_get_font_family(IntPtr fsd);

		[DllImport(LIBRARY_FILENAME)]
		private static extern IntPtr gtk_font_chooser_get_font_face(IntPtr fsd);

		[DllImport(LIBRARY_FILENAME)]
		private static extern int gtk_font_selection_get_size(IntPtr fsd);
		[DllImport(LIBRARY_FILENAME)]
		private static extern int gtk_font_chooser_get_font_size(IntPtr fsd);

		public static UniversalWidgetToolkit.Drawing.Font gtk_font_dialog_get_font(IntPtr handle, bool useLegacyFunctionality = false)
		{
			IntPtr hFontFamily = IntPtr.Zero;
			IntPtr hFontFace = IntPtr.Zero;
			int fontSize = 0;

			if (LIBRARY_FILENAME == LIBRARY_FILENAME_V3 && !useLegacyFunctionality)
			{
				hFontFamily = gtk_font_chooser_get_font_family(handle);
				hFontFace = gtk_font_chooser_get_font_face(handle);
				fontSize = gtk_font_chooser_get_font_size(handle);
			}
			else
			{
				IntPtr hsel = gtk_font_selection_dialog_get_font_selection(handle);
				hFontFamily = gtk_font_selection_get_family(hsel);
				hFontFace = gtk_font_selection_get_face(hsel);
				fontSize = gtk_font_selection_get_size(hsel);
			}

			string strFontFamily = Internal.Pango.Methods.pango_font_family_get_name(hFontFamily);
			string strFontFace = Internal.Pango.Methods.pango_font_face_get_face_name(hFontFace);

			bool isBold = false, isItalic = false;
			string[] strFontFaceParts = strFontFace.Split(new char[] { ' ' });
			foreach (string strFontFacePart in strFontFaceParts)
			{
				switch (strFontFacePart.ToLower())
				{
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

			UniversalWidgetToolkit.Drawing.Font font = new UniversalWidgetToolkit.Drawing.Font();
			if (isBold)
			{
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
		public static extern IntPtr gtk_about_dialog_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_about_dialog_get_program_name(IntPtr handle);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_about_dialog_set_program_name(IntPtr handle, string value);
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

		#region Check Button
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_check_button_new_with_label(string text);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_check_button_new_with_mnemonic(string text);
		#endregion

		#region Drawing Area
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_drawing_area_new();
		#endregion

		#region Scrolled Window
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_scrolled_window_new(IntPtr /*GtkAdjustment*/ hadjustment, IntPtr /*GtkAdjustment*/ vadjustment);
		#endregion

		#region Adjustment
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkAdjustment*/ gtk_adjustment_new(double value, double lower, double upper, double step_increment, double page_increment, double page_size);
		#endregion

		#region Drag-n-Drop
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_drag_source_set(IntPtr /*GtkWidget*/ widget, GDK.Constants.GdkModifierType start_button_mask, Structures.GtkTargetEntry[] targets, int n_targets, GDK.Constants.GdkDragAction actions);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_drag_dest_set(IntPtr /*GtkWidget*/ widget, Constants.GtkDestDefaults flags, Structures.GtkTargetEntry[] targets, int n_targets, GDK.Constants.GdkDragAction actions);
		#endregion

		#region GtkImage
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_image_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_image_new_from_file(string filename);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr gtk_image_new_from_icon_name(string iconName);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_image_set_pixel_size(IntPtr /*GtkImage*/ image, int pixel_size);
		#endregion

		#region GtkGlArea
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GtkWidget*/ gtk_gl_area_new();
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_gl_area_make_current(IntPtr /*GtkWidget*/ area);
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_gl_area_attach_buffers(IntPtr /*GtkWidget*/ area);
		[DllImport(LIBRARY_FILENAME)]
		public static extern IntPtr /*GError*/ gtk_gl_area_get_error(IntPtr /*GtkWidget*/ area);
		#endregion

		#region GtkSelection
		/// <summary>
		/// </summary>
		/// <param name="selection_data">a pointer to a GtkSelectionData</param>
		/// <param name="type">the type of selection data</param>
		/// <param name="format">format (number of bits in a unit)</param>
		/// <param name="data">pointer to the data (will be copied)</param>
		/// <param name="length">length of the data</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_selection_data_set(IntPtr /*GtkSelectionData*/ selection_data, IntPtr /*GdkAtom*/ type, int format, byte[] data, int length);
		/// <summary>
		/// Sets the contents of the selection from a UTF-8 encoded string. The string is converted to the form determined by selection_data->target.
		/// </summary>
		/// <param name="selection_data">a GtkSelectionData</param>
		/// <param name="str">a UTF-8 string</param>
		/// <param name="len">the length of str , or -1 if str is nul-terminated.</param>
		[DllImport(LIBRARY_FILENAME)]
		public static extern void gtk_selection_data_set_text(IntPtr /*GtkSelectionData*/ selection_data, string str, int len);
		/// <summary>
		/// Gets the contents of the selection data as a UTF-8 string.
		/// </summary>
		/// <returns>if the selection data contained a recognized text type and it could be converted to UTF-8, a newly allocated string containing the converted text, otherwise NULL. If the result is non-NULL it must be freed with g_free().</returns>
		[DllImport(LIBRARY_FILENAME)]
		public static extern string gtk_selection_data_get_text (IntPtr /*GtkSelectionData*/ selection_data);
		#endregion

	}
}

