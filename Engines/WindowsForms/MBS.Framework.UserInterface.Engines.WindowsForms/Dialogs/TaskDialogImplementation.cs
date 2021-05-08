using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs;
using MBS.Framework.UserInterface.Engines.WindowsForms.Internal.Windows;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Dialogs
{
	[ControlImplementation(typeof(TaskDialog))]
	public class TaskDialogImplementation : WindowsFormsDialogImplementation
	{
		public TaskDialogImplementation(Engine engine, Control control)
			: base(engine, control)
		{

		}


		// taskdialog icons defined under Vista. All text
		// on white dialog background
		//
		/// <summary>
		/// Exclamation point in a yellow triangle.
		/// </summary>
		private const int TD_WARNING_ICON = -1;

		// round red circle containg 'X' (same as IDI_HAND)
		private const int TD_ERROR_ICON = -2;

		// round blue circle containing 'i' (same image as IDI_ASTERISK)
		private const int TD_INFORMATION_ICON = -3;

		// Vista's security shield
		private const int TD_SHIELD_ICON = -4;


		// icons defined under Windows that can also be used.
		// Text on white dialog background

		// miniature picture of an application window
		private const int IDI_APPLICATION = 32512;

		// round blue circle containing '?'
		private const int IDI_QUESTION = 32514;

		// no icon; text on white background
		private const int TD_NO_ICON = 0;

		protected override bool AcceptInternal()
		{
			return true;
		}

		protected override WindowsFormsNativeDialog CreateDialogInternal(Dialog dialog, List<Button> buttons)
		{
			return new WindowsFormsNativeDialog(null);
		}

		private static Random rnd = new Random();
		public override DialogResult Run(System.Windows.Forms.IWin32Window parentHandle)
		{
			TaskDialog dlg = (Control as TaskDialog);
			int pnButton = 0;
			int pnRadioButton = 0;
			bool pfVerificationFlagChecked = false;
			int retval = 0;

			try
			{
				Structures.TASKDIALOGCONFIG tdc = new Structures.TASKDIALOGCONFIG();

				List<Structures.TASKDIALOG_BUTTON> tdb_array = new List<Structures.TASKDIALOG_BUTTON>();
				for (int i = 0; i < dlg.Buttons.Count; i++)
				{
					tdb_array.Add(new Structures.TASKDIALOG_BUTTON(dlg.Buttons[i].ResponseValue, dlg.Buttons[i].Text.Replace('_', '&')));
				}

				Constants.TaskDialogFlags flags = Constants.TaskDialogFlags.None;

				if (tdb_array.Count > 0)
				{
					tdc.cButtons = (uint)tdb_array.Count;

					IntPtr initialPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Structures.TASKDIALOG_BUTTON)) * tdb_array.Count);
					IntPtr currentPtr = initialPtr;
					for (int i = 0; i < tdb_array.Count; i++)
					{
						Marshal.StructureToPtr(tdb_array[i], currentPtr, false);
						if (IntPtr.Size == 8)
						{
							currentPtr = (IntPtr)(currentPtr.ToInt64() + Marshal.SizeOf(tdb_array[i]));
						}
						else
						{
							currentPtr = (IntPtr)(currentPtr.ToInt32() + Marshal.SizeOf(tdb_array[i]));
						}
					}

					tdc.pButtons = initialPtr;

					// UseCommandLinks only works if we specify custom buttons, apparently...
					switch (dlg.ButtonStyle)
					{
						case TaskDialogButtonStyle.Buttons:
						{
							// do nothing
							break;
						}
						case TaskDialogButtonStyle.Commands:
						{
							flags |= Constants.TaskDialogFlags.UseCommandLinks;
							break;
						}
						case TaskDialogButtonStyle.CommandsNoIcon:
						{
							flags |= Constants.TaskDialogFlags.UseCommandLinksNoIcon;
							break;
						}
					}
				}
				if ((int)dlg.ButtonsPreset > (int)TaskDialogButtons.None)
				{
					tdc.dwCommonButtons = (Constants.TaskDialogCommonButtonFlags)((int)dlg.ButtonsPreset);
				}
				else
				{
					tdc.dwCommonButtons = Constants.TaskDialogCommonButtonFlags.OK;
				}
				if (dlg.EnableHyperlinks)
				{
					flags |= Constants.TaskDialogFlags.EnableHyperlinks;
				}

				tdc.hMainIcon = new IntPtr((int)dlg.Icon);
				tdc.dwFlags = flags;
				tdc.hInstance = IntPtr.Zero;
				tdc.hwndParent = IntPtr.Zero;
				tdc.pfCallback += Tdc_PfCallback;

				if (parentHandle != null)
				{
					Console.WriteLine("setting hwndParent to {0}", parentHandle.Handle);
					tdc.hwndParent = parentHandle.Handle;
				}
				else
				{
					Console.WriteLine("parentHandle is NULL");
					tdc.hwndParent = IntPtr.Zero;
				}
				tdc.hInstance = Methods.GetWindowLongPtr(tdc.hwndParent, Constants.WindowLong.HInstance);

				tdc.hMainIcon = new IntPtr((int)dlg.Icon);
				tdc.pszMainInstruction = Marshal.StringToHGlobalAuto(dlg.Prompt);
				tdc.pszWindowTitle = Marshal.StringToHGlobalAuto(dlg.Text);
				tdc.pszContent = Marshal.StringToHGlobalAuto(dlg.Content);
				tdc.pszVerificationText = Marshal.StringToHGlobalAuto(dlg.VerificationText);
				// tdc.pszMainInstruction = dlg.Prompt;
				// tdc.pszWindowTitle = dlg.Text;
				// tdc.pszContent = dlg.Content;
				// tdc.pszVerificationText = dlg.VerificationText;
				// tdc.pszFooter = dlg.Footer;

				tdc.cbSize = (uint)Marshal.SizeOf(tdc);

				int num = rnd.Next();
				tdc.lpCallbackData = new IntPtr(num);

				// retval = Internal.Windows.Methods.TaskDialog((parentHandle?.Handle).GetValueOrDefault(IntPtr.Zero), IntPtr.Zero, dlg.Text, dlg.Prompt, dlg.Content, (int)dlg.ButtonsPreset, new IntPtr((int)dlg.Icon), out pnButton);

				RegisterTaskDialogHandle(dlg, num);
				uint ptr = Methods.TaskDialogIndirect(ref tdc, out pnButton, out pnRadioButton, out pfVerificationFlagChecked);
				// if (ptr.ToInt64() == 0x80070057)
				// 	throw new ArgumentException();
				switch ((Constants.TaskDialogResult)pnButton)
				{
					case Constants.TaskDialogResult.OK: return DialogResult.OK;
					case Constants.TaskDialogResult.Cancel: return DialogResult.Cancel;
					case Constants.TaskDialogResult.Retry: return DialogResult.Retry;
					case Constants.TaskDialogResult.Yes: return DialogResult.Yes;
					case Constants.TaskDialogResult.No: return DialogResult.No;
					// case 8: return DialogResult.Close;
				}

				// Console.WriteLine("return value from TaskDialogIndirect: {0}", ptr);
			}
			catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
			{
				// on pre-Vista systems this falls back nicely to a standard MessageBox
				MessageDialogButtons mdb = MessageDialogButtons.OK;
				MessageDialogIcon mic = MessageDialogIcon.Information;
				switch (dlg.ButtonsPreset)
				{
					case TaskDialogButtons.OK:
					{
						mdb = MessageDialogButtons.OK;
						break;
					}
					case TaskDialogButtons.OK | TaskDialogButtons.Cancel:
					{
						mdb = MessageDialogButtons.OKCancel;
						break;
					}
					case TaskDialogButtons.Retry | TaskDialogButtons.Cancel:
					{
						mdb = MessageDialogButtons.RetryCancel;
						break;
					}
					case TaskDialogButtons.Retry | TaskDialogButtons.Cancel | TaskDialogButtons.Close:
					{
						mdb = MessageDialogButtons.AbortRetryIgnore;
						break;
					}
					case TaskDialogButtons.Yes | TaskDialogButtons.No | TaskDialogButtons.Cancel:
					{
						mdb = MessageDialogButtons.YesNoCancel;
						break;
					}
					case TaskDialogButtons.Yes | TaskDialogButtons.No:
					{
						mdb = MessageDialogButtons.YesNo;
						break;
					}
				}

				switch (dlg.Icon)
				{
					case TaskDialogIcon.Error:
					case TaskDialogIcon.SecurityError:
					{
						mic = MessageDialogIcon.Error;
						break;
					}
					case TaskDialogIcon.Warning:
					case TaskDialogIcon.SecurityWarning:
					case TaskDialogIcon.SecurityUntrusted:
					{
						mic = MessageDialogIcon.Warning;
						break;
					}
					case TaskDialogIcon.Information:
					case TaskDialogIcon.Security:
					case TaskDialogIcon.SecurityTrusted:
					{
						mic = MessageDialogIcon.Information;
						break;
					}
					case TaskDialogIcon.Question:
					{
						mic = MessageDialogIcon.Question;
						break;
					}
				}
				DialogResult dr = MessageDialog.ShowDialog(dlg.Prompt + "\r\n\r\n" + dlg.Content, dlg.Text, mdb, mic);
				return dr;
			}
			return base.Run(parentHandle);
		}

		private Dictionary<TaskDialog, int> _TaskDialogHandles = new Dictionary<TaskDialog, int>();
		private Dictionary<int, TaskDialog> _TaskDialogsByHandle = new Dictionary<int, TaskDialog>();
		private void RegisterTaskDialogHandle(TaskDialog dlg, int tdc)
		{
			_TaskDialogHandles[dlg] = tdc;
			_TaskDialogsByHandle[tdc] = dlg;
		}

		int /*HRESULT*/ Tdc_PfCallback(IntPtr hwnd, uint msg, UIntPtr wParam, IntPtr lParam, IntPtr lpRefData)
		{
			switch ((Constants.TaskDialogNotification)msg)
			{
				case Constants.TaskDialogNotification.HyperlinkClicked:
				{
					int num = lpRefData.ToInt32();
					if (_TaskDialogsByHandle.ContainsKey(num))
					{
						InvokeMethod(_TaskDialogsByHandle[num], "OnHyperlinkClicked", new object[]
						{
							new TaskDialogHyperlinkClickedEventArgs()
						});
					}
					break;
				}
			}
			return 0;
		}

	}
}
