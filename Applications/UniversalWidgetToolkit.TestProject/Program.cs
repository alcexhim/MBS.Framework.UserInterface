using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Dialogs;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Layouts;

namespace UniversalWidgetToolkit.TestProject
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AbsoluteLayout layout = new AbsoluteLayout();

			Monitor[] monitors = Monitor.Get();

			Button button = new Button();
			button.Title = "&OK";

			Window window = new Window();

			window.Bounds = new Drawing.Rectangle(320, 240, 400, 300);
			window.Controls.Add(button);

			layout.SetControlBounds(button, new Rectangle(24, 24, 96, 24));

			window.ClassName = "FuckingAwesomeFormClass";
			window.Layout = layout;
			window.Title = "Test Application";
			window.Closing += window_Closing;
			window.Show();

			int nExitCode = Application.Start(window);
		}

		private static void window_Closing(object sender, CancelEventArgs e)
		{
			if (MessageDialog.ShowDialog("Prevent shutdown?", "HELP!!!", MessageDialogButtons.YesNo, MessageDialogIcon.None, MessageDialogModality.ApplicationModal, false, (sender as Control)) == CommonDialogResult.Yes)
			{
				Application.Stop(3257);
			}
		}
	}
}
