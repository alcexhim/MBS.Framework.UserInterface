using System.Drawing;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Theming
{
	public class FontTable
	{
		public Font Default { get; set; } = System.Drawing.SystemFonts.MenuFont;
		public Font CommandBar { get; set; } = System.Drawing.SystemFonts.MenuFont;
		public Font DialogFont { get; set; } = System.Drawing.SystemFonts.MenuFont;
		public Font DocumentTabTextSelected { get; set; } = null;
	}
}
