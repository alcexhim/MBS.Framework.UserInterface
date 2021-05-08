using System;
namespace MBS.Framework.UserInterface.Controls.WebBrowser
{
	namespace Native
	{
		public interface IWebBrowserControlImplementation
		{
			void LoadHTMLFromString(string value, Uri baseUri);
		}
	}
	public class WebBrowserControl : SystemControl
	{
		public WebBrowserControl()
		{
		}

		public event WebBrowserDialogEventHandler Dialog;
		protected virtual void OnDialog(WebBrowserDialogEventArgs e)
		{
			Dialog?.Invoke(this, e);
		}

		public void LoadHTML(string value, Uri baseUri = null)
		{
			(ControlImplementation as Native.IWebBrowserControlImplementation).LoadHTMLFromString(value, baseUri);
		}
	}
}
