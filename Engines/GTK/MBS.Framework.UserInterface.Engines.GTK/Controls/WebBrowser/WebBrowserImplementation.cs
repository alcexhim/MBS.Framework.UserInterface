using System;
using MBS.Framework.UserInterface.Controls.WebBrowser;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls.WebBrowser
{
	[ControlImplementation(typeof(WebBrowserControl))]
	public class WebBrowserImplementation : GTKNativeImplementation, MBS.Framework.UserInterface.Controls.WebBrowser.Native.IWebBrowserControlImplementation
	{
		public WebBrowserImplementation(Engine engine, WebBrowserControl control) : base(engine, control)
		{
			script_dialog_d = new Func<IntPtr, IntPtr, bool>(script_dialog);
		}

		public void LoadHTMLFromString(string content, Uri baseUri = null)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.WebKit.Methods.webkit_web_view_load_html(handle, content, baseUri?.ToString() ?? String.Empty);
		}

		private Func<IntPtr, IntPtr, bool> script_dialog_d = null;
		private bool script_dialog(IntPtr /*WebKitWebView*/ web_view, IntPtr /*WebKitScriptDialog*/ dialog)
		{
			Internal.WebKit.Constants.WebKitScriptDialogType type = Internal.WebKit.Methods.webkit_script_dialog_get_dialog_type(dialog);
			string message = Internal.WebKit.Methods.webkit_script_dialog_get_message(dialog);

			WebBrowserDialogEventArgs e = new WebBrowserDialogEventArgs(WebKitScriptDialogTypeToWebBrowserDialogType(type));
			e.Content = message;
			OnDialog(e);
			return e.Cancel;
		}

		private WebBrowserDialogType WebKitScriptDialogTypeToWebBrowserDialogType(Internal.WebKit.Constants.WebKitScriptDialogType type)
		{
			switch (type)
			{
				case Internal.WebKit.Constants.WebKitScriptDialogType.Alert: return WebBrowserDialogType.Alert;
				case Internal.WebKit.Constants.WebKitScriptDialogType.BeforeUnloadConfirm: return WebBrowserDialogType.ConfirmExit;
				case Internal.WebKit.Constants.WebKitScriptDialogType.Confirm: return WebBrowserDialogType.Confirm;
				case Internal.WebKit.Constants.WebKitScriptDialogType.Prompt: return WebBrowserDialogType.Prompt;
			}
			throw new ArgumentOutOfRangeException();
		}

		protected virtual void OnDialog(WebBrowserDialogEventArgs e)
		{
			InvokeMethod((Control as WebBrowserControl), "OnDialog", new object[] { e });
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			IntPtr handle = Internal.WebKit.Methods.webkit_web_view_new();

			Internal.GObject.Methods.g_signal_connect(handle, "script_dialog", script_dialog_d);
			return new GTKNativeControl(handle);
		}
	}
}
