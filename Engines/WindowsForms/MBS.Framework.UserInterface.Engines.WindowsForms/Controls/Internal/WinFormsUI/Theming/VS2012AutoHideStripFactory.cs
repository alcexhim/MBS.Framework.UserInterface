using WeifenLuo.WinFormsUI.Docking;

namespace WeifenLuo.WinFormsUI.Theming
{
	internal class VS2012AutoHideStripFactory : DockPanelExtender.IAutoHideStripFactory
	{
		public AutoHideStripBase CreateAutoHideStrip(DockPanel panel)
		{
			return new ThemedAutoHideStrip(panel);
		}
	}
}
