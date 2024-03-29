using System;

namespace MBS.Framework.UserInterface
{
	public abstract class TreeModel
	{
		public TreeModelColumn.TreeModelColumnCollection Columns { get; private set; } = null;

		public TreeModel(Type[] columnTypes)
		{
			Columns = new TreeModelColumn.TreeModelColumnCollection(this);
			foreach (Type t in columnTypes)
			{
				TreeModelColumn c = new TreeModelColumn(t);
				Columns.Add(c);
			}
		}

		protected abstract TreeModelRow FindInternal(object value);
		public TreeModelRow Find(object value)
		{
			return FindInternal(value);
		}

		public event TreeModelRowCompareEventHandler RowCompare;
		protected virtual void OnRowCompare(TreeModelRowCompareEventArgs e)
		{
			RowCompare?.Invoke(this, e);
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
				if (!((UIApplication)Application.Instance).Engine.TreeModelManager.IsTreeModelCreated(this))
				{
					((UIApplication)Application.Instance).Engine.TreeModelManager.CreateTreeModel(this);
					// exit early to prevent TreeModelManager from attempting to add duplicate rows in the switch statement a few lines down
					// TreeModelManager.CreateTreeModel automatically adds all existing rows to the underlying native tree model
					return;
				}

				switch (e.Action)
				{
					case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
						{
							TreeModelRow[] items = new TreeModelRow[e.NewItems.Count];
							for (int i = 0; i < e.NewItems.Count; i++)
							{
								items[i] = (e.NewItems[i] as TreeModelRow);
								((UIApplication)Application.Instance).Engine.TreeModelManager.CreateTreeModelRow(e.NewItems[i] as TreeModelRow, this);
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
					OnTreeModelChanged(ee);
				}
			};
		}

		public static DefaultTreeModel Create(int columnCount)
		{
			Type[] types = new Type[columnCount];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = typeof(string);
			}
			return new DefaultTreeModel(types);
		}

		private TreeModelRow RecursiveCreateTreeModelRow(TreeModelColumn displayColumn, string[] titles, TreeModelRowColumn[] additionalColumns, TreeModelRow parent, int titleIndex)
		{
			TreeModelRow theRow = null;
			TreeModelRow.TreeModelRowCollection coll = null;
			if (parent == null)
			{
				coll = Rows;
			}
			else
			{
				coll = parent.Rows;
			}

			for (int i = 0; i < coll.Count; i++)
			{
				if (coll[i].RowColumns[displayColumn].Value.Equals(titles[titleIndex]))
				{
					theRow = coll[i];
					break;
				}
			}

			if (theRow == null)
			{
				theRow = new TreeModelRow(new TreeModelRowColumn[]
				{
					new TreeModelRowColumn(displayColumn, titles[titleIndex])
				});

				for (int i = 0; i < additionalColumns.Length; i++)
					theRow.RowColumns.Add(additionalColumns[i]);

				coll.Add(theRow);
			}

			if (titleIndex == titles.Length - 1)
			{
				return theRow;
			}
			return RecursiveCreateTreeModelRow(displayColumn, titles, additionalColumns, theRow, titleIndex + 1);
		}
		public TreeModelRow RecursiveCreateTreeModelRow(TreeModelColumn displayColumn, string[] titles, TreeModelRowColumn[] additionalColumns = null)
		{
			return RecursiveCreateTreeModelRow(displayColumn, titles, additionalColumns, null, 0);
		}

		public event TreeModelChangedEventHandler TreeModelChanged;
		protected virtual void OnTreeModelChanged(TreeModelChangedEventArgs e)
		{
			TreeModelChanged?.Invoke(this, e);
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
	}
}
