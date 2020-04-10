using System;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Layouts;

using MBS.Framework.Drawing;

namespace MBS.Framework.UserInterface.Dialogs
{
	public enum SettingsDialogAppearance
	{
		System,
		/// <summary>
		/// The settings categories are displayed with a flat list of top-level categories in the sidebar, and lower-level categories as headers in the main control area.
		/// </summary>
		GNOME,
		/// <summary>
		/// The settings categories are displayed with a tree list of multi-level categories in the sidebar.
		/// </summary>
		VisualStudio
	}

	/// <summary>
	/// A dialog for configuring Universal Widget Toolkit <see cref="Option" />s.
	/// Option groups defined in <see cref="OptionProvider"/>s added to the <see cref="OptionProviders" /> collection will appear in this dialog for configuration.
	/// </summary>
	public class SettingsDialog : Dialog
	{
		private DefaultTreeModel tmOptionGroups = null;
		private ListView tv = null;
		private SplitContainer vpaned = null;

		private StackSidebar sidebar = null;

		public SettingsDialog()
		{
			tmOptionGroups = new DefaultTreeModel (new Type[] { typeof(string) });

			this.Layout = new BoxLayout(Orientation.Vertical);

			this.Buttons.Add(new Button(StockType.OK, DialogResult.OK));
			this.Buttons.Add(new Button(StockType.Cancel, DialogResult.Cancel));

			this.Buttons[0].Click += cmdOK_Click;

			this.DefaultButton = this.Buttons[0];

			this.Text = "Options";
			this.Size = new Dimension2D(600, 400);

			foreach (SettingsProvider provider in Application.SettingsProviders) {
				this.SettingsProviders.Add (provider);
			}
		}

		private void cmdOK_Click (object sender, EventArgs e)
		{
			if (sidebar == null) {
				foreach (Container ct in vpaned.Panel2.Controls) {
					foreach (Control ctl in ct.Controls) {
						SaveSettingForControl (ctl);
					}
				}
			} else {
				foreach (StackSidebarPanel panel in sidebar.Items) {
					Container ct = (panel.Control as Container);
					if (ct == null)
						continue;
					
					foreach (Control ctl in ct.Controls) {
						SaveSettingForControl (ctl);
					}
				}
			}
		}

		private void SaveSettingForControl(Control ctl)
		{
			if (ctl is Label)
				return;

			Setting setting = ctl.GetExtraData<Setting> ("setting");
			if (setting == null)
				return;

			if (ctl is CheckBox) {
				setting.SetValue ((ctl as CheckBox).Checked);
			} else if (ctl is TextBox) {
				setting.SetValue ((ctl as TextBox).Text);
			}
		}

		private void CreateVSLayout()
		{
			vpaned = new SplitContainer(Orientation.Vertical);
			vpaned.Panel1.Layout = new Layouts.BoxLayout(Orientation.Horizontal);
			vpaned.Panel2.Layout = new Layouts.BoxLayout(Orientation.Horizontal);

			vpaned.SplitterPosition = 140;

			this.Controls.Add(vpaned, new BoxLayout.Constraints(true, true, 0, BoxLayout.PackType.Start));

			tv = new ListView();
			tv.Model = tmOptionGroups;
			tv.Columns.Add (new ListViewColumnText (tmOptionGroups.Columns [0], "Group"));
			tv.HeaderStyle = ColumnHeaderStyle.None;
			tv.SelectionChanged += tv_SelectionChanged;

			vpaned.Panel1.Controls.Add(tv, new BoxLayout.Constraints(true, true));
		}
		private void CreateGNOMELayout()
		{
			sidebar = new StackSidebar ();
			sidebar.Style.Classes.Add ("view");

			Controls.Add (sidebar, new BoxLayout.Constraints (true, true));
		}

