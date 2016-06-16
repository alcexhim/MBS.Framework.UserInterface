using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Dialogs;
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

			// FlowLayout layout = new FlowLayout();
			BoxLayout layout = new BoxLayout();
			layout.Orientation = Orientation.Vertical;
			layout.Spacing = 13;

			// this.MenuBar.Items.Add (new MenuItem ("Test"));

			Label label = new Label();
			label.Padding = new Padding (10);
			label.HorizontalAlignment = HorizontalAlignment.Left;
			label.Text = "System information goes here.";
			this.Controls.Add(label);

			Container containerButtons = new Container();
			containerButtons.Layout = new BoxLayout();
			(containerButtons.Layout as BoxLayout).Orientation = Orientation.Horizontal;
			(containerButtons.Layout as BoxLayout).Alignment = Alignment.Far;
			(containerButtons.Layout as BoxLayout).Spacing = 10;

			Button button = new Button(StockButtonType.OK);
			// button.Margin = new Padding(8);
			button.Click += button_Click;

			Button button2 = new Button(StockButtonType.Cancel);
			button2.Click += button2_Click;
			button2.Margin = new Padding(8);

			Button button3 = new Button();
			button3.Text = "&Apply";
			button3.Margin = new Padding(8);
			
			this.Bounds = new Drawing.Rectangle(320, 240, 600, 400);
			containerButtons.Controls.Add(button);
			containerButtons.Controls.Add(button2);
			containerButtons.Controls.Add(button3);

			this.Controls.Add(containerButtons);

			// layout.SetControlMinimumSize(button, new Dimension2D(75, 23));
			// layout.SetControlMinimumSize(button2, new Dimension2D(75, 23));

			// layout.SetControlBounds(button, new Rectangle(24, 24, 96, 24));

			this.ClassName = "FuckingAwesomeFormClass";
			this.Layout = layout;
			this.Text = "Test Application";
		}

		public override void OnClosed (EventArgs e)
		{
			base.OnClosed (e);

			Application.Stop ();
		}

		void button_Click(object sender, EventArgs e)
		{
			CommonDialogResult result = MessageDialog.ShowDialog("Do you want to frob the widgitator?", "Frob the Widgitator", MessageDialogButtons.YesNo, MessageDialogIcon.Question, MessageDialogModality.ApplicationModal, false, this);
			switch (result)
			{
				case CommonDialogResult.Yes:
				{
					MessageDialog.ShowDialog ("Widgitator frobnicated successfully.", "Good News", MessageDialogButtons.OK, MessageDialogIcon.Information, MessageDialogModality.ApplicationModal, false, this);
					break;
				}
				case CommonDialogResult.No:
				{
					MessageDialog.ShowDialog ("Widgitator not frobnicated. Try again later.", "As you wish", MessageDialogButtons.OK, MessageDialogIcon.Warning, MessageDialogModality.ApplicationModal, false, this);
					break;
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Application.Stop();
		}
	}
}
