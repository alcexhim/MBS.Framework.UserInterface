using System;
using System.Collections;
using System.Collections.Generic;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Controls.ListView
{
	namespace Native
	{
		public interface IListViewNativeImplementation
		{
			void UpdateTreeModel();
			void UpdateTreeModel(NativeControl handle, TreeModelChangedEventArgs e);

			SelectionMode GetSelectionMode();
			void SetSelectionMode(SelectionMode value);

			ListViewHitTestInfo HitTest(double x, double y);

			bool IsColumnReorderable(ListViewColumn column);
			void SetColumnReorderable(ListViewColumn column, bool value);

			bool IsColumnCreated(ListViewColumn column);
			bool IsColumnResizable(ListViewColumn column);
			void SetColumnResizable(ListViewColumn column, bool value);
			void SetCellRendererEditable(CellRenderer renderer, bool value);

			bool GetSingleClickActivation();
			void SetSingleClickActivation(bool value);
			void Focus(TreeModelRow row, ListViewColumn column, CellRenderer renderer, bool edit);

			bool GetEnableDragSelection();
			void SetEnableDragSelection(bool value);
		}
	}

	public class ListViewControl : SystemControl
	{
		public ListViewControl()
		{
			this.SelectedRows = new TreeModelRow.TreeModelSelectedRowCollection(this);
			mvarColumns = new ListViewColumn.ListViewColumnCollection(this);
		}

		private int _selectedIndex = -1;
		public int SelectedIndex
		{
			get
			{
				if (Model != null)
				{
					if (SelectedRows.Count > 0)
					{
						return Model.Rows.IndexOf(SelectedRows[0]);
					}
					return -1;
				}
				return _selectedIndex;
			}
			set
			{
				_selectedIndex = value;
				if (Model != null)
				{
					SelectedRows.Clear();

					if (value >= 0 && value < Model.Rows.Count)
					{
						SelectedRows.Add(Model.Rows[value]);
					}
				}
			}
		}

		private bool mvarEnableDragSelection = false;
		public bool EnableDragSelection
		{
			get
			{
				if (IsCreated)
					mvarEnableDragSelection = (ControlImplementation as Native.IListViewNativeImplementation).GetEnableDragSelection();
				return mvarEnableDragSelection;
			}
			set
			{
				if (IsCreated)
					(ControlImplementation as Native.IListViewNativeImplementation).SetEnableDragSelection(value);
				mvarEnableDragSelection = value;
			}
		}

		private SelectionMode mvarSelectionMode = SelectionMode.Single;
		public SelectionMode SelectionMode
		{
			get
			{
				try
				{
					if (this.IsCreated)
						mvarSelectionMode = (ControlImplementation as Native.IListViewNativeImplementation).GetSelectionMode();
				}
				catch (Exception)
				{
				}
				return mvarSelectionMode;
			}
			set
			{
				if (this.IsCreated)
					(ControlImplementation as Native.IListViewNativeImplementation).SetSelectionMode(value);
				mvarSelectionMode = value;
			}
		}

		private DefaultTreeModel mvarModel = null;
		public DefaultTreeModel Model
		{
			get { return mvarModel; }
			set
			{
				mvarModel = value;
				if (mvarModel != null)
				{
					mvarModel.TreeModelChanged += MvarModel_TreeModelChanged;
				}
				(ControlImplementation as Native.IListViewNativeImplementation)?.UpdateTreeModel();
			}
		}

		public ListViewHitTestInfo LastHitTest { get; private set; } = new ListViewHitTestInfo(null, null);
		protected internal override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			LastHitTest = HitTest(e.X, e.Y);
		}

		public event EventHandler<CellEditingEventArgs> CellEditing;
		protected virtual void OnCellEditing(CellEditingEventArgs e)
		{
			CellEditing?.Invoke(this, e);
		}

		public event EventHandler<CellEditedEventArgs> CellEdited;
		protected virtual void OnCellEdited(CellEditedEventArgs e)
		{
			CellEdited?.Invoke(this, e);
		}

		public event TreeModelChangedEventHandler TreeModelChanged;
		public void OnTreeModelChanged(object sender, TreeModelChangedEventArgs e)
		{
			TreeModelChanged?.Invoke(sender, e);
		}
		public event ListViewRowActivatedEventHandler RowActivated;
		public virtual void OnRowActivated(ListViewRowActivatedEventArgs e)
		{
			RowActivated?.Invoke(this, e);
		}

		public event ListViewSelectionChangingEventHandler SelectionChanging;
		public virtual void OnSelectionChanging(ListViewSelectionChangingEventArgs e)
		{
			SelectionChanging?.Invoke(this, e);
		}

		public event EventHandler SelectionChanged;
		public virtual void OnSelectionChanged(EventArgs e)
		{
			SelectionChanged?.Invoke(this, e);
		}

		private void MvarModel_TreeModelChanged(object sender, TreeModelChangedEventArgs e)
		{
			OnTreeModelChanged(sender, e);

			(ControlImplementation as Native.IListViewNativeImplementation)?.UpdateTreeModel(ControlImplementation.Handle, e);
		}

		private ListViewColumn.ListViewColumnCollection mvarColumns = null;
		public ListViewColumn.ListViewColumnCollection Columns { get { return mvarColumns; } }

		public ColumnHeaderStyle HeaderStyle { get; set; } = ColumnHeaderStyle.Clickable;
		public TreeModelRow.TreeModelSelectedRowCollection SelectedRows { get; private set; } = null;

		public ListViewMode Mode { get; set; } = ListViewMode.Detail;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ListViewItemSorter"/> sorts container rows (i.e. rows that contain other rows) first. Useful for
		/// implementing a "sort folders before files" feature.
		/// </summary>
		/// <value><c>true</c> if container rows should be sorted first; otherwise, <c>false</c>.</value>
		public bool SortContainerRowsFirst { get; set; } = false;

		private Dictionary<TreeModelRow, bool> _IsExpanded = new Dictionary<TreeModelRow, bool>();

		private bool mvarExpanded = false;
		public bool IsExpanded(TreeModelRow row)
		{
			if (!IsCreated)
			{
				if (!_IsExpanded.ContainsKey(row))
					_IsExpanded[row] = false;
				return _IsExpanded[row];
			}
			_IsExpanded[row] = ((ControlImplementation as UserInterface.Native.ITreeModelRowCollectionNativeImplementation)?.IsRowExpanded(row)).GetValueOrDefault(false);
			return _IsExpanded[row];
		}

		public void SetExpanded(TreeModelRow row, bool expanded)
		{
			if (ControlImplementation == null)
			{
				Console.Error.WriteLine("uwt: TreeModelRow: NativeImplementation is NULL");
			}
			(ControlImplementation as UserInterface.Native.ITreeModelRowCollectionNativeImplementation)?.SetRowExpanded(row, expanded);
			_IsExpanded[row] = expanded;
		}

		public void ExpandAll(TreeModelRow row = null)
		{
			if (row != null)
			{
				SetExpanded(row, true);
				foreach (TreeModelRow row2 in row.Rows)
				{
					ExpandAll(row2);
				}
			}
			else
			{
				foreach (TreeModelRow row2 in Model.Rows)
				{
					ExpandAll(row2);
				}
			}
		}
		public void CollapseAll(TreeModelRow row = null)
		{
			if (row != null)
			{
				SetExpanded(row, false);
				foreach (TreeModelRow row2 in row.Rows)
				{
					CollapseAll(row2);
				}
			}
			else
			{
				foreach (TreeModelRow row2 in Model.Rows)
				{
					CollapseAll(row2);
				}
			}
		}


		public void EnsureVisible(TreeModelRow row)
		{
			TreeModelRow parentRow = row;
			while (parentRow != null)
			{
				SetExpanded(parentRow, true);
				parentRow = parentRow.ParentRow;
			}
		}

		private bool _SingleClickActivation = false;
		public bool SingleClickActivation
		{
			get
			{
				if (IsCreated && ControlImplementation is Native.IListViewNativeImplementation)
				{
					return (ControlImplementation as Native.IListViewNativeImplementation).GetSingleClickActivation();
				}
				return _SingleClickActivation;
			}
			set { _SingleClickActivation = value; (ControlImplementation as Native.IListViewNativeImplementation).SetSingleClickActivation(value); }
		}

		/// <summary>
		/// Hits the test.
		/// </summary>
		/// <returns>A <see cref="ListViewHitTestInfo" /> indicating the results of the hit test. For <see cref="ListViewControl" /> instances with a <see cref="ControlImplementation" />, this method SHOULD NEVER return null.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public ListViewHitTestInfo HitTest(double x, double y)
		{
			Native.IListViewNativeImplementation impl = (ControlImplementation as Native.IListViewNativeImplementation);
			if (impl != null)
				return impl.HitTest(x, y);
			return null;
		}

		/// <summary>
		/// Selects the specified <see cref="TreeModelRow"/>.
		/// </summary>
		/// <param name="row">Tree model row.</param>
		public void Select(TreeModelRow row)
		{
			SelectedRows.Add(row);
		}

		public event TreeModelRowColumnEditingEventHandler RowColumnEditing;
		protected virtual void OnRowColumnEditing(TreeModelRowColumnEditingEventArgs e)
		{
			RowColumnEditing?.Invoke(this, e);
		}
		public event TreeModelRowColumnEditedEventHandler RowColumnEdited;
		protected virtual void OnRowColumnEdited(TreeModelRowColumnEditedEventArgs e)
		{
			RowColumnEdited?.Invoke(this, e);
		}

		/// <summary>
		/// Selects the specified <see cref="TreeModelRow" />, and optionally
		/// sets focus to the specified <see cref="ListViewColumn" /> and
		/// <see cref="CellRenderer" />. If <paramref name="edit" /> is
		/// <see langword="true" />, initiates editing of the given
		/// <see cref="CellRenderer" />.
		/// </summary>
		/// <param name="row">Row.</param>
		/// <param name="listViewColumn">List view column.</param>
		/// <param name="cellRenderer">Cell renderer.</param>
		/// <param name="v">If set to <c>true</c> v.</param>
		public void Focus(TreeModelRow row, ListViewColumn column = null, CellRenderer renderer = null, bool edit = false)
		{
			(ControlImplementation as Native.IListViewNativeImplementation).Focus(row, column, renderer, edit);
		}
	}
}
