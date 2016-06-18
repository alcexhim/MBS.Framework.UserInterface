using System;

namespace UniversalWidgetToolkit
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
		private string mvarTitle = null;
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private FileDialogMode mvarMode = FileDialogMode.Open;
		public FileDialogMode Mode { get { return mvarMode; } set { mvarMode = value; } }

	}
}

