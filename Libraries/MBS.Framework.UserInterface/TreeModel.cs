using System;

namespace MBS.Framework.UserInterface
{
	public abstract class TreeModel
	{
		private TreeModelColumn.TreeModelColumnCollection mvarColumns = new TreeModelColumn.TreeModelColumnCollection();
		public TreeModelColumn.TreeModelColumnCollection Columns { get { return mvarColumns; } }

		public TreeModel(Type[] columnTypes)
		{
			foreach (Type t in columnTypes)
			{
				TreeModelColumn c = new TreeModelColumn(t);
				mvarColumns.Add(c);
			}
		}

		protected abstract TreeModelRow FindInternal(object value);
		public TreeModelRow Find(object value)
		{
			return FindInternal(value);
		}
	}
	public class DefaultTreeModel : TreeModel
	{
		public DefaultTreeModel(Type[] columnTypes)
			: base(columnTypes)
		{
			this.Rows = new TreeModelRow.TreeModelRowCollection(this);
			this.Rows.CollectionChanged += (sender, e) =>
			{
				TreeModelChangedEventArgs ee = null;

				switch (e.Action)
				{
					case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					{
						TreeModelRow[] items = new TreeModelRow[e.NewItems.Count];
						for (int i = 0; i < e.NewItems.Count; i++)
						{
								items[i] = (e.NewItems[i] as TreeModelRow);
						}
						ee = new TreeModelChangedEventArgs(TreeModelChangedAction.Add, items, (TreeModelRow)null);
						break;
					}
					case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
					{
							Console.WriteLine("NotifyCollection: treemodel: move not supported");
						break;
					}
					case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					{
							TreeModelRow[] items = new TreeModelRow[e.OldItems.Count];
							for (int i = 0; i < e.OldItems.Count; i++)
							{
								items[i] = (e.OldItems[i] as TreeModelRow);
							}
							ee = new TreeModelChangedEventArgs(TreeModelChangedAction.Remove, items, (TreeModelRow)null);
							break;
					}
					case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					{
						ee = new TreeModelChangedEventArgs(TreeModelChangedAction.Clear);
						break;
					}
					case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
					{
						// ee = new TreeModelChangedEventArgs(e.Action, e.NewItems, e.OldItems, e.NewStartingIndex, (TreeModelRow)null);
						// not sure what to do here
						break;
					}
				}

				if (ee != null)
				{
					OnTreeModelChanged(sender, ee);
				}
			};
		}

		public event TreeModelChangedEventHandler TreeModelChanged;
		protected virtual void OnTreeModelChanged(object sender, TreeModelChangedEventArgs e)
		{
			TreeModelChanged?.Invoke(sender, e);
		}

		public TreeModelRow.TreeModelRowCollection Rows { get; private set; } = null;

		private TreeModelRow FindRecursive(object value, TreeModelRow where)
		{
			for (int i = 0; i < where.RowColumns.Count; i++)
			{
				if (where.RowColumns[i].Value == null)
					continue;

				if (where.RowColumns[i].Value is string && String.IsNullOrEmpty((where.RowColumns[i].Value as string)))
					continue;

				if (where.RowColumns[i].Value.Equals(value))
					return where;
			}

			for (int i = 0; i < where.Rows.Count; i++)
			{
				TreeModelRow ret = FindRecursive(value, where.Rows[i]);
				if (ret != null)
					return ret;
			}
			return null;
		}
		protected override TreeModelRow FindInternal(object value)
		{
			for (int i = 0; i < Rows.Count; i++)
			{
				TreeModelRow ret = FindRecursive(value, Rows[i]);
				if (ret != null)
					return ret;
			}
			return null;
		}

		public void ExpandAll()
		{
			for (int i = 0; i < Rows.Count; i++)
			{
				Rows[i].ExpandAll();
			}
		}

		public void CollapseAll()
		{
			for (int i = 0; i < Rows.Count; i++)
			{
				Rows[i].CollapseAll();
			}
		}
	}
}

