using System;
using System.Diagnostics;
using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit
{
	public class Dialog : Window
	{
		private Control mvarParent = null;
		public new Control Parent { get { return mvarParent; } set { mvarParent = value; } }

		private Button.ButtonCollection mvarButtons = new Button.ButtonCollection ();
		public Button.ButtonCollection Buttons { get { return mvarButtons; } }

		private Button mvarDefaultButton = null;
		public Button DefaultButton { get { return mvarDefaultButton; } set { mvarDefaultButton = value; } }

		[DebuggerNonUserCode()]
		public DialogResult ShowDialog(Window parent = null)
		{
			return Application.Engine.ShowDialog(this, parent);
		}
	}
}

