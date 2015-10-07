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
			MainWindow window = new MainWindow();
			
			int nExitCode = Application.Start(window);
		}
	}
}
