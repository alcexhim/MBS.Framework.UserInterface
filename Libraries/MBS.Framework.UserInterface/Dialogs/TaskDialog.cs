using System;
namespace MBS.Framework.UserInterface.Dialogs
{
	public class TaskDialogHyperlinkClickedEventArgs
	{
		public TaskDialogHyperlinkClickedEventArgs()
		{
		}
	}
	public delegate void TaskDialogHyperlinkClickedEventHandler(object sender, TaskDialogHyperlinkClickedEventArgs e);

	public enum TaskDialogIcon
	{
		None = 0,

		Warning = (int)(UInt16.MaxValue),
		Error = (int)(UInt16.MaxValue - 1),
		Information = (int)(UInt16.MaxValue - 2),
		Question = 0,

		Security = (int)(UInt16.MaxValue - 3),
		SecurityTrusted = (int)(UInt16.MaxValue - 4),
		SecurityWarning = (int)(UInt16.MaxValue - 5),
		SecurityError = (int)(UInt16.MaxValue - 6),
		SecurityOK = (int)(UInt16.MaxValue - 7),
		SecurityUntrusted = (int)(UInt16.MaxValue - 8)
	}

	[Flags()]
	public enum TaskDialogButtons
	{
		Custom = -1,
		None = 0x00,

		OK = 0x01,     // returns (1)(IDOK)
		Yes = 0x02,     // returns (6)(IDYES)
		No = 0x04,      // returns (7)(IDNO)
		Cancel = 0x08,  // returns (2)(IDCANCEL)
		Retry = 0x10,  // returns (4)(IDRETRY)
		Close = 0x20  // returns (8)(IDCLOSE)
	}
	public enum TaskDialogButtonStyle
	{
		Buttons,
		Commands,
		CommandsNoIcon
	}
	public class TaskDialog : CommonDialog
	{
		public TaskDialogButtonStyle ButtonStyle { get; set; } = TaskDialogButtonStyle.Buttons;

		public string Prompt { get; set; } = null;
		public string Content { get; set; } = null;
		public string Footer { get; set; } = null;

		public string VerificationText { get; set; } = null;
		public bool VerificationChecked { get; set; } = false;

		public TaskDialogButtons ButtonsPreset { get; set; } = TaskDialogButtons.None;
		public TaskDialogIcon Icon { get; set; } = TaskDialogIcon.None;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:MBS.Framework.UserInterface.Dialogs.TaskDialog"/> enables hyperlink processing for the strings specified in the <see cref="Content"/>, <see cref="ExpandedInformation"/> and <see cref="Footer"/> members. When enabled, these members may point to strings that contain hyperlinks in the following form:
		/// &lt;A HREF="executablestring"&gt;Hyperlink Text&lt;/A&gt;
		/// Warning: Enabling hyperlinks when using content from an unsafe source may cause security vulnerabilities.
		/// Note: Task Dialogs will not actually execute any hyperlinks. Hyperlink execution must be handled in the callback function specified by pfCallback. For more details, see TaskDialogCallbackProc.
		/// </summary>
		/// <value><c>true</c> if hyperlinks should be enabled; otherwise, <c>false</c>.</value>
		public bool EnableHyperlinks { get; set; } = false;

		public event TaskDialogHyperlinkClickedEventHandler HyperlinkClicked;
		protected virtual void OnHyperlinkClicked(TaskDialogHyperlinkClickedEventArgs e)
		{
			HyperlinkClicked?.Invoke(this, e);
		}

		public static DialogResult ShowDialog(string instruction, string content, string title, Controls.Button[] buttons, TaskDialogIcon icon)
		{
			TaskDialog td = new TaskDialog();
			td.Prompt = instruction;
			td.Content = content;
			td.Text = title;
			td.ButtonsPreset = TaskDialogButtons.Custom;
			td.ButtonStyle = TaskDialogButtonStyle.Commands;
			for (int i = 0; i < buttons.Length; i++)
			{
				td.Buttons.Add(buttons[i]);
			}
			td.Icon = icon;
			return td.ShowDialog();
		}
		public static DialogResult ShowDialog(string instruction, string content, string title, TaskDialogButtons buttons, TaskDialogIcon icon)
		{
			TaskDialog td = new TaskDialog();
			td.Prompt = instruction;
			td.Content = content;
			td.Text = title;
			td.ButtonsPreset = buttons;
			td.Icon = icon;
			return td.ShowDialog();
		}
	}
}
