using System;

namespace MBS.Framework.UserInterface
{
	public class SessionEndingEventArgs : EventArgs
	{
		public string PreventReason { get; set; } = null;
	}
}
