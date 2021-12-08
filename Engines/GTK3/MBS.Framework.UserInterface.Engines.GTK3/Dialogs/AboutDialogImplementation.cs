using System;
using System.Collections.Generic;

using MBS.Framework.Collections;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs;

namespace MBS.Framework.UserInterface.Engines.GTK3.Dialogs
{
	[ControlImplementation(typeof(AboutDialog))]
	public class AboutDialogImplementation : GTKDialogImplementation
	{
		public AboutDialogImplementation(Engine engine, AboutDialog dialog) : base(engine, dialog)
		{
		}

		protected override bool AcceptInternal()
		{
			return true;
		}

		protected override GTKNativeControl CreateDialogInternal(Dialog dialog, List<Button> buttons)
		{
			AboutDialog dlg = (dialog as AboutDialog);
			IntPtr handle = Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_new();Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_program_name(handle, dlg.ProgramName);
			if (dlg.Version != null)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_version(handle, dlg.Version.ToString());
			}
			Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_copyright(handle, dlg.Copyright);
			Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_comments(handle, dlg.Comments);
			if (dlg.LicenseText != null)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license(handle, dlg.LicenseText);
			}

			if (dlg.Website != null)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_website(handle, dlg.Website);
			}
			if (dlg.WebsiteLabel != null)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_website_label(handle, dlg.WebsiteLabel);
			}

			foreach (string s in dlg.Authors)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_authors(handle, dlg.Authors.ToNullTerminatedArray<string>());
			}
			foreach (string s in dlg.Artists)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_artists(handle, dlg.Artists.ToNullTerminatedArray<string>());
			}
			foreach (string s in dlg.Documenters)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_documenters(handle, dlg.Documenters.ToNullTerminatedArray<string>());
			}
			if (dlg.TranslatorCredits != null)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_translator_credits(handle, dlg.TranslatorCredits);
			}
			foreach (AboutDialog.CreditSection sect in dlg.AdditionalCreditSections)
			{
				Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_add_credit_section(handle, sect.Title, sect.Names.ToNullTerminatedArray<string>());
			}

			if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V3)
			{
				if (dlg.LicenseType != LicenseType.Unknown)
				{
					switch (dlg.LicenseType)
					{
						case LicenseType.Artistic:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Artistic);
							break;
						}
						case LicenseType.BSD:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.BSD);
							break;
						}
						case LicenseType.Custom:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Custom);
							break;
						}
						case LicenseType.GPL20:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.GPL20);
							break;
						}
						case LicenseType.GPL30:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.GPL30);
							break;
						}
						case LicenseType.LGPL21:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.LGPL21);
							break;
						}
						case LicenseType.LGPL30:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.LGPL30);
							break;
						}
						case LicenseType.MITX11:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.MITX11);
							break;
						}
						case LicenseType.Unknown:
						{
							Internal.GTK.Methods.GtkAboutDialog.gtk_about_dialog_set_license_type(handle, Internal.GTK.Constants.GtkLicense.Unknown);
							break;
						}
					}
				}
			}
			return new GTKNativeControl(handle);
		}
	}
}
