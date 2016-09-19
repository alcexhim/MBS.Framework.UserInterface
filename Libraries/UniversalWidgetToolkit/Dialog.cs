using System;

using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit
{
	public class Dialog : Container
	{
		private string mvarTitle = String.Empty;
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private Control mvarParent = null;
		public Control Parent { get { return mvarParent; } set { mvarParent = value; } }

		private Button.ButtonCollection mvarButtons = new Button.ButtonCollection ();
		public Button.ButtonCollection Buttons { get { return mvarButtons; } }

		private Button mvarDefaultButton = null;
		public Button DefaultButton { get { return mvarDefaultButton; } set { mvarDefaultButton = value; } }

		public DialogResult ShowDialog()
		{
			return Application.Engine.ShowDialog(this);
		}
	}
}

