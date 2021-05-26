using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using MBS.Framework.UserInterface;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Native;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;

using LISTVIEWTYPE = MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.ListView.ListView; // BrightIdeasSoftware.TreeListView;
using LISTVIEWITEMTYPE = System.Windows.Forms.ListViewItem;
using LISTVIEWCOLUMNHEADERTYPE = System.Windows.Forms.ColumnHeader; // BrightIdeasSoftware.OLVColumn;

using MBS.Framework.UserInterface.Controls.ListView;
using MBS.Framework.UserInterface.Controls.ListView.Native;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls
{
	[ControlImplementation(typeof(ListViewControl))]
	public class ListViewImplementation : WindowsFormsNativeImplementation, IListViewNativeImplementation, MBS.Framework.UserInterface.Native.ITreeModelRowCollectionNativeImplementation
	{
		private enum ImplementedAsType
		{
			/// <summary>
			/// The control can be implemented as a Internal.TreeView.ExplorerTreeView control.
			/// </summary>
			TreeView,
			/// <summary>
			/// The control can be implemented as a Internal.ListView.ListViewControl control.
			/// </summary>
			ListView
		}
		private static ImplementedAsType ImplementedAs(ListViewControl tv)
		{
			return ImplementedAsType.ListView;

			bool rowsHaveChildren = false;
			if (tv.Model != null)
			{
				for (int i = 0; i < tv.Model.Rows.Count; i++)
				{
					if (tv.Model.Rows[i].Rows.Count > 0)
					{
						rowsHaveChildren = true;
						break;
					}
				}
			}

			if (rowsHaveChildren && tv.Columns.Count == 1)
			{
				// ListView cannot have child rows, and we only have one detail column, so...
				// might as well implement it using native TreeView
				return ImplementedAsType.TreeView;
			}

			// we may or may not have to build our own fake-treeview
			return ImplementedAsType.ListView;
		}

		public ListViewImplementation(Engine engine, Control control) : base(engine, control)
		{
		}

		private SelectionMode _SelectionMode = SelectionMode.Single;
		private void SetSelectionModeInternal(System.Windows.Forms.Control handle, ListViewControl tv, SelectionMode value)
		{
			_SelectionMode = value;
			switch (value)
			{
				case SelectionMode.Browse:
				{
					break;
				}
				case SelectionMode.Multiple:
				{
					if (handle is Internal.TreeView.ExplorerTreeView)
					{
					}
					else if (handle is LISTVIEWTYPE)
					{
						(handle as LISTVIEWTYPE).MultiSelect = true;
					}
					break;
				}
				case SelectionMode.Single:
				{
					if (handle is Internal.TreeView.ExplorerTreeView)
					{
					}
					else if (handle is LISTVIEWTYPE)
					{
						(handle as LISTVIEWTYPE).MultiSelect = false;
					}
					break;
				}
			}
		}

		private SelectionMode GetSelectionModeInternal(System.Windows.Forms.Control handle, ListViewControl tv)
		{
			switch (ImplementedAs(tv))
			{
				case ImplementedAsType.TreeView:
				{
					break;
				}
				case ImplementedAsType.ListView:
				{
					if (_SelectionMode != SelectionMode.None && _SelectionMode != SelectionMode.Browse)
					{
						if ((handle as LISTVIEWTYPE).MultiSelect)
						{
							_SelectionMode = SelectionMode.Multiple;
						}
						else
						{
							_SelectionMode = SelectionMode.Single;
						}
					}
					break;
				}
			}
			return _SelectionMode;
		}

		public void SetSelectionMode(SelectionMode value)
		{
			System.Windows.Forms.Control handle = ((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.Control);
			ListViewControl tv = Control as ListViewControl;
			SetSelectionModeInternal(handle, tv, value);
		}
		public SelectionMode GetSelectionMode()
		{
			System.Windows.Forms.Control handle = ((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.Control);
			ListViewControl tv = Control as ListViewControl;
			return GetSelectionModeInternal(handle, tv);
		}

		public ListViewHitTestInfo HitTest(double x, double y)
		{
			return HitTestInternal((Handle as WindowsFormsNativeControl).Handle, x, y);
		}
		private ListViewHitTestInfo HitTestInternal(System.Windows.Forms.Control handle, double x, double y)
		{
			TreeModelRow row = null;
			TreeModelColumn column = null;
			if (handle is LISTVIEWTYPE)
			{
				System.Windows.Forms.ListViewHitTestInfo info = (handle as LISTVIEWTYPE).HitTest((int)x, (int)y);
				if (info?.Item != null)
				{
					row = (info.Item.Tag as TreeModelRow);
				}
				/*
				if (info?.SubItem != null)
				{
					if (info.SubItem.Tag is TreeModelRow)
					{
					}
					else if (info.SubItem.Tag is TreeModelRowColumn)
					{
						column = (info.SubItem.Tag as TreeModelRowColumn).Column;
					}
				}
				*/
			}
			else if (handle is Internal.TreeView.ExplorerTreeView)
			{
				System.Windows.Forms.TreeViewHitTestInfo info = (handle as Internal.TreeView.ExplorerTreeView).HitTest((int)x, (int)y);
				if (info?.Node != null)
				{
					row = (info.Node.Tag as TreeModelRow);
				}
			}
			return new ListViewHitTestInfo(row, column);
		}

		public void UpdateTreeModel ()
		{
			UpdateTreeModel ((Handle as WindowsFormsNativeControl).Handle);
		}

		public void UpdateTreeModelColumn(TreeModelRowColumn rc)
		{
			TreeModel tm = (rc.Parent.ParentControl as ListViewControl).Model;

			// Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_set_value(hTreeStore, ref hIter, tm.Columns.IndexOf(rc.Column), ref val);
		}

		private Dictionary<ListViewColumn, IntPtr> _ColumnHandles = new Dictionary<ListViewColumn, IntPtr>();
		private Dictionary<IntPtr, ListViewColumn> _HandleColumns = new Dictionary<IntPtr, ListViewColumn>();
		private IntPtr GetHandleForTreeViewColumn(ListViewColumn column)
		{
			if (!_ColumnHandles.ContainsKey(column))
				return IntPtr.Zero;
			return _ColumnHandles[column];
		}
		private ListViewColumn GetTreeViewColumnForHandle(IntPtr handle)
		{
			if (!_HandleColumns.ContainsKey(handle))
				return null;
			return _HandleColumns[handle];
		}
		private void RegisterTreeViewColumn(ListViewColumn column, IntPtr handle)
		{
			_ColumnHandles[column] = handle;
			_HandleColumns[handle] = column;
		}

		private Dictionary<ListViewColumn, bool> _IsColumnResizable = new Dictionary<ListViewColumn, bool>();
		public bool IsColumnResizable(ListViewColumn column)
		{
			if (_IsColumnResizable.ContainsKey(column))
				return _IsColumnResizable[column];
			return true; // most Windows Forms ListView columns are
		}
		public void SetColumnResizable(ListViewColumn column, bool value)
		{
			_IsColumnResizable[column] = value;
		}

		private bool? _IsColumnReorderableSet = null;
		private Dictionary<ListViewColumn, bool> _IsColumnReorderable = new Dictionary<ListViewColumn, bool>();
		public bool IsColumnReorderable(ListViewColumn column)
		{
			LISTVIEWTYPE lv = ((Handle as WindowsFormsNativeControl).Handle as LISTVIEWTYPE);

			if (_IsColumnReorderableSet == null)
			{
				/*
				bool reorderable = lv.AllowColumnReorder, oops = false;
				for (int i = 0; i < lv.Columns.Count; i++)
				{
					if ((lv.Columns[i].Tag as ListViewColumn).Reorderable != reorderable)
					{
						oops = true;
						break;
					}
				}

				if (oops)
				{
					_IsColumnReorderableSet = true;
				}
				else
				{
					_IsColumnReorderableSet = false;
				}
				*/
			}

			if (_IsColumnReorderableSet == true)
			{
				return _IsColumnReorderable[column];
			}
			return lv.AllowColumnReorder;
		}
		public void SetColumnReorderable(ListViewColumn column, bool value)
		{
			Console.WriteLine("SetColumnReorderable: in function");
			LISTVIEWTYPE lv = ((Handle as WindowsFormsNativeControl).Handle as LISTVIEWTYPE);
			if (lv == null)
				return;

			Console.WriteLine("lv is a ListView");
			bool reorderable = lv.AllowColumnReorder, oops = false;
			for (int i = 0; i < lv.Columns.Count; i++)
			{
				if ((lv.Columns[i].Tag as ListViewColumn).Reorderable != reorderable)
				{
					oops = true;
					break;
				}
			}

			if (oops)
			{
				Console.WriteLine("oops");
				_IsColumnReorderableSet = true;
				_IsColumnReorderable[column] = value;
			}
			else
			{
				Console.WriteLine("ok");
				lv.AllowColumnReorder = value;
			}
		}

		protected virtual void OnRowColumnEditing(TreeModelRowColumnEditingEventArgs e)
		{
			InvokeMethod((Control as ListViewControl), "OnRowColumnEditing", new object[] { e });
		}
		protected virtual void OnRowColumnEdited(TreeModelRowColumnEditedEventArgs e)
		{
			InvokeMethod((Control as ListViewControl), "OnRowColumnEdited", new object[] { e });
		}

		private TreeModel _OldModel = null;

		protected void UpdateTreeModel (System.Windows.Forms.Control handle)
		{
			ListViewControl tv = (Control as ListViewControl);

			if (tv.Model != null)
			{
				switch (ImplementedAs(tv))
				{
					case ImplementedAsType.ListView:
					{
						LISTVIEWTYPE lv = (handle as LISTVIEWTYPE);

						foreach (ListViewColumn tvc in tv.Columns)
						{
							TreeModelColumn c = tvc.Column;
							LISTVIEWCOLUMNHEADERTYPE lvh = new LISTVIEWCOLUMNHEADERTYPE();
							lvh.Text = tvc.Title;
							lvh.Tag = tvc;
							lv.Columns.Add(lvh);
							// SetColumnEditable(tvc, tvc.Editable);
						}

						if (lv.VirtualMode)
						{
							lv.VirtualListSize = GetVirtualListSize(tv.Model.Rows);
						}
						else
						{
							lv.Items.Clear();
							for (int i = 0; i < tv.Model.Rows.Count; i++)
							{
								System.Windows.Forms.ListViewItem lvi = TreeModelRowToListViewItem(tv.Model.Rows[i]);
								lv.Items.Add(lvi);
							}
						}
						break;
					}
					case ImplementedAsType.TreeView:
					{
						Internal.TreeView.ExplorerTreeView natv = (handle as Internal.TreeView.ExplorerTreeView);
						natv.Nodes.Clear();
						for (int i = 0; i < tv.Model.Rows.Count; i++)
						{
							RecursiveTreeStoreInsertRow(tv.Model, tv.Model.Rows[i], natv, null);
						}
						break;
					}
				}
			}
			_OldModel = tv.Model;
		}

		public List<TreeModelRow> GetVirtualListRows(TreeModelRow.TreeModelRowCollection rows)
		{
			// HACK: i'm not smart enough to figure out how to do this without creating a temporary list...
			List<TreeModelRow> list = new List<TreeModelRow>();
			List<TreeModelRow> templist = new List<TreeModelRow>();

			ListViewControl lv = (Control as ListViewControl);
			for (int i = 0; i < rows.Count; i++)
			{
				templist.Add(rows[i]);
			}
			if (lvwColumnSorter != null)
			{
				templist.Sort(lvwColumnSorter);
			}

			for (int i = 0; i < templist.Count; i++)
			{
				list.Add(templist[i]);
				if (templist[i].Expanded && templist[i].Rows.Count > 0)
				{
					List<TreeModelRow> sublist = GetVirtualListRows(templist[i].Rows);
					list.AddRange(sublist);
				}
			}
			return list;
		}
		public TreeModelRow GetVirtualListRow(int index)
		{
			ListViewControl lv = (Control as ListViewControl);
			List<TreeModelRow> rows = GetVirtualListRows(lv.Model.Rows);
			if (index >= 0 && index < rows.Count)
			{
				return rows[index];
			}
			return null;
		}

		public int GetVirtualListIndex(TreeModelRow row)
		{
			ListViewControl lv = (Control as ListViewControl);
			if (row.ParentRow == null)
			{
				int end = lv.Model.Rows.IndexOf(row);
				int rowindex = end;
				for (int i = 0; i < end; i++)
				{
					if (lv.Model.Rows[i].Expanded)
					{
						rowindex += GetVirtualListSize(lv.Model.Rows[i].Rows);
					}
				}
				return rowindex;
			}
			return 0;
		}
		private int GetVirtualListSize(TreeModelRow.TreeModelRowCollection rows)
		{
			int size = rows.Count;
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].Expanded)
				{
					size += GetVirtualListSize(rows[i].Rows);
				}
			}
			return size;
		}

		private Dictionary<ListViewColumn, bool> _ColumnsEditable = new Dictionary<ListViewColumn, bool>();
		public void SetColumnEditable(ListViewColumn tvc, bool editable)
		{
			_ColumnsEditable[tvc] = editable;
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			ListViewControl tv = (control as ListViewControl);
			System.Windows.Forms.Control handle = null;

			// TODO: fix GtkTreeView implementation
			// Internal.GTK.Methods.Methods.gtk_tree_store_insert_with_valuesv(hTreeStore, ref hIterFirst, IntPtr.Zero, 0, ref columns, values, values.Length);
			tv.SelectedRows.ItemRequested += SelectedRows_ItemRequested;
			tv.SelectedRows.Cleared += SelectedRows_Cleared;

			if (tv.Model != null && !TreeModelAssociatedControls.ContainsKey(tv.Model))
			{
				TreeModelAssociatedControls.Add(tv.Model, new List<System.Windows.Forms.Control>());
			}

			switch (ImplementedAs (tv))
			{
				case ImplementedAsType.TreeView:
				{
					handle = new Internal.TreeView.ExplorerTreeView();
					handle.Tag = tv;
					(handle as Internal.TreeView.ExplorerTreeView).AfterLabelEdit += tv_AfterLabelEdit;

					(handle as Internal.TreeView.ExplorerTreeView).NodeMouseDoubleClick += tv_NodeMouseDoubleClick;
					(handle as Internal.TreeView.ExplorerTreeView).BeforeSelect += tv_BeforeSelect;
					(handle as Internal.TreeView.ExplorerTreeView).AfterSelect += tv_AfterSelect;

					if (tv.Model != null && !TreeModelAssociatedControls[tv.Model].Contains(handle))
					{
						TreeModelAssociatedControls[tv.Model].Add(handle);
					}
					break;
				}
				case ImplementedAsType.ListView:
				{
					handle = new LISTVIEWTYPE(this);
					handle.Tag = tv;
					(handle as LISTVIEWTYPE).HeaderStyle = WindowsFormsEngine.HeaderStyleToSWFHeaderStyle(tv.HeaderStyle);
					(handle as LISTVIEWTYPE).ItemActivate += lv_ItemActivate;
					(handle as LISTVIEWTYPE).ItemSelectionChanged += lv_ItemSelectionChanged;
					(handle as LISTVIEWTYPE).FullRowSelect = true;
					(handle as LISTVIEWTYPE).View = System.Windows.Forms.View.Details;
					(handle as LISTVIEWTYPE).ColumnClick += Handle_ColumnClick;

					lvwColumnSorter = new ListViewItemSorter(control as ListViewControl, handle as LISTVIEWTYPE);

					if (tv.Model != null && !TreeModelAssociatedControls[tv.Model].Contains(handle))
					{
						TreeModelAssociatedControls[tv.Model].Add(handle);
					}
					break;
				}
			}

			if (tv.Model != null)
				UpdateTreeModel (handle);

			SetSelectionModeInternal(handle, tv, tv.SelectionMode);

			return new WindowsFormsNativeControl(handle);
		}

		private class ListViewItemSorter : IComparer<TreeModelRow>
		{
			public LISTVIEWTYPE Parent { get; private set; } = null;
			public ListViewControl Control { get; private set; } = null;

			public ListViewItemSorter(ListViewControl ctl, LISTVIEWTYPE lvparent)
			{
				Parent = lvparent;
				Control = ctl;
			}

			/// <summary>
			/// Specifies the column to be sorted
			/// </summary>
			public int ColumnIndex { get; set; }
			/// <summary>
			/// Specifies the order in which to sort (i.e. 'Ascending').
			/// </summary>
			public System.Windows.Forms.SortOrder SortOrder { get; set; }

			public int Compare(TreeModelRow left, TreeModelRow right)
			{
				int compareResult = 0;
				// Cast the objects to be compared to ListViewItem objects
				if (Control.SortContainerRowsFirst)
				{
					if (left.Rows.Count > 0 && right.Rows.Count == 0)
						return 1;
					if (left.Rows.Count == 0 && right.Rows.Count > 0)
						return -1;
				}

				// Compare the two items
				if (left.RowColumns[ColumnIndex].Value == null && right.RowColumns[ColumnIndex].Value == null)
				{
					compareResult = 0;
				}
				else if (left.RowColumns[ColumnIndex].Value is IComparable)
				{
					compareResult = (left.RowColumns[ColumnIndex].Value as IComparable).CompareTo(right.RowColumns[ColumnIndex].Value);
				}
				else
				{
					compareResult = (left == right) ? 0 : -1;
				}

				// Calculate correct return value based on object comparison
				if (SortOrder == System.Windows.Forms.SortOrder.Ascending)
				{
					// Ascending sort is selected, return normal result of compare operation
					return compareResult;
				}
				else if (SortOrder == System.Windows.Forms.SortOrder.Descending)
				{
					// Descending sort is selected, return negative result of compare operation
					return (-compareResult);
				}
				else
				{
					// Return '0' to indicate they are equal
					return 0;
				}
			}
		}

		private ListViewItemSorter lvwColumnSorter = null;

		void Handle_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			LISTVIEWTYPE lv = (sender as LISTVIEWTYPE);

			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == lvwColumnSorter.ColumnIndex)
			{
				// Reverse the current sort direction for this column.
				if (lvwColumnSorter.SortOrder == System.Windows.Forms.SortOrder.Ascending)
				{
					lvwColumnSorter.SortOrder = System.Windows.Forms.SortOrder.Descending;
				}
				else
				{
					lvwColumnSorter.SortOrder = System.Windows.Forms.SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				lvwColumnSorter.ColumnIndex = e.Column;
				lvwColumnSorter.SortOrder = System.Windows.Forms.SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			lv.Refresh();
		}


		void lv_ItemSelectionChanged(object sender, System.Windows.Forms.ListViewItemSelectionChangedEventArgs e)
		{
			LISTVIEWTYPE _lv = (sender as LISTVIEWTYPE);
			ListViewControl lv = (Control as ListViewControl);

			Console.WriteLine("selected rows: {0}", lv.SelectedRows.Count);
			InvokeMethod(lv, "OnSelectionChanged", new object[] { e });
		}


		void tv_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			((sender as Internal.TreeView.ExplorerTreeView).Tag as ListViewControl).OnSelectionChanged(e);
		}


		void tv_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			/*
			ListViewSelectionChangingEventArgs ee = new ListViewSelectionChangingEventArgs();
			((sender as Internal.TreeView.ExplorerTreeView).Tag as ListView).OnSelectionChanging(ee);
			e.Cancel = ee.Cancel;
			*/
		}


		private void lv_ItemActivate(object sender, EventArgs e)
		{
			LISTVIEWTYPE handle = (sender as LISTVIEWTYPE);
			ListViewControl lv = (handle.Tag as ListViewControl);

			if (handle.SelectedIndices.Count > 0)
			{
				lv.OnRowActivated(new ListViewRowActivatedEventArgs(handle.Items[handle.SelectedIndices[0]].Tag as TreeModelRow));
			}
		}
		private void tv_NodeMouseDoubleClick(object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e)
		{
			Internal.TreeView.ExplorerTreeView tv = (sender as Internal.TreeView.ExplorerTreeView);
			ListViewControl lv = (tv.Tag as ListViewControl);
			TreeModelRow row = (e.Node.Tag as TreeModelRow);
			lv.OnRowActivated(new ListViewRowActivatedEventArgs(row));
		}


		private void tv_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			ListViewControl lv = (Control as ListViewControl);
			if (lv.Model == null)
				return;

			TreeModelRow row = (e.Node.Tag as TreeModelRow);
			TreeModelRowColumn rc = row.RowColumns[0];
			if (row != null)
			{
				TreeModelRowColumnEditingEventArgs ee = new TreeModelRowColumnEditingEventArgs(row, rc, rc.Value, e.Label);
				OnRowColumnEditing(ee);
				if (!ee.Cancel)
				{
					rc.Value = e.Label;
					OnRowColumnEdited(new TreeModelRowColumnEditedEventArgs(row, rc, rc.Value, e.Label));
				}
			}
		}


		private static void SelectedRows_ItemRequested(object sender, TreeModelRowItemRequestedEventArgs e)
		{
			TreeModelRow.TreeModelSelectedRowCollection coll = (sender as TreeModelRow.TreeModelSelectedRowCollection);
			ControlImplementation impl = coll.Parent.ControlImplementation;
			if (coll.Parent != null)
			{
				if ((coll.Parent.ControlImplementation?.Handle as WindowsFormsNativeControl).Handle is LISTVIEWTYPE)
				{
					LISTVIEWTYPE lv = ((coll.Parent.ControlImplementation?.Handle as WindowsFormsNativeControl)?.Handle as LISTVIEWTYPE);

					if (e.Index == -1 && e.Count == 1 && e.Item != null)
					{
						// we are adding a new row to the selected collection
						// GetListViewItem(e.Item)
						// lv.Items[index].Selected = true;
						return;
					}
					else
					{
						e.Count = lv.SelectedIndices.Count;
						if (e.Count > 0 && e.Index > -1)
						{
							TreeModelRow row = (lv.Items[lv.SelectedIndices[e.Index]].Tag as TreeModelRow);
							e.Item = row;
						}
						else if (e.Count > 0 && e.Index == -1 && e.Item != null)
						{
							// we are checking if selection contains a row
							bool found = false;
							for (int i = 0; i < lv.SelectedIndices.Count; i++)
							{
								TreeModelRow rowtest = (lv.Items[lv.SelectedIndices[i]].Tag as TreeModelRow);
								if (rowtest == e.Item)
								{
									found = true;
									break;
								}
							}
							if (!found) e.Item = null;
						}
						else
						{
							e.Item = null;
						}
					}
				}
				else if ((coll.Parent.ControlImplementation?.Handle as WindowsFormsNativeControl).Handle is Internal.TreeView.ExplorerTreeView)
				{
					Internal.TreeView.ExplorerTreeView tv = ((coll.Parent.ControlImplementation?.Handle as WindowsFormsNativeControl)?.Handle as Internal.TreeView.ExplorerTreeView);

					if (e.Index == -1 && e.Count == 1 && e.Item != null)
					{
						// we are adding a new row to the selected collection
						// GetListViewItem(e.Item)
						// lv.Items[index].Selected = true;
						return;
					}
					else if (tv.SelectedNode != null)
					{
						e.Count = 1;
						if (e.Count > 0 && e.Index > -1)
						{
							TreeModelRow row = (tv.SelectedNode.Tag as TreeModelRow);
							e.Item = row;
						}
						else if (e.Count > 0 && e.Index == -1 && e.Item != null)
						{
							// we are checking if selection contains a row
							if (tv.SelectedNode.Tag != e.Item)
							{
								e.Item = null;
							}
						}
						else
						{
							e.Item = null;
						}
					}
				}
			}
		}
		private static void SelectedRows_Cleared(object sender, EventArgs e)
		{
			TreeModelRow.TreeModelSelectedRowCollection coll = (sender as TreeModelRow.TreeModelSelectedRowCollection);
			if (coll.Parent != null)
			{
				ImplementedAsType implementedAs = ImplementedAs(coll.Parent);
				if (implementedAs == ImplementedAsType.TreeView)
				{
					Internal.TreeView.ExplorerTreeView tv = ((coll.Parent.ControlImplementation?.Handle as WindowsFormsNativeControl)?.Handle as Internal.TreeView.ExplorerTreeView);
					if (tv != null)
						tv.SelectedNode = null;
				}
				else if (implementedAs == ImplementedAsType.ListView)
				{
					LISTVIEWTYPE lv = ((coll.Parent.ControlImplementation?.Handle as WindowsFormsNativeControl)?.Handle as LISTVIEWTYPE);
					if (lv != null)
						lv.SelectedIndices.Clear();
				}
			}
		}

		protected override void OnKeyDown (KeyEventArgs e)
		{
			base.OnKeyDown (e);

			ListViewControl lv = Control as ListViewControl;
			if (lv == null) return;

			if (lv.SelectedRows.Count != 1) return;

			if (e.Key == KeyboardKey.ArrowRight) {
				lv.SelectedRows [0].Expanded = true;
				e.Cancel = true;
			}
			else if (e.Key == KeyboardKey.ArrowLeft) {
				if (!lv.SelectedRows [0].Expanded) {
					// we're already closed, so move selection up to our parent
					TreeModelRow rowCurrent = lv.SelectedRows [0];
					if (rowCurrent.ParentRow != null) {
						lv.SelectedRows.Clear ();
						lv.SelectedRows.Add (rowCurrent.ParentRow);
					}
				} else {
					lv.SelectedRows [0].Expanded = false;
				}
				e.Cancel = true;
			}
		}

		private Dictionary<TreeModel, List<System.Windows.Forms.Control>> TreeModelAssociatedControls = new Dictionary<TreeModel, List<System.Windows.Forms.Control>>();

		private System.Windows.Forms.ListViewItem TreeModelRowToListViewItem(TreeModelRow row)
		{
			System.Windows.Forms.ListViewItem tn = new System.Windows.Forms.ListViewItem();
			if (row.RowColumns.Count > 0)
			{
				tn.Text = row.RowColumns[0].Value?.ToString();
			}
			for (int i = 1; i < row.RowColumns.Count; i++)
			{
				tn.SubItems.Add(row.RowColumns[i].Value?.ToString());
			}
			tn.Tag = row;
			row.SetExtraData<System.Windows.Forms.ListViewItem>("lvi", tn);
			return tn;
		}

		private System.Windows.Forms.TreeNode TreeModelRowToTreeNode(TreeModelRow row)
		{
			System.Windows.Forms.TreeNode tn = new System.Windows.Forms.TreeNode();
			if (row.RowColumns.Count > 0)
			{
				tn.Text = row.RowColumns[0].Value?.ToString();
			}
			tn.Tag = row;
			row.SetExtraData<System.Windows.Forms.TreeNode>("tn", tn);

			foreach (TreeModelRow row2 in row.Rows)
			{
				tn.Nodes.Add(TreeModelRowToTreeNode(row2));
			}
			return tn;
		}

		public void UpdateTreeModel(TreeModel tm, TreeModelChangedEventArgs e)
		{
			if (!TreeModelAssociatedControls.ContainsKey(tm))
				return;

			List<System.Windows.Forms.Control> list = TreeModelAssociatedControls[tm];
			switch (e.Action)
			{
				case TreeModelChangedAction.Add:
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] is Internal.TreeView.ExplorerTreeView)
						{
							Internal.TreeView.ExplorerTreeView tv = (list[i] as Internal.TreeView.ExplorerTreeView);
							for (int j = 0; j < e.Rows.Count; j++)
							{
								tv.Nodes.Add(TreeModelRowToTreeNode(e.Rows[j]));
							}
						}
						else if (list[i] is LISTVIEWTYPE)
						{
							LISTVIEWTYPE lv = (list[i] as LISTVIEWTYPE);
							if (lv.VirtualMode)
							{
								lv.VirtualListSize = GetVirtualListSize((tm as DefaultTreeModel).Rows);
							}
							else
							{
								for (int j = 0; j < e.Rows.Count; j++)
								{
									lv.Items.Add(TreeModelRowToListViewItem(e.Rows[j]));
								}
							}
						}
					}

					break;
				}
				case TreeModelChangedAction.Remove:
				{
					foreach (TreeModelRow row in e.Rows)
					{
						System.Windows.Forms.ListViewItem lvi = row.GetExtraData<System.Windows.Forms.ListViewItem>("lvi");
						if (lvi == null)
						{
							System.Windows.Forms.TreeNode tn = row.GetExtraData<System.Windows.Forms.TreeNode>("tn");
							if (tn == null)
							{
								Console.Error.WriteLine("attempted to remove TreeModelRow without associated ListViewItem or TreeNode");
								return;
							}
							tn.Remove();
							return;
						}
						lvi.Remove();
					}
					break;
				}
				case TreeModelChangedAction.Clear:
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] is Internal.TreeView.ExplorerTreeView)
						{
							(list[i] as Internal.TreeView.ExplorerTreeView).Nodes.Clear();
						}
						else if (list[i] is LISTVIEWTYPE)
						{
							(list[i] as LISTVIEWTYPE).Items.Clear();
						}
					}
					break;
				}
			}
		}

		public void UpdateTreeModel(NativeControl handle, TreeModelChangedEventArgs e)
		{
			System.Windows.Forms.Control hctrl = ((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.Control);
			if (hctrl == null) return;

			if (hctrl is Internal.TreeView.ExplorerTreeView)
			{

			}
			else if (hctrl is LISTVIEWTYPE)
			{
			}

			UpdateTreeModel((Control as ListViewControl).Model, e);
		}


		public bool IsRowExpanded(TreeModelRow row)
		{
			ListViewControl lv = Control as ListViewControl;
			if (lv == null)
				return false;


			if ((Handle as WindowsFormsNativeControl).Handle is Internal.TreeView.ExplorerTreeView)
			{
				if (_NodesForRow.ContainsKey(row))
				{
					return _NodesForRow[row].IsExpanded;
				}
			}
			else if ((Handle as WindowsFormsNativeControl).Handle is LISTVIEWTYPE)
			{
				if (_RowExpanded.ContainsKey(row))
				{
					return _RowExpanded[row];
				}
			}
			return false;
		}

		private Dictionary<TreeModelRow, bool> _RowExpanded = new Dictionary<TreeModelRow, bool>();
		private Dictionary<TreeModelRow, System.Windows.Forms.TreeNode> _NodesForRow = new Dictionary<TreeModelRow, System.Windows.Forms.TreeNode>();
		public void SetRowExpanded(TreeModelRow row, bool expanded)
		{
			ListViewControl lv = Control as ListViewControl;
			if (lv == null)
				return;

			if ((Handle as WindowsFormsNativeControl).Handle is Internal.TreeView.ExplorerTreeView)
			{
				if (_NodesForRow.ContainsKey(row))
				{
					if (expanded)
					{
						_NodesForRow[row].Expand();
					}
					else
					{
						_NodesForRow[row].Collapse();
					}
				}
			}
			else if ((Handle as WindowsFormsNativeControl).Handle is Internal.ListView.ListView)
			{
				_RowExpanded[row] = expanded;

				if (expanded)
				{
					((Handle as WindowsFormsNativeControl).Handle as LISTVIEWTYPE).VirtualListSize = ((Handle as WindowsFormsNativeControl).Handle as LISTVIEWTYPE).VirtualListSize + row.Rows.Count;
				}
				else
				{
					((Handle as WindowsFormsNativeControl).Handle as LISTVIEWTYPE).VirtualListSize = ((Handle as WindowsFormsNativeControl).Handle as LISTVIEWTYPE).VirtualListSize - row.Rows.Count;
				}
			}
		}

		private void RecursiveTreeStoreInsertRow(TreeModel tm, TreeModelRow row, Internal.TreeView.ExplorerTreeView parentView, System.Windows.Forms.TreeNode parentNode, int position = -1)
		{
			Contract.Requires(parentView != null);

			System.Windows.Forms.TreeNode tn = TreeModelRowToTreeNode(row);
			if (parentNode == null)
			{
				if (position == -1)
				{
					parentView.Nodes.Add(tn);
				}
				else
				{
					parentView.Nodes.Insert(position, tn);
				}
			}
			else
			{
				if (position == -1)
				{
					parentNode.Nodes.Add(tn);
				}
				else
				{
					parentNode.Nodes.Insert(position, tn);
				}
			}
		}

		public bool GetSingleClickActivation()
		{
			switch (ImplementedAs(Control as ListViewControl))
			{
				case ImplementedAsType.ListView: return ((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.ListView).HotTracking;
				case ImplementedAsType.TreeView: return ((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.TreeView).HotTracking;
			}
			throw new NotSupportedException();
		}

		public void SetSingleClickActivation(bool value)
		{
			switch (ImplementedAs(Control as ListViewControl))
			{
				case ImplementedAsType.ListView:
				{
					((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.ListView).HoverSelection = true;
					((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.ListView).HotTracking = true;
					break;
				}
				case ImplementedAsType.TreeView:
				{
					((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.TreeView).HotTracking = true;
					break;
				}
			}
			throw new NotSupportedException();
		}
	}
}
