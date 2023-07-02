using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBS.Framework.UserInterface.Drawing;

namespace MBS.Framework.UserInterface
{
	public static class SystemFonts
	{
		public static Font DefaultFont { get { return ((UIApplication)Application.Instance).Engine.GetSystemFont(SystemFont.DefaultFont); } }
		public static Font MenuFont { get { return ((UIApplication)Application.Instance).Engine.GetSystemFont(SystemFont.MenuFont); } }
		public static Font Monospace { get { return ((UIApplication)Application.Instance).Engine.GetSystemFont(SystemFont.Monospace); } }
	}
}
