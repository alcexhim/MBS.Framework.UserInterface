using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Layouts;

namespace UniversalWidgetToolkit.TestProject
{
	public class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Padding = new Padding(13);

			FlowLayout layout = new FlowLayout();
			
			Button button = new Button();
			button.Title = "&OK";
			button.Margin = new Padding(8);

			Button button2 = new Button();
			button2.Title = "&Cancel";
			button2.Margin = new Padding(8);
			
			this.Bounds = new Drawing.Rectangle(320, 240, 400, 300);
			this.Controls.Add(button);
			this.Controls.Add(button2);

			layout.SetControlMinimumSize(button, new Dimension2D(75, 23));
			layout.SetControlMinimumSize(button2, new Dimension2D(75, 23));

			// layout.SetControlBounds(button, new Rectangle(24, 24, 96, 24));

			this.ClassName = "FuckingAwesomeFormClass";
			this.Layout = layout;
			this.Title = "Test Application";
		}
	}
}
