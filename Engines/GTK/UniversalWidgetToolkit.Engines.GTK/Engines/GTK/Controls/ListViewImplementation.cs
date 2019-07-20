using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Engines.GTK.Internal.GLib;
using UniversalWidgetToolkit.Engines.GTK.Internal.GTK;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[NativeImplementation(typeof(ListView))]
	public class ListViewImplementation : GTKNativeImplementation, UniversalWidgetToolkit.Controls.Native.IListViewNativeImplementation
	{
		public ListViewImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		public void SetSelectionMode(SelectionMode value)
		{
			IntPtr hTreeSelection = Internal.GTK.Methods.gtk_tree_view_get_selection((Handle as GTKNativeControl).GetNamedHandle("TreeView"));
			switch (value)
			{
				case SelectionMode.None:  Internal.GTK.Methods.gtk_tree_selection_set_mode(hTreeSelection, Internal.GTK.Constants.GtkSelectionMode.None); break;
				case SelectionMode.Single:  Internal.GTK.Methods.gtk_tree_selection_set_mode(hTreeSelection, Internal.GTK.Constants.GtkSelectionMode.Single); break;
				case SelectionMode.Browse:  Internal.GTK.Methods.gtk_tree_selection_set_mode(hTreeSelection, Internal.GTK.Constants.GtkSelectionMode.Browse); break;
				case SelectionMode.Multiple:  Internal.GTK.Methods.gtk_tree_selection_set_mode(hTreeSelection, Internal.GTK.Constants.GtkSelectionMode.Multiple); break;
			}
		}
		public SelectionMode GetSelectionMode()
		{
			IntPtr hTreeSelection = Internal.GTK.Methods.gtk_tree_view_get_selection((Handle as GTKNativeControl).GetNamedHandle("TreeView"));
			Internal.GTK.Constants.GtkSelectionMode mode = Internal.GTK.Methods.gtk_tree_selection_get_mode(hTreeSelection);
			switch (mode)
			{
				case Internal.GTK.Constants.GtkSelectionMode.None: return SelectionMode.None;
				case Internal.GTK.Constants.GtkSelectionMode.Single: return SelectionMode.Single;
				case Internal.GTK.Constants.GtkSelectionMode.Browse: return SelectionMode.Browse;
				case Internal.GTK.Constants.GtkSelectionMode.Multiple: return SelectionMode.Multiple;
			}
			throw new InvalidOperationException();
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			ListView tv = (control as ListView);
			IntPtr handle = IntPtr.Zero;

			// TODO: fix GtkTreeView implementation
			// Internal.GTK.Methods.gtk_tree_store_insert_with_valuesv(hTreeStore, ref hIterFirst, IntPtr.Zero, 0, ref columns, values, values.Length);
			tv.SelectedRows.ItemRequested += SelectedRows_ItemRequested;
			tv.SelectedRows.Cleared += SelectedRows_Cleared;

			List<IntPtr> listColumnTypes = new List<IntPtr>();
			if (tv.Model != null)
			{
				foreach (TreeModelColumn c in tv.Model.Columns)
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

			IntPtr hTreeStore = Internal.GTK.Methods.gtk_tree_store_newv(columnTypes.Length, columnTypes);
			if (hTreeStore != IntPtr.Zero)
			{
				if (tv.Model != null)
					RegisterTreeModel(tv.Model, hTreeStore);
			}

			Internal.GTK.Structures.GtkTreeIter hIter = new Internal.GTK.Structures.GtkTreeIter();

			if (tv.Model is DefaultTreeModel)
			{
				DefaultTreeModel tm = (tv.Model as DefaultTreeModel);
				foreach (TreeModelRow row in tm.Rows)
				{
					RecursiveTreeStoreInsertRow(tm, row, hTreeStore, out hIter, null, tm.Rows.Count - 1);
				}
			}

			switch (tv.Mode)
			{
				case ListViewMode.Detail:
				{
					handle = Internal.GTK.Methods.gtk_tree_view_new();

					Internal.GTK.Methods.gtk_tree_view_set_headers_visible(handle, (tv.HeaderStyle != ColumnHeaderStyle.None));
					Internal.GTK.Methods.gtk_tree_view_set_headers_clickable(handle, (tv.HeaderStyle == ColumnHeaderStyle.Clickable));

					foreach (ListViewColumn tvc in tv.Columns)
					{
						TreeModelColumn c = tvc.Column;
						IntPtr renderer = IntPtr.Zero;

						if (tvc is ListViewColumnText)
						{
							renderer = Internal.GTK.Methods.gtk_cell_renderer_text_new();
						}
						if (renderer == IntPtr.Zero) continue;
						
						if (tv.Model != null)
						{
							int columnIndex = tv.Model.Columns.IndexOf(tvc.Column);
							Internal.GTK.Methods.gtk_tree_view_insert_column_with_attributes(handle, -1, tvc.Title, renderer, "text", columnIndex, IntPtr.Zero);
						}
					}

					Internal.GTK.Methods.gtk_tree_view_set_model(handle, hTreeStore);
					break;
				}
				case ListViewMode.LargeIcon:
				{
					handle = Internal.GTK.Methods.gtk_icon_view_new();
					Internal.GTK.Methods.gtk_icon_view_set_model(handle, hTreeStore);

					Internal.GTK.Methods.gtk_icon_view_set_item_width(handle, 96);
					Internal.GTK.Methods.gtk_icon_view_set_text_column(handle, 0);
					break;
				}
			}

			IntPtr hHAdjustment = Internal.GTK.Methods.gtk_adjustment_new(0, 0, 100, 1, 10, 10);
			IntPtr hVAdjustment = Internal.GTK.Methods.gtk_adjustment_new(0, 0, 100, 1, 10, 10);

			IntPtr hScrolledWindow = Internal.GTK.Methods.gtk_scrolled_window_new(hHAdjustment, hVAdjustment);
			Internal.GTK.Methods.gtk_container_add(hScrolledWindow, handle);

			// connect the signals
			switch (tv.Mode)
			{
				case ListViewMode.Detail:
				{
					Internal.GObject.Methods.g_signal_connect(handle, "row_activated", (Internal.GTK.Delegates.GtkTreeViewRowActivatedFunc)gc_row_activated);
					Internal.GObject.Methods.g_signal_connect(handle, "cursor_changed", (Internal.GTK.Delegates.GtkTreeViewFunc)gc_cursor_or_selection_changed);
					break;
				}
				case ListViewMode.LargeIcon:
				{
					Internal.GObject.Methods.g_signal_connect(handle, "item_activated", (Internal.GTK.Delegates.GtkTreeViewRowActivatedFunc)gc_row_activated);
					Internal.GObject.Methods.g_signal_connect(handle, "selection_changed", (Internal.GTK.Delegates.GtkTreeViewFunc)gc_cursor_or_selection_changed);
					break;
				}
			}
			RegisterListViewHandle(tv, handle);
			
			switch (tv.Mode)
			{
				case ListViewMode.Detail:
				{
					IntPtr hTreeSelection = Internal.GTK.Methods.gtk_tree_view_get_selection(handle);
					if (hTreeSelection != IntPtr.Zero)
					{
						switch (tv.SelectionMode)
						{
							case SelectionMode.None:
							{
								Internal.GTK.Methods.gtk_tree_selection_set_mode(hTreeSelection, Internal.GTK.Constants.GtkSelectionMode.None);
								break;
							}
							case SelectionMode.Single:
							{
								Internal.GTK.Methods.gtk_tree_selection_set_mode(hTreeSelection, Internal.GTK.Constants.GtkSelectionMode.Single);
								break;
							}
							case SelectionMode.Browse:
							{
								Internal.GTK.Methods.gtk_tree_selection_set_mode(hTreeSelection, Internal.GTK.Constants.GtkSelectionMode.Browse);
								break;
							}
							case SelectionMode.Multiple:
							{
								Internal.GTK.Methods.gtk_tree_selection_set_mode(hTreeSelection, Internal.GTK.Constants.GtkSelectionMode.Multiple);
								break;
							}
						}
					}
					break;
				}
				case ListViewMode.LargeIcon:
				{
					switch (tv.SelectionMode)
					{
						case SelectionMode.None:
						{
							Internal.GTK.Methods.gtk_icon_view_set_selection_mode(handle, Internal.GTK.Constants.GtkSelectionMode.None);
							break;
						}
						case SelectionMode.Single:
						{
							Internal.GTK.Methods.gtk_icon_view_set_selection_mode(handle, Internal.GTK.Constants.GtkSelectionMode.Single);
							break;
						}
						case SelectionMode.Browse:
						{
							Internal.GTK.Methods.gtk_icon_view_set_selection_mode(handle, Internal.GTK.Constants.GtkSelectionMode.Browse);
							break;
						}
						case SelectionMode.Multiple:
						{
							Internal.GTK.Methods.gtk_icon_view_set_selection_mode(handle, Internal.GTK.Constants.GtkSelectionMode.Multiple);
							break;
						}
					}
					break;
				}
			}
			
			GTKNativeControl native = new GTKNativeControl(hScrolledWindow, handle);
			native.SetNamedHandle("TreeView", handle);
			native.SetNamedHandle("ScrolledWindow", hScrolledWindow);
			return native;
		}

		private static void SelectedRows_ItemRequested(object sender, TreeModelRowItemRequestedEventArgs e)
		{
			TreeModelRow.TreeModelSelectedRowCollection coll = (sender as TreeModelRow.TreeModelSelectedRowCollection);
			if (coll.Parent != null)
			{
				IntPtr hTreeView = GetHandleForControl(coll.Parent);
				IntPtr hTreeModel = IntPtr.Zero;
				IntPtr hListRows = IntPtr.Zero;
				int count = 0;
				if (coll.Parent.Mode == ListViewMode.Detail)
				{
					hTreeModel = Internal.GTK.Methods.gtk_tree_view_get_model(hTreeView);
					
					IntPtr hTreeSelection = Internal.GTK.Methods.gtk_tree_view_get_selection(hTreeView);
					count = Internal.GTK.Methods.gtk_tree_selection_count_selected_rows(hTreeSelection);
					hListRows = Internal.GTK.Methods.gtk_tree_selection_get_selected_rows(hTreeSelection, ref hTreeModel);
				}
				else if (coll.Parent.Mode == ListViewMode.LargeIcon)
				{
					hTreeModel = Internal.GTK.Methods.gtk_icon_view_get_model(hTreeView);
					hListRows = Internal.GTK.Methods.gtk_icon_view_get_selected_items(hTreeView);
				}

				if (hTreeModel == IntPtr.Zero || hListRows == IntPtr.Zero)
				{
					if (hTreeModel == IntPtr.Zero) Console.Error.WriteLine("uwt: gtk: tree model is null");
					if (hListRows == IntPtr.Zero) Console.Error.WriteLine("uwt: gtk: row list is null");
					return;
				}
				
				e.Count = count;
				if (count > 0 && e.Index > -1)
				{
					IntPtr hTreePath = Internal.GLib.Methods.g_list_nth_data(hListRows, (uint)e.Index);
					Internal.GTK.Structures.GtkTreeIter iter = new Internal.GTK.Structures.GtkTreeIter();
					bool ret = Internal.GTK.Methods.gtk_tree_model_get_iter(hTreeModel, ref iter, hTreePath);
					if (ret)
					{
						TreeModelRow row = _TreeModelRowForGtkTreeIter[iter];
						e.Item = row;
					}
				}
				else
				{
					e.Item = null;
				}
			}
		}
		private static void SelectedRows_Cleared(object sender, EventArgs e)
		{
			TreeModelRow.TreeModelSelectedRowCollection coll = (sender as TreeModelRow.TreeModelSelectedRowCollection);
			if (coll.Parent != null)
			{
				IntPtr hTreeView = GetHandleForControl(coll.Parent);
				if (coll.Parent.Mode == ListViewMode.Detail)
				{
					IntPtr hTreeSelection = Internal.GTK.Methods.gtk_tree_view_get_selection(hTreeView);
					Internal.GTK.Methods.gtk_tree_selection_unselect_all(hTreeSelection);
				}
				else if (coll.Parent.Mode == ListViewMode.LargeIcon)
				{
				}
			}
		}

		// TODO: figure out why _ControlsByHandle is ONLY SOMETIMES
		//       (for certain instances) but CONSISTENTLY (for the
		//       same instance, multiple times), null...
		private static Dictionary<IntPtr, ListView> _ControlsByHandle = new Dictionary<IntPtr, ListView>();
		private static Dictionary<ListView, IntPtr> _HandlesByControl = new Dictionary<ListView, IntPtr>();
		private static void RegisterListViewHandle(ListView lv, IntPtr handle)
		{
			_ControlsByHandle[handle] = lv;
			_HandlesByControl[lv] = handle;
		}
		private static IntPtr GetHandleForControl(ListView lv)
		{
			if (_HandlesByControl.ContainsKey(lv))
				return _HandlesByControl[lv];
			return IntPtr.Zero;
		}
		private static ListView GetControlByHandle(IntPtr handle)
		{
			if (_ControlsByHandle.ContainsKey(handle))
				return _ControlsByHandle[handle];
			return null;
		}

		private static void gc_cursor_or_selection_changed(IntPtr handle)
		{
			ListView ctl = GetControlByHandle(handle);
			if (ctl != null)
			{
				ctl.OnSelectionChanged(EventArgs.Empty);
			}
		}
		
		private static void gc_row_activated(IntPtr handle, IntPtr /*GtkTreePath*/ path, IntPtr /*GtkTreeViewColumn*/ column)
		{
			ListView lv = (GetControlByHandle(handle) as ListView);
			if (lv == null) return;

			IntPtr hModel = GetHandleForTreeModel(lv.Model);

			Internal.GTK.Structures.GtkTreeIter iter = new Internal.GTK.Structures.GtkTreeIter();

			Internal.GTK.Methods.gtk_tree_model_get_iter(hModel, ref iter, path);
			TreeModelRow row = _TreeModelRowForGtkTreeIter[iter];

			lv.OnRowActivated(new ListViewRowActivatedEventArgs(row));
		}

		private static Dictionary<TreeModelRow, Internal.GTK.Structures.GtkTreeIter> _GtkTreeIterForTreeModelRow = new Dictionary<TreeModelRow, Internal.GTK.Structures.GtkTreeIter>();
		private static Dictionary<Internal.GTK.Structures.GtkTreeIter, TreeModelRow> _TreeModelRowForGtkTreeIter = new Dictionary<Internal.GTK.Structures.GtkTreeIter, TreeModelRow>();

		private static Dictionary<IntPtr, TreeModel> _TreeModelForHandle = new Dictionary<IntPtr, TreeModel>();
		private static Dictionary<TreeModel, IntPtr> _HandleForTreeModel = new Dictionary<TreeModel, IntPtr>();
		private static TreeModel TreeModelFromHandle(IntPtr handle)
		{
			if (_TreeModelForHandle.ContainsKey(handle)) return _TreeModelForHandle[handle];
			return null;
		}
		private static IntPtr GetHandleForTreeModel(TreeModel tm)
		{
			if (_HandleForTreeModel.ContainsKey(tm)) return _HandleForTreeModel[tm];
			return IntPtr.Zero;
		}

		private static void RegisterTreeModel(TreeModel tm, IntPtr handle)
		{
			_TreeModelForHandle[handle] = tm;
			_HandleForTreeModel[tm] = handle;
		}
		
		/// <summary>
		/// Registers the control with the given handle as a drag source. Overridden for GtkTreeView (which in UWT is always a child of a GtkScrolledWindow).
		/// </summary>
		/// <remarks>
		/// We need to override this to handle GTK tree view DnD. Not mentioned in the docs at all. Took about a half hour to figure out... Also to properly identify
		/// control handle since the GtkTreeView in UWT is always a child of a GtkScrolledWindow.
		/// 
		/// Even still, we need to figure out how to actually make use of the dragged row. How to pass data? Also why does it only work with the primary mouse button...
		/// </remarks>
		internal override void RegisterDragSourceGTK(IntPtr hScrolledWindow, Internal.GDK.Constants.GdkModifierType modifiers, Internal.GTK.Structures.GtkTargetEntry[] targets, Internal.GDK.Constants.GdkDragAction actions)
		{
			IntPtr hTreeView = GetHTreeView(hScrolledWindow);
			Internal.GTK.Methods.gtk_tree_view_enable_model_drag_source(hTreeView, modifiers, targets, targets.Length, actions);
		}

		public void UpdateTreeModel(NativeControl handle, TreeModelChangedEventArgs e)
		{
			IntPtr hScrolledWindow = (handle as GTKNativeControl).Handle;
			IntPtr hTreeView = GetHTreeView(hScrolledWindow);

			ListView ctl = (_ControlsByHandle[hTreeView] as ListView);
			if (ctl == null) {
				Console.Error.WriteLine("UpdateTreeModel: _ControlsByHandle[" + hTreeView.ToString() + "] is null");
				return;
			}
			
			
			IntPtr hTreeModel = IntPtr.Zero;
			if (ctl.Mode == ListViewMode.Detail)
			{
				hTreeModel = Internal.GTK.Methods.gtk_tree_view_get_model(hTreeView);
			}
			else if (ctl.Mode == ListViewMode.LargeIcon)
			{
				hTreeModel = Internal.GTK.Methods.gtk_icon_view_get_model(hTreeView);
			}
			TreeModel tm = TreeModelFromHandle(hTreeModel);

			switch (e.Action)
			{
				case TreeModelChangedAction.Add:
				{
					Internal.GTK.Structures.GtkTreeIter iter = new Internal.GTK.Structures.GtkTreeIter();

					for (int i = 0; i < e.Rows.Count; i++)
					{
						TreeModelRow row = e.Rows[i];
					
						// as written we currently cannot do this...
						// int itemsCount = Internal.GTK.Methods.gtk_tree_store_
						if (e.ParentRow != null && _GtkTreeIterForTreeModelRow.ContainsKey(e.ParentRow))
						{
							// fixed 2019-07-16 16:44 by beckermj
							Internal.GTK.Structures.GtkTreeIter iterParent = _GtkTreeIterForTreeModelRow[e.ParentRow];
							RecursiveTreeStoreInsertRow(tm, row, hTreeModel, out iter, iterParent, 0, true);
						}
						else
						{
							RecursiveTreeStoreInsertRow(tm, row, hTreeModel, out iter, null, 0, true);
						}
					}
					break;
				}
				case TreeModelChangedAction.Clear:
				{
					Internal.GTK.Methods.gtk_tree_store_clear(hTreeModel);
					break;
				}
			}
		}

		private IntPtr GetHTreeView(IntPtr hScrolledWindow)
		{
			IntPtr hList = Internal.GTK.Methods.gtk_container_get_children(hScrolledWindow);
			IntPtr hTreeView = Internal.GLib.Methods.g_list_nth_data(hList, 0);
			return hTreeView;
		}

		private void RecursiveTreeStoreInsertRow(TreeModel tm, TreeModelRow row, IntPtr hTreeStore, out Internal.GTK.Structures.GtkTreeIter hIter, Internal.GTK.Structures.GtkTreeIter? parent, int position, bool append = false)
		{
			if (parent == null)
			{
				if (append)
				{
					Internal.GTK.Methods.gtk_tree_store_append(hTreeStore, out hIter, IntPtr.Zero);
				}
				else
				{
					Internal.GTK.Methods.gtk_tree_store_insert(hTreeStore, out hIter, IntPtr.Zero, position);
				}
			}
			else
			{
				Internal.GTK.Structures.GtkTreeIter hIterParent = parent.Value;
				if (append)
				{
					Internal.GTK.Methods.gtk_tree_store_append(hTreeStore, out hIter, ref hIterParent);
				}
				else
				{
					Internal.GTK.Methods.gtk_tree_store_insert(hTreeStore, out hIter, ref hIterParent, position);
				}
			}

			RegisterGtkTreeIter(row, hIter);

			foreach (TreeModelRowColumn rc in row.RowColumns)
			{
				// since "Marshalling of type object is not implemented"
				// (mono/metadata/marshal.c:6507) we have to do it ourselves


				Internal.GLib.Structures.Value val = Internal.GLib.Structures.Value.FromObject(rc.Value);

				// Internal.GTK.Methods.gtk_tree_store_insert(hTreeStore, out hIter, IntPtr.Zero, 0);
				Internal.GTK.Methods.gtk_tree_store_set_value(hTreeStore, ref hIter, tm.Columns.IndexOf(rc.Column), ref val);

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

		private static void RegisterGtkTreeIter(TreeModelRow row, Internal.GTK.Structures.GtkTreeIter hIter)
		{
			_GtkTreeIterForTreeModelRow[row] = hIter;
			_TreeModelRowForGtkTreeIter[hIter] = row;
		}
	}
}
