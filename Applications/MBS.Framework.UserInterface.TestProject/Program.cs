using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.TestProject
{
	class Program : UIApplication
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main()
		{
			return (new Program()).Start();
		}

		public Program()
		{
			DpiAwareness = DpiAwareness.SystemAware;
			ShortName = "uwt-testproject";

			// ((UIApplication)Application.Instance).Engine.SetProperty("Windowless", true);

			Theming.ThemeManager.CurrentTheme = Theming.ThemeManager.GetByID(new Guid("{4D86F538-E277-4E6F-9CAC-60F82D49A19D}"));

			ConfigurationFileNameFilter = "*.uwtxml";
		}

		protected override void OnActivated(ApplicationActivatedEventArgs e)
		{
			base.OnActivated(e);

			MainWindow window = new MainWindow();
			window.Show();
		}

	}
}
