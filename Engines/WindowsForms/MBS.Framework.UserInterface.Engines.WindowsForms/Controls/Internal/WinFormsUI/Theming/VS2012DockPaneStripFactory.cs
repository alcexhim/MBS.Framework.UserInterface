using WeifenLuo.WinFormsUI.Docking;

namespace WeifenLuo.WinFormsUI.Theming
{
	internal class VS2012DockPaneStripFactory : DockPanelExtender.IDockPaneStripFactory
	{
		public DockPaneStripBase CreateDockPaneStrip(DockPane pane)
		{
			return new ThemedDockPaneStrip(pane);
		}
	}
}
