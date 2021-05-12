using System;
using System.Collections;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Controls.ListView
{
	namespace Native
	{
		public interface IListViewNativeImplementation
		{
			void UpdateTreeModel ();
			void UpdateTreeModel(NativeControl handle, TreeModelChangedEventArgs e);

			void UpdateTreeModelColumn(TreeModelRowColumn rc);

			SelectionMode GetSelectionMode();
			void SetSelectionMode(SelectionMode value);

			ListViewHitTestInfo HitTest(double x, double y);

			bool IsColumnReorderable(ListViewColumn column);
			void SetColumnReorderable(ListViewColumn column, bool value);

			bool IsColumnResizable(ListViewColumn column);
			void SetColumnResizable(ListViewColumn column, bool value);
			void SetColumnEditable(ListViewColumn column, bool value);

			bool GetSingleClickActivation();
			void SetSingleClickActivation(bool value);
		}
	}

	public class ListViewControl : SystemControl
	{
		public ListViewControl()
		{
			this.SelectedRows = new TreeModelRow.TreeModelSelectedRowCollection(this);
			mvarColumns = new ListViewColumn.ListViewColumnCollection(this);
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

		private void RecursiveSetControlParent (TreeModelRow row)
		{
			row.ParentControl = this;
			foreach (TreeModelRow row2 in row.Rows) {
				RecursiveSetControlParent (row2);
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
					foreach (TreeModelRow row in mvarModel.Rows)
					{
						RecursiveSetControlParent(row);
					}
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

			switch (e.Action)
			{
				case TreeModelChangedAction.Add:
				{
					foreach (TreeModelRow row in e.Rows)
					{
						row.ParentControl = this;
					}
					break;
				}
			}

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
	}
}
