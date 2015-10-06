using System;
using System.Collections.Generic;
using System.Linq;

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
			Window window = new Window();
			window.ClassName = "FuckingAwesomeFormClass";
			window.Title = "Test Application";
			window.Show();

			Application.Start();
		}
	}
}
