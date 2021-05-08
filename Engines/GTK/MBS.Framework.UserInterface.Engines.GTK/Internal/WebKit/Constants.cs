using System;
namespace MBS.Framework.UserInterface.Engines.GTK.Internal.WebKit
{
	internal static class Constants
	{
		public enum WebKitScriptDialogType
		{
			Alert = 0,
			Confirm = 1,
			Prompt = 2,
			BeforeUnloadConfirm = 3
		}
	}
}
