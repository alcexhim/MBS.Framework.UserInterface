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
	}
	public class DefaultTreeModel : TreeModel
	{
		public DefaultTreeModel(Type[] columnTypes)
			: base(columnTypes)
		{
			this.Rows = new TreeModelRow.TreeModelRowCollection();
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

	}
}

