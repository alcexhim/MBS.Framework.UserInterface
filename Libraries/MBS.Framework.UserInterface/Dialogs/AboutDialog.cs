using System;

namespace MBS.Framework.UserInterface.Dialogs
{
	public class AboutDialog : CommonDialog
	{
		public class CreditSection
		{
			public class CreditSectionCollection
				: System.Collections.ObjectModel.Collection<CreditSection>
			{

			}

			public string Title { get; set; } = null;
			public System.Collections.Specialized.StringCollection Names { get; } = new System.Collections.Specialized.StringCollection();
		}

		public string ProgramName { get; set; } = String.Empty;
		public Version Version { get; set; } = null;
		public string Copyright { get; set; } = String.Empty;
		public string Comments { get; set; } = String.Empty;
		public string Website { get; set; } = null;
		public string WebsiteLabel { get; set; } = null;

		public System.Collections.Specialized.StringCollection Authors { get; } = new System.Collections.Specialized.StringCollection();
		public System.Collections.Specialized.StringCollection Artists { get; } = new System.Collections.Specialized.StringCollection();
		public System.Collections.Specialized.StringCollection Documenters { get; } = new System.Collections.Specialized.StringCollection();
		public string TranslatorCredits { get; set; } = null;
		public CreditSection.CreditSectionCollection AdditionalCreditSections { get; } = new CreditSection.CreditSectionCollection();

		private string mvarLicenseText = null;
		public string LicenseText
		{
			get { return mvarLicenseText; }
			set
			{
				mvarLicenseText = value;
				mvarLicenseType = LicenseType.Unknown;
			}
		}

		private LicenseType mvarLicenseType = LicenseType.Unknown;
		public LicenseType LicenseType
		{
			get { return mvarLicenseType; }
			set
			{
				mvarLicenseType = value;
				switch (mvarLicenseType)
				{
					case LicenseType.Artistic:
					{
						break;
					}
					case LicenseType.BSD:
					{
						break;
					}
					case LicenseType.GPL20:
					{
						break;
					}
					case LicenseType.GPL30:
					{
						break;
					}
					case LicenseType.LGPL21:
					{
						break;
					}
					case LicenseType.LGPL30:
					{
						break;
					}
					case LicenseType.MITX11:
					{
						break;
					}
				}
			}
		}

		public void LoadLicenseTextFromFile(string filename) {
			mvarLicenseText = System.IO.File.ReadAllText (filename);
			mvarLicenseType = LicenseType.Unknown;
		}
	}
}
