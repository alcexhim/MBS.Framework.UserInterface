using System;
namespace UniversalWidgetToolkit
{
	public class TreeModelRowColumn
	{
		public class TreeModelRowColumnCollection
			: System.Collections.ObjectModel.Collection<TreeModelRowColumn>
		{

		}

		private TreeModelColumn mvarColumn = null;
		public TreeModelColumn Column { get { return mvarColumn; } }
		private object mvarValue = null;
		public object Value { get { return mvarValue; } set { mvarValue = value; } }

		public TreeModelRowColumn(TreeModelColumn column, object value)
		{
			mvarColumn = column;
			mvarValue = value;
		}
	}
}
