using System;

namespace UniversalWidgetToolkit.Dialogs
{
	public enum FileDialogMode
	{
		Open,
		Save,
		SelectFolder,
		CreateFolder
	}
	public class FileDialog : CommonDialog
	{
		private FileDialogMode mvarMode = FileDialogMode.Open;
		public FileDialogMode Mode { get { return mvarMode; } set { mvarMode = value; } }

		private bool mvarMultiSelect = false;
		public bool MultiSelect { get { return mvarMultiSelect; } set { mvarMultiSelect = value; } }

		private System.Collections.Generic.List<string> mvarSelectedFileNames = new System.Collections.Generic.List<string> ();
		public System.Collections.Generic.List<string> SelectedFileNames { get { return mvarSelectedFileNames; } }

	}
}

