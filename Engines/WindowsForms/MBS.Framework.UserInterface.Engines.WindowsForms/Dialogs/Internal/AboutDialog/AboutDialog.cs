using System;
using System.Reflection;
using System.Windows.Forms;

using MBS.Framework.Collections;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Dialogs.Internal.AboutDialog
{
	partial class AboutDialog : Form
	{
		public MBS.Framework.UserInterface.Dialogs.AboutDialog Dialog { get; set; } = null;

		public AboutDialog()
		{
			InitializeComponent();

			//  Initialize the AboutBox to display the product information from the assembly information.
			//  Change assembly information settings for your application through either:
			//  - Project->Properties->Application->Assembly Information
			//  - AssemblyInfo.cs

			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.Font = System.Drawing.SystemFonts.MenuFont;
		}

		protected override void OnShown(EventArgs e)
		{
			if (Dialog != null)
			{
				this.Text = String.Format("About {0}", Dialog.ProgramName);
				this.labelProductName.Text = Dialog.ProgramName;
				this.labelVersion.Text = String.Format("Version {0}", Dialog.Version.ToString());
				this.labelCopyright.Text = Dialog.Copyright;
				this.labelCompanyName.Text = Dialog.Website;
				this.textBoxDescription.Text = Dialog.Comments;

				if (Dialog.Authors.Count == 0 && Dialog.Documenters.Count == 0 && Dialog.TranslatorCredits == null &&
				Dialog.Artists.Count == 0 && Dialog.AdditionalCreditSections.Count == 0)
				{
					tbs.TabPages.Remove(tabPage2);
				}
				else
				{
					if (Dialog.Authors.Count > 0)
					{
						AddCreditSection("Created by", Dialog.Authors);
					}
					if (Dialog.Documenters.Count > 0)
					{
						AddCreditSection("Documented by", Dialog.Documenters);
					}
					if (Dialog.TranslatorCredits != null)
					{
						AddCreditSection("Translated by", new string[] { Dialog.TranslatorCredits });
					}
					if (Dialog.Artists.Count > 0)
					{
						AddCreditSection("Artwork by", Dialog.Artists);
					}
					foreach (MBS.Framework.UserInterface.Dialogs.AboutDialog.CreditSection cs in Dialog.AdditionalCreditSections)
					{
						AddCreditSection(cs.Title, cs.Names.ToArray<string>());
					}

					lvCredits.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
					lvCredits.ItemSelectionChanged += delegate (object sender1, System.Windows.Forms.ListViewItemSelectionChangedEventArgs e1)
					{
					// this emulates UWT SelectionMode.None on Windows Forms
					// thanks https://stackoverflow.com/questions/12647752
						if (e1.IsSelected)
						{
							e1.Item.Selected = false;
							e1.Item.Focused = false;
						}
					};
				}


				if (Dialog.LicenseText != null)
				{
					System.Windows.Forms.TabPage tabLicense = new System.Windows.Forms.TabPage();
					tabLicense.Text = "License";

					System.Windows.Forms.TextBox txtLicense = new System.Windows.Forms.TextBox();
					txtLicense.Dock = System.Windows.Forms.DockStyle.Fill;
					txtLicense.Text = Dialog.LicenseText.Replace("\r\n", "\r").Replace("\r", "\r\n");
					txtLicense.ReadOnly = true;
					txtLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
					txtLicense.Multiline = true;

					tabLicense.Controls.Add(txtLicense);

					tbs.TabPages.Add(tabLicense);
				}
			}
			else
			{
				this.Text = String.Format("About {0}", AssemblyTitle);
				this.labelProductName.Text = AssemblyProduct;
				this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
				this.labelCopyright.Text = AssemblyCopyright;
				this.labelCompanyName.Text = AssemblyCompany;
				this.textBoxDescription.Text = AssemblyDescription;
			}
		}

		private void AddCreditSection(string title, System.Collections.IEnumerable names)
		{
			bool t = false;
			foreach (object n in names)
			{
				if (n == null)
					continue;

				System.Windows.Forms.ListViewItem lvi = new System.Windows.Forms.ListViewItem();
				if (!t)
				{
					lvi.Text = title;
					lvi.SubItems.Add(n.ToString());
					t = true;
				}
				else
				{
					lvi.Text = String.Empty;
					lvi.SubItems.Add(n.ToString());
				}
				lvCredits.Items.Add(lvi);
			}
		}

		#region Assembly Attribute Accessors

		public string AssemblyTitle
		{
			get
			{
				// Get all Title attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				// If there is at least one Title attribute
				if (attributes.Length > 0)
				{
					// Select the first one
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					// If it is not an empty string, return it
					if (titleAttribute.Title != "")
						return titleAttribute.Title;
				}
				// If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				// Get all Description attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				// If there aren't any Description attributes, return an empty string
				if (attributes.Length == 0)
					return "";
				// If there is a Description attribute, return its value
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				// Get all Product attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				// If there aren't any Product attributes, return an empty string
				if (attributes.Length == 0)
					return "";
				// If there is a Product attribute, return its value
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				// Get all Copyright attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				// If there aren't any Copyright attributes, return an empty string
				if (attributes.Length == 0)
					return "";
				// If there is a Copyright attribute, return its value
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				// Get all Company attributes on this assembly
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				// If there aren't any Company attributes, return an empty string
				if (attributes.Length == 0)
					return "";
				// If there is a Company attribute, return its value
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
		#endregion
	}
}
