using System;
using System.ComponentModel;

namespace MBS.Framework.UserInterface.Dialogs
{
	public class UpdateDropDownListEventArgs : CancelEventArgs
	{
		public string Query { get; private set; } = null;
		public System.Collections.Generic.List<object[]> Items { get; } = new System.Collections.Generic.List<object[]>();

		public UpdateDropDownListEventArgs(string query)
		{
			Query = query;
		}
	}
}
