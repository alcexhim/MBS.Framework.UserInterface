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

			Button button = new Button(StockType.OK);
			// button.Margin = new Padding(8);
			button.Click += button_Click;

			Button button2 = new Button(StockType.Cancel);
			button2.Click += button2_Click;
			button2.Margin = new Padding(8);

			this.Bounds = new Drawing.Rectangle(320, 240, 600, 400);
			containerButtons.Controls.Add(button);
			containerButtons.Controls.Add(button2);

			this.Controls.Add(containerButtons);

			// layout.SetControlMinimumSize(button, new Dimension2D(75, 23));
			// layout.SetControlMinimumSize(button2, new Dimension2D(75, 23));

			// layout.SetControlBounds(button, new Rectangle(24, 24, 96, 24));

			this.ClassName = "FuckingAwesomeFormClass";
			this.Layout = layout;
			this.Text = "Test Application";

			this.MenuBar.Items.AddRange(new MenuItem[]
			{
				new CommandMenuItem("_File", new MenuItem[]
				{
					new CommandMenuItem("_New", new MenuItem[]
					{
						new CommandMenuItem("New _Document"),
						new CommandMenuItem("New _Project")
					}),
					new CommandMenuItem("_Open", new MenuItem[]
					{
						new CommandMenuItem("Open _Document", null, delegate(object sender, EventArgs e)
						{
							FileDialog dlg = new FileDialog();
							dlg.Mode = FileDialogMode.Open;
							dlg.MultiSelect = true;
							if (dlg.ShowDialog() == CommonDialogResult.OK) {
							}
						}),
						new CommandMenuItem("Open _Project")
					}),
					new CommandMenuItem("_Save", new MenuItem[]
					{
						new CommandMenuItem("Save _Document"),
						new CommandMenuItem("Save _Project")
					}),
					new SeparatorMenuItem(),
					new CommandMenuItem("E_xit", null, delegate(object sender, EventArgs e)
					{
						Application.Stop ();
					})
				}),
				new CommandMenuItem("_Edit", new MenuItem[]
				{
					new CommandMenuItem("_Undo"),
					new CommandMenuItem("_Redo"),
					new SeparatorMenuItem(),
					new CommandMenuItem("Cu_t"),
					new CommandMenuItem("_Copy"),
					new CommandMenuItem("_Paste"),
					new SeparatorMenuItem(),
					new CommandMenuItem("_Select All"),
					new CommandMenuItem("_Invert Selection")
				}),
				new CommandMenuItem("_View", new MenuItem[]
				{
					new CommandMenuItem("_Toolbars"),
					new CommandMenuItem("Status _Bar"),
					new SeparatorMenuItem(),
					new CommandMenuItem("_Refresh")
				}),
				new CommandMenuItem("_Tools", new MenuItem[]
				{
					new CommandMenuItem("Select _Color", null, delegate (object sender, EventArgs e)
					{
						ColorDialog dlg = new ColorDialog();
						if (dlg.ShowDialog() == CommonDialogResult.OK)
						{
							Color color = dlg.SelectedColor;
						}
					}),
					new CommandMenuItem("Select _Font", null, delegate (object sender, EventArgs e)
					{
						FontDialog dlg = new FontDialog();
						// dlg.AutoUpgradeEnabled = false;
						if (dlg.ShowDialog() == CommonDialogResult.OK)
						{
							MessageDialog.ShowDialog(dlg.SelectedFont.ToString());
						}
					})
				}),
				new CommandMenuItem("_Help", new MenuItem[]
				{
					new CommandMenuItem("_About", null, delegate (object sender, EventArgs e)
					{
						AboutDialog dlg = new AboutDialog();
						dlg.ProgramName = "Universal Widget Toolkit test application";
						dlg.Version = new Version(1, 0);
						dlg.Copyright = "Copyright (c) 1997-2016 Mike Becker's Software";
						dlg.Comments = "Provides a way to test various elements of the Universal Widget Toolkit on various operating systems.";
						dlg.LicenseType = LicenseType.BSD;
						dlg.Website = "http://www.alce.io/uwt";
						dlg.ShowDialog();
					})
				})
			});
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
