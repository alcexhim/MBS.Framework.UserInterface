using System;
namespace MBS.Framework.UserInterface
{
	public class TreeModelRowColumn
	{
		public class TreeModelRowColumnCollection
			: System.Collections.ObjectModel.Collection<TreeModelRowColumn>
		{
			private TreeModelRow _parent = null;
			public TreeModelRowColumnCollection(TreeModelRow parent)
			{
				_parent = parent;
			}

			protected override void ClearItems()
			{
				for (int i = 0; i < Count; i++)
					this[i].Parent = null;
				base.ClearItems();
			}
			protected override void InsertItem(int index, TreeModelRowColumn item)
			{
				base.InsertItem(index, item);
				item.Parent = _parent;
			}
			protected override void RemoveItem(int index)
			{
				this[index].Parent = null;
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, TreeModelRowColumn item)
			{
				this[index].Parent = null;
				base.SetItem(index, item);
				item.Parent = _parent;
			}
		}

		public TreeModelRow Parent { get; private set; } = null;

		private TreeModelColumn mvarColumn = null;
		public TreeModelColumn Column { get { return mvarColumn; } }
		private object mvarValue = null;
		public object Value
		{
			get { return mvarValue; }
			set
			{
				mvarValue = value;
				if (Parent != null)
					Parent.UpdateColumnValue(this);
			}
		}

		public TreeModelRowColumn(TreeModelColumn column, object value)
		{
			mvarColumn = column;
			mvarValue = value;
		}
	}
}
