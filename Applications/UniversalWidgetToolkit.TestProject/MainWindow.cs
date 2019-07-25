﻿using System;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Dialogs;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Layouts;

using MBS.Framework.Drawing;

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

			TabContainer tbsTabs = new TabContainer ();
			Button cmdCloseTab = new Button();
			cmdCloseTab.AlwaysShowImage = true;
			cmdCloseTab.StockType = ButtonStockType.Close;
			cmdCloseTab.BorderStyle = ButtonBorderStyle.Half;
			cmdCloseTab.Click += (sender, e) =>
			{
				Button sb = (sender as Button);
				TabPage tabPageParent = (sb.Parent.Parent as TabPage);
				MessageDialog.ShowDialog("Closing tab '" + tabPageParent.Text + "'");
			};
			tbsTabs.TabTitleControls.Add(cmdCloseTab);

			TabPage tabGeneral = new TabPage ();
			tabGeneral.Text = "General";
			tabGeneral.Layout = new BoxLayout(Orientation.Vertical);
			tabGeneral.Controls.Add(new Label("Test"), new BoxLayout.Constraints(true, true));

			Button btn1 = new Button(ButtonStockType.OK, DialogResult.None);
			btn1.Click += (sender, e) =>
			{
				MessageDialog.ShowDialog("You clicked the button!");
			};
			tabGeneral.Controls.Add(btn1);

			tbsTabs.TabPages.Add (tabGeneral);
			
			TabPage tabCodeEditor = new TabPage ();
			tabCodeEditor.Layout = new BoxLayout (Orientation.Vertical);
			tabCodeEditor.Text = "Code Editor";
			SyntaxTextBox txtCodeEditor = new SyntaxTextBox ();
			// txtCodeEditor.Multiline = true;
			tabCodeEditor.Controls.Add (txtCodeEditor, new BoxLayout.Constraints(true, true));
			tbsTabs.TabPages.Add (tabCodeEditor);

			TabPage tabTreeView = new TabPage();
			tabTreeView.Layout = new BoxLayout(Orientation.Horizontal);
			tabTreeView.Text = "Tree View Test";

			ListView tv = new ListView();

			DefaultTreeModel tm = new DefaultTreeModel(new Type[]
			{
				typeof(String),
				typeof(Int32)
			});


			tv.Model = tm;
			tv.Columns.Add(new ListViewColumnText(tm.Columns[0], "Name"));
			tv.Columns.Add(new ListViewColumnText(tm.Columns[1], "Age"));

			tm.Rows.Add(new TreeModelRow(new TreeModelRowColumn[]
			{
				new TreeModelRowColumn(tm.Columns[0], "Heinz El-Mann"),
				new TreeModelRowColumn(tm.Columns[1], 51)
			}));
			tm.Rows[0].Rows.Add(new TreeModelRow(new TreeModelRowColumn[]
			{
				new TreeModelRowColumn(tm.Columns[0], "Franz El-Mann"),
				new TreeModelRowColumn(tm.Columns[1], 22)
			}));
			tm.Rows[0].Rows.Add(new TreeModelRow(new TreeModelRowColumn[]
			{
				new TreeModelRowColumn(tm.Columns[0], "Another one"),
				new TreeModelRowColumn(tm.Columns[1], 25)
			}));
			tm.Rows.Add(new TreeModelRow(new TreeModelRowColumn[]
			{
				new TreeModelRowColumn(tm.Columns[0], "Jane Doe"),
				new TreeModelRowColumn(tm.Columns[1], 23)
			}));
			tm.Rows.Add(new TreeModelRow(new TreeModelRowColumn[]
			{
				new TreeModelRowColumn(tm.Columns[0], "Joe Bungop"),
				new TreeModelRowColumn(tm.Columns[1], 91)
			}));

			tabTreeView.Controls.Add(tv);

			tbsTabs.TabPages.Add(tabTreeView);


			TabPage tabCustom = new TabPage();
			tabCustom.Layout = new BoxLayout(Orientation.Vertical);
			TestCustomControl ctl = new TestCustomControl();

			tabCustom.Text = "Custom Control";

			Container cmdCustomButtons = new Container();
			cmdCustomButtons.Layout = new BoxLayout(Orientation.Horizontal, 8, true);

			Button cmdCustomAdd = new Button("Add Green Box");
			cmdCustomAdd.Click += (sender, e) =>
			{
				ctl.ShowGreenBox = true;
				// we do not needs this Invalidate call ?
				// ctl.Invalidate();
			};
			cmdCustomButtons.Controls.Add(cmdCustomAdd, new BoxLayout.Constraints(false, false, 8));
			Button cmdCustomRemove = new Button("Remove Green Box");
			cmdCustomRemove.Click += (sender, e) =>
			{
				ctl.ShowGreenBox = false;
				// we do not needs this Invalidate call ?
				// ctl.Invalidate();
			};
			cmdCustomButtons.Controls.Add(cmdCustomRemove, new BoxLayout.Constraints(false, false, 8));

			tabCustom.Controls.Add(cmdCustomButtons);
			tabCustom.Controls.Add(ctl);

			tbsTabs.TabPages.Add(tabCustom);

			this.Controls.Add (tbsTabs);

			BoxLayout layout = new BoxLayout(Orientation.Vertical);
			layout.SetControlConstraints (tbsTabs, new BoxLayout.Constraints (true, true));
			this.Layout = layout;

			/*
			// FlowLayout layout = new FlowLayout();
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

			containerButtons.Controls.Add(button);
			containerButtons.Controls.Add(button2);

			this.Controls.Add(containerButtons);
			*/

			// layout.SetControlMinimumSize(button, new Dimension2D(75, 23));
			// layout.SetControlMinimumSize(button2, new Dimension2D(75, 23));

			// layout.SetControlBounds(button, new Rectangle(24, 24, 96, 24));
			
			this.Bounds = new Drawing.Rectangle(320, 240, 600, 400);
			this.ClassName = "FuckingAwesomeFormClass";
			// this.Layout = layout;
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
							if (dlg.ShowDialog() == DialogResult.OK) {
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
					new CommandMenuItem("_Print", null, delegate(object sender, EventArgs e)
					{
						PrintDialog dlg = new PrintDialog();
						dlg.ShowDialog();
					}, new Shortcut(Input.Keyboard.KeyboardKey.P, Input.Keyboard.KeyboardModifierKey.Control)),
					new SeparatorMenuItem(),
					new CommandMenuItem("E_xit", null, delegate(object sender, EventArgs e)
					{
						Application.Stop ();
					}, new Shortcut(Input.Keyboard.KeyboardKey.Q, Input.Keyboard.KeyboardModifierKey.Control))
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
					new CommandMenuItem("_Ask a Question", null, delegate (object sender, EventArgs e)
					{
						DialogResult result = MessageDialog.ShowDialog("Do you want to frob the widgitator?", "Frob the Widgitator", MessageDialogButtons.YesNo, MessageDialogIcon.Question, MessageDialogModality.ApplicationModal, false, this);
						switch (result)
						{
							case DialogResult.Yes:
							{
								MessageDialog.ShowDialog ("Widgitator frobnicated successfully.", "Good News", MessageDialogButtons.OK, MessageDialogIcon.Information, MessageDialogModality.ApplicationModal, false, this);
								break;
							}
							case DialogResult.No:
							{
								MessageDialog.ShowDialog ("Widgitator not frobnicated. Try again later.", "As you wish", MessageDialogButtons.OK, MessageDialogIcon.Warning, MessageDialogModality.ApplicationModal, false, this);
								break;
							}
						}
					}),
					new CommandMenuItem("Select _Color", null, delegate (object sender, EventArgs e)
					{
						ColorDialog dlg = new ColorDialog();
						if (dlg.ShowDialog() == DialogResult.OK)
						{
							Color color = dlg.SelectedColor;
                            MessageDialog.ShowDialog("You selected color " + dlg.SelectedColor.ToString());
						}
					}),
					new CommandMenuItem("Select _Font", null, delegate (object sender, EventArgs e)
					{
						FontDialog dlg = new FontDialog();
						// dlg.AutoUpgradeEnabled = false;
						if (dlg.ShowDialog() == DialogResult.OK)
						{
							MessageDialog.ShowDialog(dlg.SelectedFont.ToString());
						}
					}),
					new CommandMenuItem("Select _Application", null, delegate (object sender, EventArgs e)
					{
						AppChooserDialog dlg = new AppChooserDialog();
						dlg.ContentType = "text/plain";
						// dlg.AutoUpgradeEnabled = false;
						if (dlg.ShowDialog() == DialogResult.OK)
						{
							// MessageDialog.ShowDialog(dlg.SelectedFont.ToString());
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

		protected override void OnClosed (EventArgs e)
		{
			base.OnClosed (e);

			Application.Stop ();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Application.Stop();
		}
	}
}
