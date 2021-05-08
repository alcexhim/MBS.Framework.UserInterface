using System;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.CustomListView
{
	public delegate void ListViewItemDragEventHandler(object sender, ListViewItemDragEventArgs e);
	public class ListViewItemDragEventArgs : EventArgs
	{
		public System.Windows.Forms.IDataObject DataObject { get; set; } = null;
		public System.Windows.Forms.DragDropEffects Effects { get; set; } = System.Windows.Forms.DragDropEffects.None;
	}
}