		/// <summary>
		/// Contains the <see cref="OptionProvider" />s used to display options in this <see cref="OptionsDialog" />.
		/// </summary>
		/// <value>The collection of <see cref="OptionProvider" />s used to display options in this <see cref="OptionsDialog" />.</value>
		public SettingsProvider.SettingsProviderCollection SettingsProviders { get; } = new SettingsProvider.SettingsProviderCollection();

		public SettingsDialogAppearance Appearance { get; set; } = SettingsDialogAppearance.System;

		Container ctDefault = new Container ();
		internal protected override void OnCreating (EventArgs e)
		{
			base.OnCreating (e);

			if (Appearance == SettingsDialogAppearance.System)
			{
				switch (Environment.OSVersion.Platform)
				{
					case PlatformID.MacOSX:
					case PlatformID.Unix:
					{
						CreateGNOMELayout();
						break;
					}
					case PlatformID.Win32NT:
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.WinCE:
					case PlatformID.Xbox:
					{
						CreateVSLayout();
						break;
					}
				}
			}
			else if (Appearance == SettingsDialogAppearance.GNOME)
			{
				CreateGNOMELayout();
			}
			else if (Appearance == SettingsDialogAppearance.VisualStudio)
			{
				CreateVSLayout();
			}

			Label lblNoOptions = new Label ("The selected group has no options available to configure");
			lblNoOptions.HorizontalAlignment = HorizontalAlignment.Center;
			lblNoOptions.VerticalAlignment = VerticalAlignment.Middle;
			ctDefault.Controls.Add(lblNoOptions, new BoxLayout.Constraints(true, true));

			if (sidebar == null) {
				vpaned.Panel2.Controls.Add (ctDefault, new BoxLayout.Constraints (true, true));
			} else {
			}

			System.Collections.Generic.List<SettingsGroup> grps = new System.Collections.Generic.List<SettingsGroup> ();
			foreach (SettingsProvider provider in SettingsProviders) {
				provider.Initialize();
				foreach (SettingsGroup grp in provider.SettingsGroups) {
					if (grps.Contains (grp))
						continue;
					
					grps.Add (grp);

					Container ctSettingsGroup = new Container ();
					ctSettingsGroup.Layout = new BoxLayout(Orientation.Vertical);
					ctSettingsGroup.BorderStyle = ControlBorderStyle.FixedSingle;
					ctSettingsGroup.Margin = new Padding (16);

					Container ctSettingsSubgroup = new Container();
					ctSettingsSubgroup.BorderStyle = ControlBorderStyle.FixedSingle;
					ctSettingsSubgroup.Layout = new ListLayout();
					(ctSettingsSubgroup.Layout as ListLayout).SelectionMode = SelectionMode.None;

					int iRow = 0;
					foreach (Setting opt in grp.Settings) {

						if (opt is GroupSetting)
						{
							CloseSettingsSubgroup(ctSettingsGroup, ctSettingsSubgroup);

							ctSettingsSubgroup = new Container();
							ctSettingsSubgroup.BorderStyle = ControlBorderStyle.FixedSingle;
							ctSettingsSubgroup.Layout = new ListLayout();
							(ctSettingsSubgroup.Layout as ListLayout).SelectionMode = SelectionMode.None;

							GroupSetting o = (opt as GroupSetting);

							int jrow = iRow;
							for (int j = 0; j < o.Options.Count; j++)
							{
								LoadOptionIntoList(((GroupSetting)opt).Options[j], ctSettingsSubgroup, ref jrow);
								jrow++;
							}

							Label lblTitle = new Label();
							lblTitle.HorizontalAlignment = HorizontalAlignment.Left;
							// lblTitle.Font = SystemFonts.MenuFont;
							// lblTitle.Font.Weight = FontWeights.Bold;
							lblTitle.Text = o.Title;
							ctSettingsGroup.Controls.Add(lblTitle, new BoxLayout.Constraints(true, true));
							continue;
						}

						LoadOptionIntoList(opt, ctSettingsSubgroup, ref iRow);
						iRow++;
					}
					CloseSettingsSubgroup(ctSettingsGroup, ctSettingsSubgroup);

					if (sidebar == null) {
						vpaned.Panel2.Controls.Add (ctSettingsGroup, new BoxLayout.Constraints (true, true));
					} else {
						if (grp.Path != null && grp.Path.Length > 0) {
							StackSidebarPanel ctp = new StackSidebarPanel ();
							ctSettingsGroup.Name = String.Join (":", grp.Path);
							ctSettingsGroup.Text = grp.Path [grp.Path.Length - 1];
							ctp.Control = ctSettingsGroup;
							sidebar.Items.Add (ctp);
						}
					}

					optionGroupContainers [grp] = ctSettingsGroup;
				}
			}
			grps.Sort ();
			foreach (SettingsGroup grp in grps) {
				AddOptionGroupPathPart (grp, grp.Path, 0);
			}
		}

