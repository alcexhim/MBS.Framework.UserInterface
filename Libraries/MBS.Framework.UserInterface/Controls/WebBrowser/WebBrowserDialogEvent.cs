using System;
using System.ComponentModel;

namespace MBS.Framework.UserInterface.Controls.WebBrowser
{
	public class WebBrowserDialogEventArgs : CancelEventArgs
	{
		public WebBrowserDialogEventArgs(WebBrowserDialogType dialogType)
		{
			DialogType = dialogType;
		}

		public WebBrowserDialogType DialogType { get; private set; }
		public string Content { get; set; } = null;
	}
	public delegate void WebBrowserDialogEventHandler(object sender, WebBrowserDialogEventArgs e);
}
