using System;

using MBS.Framework.Settings;

using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.ListView;
using MBS.Framework.UserInterface.Layouts;

using MBS.Framework.Drawing;
using System.Collections.Generic;
using MBS.Framework.UserInterface.Drawing;

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
		private ListViewControl tv = null;
		private SplitContainer vpaned = null;

		private StackSidebar sidebar = null;

		public string[] SelectedPath { get; set; } = null;

		public SettingsDialog(SettingsProvider[] providers = null, SettingsProfile[] profiles = null)
		{
			tmOptionGroups = new DefaultTreeModel(new Type[] { typeof(string) });

			this.Layout = new BoxLayout(Orientation.Vertical);

			this.Buttons.Add(new Button(StockType.OK, DialogResult.OK));
			this.Buttons.Add(new Button(StockType.Cancel, DialogResult.Cancel));

			this.Buttons[0].Click += cmdOK_Click;

			this.DefaultButton = this.Buttons[0];

			this.Text = "Options";
			this.Size = new Dimension2D(800, 500);

			if (profiles == null)
			{
				foreach (SettingsProfile profile in ((UIApplication)Application.Instance).SettingsProfiles)
				{
					if (profile.ID == SettingsProfile.AllUsersGUID || profile.ID == SettingsProfile.ThisUserGUID)
						continue;

					this.SettingsProfiles.Add(profile);
				}
			}
			else
			{
				for (int i = 0; i < profiles.Length; i++)
				{
					this.SettingsProfiles.Add(profiles[i]);
				}
			}

			if (providers == null)
			{
				foreach (SettingsProvider provider in ((UIApplication)Application.Instance).SettingsProviders)
				{
					this.SettingsProviders.Add(provider);
				}
			}
			else
			{
				for (int i = 0;  i < providers.Length;  i++)
				{
					this.SettingsProviders.Add(providers[i]);
				}
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
			vpaned.Panel2.VerticalAdjustment.ScrollType = AdjustmentScrollType.Always;

			vpaned.SplitterPosition = 140;

			this.Controls.Add(vpaned, new BoxLayout.Constraints(true, true, 0, BoxLayout.PackType.Start));

			tv = new ListViewControl();
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

		public SettingsProfile.SettingsProfileCollection SettingsProfiles { get; } = new SettingsProfile.SettingsProfileCollection();

		public SettingsDialogAppearance Appearance { get; set; } = SettingsDialogAppearance.VisualStudio;

		/// <summary>
		/// Gets or sets a value indicating whether settings defined in this <see cref="SettingsDialog"/> can be loaded and saved as a profile.
		/// </summary>
		/// <value><c>true</c> if profiles should be enabled for this <see cref="SettingsDialog" />; otherwise, <c>false</c>.</value>
		public bool EnableProfiles { get; set; } = true;

		private ComboBox cboProfile;
		private TextBox txtProfile;
		Container ctDefault = new Container ();
		internal protected override void OnCreating(EventArgs e)
		{
			base.OnCreating(e);

			Container ctProfile = new Container(new BoxLayout(Orientation.Horizontal));
			ctProfile.Controls.Add(new Label("_Profile "), new BoxLayout.Constraints(false, false));

			/*
			cboProfile = new ComboBox();
			cboProfile.Changed += cboProfile_Changed;
			cboProfile.Margin = new Padding(0, 0, 16, 16);
			cboProfile.ReadOnly = true;
			cboProfile.Model = new DefaultTreeModel(new Type[] { typeof(string) });
			ctProfile.Controls.Add(cboProfile, new BoxLayout.Constraints(true, true));
			*/

			txtProfile = new TextBox();
			txtProfile.Margin = new Padding(0, 0, 16, 16);
			txtProfile.Enabled = false;
			txtProfile.Editable = false;
			txtProfile.Text = "(none)";
			ctProfile.Controls.Add(txtProfile, new BoxLayout.Constraints(true, true));

			Button cmdChooseProfile = new Button();
			cmdChooseProfile.Click += cmdChooseProfile_Click;
			cmdChooseProfile.Text = "_Choose...";
			cmdChooseProfile.Margin = new Padding(0, 0, 0, 8);
			ctProfile.Controls.Add(cmdChooseProfile, new BoxLayout.Constraints(false, false));

			Button cmdResetProfile = new Button();
			cmdResetProfile.Text = "_Reset";
			ctProfile.Controls.Add(cmdResetProfile, new BoxLayout.Constraints(false, false));

			/*
			Button cmdConfigureProfiles = new Button();
			cmdConfigureProfiles.Click += cmdConfigureProfiles_Click;
			cmdConfigureProfiles.Text = "_Configure...";
			ctProfile.Controls.Add(cmdConfigureProfiles, new BoxLayout.Constraints(false, false));
			*/

			ctProfile.Margin = new Padding(16);

			/*
			for (int i = 0; i < SettingsProfiles.Count; i++)
			{
				TreeModelRow row = new TreeModelRow(new TreeModelRowColumn[]
				{
					new TreeModelRowColumn(cboProfile.Model.Columns[0], SettingsProfiles[i].Title)
				});
				row.SetExtraData<SettingsProfile>("profile", SettingsProfiles[i]);
				(cboProfile.Model as DefaultTreeModel).Rows.Add(row);
			}
			*/
			this.Controls.Add(ctProfile);

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

			Label lblNoOptions = new Label("The selected group has no options available to configure");
			lblNoOptions.HorizontalAlignment = HorizontalAlignment.Center;
			lblNoOptions.VerticalAlignment = VerticalAlignment.Middle;
			ctDefault.Controls.Add(lblNoOptions, new BoxLayout.Constraints(true, true));

			if (sidebar == null)
			{
				vpaned.Panel2.Controls.Add(ctDefault, new BoxLayout.Constraints(true, true));
			}
			else
			{
			}

			System.Collections.Generic.List<SettingsGroup> grps = new System.Collections.Generic.List<SettingsGroup>();
			foreach (SettingsProvider provider in SettingsProviders)
			{
				provider.Initialize();
				foreach (SettingsGroup grp in provider.SettingsGroups)
				{
					if (grps.Contains(grp))
						continue;

					grps.Add(grp);

					Container ctSettingsGroupWrapper = new Container();
					ctSettingsGroupWrapper.Layout = new BoxLayout(Orientation.Vertical);

					Container ctSettingsGroup = new Container();
					ctSettingsGroup.Layout = new BoxLayout(Orientation.Vertical);
					ctSettingsGroup.BorderStyle = ControlBorderStyle.FixedSingle;
					ctSettingsGroup.Margin = new Padding(16);

					Container ctSettingsSubgroup = null;

					int iRow = 0;
					bool lastWasCommand = false;
					Container ctButtonContainer = null;

					foreach (Setting opt in grp.Settings)
					{
						InsertSetting(opt, ctSettingsGroup, ref ctSettingsSubgroup, ref iRow, ref ctButtonContainer, ref lastWasCommand);
					}
					CloseSettingsSubgroup(ctSettingsGroup, ctSettingsSubgroup);

					if (sidebar == null)
					{
						vpaned.Panel2.Controls.Add(ctSettingsGroupWrapper, new BoxLayout.Constraints(true, false));
					}
					else
					{
						if (grp.Path != null && grp.Path.Length > 0)
						{
							StackSidebarPanel ctp = new StackSidebarPanel();
							ctSettingsGroup.Name = String.Join(":", grp.Path);
							ctSettingsGroup.Text = grp.Path[grp.Path.Length - 1];
							ctp.Control = ctSettingsGroupWrapper;
							sidebar.Items.Add(ctp);
						}
					}

					ctSettingsGroupWrapper.Controls.Add(ctSettingsGroup, new BoxLayout.Constraints(false, false));
					ctSettingsGroupWrapper.Visible = false;

					optionGroupContainers[grp] = ctSettingsGroupWrapper;
				}
			}
			grps.Sort();
			foreach (SettingsGroup grp in grps)
			{
				AddOptionGroupPathPart(grp, grp.Path, 0);
			}
		}
		protected internal override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			if (SelectedPath != null)
			{
				SettingsGroup group = FindSettingsGroup(SelectedPath);
				if (group != null)
				{
					if (optionGroupContainers.ContainsKey(group))
					{
						ctDefault.Visible = false;
						optionGroupContainers[group].Visible = true;
					}
					else
					{
						ctDefault.Visible = true;
					}
				}

				foreach (TreeModelRow row in tv.Model.Rows)
				{
					if (SetTreeModelRowSelected(row, group))
						break;
				}
			}
		}

		private bool SetTreeModelRowSelected(TreeModelRow row, SettingsGroup group)
		{
			if (row.GetExtraData<SettingsGroup>("group") == group)
			{
				// at least on gtk, must be called BEFORE adding to selected row https://stackoverflow.com/questions/11543716
				row.EnsureVisible();

				tv.SelectedRows.Clear();
				tv.SelectedRows.Add(row);
				return true;
			}

			foreach (TreeModelRow row2 in row.Rows)
			{
				if (SetTreeModelRowSelected(row2, group))
					return true;
			}
			return false;
		}

		private SettingsGroup FindSettingsGroup(string[] selectedPath)
		{
			foreach (SettingsProvider sp in SettingsProviders)
			{
				foreach (SettingsGroup grp in sp.SettingsGroups)
				{
					if (grp.Path.Matches(selectedPath))
						return grp;
				}
			}
			return null;
		}


		private void InsertSetting(Setting opt, Container ctSettingsGroup, ref Container ctSettingsSubgroup, ref int iRow, ref Container ctButtonContainer, ref bool lastWasCommand)
		{
			if (lastWasCommand)
			{
				if (ctButtonContainer != null)
				{
					ctSettingsGroup.Controls.Add(ctButtonContainer, new BoxLayout.Constraints(false, false, 16));
				}
				ctButtonContainer = null;
				ctSettingsSubgroup = null;
			}

			if (opt is GroupSetting)
			{
				InsertGroupSetting(opt as GroupSetting, ctSettingsGroup, ref ctSettingsSubgroup, iRow, ref ctButtonContainer, ref lastWasCommand);
				return;
			}
			else if (opt is CommandSetting)
			{
				InsertCommandSetting(opt as CommandSetting, ctSettingsGroup, ctSettingsSubgroup, ref ctButtonContainer);
				lastWasCommand = true;
				return;
			}

			if (ctSettingsSubgroup == null)
			{
				ctSettingsSubgroup = CreateSettingsSubgroup();
			}

			LoadOptionIntoList(opt, ctSettingsSubgroup, ref iRow);
			iRow++;
		}

		private void InsertGroupSetting(GroupSetting opt, Container ctSettingsGroup, ref Container ctSettingsSubgroup, int iRow, ref Container ctButtonContainer, ref bool lastWasCommand)
		{
			CloseSettingsSubgroup(ctSettingsGroup, ctSettingsSubgroup);

			ctSettingsSubgroup = CreateSettingsSubgroup();
			GroupSetting o = (opt as GroupSetting);

			Container ctTitleAndDscription = new Container(new BoxLayout(Orientation.Vertical));

			Label lblTitle = new Label();
			lblTitle.HorizontalAlignment = HorizontalAlignment.Left;
			lblTitle.Font = SystemFonts.MenuFont;
			lblTitle.Font.Weight = FontWeights.Bold;
			lblTitle.Text = o.Title;
			ctTitleAndDscription.Controls.Add(lblTitle, new BoxLayout.Constraints(false, false, 8));

			if (!String.IsNullOrEmpty(opt.Description))
			{
				Label lblDescription = new Label();
				lblDescription.Enabled = false;
				lblDescription.HorizontalAlignment = HorizontalAlignment.Left;
				lblDescription.Text = opt.Description;
				lblDescription.WordWrap = WordWrapMode.Always;
				ctTitleAndDscription.Controls.Add(lblDescription, new BoxLayout.Constraints(false, false, 8));
			}

			// ctSettingsGroup.Controls.Add(ctTitleAndDscription, new BoxLayout.Constraints(false, false, 16));
			Container ctTitleAndHeaderSettings = new Container(new BoxLayout(Orientation.Horizontal));
			Container ctHeaderSettings = new Container(new BoxLayout(Orientation.Horizontal));
			
			for (int i = 0; i < o.HeaderSettings.Count; i++)
			{
				Control lbl = null, ctl = null;
				LoadOption(o.HeaderSettings[i], 0, ref lbl, ref ctl);
				if (ctl != null)
				{
					ctl.VerticalAlignment = VerticalAlignment.Middle;
					ctHeaderSettings.Controls.Add(ctl, new BoxLayout.Constraints(false, false, 8));
				}
			}
			ctTitleAndHeaderSettings.Controls.Add(ctTitleAndDscription, new BoxLayout.Constraints(true, true));
			ctTitleAndHeaderSettings.Controls.Add(ctHeaderSettings, new BoxLayout.Constraints(false, false));

			ctSettingsGroup.Controls.Add(ctTitleAndHeaderSettings, new BoxLayout.Constraints(false, false, 16));


			int jrow = iRow;
			for (int j = 0; j < o.Options.Count; j++)
			{
				if (o.Options[j] is GroupSetting)
				{
					InsertGroupSetting(o.Options[j] as GroupSetting, ctSettingsGroup, ref ctSettingsSubgroup, iRow, ref ctButtonContainer, ref lastWasCommand);
					lastWasCommand = false;
					continue;
				}
				else if (o.Options[j] is CommandSetting)
				{
					InsertCommandSetting(o.Options[j] as CommandSetting, ctSettingsGroup, ctSettingsSubgroup, ref ctButtonContainer);
					lastWasCommand = true;
					continue;
				}
				else
				{
					InsertSetting(o.Options[j], ctSettingsGroup, ref ctSettingsSubgroup, ref iRow, ref ctButtonContainer, ref lastWasCommand);
				}
				jrow++;
				lastWasCommand = false;
			}

			ctSettingsGroup.Controls.Add(ctSettingsSubgroup, new BoxLayout.Constraints(false, false));
		}

		private void InsertCommandSetting(CommandSetting opt, Container ctSettingsGroup, Container ctSettingsSubgroup, ref Container ctButtonContainer)
		{
			CloseSettingsSubgroup(ctSettingsGroup, ctSettingsSubgroup);

			Control btn = null, lbl = null;
			LoadOption(opt, 0, ref lbl, ref btn);

			if (ctButtonContainer == null)
			{
				ctButtonContainer = new Container(new BoxLayout(Orientation.Horizontal));
			}
			ctButtonContainer.Controls.Add(btn, new BoxLayout.Constraints(false, false, 6, BoxLayout.PackType.End));
		}

		private Container CreateSettingsSubgroup()
		{
			Container ct = new Container();
			ct.BorderStyle = ControlBorderStyle.FixedSingle;
			ct.Layout = new ListLayout();
			(ct.Layout as ListLayout).SelectionMode = SelectionMode.None;
			return ct;
		}

		private void cboProfile_Changed(object sender, EventArgs e)
		{
			if (cboProfile.SelectedItem == null)
				return;

			SettingsProfile profile = cboProfile.SelectedItem.GetExtraData<SettingsProfile>("profile");
			if (profile == null)
			{
				MessageDialog.ShowDialog("TODO: load settings for the default profile!", "Information", MessageDialogButtons.OK, MessageDialogIcon.Information);
				return;
			}

			MessageDialog.ShowDialog(String.Format("TODO: load settings for {0}!", profile.Title), "Information", MessageDialogButtons.OK, MessageDialogIcon.Information);
		}


		private void cmdChooseProfile_Click(object sender, EventArgs e)
		{
			SettingsProfileDialog dlg = new SettingsProfileDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				if (dlg.SelectedProfile != null)
				{
					txtProfile.Text = dlg.SelectedProfile.Title;
				}
			}
		}


		protected internal override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if (tmOptionGroups.Rows.Count == 1)
			{
				tv.SelectedRows.Add(tmOptionGroups.Rows[0]);
				tv_SelectionChanged(tv, EventArgs.Empty);

				vpaned.Panel1.Expanded = false;
			}
		}

		private void CloseSettingsSubgroup(Container ctSettingsGroup, Container ctSettingsSubgroup)
		{
			if (ctSettingsSubgroup != null)
			{
				BoxLayout.Constraints constraints = new BoxLayout.Constraints(true, true);
				constraints.HorizontalExpand = true;
				ctSettingsSubgroup.MaximumSize = new Dimension2D(1024, -1);
				ctSettingsGroup.Controls.Add(ctSettingsSubgroup, constraints);
			}
		}

		private System.Collections.Generic.Dictionary<SettingsGroup, IControlContainer> optionGroupContainers = new System.Collections.Generic.Dictionary<SettingsGroup, IControlContainer>();

		private void txt_Changed(object sender, EventArgs e)
		{
			Control ctl = (sender as Control);
			Setting setting = ctl.GetExtraData<Setting>("setting");

			if (ctl is TextBox)
			{
				setting.SetValue((ctl as TextBox).Text);
			}
			else if (ctl is FileChooserButton)
			{
				setting.SetValue((ctl as FileChooserButton).SelectedFileName);
			}
		}
		private void chk_Changed(object sender, EventArgs e)
		{
			Control ctl = (sender as Control);
			Setting setting = ctl.GetExtraData<Setting>("setting");
			if (ctl is CheckBox)
			{
				setting.SetValue<bool>((ctl as CheckBox).Checked);
			}
		}

		private void LoadOption(Setting opt, int iRow, ref Control label, ref Control control)
		{

			if (opt is TextSetting || opt is FileSetting)
			{
				TextSetting o = (opt as TextSetting);

				Label lbl = new Label();
				lbl.HorizontalAlignment = HorizontalAlignment.Left;
				lbl.Text = o.Title;
				label = lbl;

				if (opt is FileSetting)
				{
					FileChooserButton txt = new FileChooserButton();
					txt.SelectedFileName = o.GetValue<string>();
					txt.SetExtraData<Setting>("setting", o);
					txt.RequireExistingFile = (opt as FileSetting).RequireExistingFile;
					txt.Changed += txt_Changed;
					control = txt;
				}
				else
				{
					TextBox txt = new TextBox();
					txt.Text = o.GetValue<string>();
					txt.SetExtraData<Setting>("setting", o);
					txt.Changed += txt_Changed;
					control = txt;
				}
			}
			else if (opt is BooleanSetting)
			{
				BooleanSetting o = (opt as BooleanSetting);

				CheckBox chk = new CheckBox();
				chk.DisplayStyle = CheckBoxDisplayStyle.Switch;
				chk.Checked = o.GetValue<bool>();
				chk.SetExtraData<Setting>("setting", o);
				chk.Changed += chk_Changed;
				// chk.Text = o.Title;
				// ct.Controls.Add(chk, new GridLayout.Constraints(iRow, 0, 1, 2, ExpandMode.Horizontal));
				control = chk;

				Label lbl = new Label();
				lbl.HorizontalAlignment = HorizontalAlignment.Left;
				lbl.Text = o.Title;
				label = lbl;
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
				txt.Minimum = (double)o.MinimumValue.GetValueOrDefault(decimal.MinValue);
				txt.Maximum = (double)o.MaximumValue.GetValueOrDefault(decimal.MaxValue);
				txt.Value = o.GetValue<double>();
				control = txt;
			}
			else if (opt is CollectionSetting)
			{
				CollectionSetting o = (opt as CollectionSetting);

				GroupBox fra = new GroupBox();
				fra.Text = o.Title;
				fra.Layout = new BoxLayout(Orientation.Vertical);

				CollectionListView clv = new CollectionListView();

				DefaultTreeModel tm = new DefaultTreeModel(new Type[0]);
				foreach (Setting sett in o.Settings)
				{
					tm.Columns.Add(new TreeModelColumn(typeof(string)));
				}
				clv.ListView.Model = tm;
				for (int i = 0; i < o.Settings.Count; i++)
				{
					clv.ListView.Columns.Add(new ListViewColumnText(tm.Columns[i], o.Settings[i].Title));
				}

				clv.ItemAdding += clv_ItemAdding;
				clv.ItemEditing += clv_ItemEditing;
				clv.SetExtraData<CollectionSetting>("setting", o);
				fra.Controls.Add(clv, new BoxLayout.Constraints(true, false));

				control = fra;
			}
			else if (opt is CommandSetting)
			{
				Button btn = new Button();
				btn.StylePreset = (opt as CommandSetting).StylePreset;
				btn.Text = opt.Title;
				btn.Click += btn_Click;
				btn.SetExtraData<CommandSetting>("setting", opt as CommandSetting);
				control = btn;
			}
		}

		private void btn_Click(object sender, EventArgs e)
		{
			Button btn = (sender as Button);
			CommandSetting sett = btn.GetExtraData<CommandSetting>("setting");

			((UIApplication)Application.Instance).ExecuteCommand(sett.CommandID);
		}

		private void clv_ItemAdding(object sender, EventArgs e)
		{
			CollectionListView clv = (sender as CollectionListView);
			CollectionSetting o = clv.GetExtraData<CollectionSetting>("setting");

			SettingsDialog dlg = new SettingsDialog();
			dlg.SettingsProviders.Clear();

			CustomSettingsProvider csp = new CustomSettingsProvider();

			SettingsGroup group = new SettingsGroup();
			for (int i = 0; i < o.Settings.Count; i++)
			{
				group.Settings.Add(o.Settings[i]);
			}
			csp.SettingsGroups.Add(group);
			dlg.SettingsProviders.Add(csp);

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				List<TreeModelRowColumn> list = new List<TreeModelRowColumn>();
				for (int i = 0; i < o.Settings.Count; i++)
				{
					object val = o.Settings[i].GetValue();
					list.Add(new TreeModelRowColumn(clv.ListView.Model.Columns[i], val));
				}

				TreeModelRow row = new TreeModelRow(list.ToArray());
				row.SetExtraData<SettingsGroup>("group", group);
				clv.ListView.Model.Rows.Add(row);
				o.Items.Add(group);
			}
		}
		private void clv_ItemEditing(object sender, EventArgs e)
		{
			CollectionListView clv = (sender as CollectionListView);
			CollectionSetting o = clv.GetExtraData<CollectionSetting>("setting");

			SettingsDialog dlg = new SettingsDialog();
			dlg.SettingsProviders.Clear();

			CustomSettingsProvider csp = new CustomSettingsProvider();

			SettingsGroup group = clv.ListView.SelectedRows[0].GetExtraData<SettingsGroup>("group");
			csp.SettingsGroups.Add(group);
			dlg.SettingsProviders.Add(csp);

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				/*
				List<TreeModelRowColumn> list = new List<TreeModelRowColumn>();
				for (int i = 0; i < o.Settings.Count; i++)
				{
					object val = o.Settings[i].GetValue();
					list.Add(new TreeModelRowColumn(clv.ListView.Model.Columns[i], val));
				}
				clv.ListView.Model.Rows.Add(new TreeModelRow(list.ToArray()));
				*/			
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

			if (!String.IsNullOrEmpty(opt.Description))
			{
				Label lblDescription = new Label();
				lblDescription.HorizontalAlignment = HorizontalAlignment.Left;
				lblDescription.Enabled = false;
				lblDescription.Attributes.Add("scale", 0.8);
				lblDescription.Text = opt.Description;
				ct.Controls.Add(lblDescription, new GridLayout.Constraints(iRow + 1, 0, 1, 1));
			}

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

			if (label != null)
			{
				label.VerticalAlignment = VerticalAlignment.Middle;
			}

			Label lblDescription = new Label();
			lblDescription.HorizontalAlignment = HorizontalAlignment.Left;
			lblDescription.Enabled = false;
			lblDescription.Attributes.Add("scale", 0.8);
			lblDescription.Text = opt.Description;

			Container ct1 = new Container();
			ct1.Layout = new BoxLayout(Orientation.Horizontal);
			ct1.Margin = new Framework.Drawing.Padding(16);

			if (!String.IsNullOrEmpty(opt.Description))
			{
				Container ct2 = new Container();
				ct2.Layout = new BoxLayout(Orientation.Vertical);

				if (label != null)
				{
					ct2.Controls.Add(label, new BoxLayout.Constraints(false, true));
				}

				ct2.Controls.Add(lblDescription, new BoxLayout.Constraints(false, true));

				ct1.Controls.Add(ct2, new BoxLayout.Constraints(true, true));
			}
			else
			{
				if (label != null)
				{
					ct1.Controls.Add(label, new BoxLayout.Constraints(true, true));
				}
			}

			if (opt is BooleanSetting)
			{
				// ct1.Click += (sender, e) => (control as CheckBox).Checked = !(control as CheckBox).Checked;
				ct1.Click += chkBooleanSetting_Click;
				ct1.SetExtraData<CheckBox>("checkbox", control as CheckBox);
			}

			ct1.Controls.Add(control, new BoxLayout.Constraints(false, false));
			control.VerticalAlignment = VerticalAlignment.Middle;

			ct.Controls.Add(ct1);

			iRow++;
		}

		private void chkBooleanSetting_Click(object sender, EventArgs e)
		{
			Container ct1 = (Container)sender;
			CheckBox chk = ct1.GetExtraData<CheckBox>("checkbox");
			chk.Checked = !chk.Checked;
		}

		private void AddOptionGroupPathPart(SettingsGroup grp, string[] path, int index, TreeModelRow parent = null)
		{
			if (path == null) path = new string[] { String.Empty };
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

		public DialogResult ShowDialog(string[] path)
		{
			SelectedPath = path;
			return ShowDialog();
		}
	}
}