		private void CloseSettingsSubgroup(Container ctSettingsGroup, Container ctSettingsSubgroup)
		{
			ctSettingsGroup.Controls.Add(ctSettingsSubgroup, new BoxLayout.Constraints(true, false));
		}

		private System.Collections.Generic.Dictionary<SettingsGroup, IControlContainer> optionGroupContainers = new System.Collections.Generic.Dictionary<SettingsGroup, IControlContainer>();

		private void LoadOption(Setting opt, int iRow, ref Control label, ref Control control)
		{

			if (opt is TextSetting)
			{
				TextSetting o = (opt as TextSetting);

				Label lbl = new Label();
				lbl.HorizontalAlignment = HorizontalAlignment.Left;
				lbl.Text = o.Title + ": ";
				label = lbl;

				TextBox txt = new TextBox();
				txt.Text = o.GetValue<string>();
				txt.SetExtraData<Setting>("setting", o);
				control = txt;
			}
			else if (opt is BooleanSetting)
			{
				BooleanSetting o = (opt as BooleanSetting);

				Label lbl = new Label();
				lbl.HorizontalAlignment = HorizontalAlignment.Left;
				lbl.Text = o.Title;
				label = lbl;

				CheckBox chk = new CheckBox();
				// chk.DisplayStyle = CheckBoxDisplayStyle.Switch;
				chk.Checked = o.GetValue<bool>();
				chk.SetExtraData<Setting>("setting", o);
				// chk.Text = o.Title;
				// ct.Controls.Add(chk, new GridLayout.Constraints(iRow, 0, 1, 2, ExpandMode.Horizontal));
				control = chk;
			}
			else if (opt is ChoiceSetting)
			{
				ChoiceSetting o = (opt as ChoiceSetting);

				Label lbl = new Label();
				lbl.HorizontalAlignment = HorizontalAlignment.Left;
				lbl.Text = o.Title;
				label = lbl;

				ComboBox cbo = new ComboBox();
				cbo.ReadOnly = true; // o.RequireSelectionFromList;
				DefaultTreeModel tm = new DefaultTreeModel(new Type[] { typeof(string) });
				foreach (ChoiceSetting.ChoiceSettingValue value in o.ValidValues)
				{
					tm.Rows.Add(new TreeModelRow(new TreeModelRowColumn[]
					{
						new TreeModelRowColumn(tm.Columns[0], value.Title)
					}));
				}
				cbo.Model = tm;
				cbo.Text = o.GetValue<string>();
				cbo.SetExtraData<Setting>("setting", o);
				control = cbo;
			}
			else if (opt is RangeSetting)
			{
				RangeSetting o = (opt as RangeSetting);

				Label lbl = new Label();
				lbl.HorizontalAlignment = HorizontalAlignment.Left;
				lbl.Text = o.Title;
				label = lbl;

				NumericTextBox txt = new NumericTextBox();
				txt.Minimum = o.MinimumValue;
				txt.Maximum = o.MaximumValue;
				txt.Value = o.GetValue<double>();
				control = txt;
			}
		}

