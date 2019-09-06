using System;

namespace UniversalWidgetToolkit.Controls
{
	namespace Native
	{
		public interface IListViewNativeImplementation
		{
			void UpdateTreeModel ();
			void UpdateTreeModel(NativeControl handle, TreeModelChangedEventArgs e);
			
			SelectionMode GetSelectionMode();
			void SetSelectionMode(SelectionMode value);

			ListViewHitTestInfo HitTest(double x, double y);
		}
	}

	public delegate void ListViewRowActivatedEventHandler(object sender, ListViewRowActivatedEventArgs e);
	public class ListViewRowActivatedEventArgs : EventArgs
	{
		/// <summary>
		/// The row that was activated.
		/// </summary>
		/// <value>The row that was activated.</value>
		public TreeModelRow Row { get; private set; } = null;

		public ListViewRowActivatedEventArgs(TreeModelRow row)
		{
			Row = row;
		}
	}

	public abstract class ListViewColumn
	{
		public class ListViewColumnCollection
			: System.Collections.ObjectModel.Collection<ListViewColumn>
		{

		}

		private TreeModelColumn mvarColumn = null;
		public TreeModelColumn Column { get { return mvarColumn; } set { mvarColumn = value; } }

		private string mvarTitle = String.Empty;
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		public ListViewColumn(TreeModelColumn column, string title = "")
		{
			mvarColumn = column;
			mvarTitle = title;
		}
	}
	public class ListViewColumnText
		: ListViewColumn
	{
		public ListViewColumnText(TreeModelColumn column, string title = "") : base(column, title)
		{
		}
	}
	public class ListView : SystemControl
	{
		public ListView()
		{
			this.SelectedRows = new TreeModelRow.TreeModelSelectedRowCollection(this);
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
		public DefaultTreeModel Model { get { return mvarModel; } set { mvarModel = value; mvarModel.TreeModelChanged += MvarModel_TreeModelChanged;
				foreach (TreeModelRow row in mvarModel.Rows) {
					RecursiveSetControlParent (row);
				}
		(ControlImplementation as Native.IListViewNativeImplementation)?.UpdateTreeModel (); } }

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

		private ListViewColumn.ListViewColumnCollection mvarColumns = new ListViewColumn.ListViewColumnCollection();
		public ListViewColumn.ListViewColumnCollection Columns { get { return mvarColumns; } }

		public ColumnHeaderStyle HeaderStyle { get; set; } = ColumnHeaderStyle.Clickable;
		public TreeModelRow.TreeModelSelectedRowCollection SelectedRows { get; private set; } = null;

		public ListViewMode Mode { get; set; } = ListViewMode.Detail;

		/// <summary>
		/// Hits the test.
		/// </summary>
		/// <returns>A <see cref="ListViewHitTestInfo" /> indicating the results of the hit test. For <see cref="ListView" /> instances with a <see cref="ControlImplementation" />, this method SHOULD NEVER return null.</returns>
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
	}
}

