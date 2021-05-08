namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls.Internal.CustomListView
{
	public class ListViewHitTestInfo
	{
		public ListViewItem Item { get; } = null;

		public ListViewHitTestInfo(ListViewItem item)
		{
			Item = item;
		}
	}
}
