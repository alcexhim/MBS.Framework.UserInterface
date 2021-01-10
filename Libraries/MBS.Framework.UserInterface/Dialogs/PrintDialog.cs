using System;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs.Native;
using MBS.Framework.UserInterface.Printing;

namespace MBS.Framework.UserInterface.Dialogs
{
	namespace Native
	{
		public interface IPrintDialogImplementation
		{
			Printer GetSelectedPrinter();
			void SetSelectedPrinter(Printer value);

			PrintSettings GetSettings();
			void SetSettings(PrintSettings value);
		}
	}
	public class PrintDialog : CommonDialog, ITabPageContainer
	{
		public bool AutoUpgradeEnabled { get; set; } = true;
		public TabPage.TabPageCollection TabPages { get; private set; } = null;
		public TabPage SelectedTab { get; set; } = null;

		public bool EnablePreview { get; set; } = false;

		public PrintDialog()
		{
			TabPages = new TabPage.TabPageCollection(this);
		}

		private Printer mvarSelectedPrinter = null;
		public Printer SelectedPrinter
		{
			get
			{
				IPrintDialogImplementation impl = (ControlImplementation as IPrintDialogImplementation);
				if (impl != null) mvarSelectedPrinter = impl.GetSelectedPrinter();
				return mvarSelectedPrinter;
			}
			set
			{
				IPrintDialogImplementation impl = (ControlImplementation as IPrintDialogImplementation);
				impl?.SetSelectedPrinter(value);
				mvarSelectedPrinter = value;
			}
		}

		private PrintSettings mvarSettings = null;
		public PrintSettings Settings
		{
			get
			{
				IPrintDialogImplementation impl = (ControlImplementation as IPrintDialogImplementation);
				if (impl != null) mvarSettings = impl.GetSettings();
				return mvarSettings;
			}
			set
			{
				IPrintDialogImplementation impl = (ControlImplementation as IPrintDialogImplementation);
				impl?.SetSettings(value);
				mvarSettings = value;
			}
		}
	}
}
