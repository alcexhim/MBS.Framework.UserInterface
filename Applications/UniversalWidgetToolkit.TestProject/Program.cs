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
			Application.Engine.SetProperty("Windowless", true);

			Theming.ThemeManager.CurrentTheme = Theming.ThemeManager.GetByID(new Guid("{4D86F538-E277-4E6F-9CAC-60F82D49A19D}"));

			MainWindow window = new MainWindow();
			
			int nExitCode = Application.Start(window);
		}
	}
}
