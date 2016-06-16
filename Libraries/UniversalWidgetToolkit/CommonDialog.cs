using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit
{
	public enum CommonDialogResult
	{
		None = 0,
		OK,
		Cancel,
		Yes,
		No,
		Abort,
		Retry,
		Ignore,
		TryAgain,
		Continue,
		Help
	}
	public class CommonDialog
	{
		private Control mvarParent = null;
		public Control Parent { get { return mvarParent; } set { mvarParent = value; } }

		public CommonDialogResult ShowDialog()
		{
			return Application.Engine.ShowDialog(this);
		}
	}
}
