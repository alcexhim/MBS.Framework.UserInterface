using System;

using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit.Dialogs
{
	public class FontDialog : CommonDialog
	{
		private bool mvarAutoUpgradeEnabled = true;
		public bool AutoUpgradeEnabled { get { return mvarAutoUpgradeEnabled; } set { mvarAutoUpgradeEnabled = value; } }

		private Font mvarSelectedFont = null;
		public Font SelectedFont { get { return mvarSelectedFont; } set { mvarSelectedFont = value; } }
	}
}

