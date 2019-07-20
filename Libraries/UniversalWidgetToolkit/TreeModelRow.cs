using System;
using System.Collections.Generic;
using UniversalEditor;
using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit
{
	public class TreeModelRow
	{
		public class TreeModelSelectedRowCollection
		{
			public ListView Parent { get; private set; }
			public TreeModelSelectedRowCollection(ListView parent)
			{
				Parent = parent;
			}

			private List<TreeModelRow> _list = new List<TreeModelRow> ();

			public void Add(TreeModelRow row)
			{
				OnItemRequested(new TreeModelRowItemRequestedEventArgs(row, 1, -1));
			}
			public TreeModelRow this[int index]
			{
				get
				{
					TreeModelRow item = (index < _list.Count ? _list[index] : null);
					TreeModelRowItemRequestedEventArgs e = new TreeModelRowItemRequestedEventArgs(item, _list.Count, index);
					OnItemRequested(e);
					if (e.Cancel) return item;
					return e.Item;
				}
			}
			public int Count
			{
				get
				{
					TreeModelRowItemRequestedEventArgs e = new TreeModelRowItemRequestedEventArgs(null, _list.Count, -1);
					OnItemRequested(e);
					if (e.Cancel) return _list.Count;
					return e.Count;
				}
			}

			public event TreeModelRowItemRequestedEventHandler ItemRequested;
			protected virtual void OnItemRequested(TreeModelRowItemRequestedEventArgs e)
			{
				ItemRequested?.Invoke(this, e);
			}
			public event EventHandler Cleared;
			protected virtual void OnCleared(EventArgs e)
			{ 
				Cleared?.Invoke(this, e);
			}

			public bool Contains(TreeModelRow tn)
			{
				TreeModelRowItemRequestedEventArgs e = new TreeModelRowItemRequestedEventArgs(tn, _list.Count, -1);
				OnItemRequested(e);
				Console.WriteLine("count: {0} item: {1}", e.Count, e.Item);
				if (e.Cancel) return _list.Contains(tn);
				if (e.Count == 0 || e.Item == null) return false;
				Console.WriteLine("found item: {0} requested item: {1}", e.Item, tn);
				return (e.Item == tn);
			}

			public void Clear()
			{
				OnCleared(EventArgs.Empty);
			}
		}

		public class TreeModelRowCollection
			: System.Collections.ObjectModel.ObservableCollection<TreeModelRow>
		{
			private Dictionary<string, TreeModelRow> _itemsByName = new Dictionary<string, TreeModelRow>();
			public TreeModelRow this[string name]
			{
				get
				{
					return _itemsByName[name];
				}
			}
			public bool Contains(string name)
			{
				return _itemsByName.ContainsKey(name);
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				_itemsByName.Clear();
			}
			protected override void InsertItem(int index, TreeModelRow item)
			{
				base.InsertItem(index, item);
				if (item.Name != null)
					_itemsByName[item.Name] = item;
			}
			protected override void RemoveItem(int index)
			{
				if (this[index].Name != null)
					_itemsByName.Remove(this[index].Name);
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, TreeModelRow item)
			{
				if (this[index].Name != null)
					_itemsByName.Remove(this[index].Name);
				base.SetItem(index, item);
				if (item.Name != null)
					_itemsByName[item.Name] = item;
			}

			public new int Count
			{
				get
				{
					TreeModelRowItemRequestedEventArgs e = new TreeModelRowItemRequestedEventArgs(null, base.Count, -1);
					OnItemRequested(e);
					if (e.Cancel) return base.Count;
					return e.Count;
				}
			}
			public new TreeModelRow this[int index]
			{
				get
				{
					TreeModelRow originalItem = index < base.Count ? base[index] : null;
					TreeModelRowItemRequestedEventArgs e = new TreeModelRowItemRequestedEventArgs(originalItem, base.Count, index);
					OnItemRequested(e);
					if (e.Cancel) return originalItem;
					return e.Item;
				}
			}

			public event TreeModelRowItemRequestedEventHandler ItemRequested;
			private void OnItemRequested(TreeModelRowItemRequestedEventArgs e)
			{
				ItemRequested?.Invoke(this, e);
			}
		}

		public TreeModelRow.TreeModelRowCollection Rows { get; } = new TreeModelRowCollection();

		private TreeModelRowColumn.TreeModelRowColumnCollection mvarRowColumns = new TreeModelRowColumn.TreeModelRowColumnCollection();
		public TreeModelRowColumn.TreeModelRowColumnCollection RowColumns { get { return mvarRowColumns; } }

		public TreeModelRow(TreeModelRowColumn[] rowColumns)
		{
			this.Rows.CollectionChanged += Rows_CollectionChanged;
			foreach (TreeModelRowColumn rc in rowColumns)
			{
				mvarRowColumns.Add(rc);
			}
		}

		public Control ParentControl { get; internal set; }
		public TreeModelRow ParentRow { get; private set; }
		public string Name { get; set; }

		void Rows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				{
					List<TreeModelRow> list = new List<TreeModelRow>();
					foreach (TreeModelRow row in e.NewItems)
					{
						Console.WriteLine("setting parent row");
						row.ParentRow = this;
						list.Add(row);
					}
					if (ParentControl != null)
					{
						(ParentControl.NativeImplementation as UniversalWidgetToolkit.Controls.Native.IListViewNativeImplementation)?.UpdateTreeModel(ParentControl.NativeImplementation.Handle, new TreeModelChangedEventArgs(TreeModelChangedAction.Add, list.ToArray(), this));
					}
					break;
				}
			}
		}

		private Dictionary<string, object> _ExtraData = new Dictionary<string, object>();
		public T GetExtraData<T>(string key, T defaultValue = default(T))
		{
			if (_ExtraData.ContainsKey(key))
				return (T)_ExtraData[key];
			return defaultValue;
		}
		public void SetExtraData<T>(string key, T value)
		{
			_ExtraData[key] = value;
		}
		public object GetExtraData(string key, object defaultValue = null)
		{
			return GetExtraData<object>(key, defaultValue);
		}
		public void SetExtraData(string key, object value)
		{
			SetExtraData<object>(key, value);
		}
	}
}
