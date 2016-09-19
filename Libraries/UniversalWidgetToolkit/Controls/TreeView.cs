using System;

namespace UniversalWidgetToolkit.Controls
{
	public class TreeView : Control
	{
		private TreeModel mvarModel = null;
		public TreeModel Model { get { return mvarModel; } set { mvarModel = value; } }
	}
}