		private void LoadOptionIntoGroup(Setting opt, IControlContainer ct, ref int iRow)
		{
			Control label = null;
			Control control = null;
			LoadOption(opt, iRow, ref label, ref control);

			if (label != null)
			{
				ct.Controls.Add(label, new GridLayout.Constraints(iRow, 0, 1, 1));
			}


			Label lblDescription = new Label();
			lblDescription.HorizontalAlignment = HorizontalAlignment.Left;
			lblDescription.Enabled = false;
			lblDescription.Attributes.Add("scale", 0.8);
			lblDescription.Text = opt.Description;
			ct.Controls.Add(lblDescription, new GridLayout.Constraints(iRow + 1, 0, 1, 1));

			if (control != null)
			{
				ct.Controls.Add(control, new GridLayout.Constraints(iRow, (label == null ? 0 : 1), 2, (label == null ? 2 : 1), ExpandMode.Both));

				iRow++;
			}
		}

		private void LoadOptionIntoList(Setting opt, IControlContainer ct, ref int iRow)
		{
			Control label = null;
			Control control = null;
			LoadOption(opt, iRow, ref label, ref control);

			Label lblDescription = new Label();
			lblDescription.HorizontalAlignment = HorizontalAlignment.Left;
			lblDescription.Enabled = false;
			lblDescription.Attributes.Add("scale", 0.8);
			lblDescription.Text = opt.Description;

			Container ct1 = new Container();
			ct1.Layout = new BoxLayout(Orientation.Horizontal);
			ct1.Margin = new Framework.Drawing.Padding(16);

			Container ct2 = new Container();
			ct2.Layout = new BoxLayout(Orientation.Vertical);

			if (label != null)
			{
				label.VerticalAlignment = VerticalAlignment.Middle;
				ct2.Controls.Add(label, new BoxLayout.Constraints(false, true));
			}

			if (!String.IsNullOrEmpty(opt.Description))
				ct2.Controls.Add(lblDescription, new BoxLayout.Constraints(false, true));

			ct1.Controls.Add(ct2, new BoxLayout.Constraints(true, true));
			ct1.Controls.Add(control, new BoxLayout.Constraints(false, false));
			control.VerticalAlignment = VerticalAlignment.Middle;

			ct.Controls.Add(ct1);

			iRow++;
		}

		private void AddOptionGroupPathPart(SettingsGroup grp, string[] path, int index, TreeModelRow parent = null)
		{
			if (index > path.Length - 1)
				return;

			string strpath = String.Join (":", path, 0, index + 1);
			TreeModelRow row = null;

			if (parent == null) {
				row = tmOptionGroups.Rows [strpath];
			} else {
				row = parent.Rows [strpath];
			}

			if (row == null) {
				row = new TreeModelRow (new TreeModelRowColumn[] { new TreeModelRowColumn (tmOptionGroups.Columns [0], path [index]) });
				row.Name = strpath;
				if (parent == null) {
					tmOptionGroups.Rows.Add(row);
				}
				else {
					parent.Rows.Add(row);
				}
			}

			if (index + 1 > path.Length - 1) {
				row.SetExtraData<SettingsGroup> ("group", grp);
			}

			AddOptionGroupPathPart (grp, path, index + 1, row);
		}

		private void tv_SelectionChanged(object sender, EventArgs e)
		{
			ctDefault.Visible = false;
			foreach (Control ctl in vpaned.Panel2.Controls)
			{
				ctl.Visible = false;
			}

			if (tv.SelectedRows.Count < 1)
			{
				ctDefault.Visible = true;
				return;
			}

			SettingsGroup thegrp = tv.SelectedRows[0].GetExtraData<SettingsGroup>("group");
			if (thegrp == null)
			{
				ctDefault.Visible = true;
				return;
			}

			if (optionGroupContainers.ContainsKey(thegrp))
			{
				optionGroupContainers[thegrp].Visible = true;
			}
			else
			{
				ctDefault.Visible = true;
			}
		}
	}
}
