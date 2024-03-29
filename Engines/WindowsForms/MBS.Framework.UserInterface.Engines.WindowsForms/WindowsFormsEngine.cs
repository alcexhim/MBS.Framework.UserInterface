//
//  WindowsFormsEngine.cs - provides a Universal Widget Toolkit Engine powered by the Windows Forms toolkit
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019-2020 Mike Becker's Software
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Windows.Forms;

using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Engines.WindowsForms.Printing;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Printing;

namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	/// <summary>
	/// Provides a Universal Widget Toolkit <see cref="Engine" /> powered by the Windows Forms toolkit.
	/// </summary>
	public class WindowsFormsEngine : Engine
	{
		protected override int Priority => (System.Environment.OSVersion.Platform == PlatformID.Win32NT ? 1 : -1);

		protected override void UpdateTreeModelInternal(TreeModel tm, TreeModelChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		protected override void RegisterInhibitorInternal(Inhibitor item)
		{
			throw new NotImplementedException();
		}
		protected override void UnregisterInhibitorInternal(Inhibitor item)
		{
			throw new NotImplementedException();
		}

		protected override void SetMenuItemEnabledInternal(MenuItem item, bool enabled)
		{
			WindowsFormsNativeControl wfnc = GetHandleForMenuItem(item) as WindowsFormsNativeControl;
			if (wfnc == null) return;
			if (wfnc.Handle == null) return;

			wfnc.Handle.Enabled = enabled;
		}

		protected override void SetToolbarItemEnabledInternal(ToolbarItem item, bool enabled)
		{
			WindowsFormsNativeToolStripItem nc = (GetHandleForToolbarItem(item) as WindowsFormsNativeToolStripItem);
			nc.Handle.Enabled = enabled;
		}
		protected override void SetToolbarItemVisibilityInternal(ToolbarItem item, bool visible)
		{
			WindowsFormsNativeToolStripItem nc = (GetHandleForToolbarItem(item) as WindowsFormsNativeToolStripItem);
			nc.Handle.Visible = visible;
		}

		public static System.Windows.Forms.DialogResult DialogResultToSWFDialogResult(DialogResult dialogResult)
		{
			switch (dialogResult)
			{
				case DialogResult.Abort: return System.Windows.Forms.DialogResult.Abort;
				case DialogResult.Cancel: return System.Windows.Forms.DialogResult.Cancel;
				case DialogResult.Ignore: return System.Windows.Forms.DialogResult.Ignore;
				case DialogResult.No: return System.Windows.Forms.DialogResult.No;
				case DialogResult.None: return System.Windows.Forms.DialogResult.None;
				case DialogResult.OK: return System.Windows.Forms.DialogResult.OK;
				case DialogResult.Retry: return System.Windows.Forms.DialogResult.Retry;
				case DialogResult.Yes: return System.Windows.Forms.DialogResult.Yes;
			}
			return System.Windows.Forms.DialogResult.None;
		}
		public static DialogResult SWFDialogResultToDialogResult(System.Windows.Forms.DialogResult dialogResult)
		{
			switch (dialogResult)
			{
				case System.Windows.Forms.DialogResult.Abort: return DialogResult.Abort;
				case System.Windows.Forms.DialogResult.Cancel: return DialogResult.Cancel;
				case System.Windows.Forms.DialogResult.Ignore: return DialogResult.Ignore;
				case System.Windows.Forms.DialogResult.No: return DialogResult.No;
				case System.Windows.Forms.DialogResult.None: return DialogResult.None;
				case System.Windows.Forms.DialogResult.OK: return DialogResult.OK;
				case System.Windows.Forms.DialogResult.Retry: return DialogResult.Retry;
				case System.Windows.Forms.DialogResult.Yes: return DialogResult.Yes;
			}
			return DialogResult.None;
		}

		protected override void UpdateSystemColorsInternal()
		{
			UpdateSystemColor(SystemColor.HighlightBackground, System.Drawing.SystemColors.Highlight);
			UpdateSystemColor(SystemColor.HighlightForeground, System.Drawing.SystemColors.HighlightText);
			UpdateSystemColor(SystemColor.WindowForeground, System.Drawing.SystemColors.WindowText);
		}

		public static System.Drawing.ContentAlignment HorizontalVerticalAlignmentToContentAlignment(HorizontalAlignment ha, VerticalAlignment va)
		{
			if (ha == HorizontalAlignment.Default) ha = HorizontalAlignment.Left;
			if (va == VerticalAlignment.Default) va = VerticalAlignment.Middle;

			if (ha == HorizontalAlignment.Center && va == VerticalAlignment.Bottom) return System.Drawing.ContentAlignment.BottomCenter;
			if (ha == HorizontalAlignment.Center && va == VerticalAlignment.Middle) return System.Drawing.ContentAlignment.MiddleCenter;
			if (ha == HorizontalAlignment.Center && va == VerticalAlignment.Top) return System.Drawing.ContentAlignment.TopCenter;
			if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Bottom) return System.Drawing.ContentAlignment.BottomLeft;
			if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Middle) return System.Drawing.ContentAlignment.MiddleLeft;
			if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Top) return System.Drawing.ContentAlignment.TopLeft;
			if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Bottom) return System.Drawing.ContentAlignment.BottomRight;
			if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Middle) return System.Drawing.ContentAlignment.MiddleRight;
			if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Top) return System.Drawing.ContentAlignment.TopRight;

			throw new NotSupportedException();
		}

		public static System.Windows.Forms.Orientation OrientationToWindowsFormsOrientation(Orientation orientation)
		{
			switch (orientation)
			{
				case Orientation.Horizontal: return System.Windows.Forms.Orientation.Horizontal;
				case Orientation.Vertical: return System.Windows.Forms.Orientation.Vertical;
			}
			throw new NotSupportedException();
		}
		public static Orientation WindowsFormsOrientationToOrientation(System.Windows.Forms.Orientation orientation)
		{
			switch (orientation)
			{
				case System.Windows.Forms.Orientation.Horizontal: return Orientation.Horizontal;
				case System.Windows.Forms.Orientation.Vertical: return Orientation.Vertical;
			}
			throw new NotSupportedException();
		}

		internal static Input.Mouse.MouseButtons SWFMouseButtonsToMouseButtons(System.Windows.Forms.MouseButtons button)
		{
			Input.Mouse.MouseButtons buttons = Input.Mouse.MouseButtons.None;
			if ((button & System.Windows.Forms.MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left) buttons |= Input.Mouse.MouseButtons.Primary;
			if ((button & System.Windows.Forms.MouseButtons.Right) == System.Windows.Forms.MouseButtons.Right) buttons |= Input.Mouse.MouseButtons.Secondary;
			if ((button & System.Windows.Forms.MouseButtons.Middle) == System.Windows.Forms.MouseButtons.Left) buttons |= Input.Mouse.MouseButtons.Wheel;
			if ((button & System.Windows.Forms.MouseButtons.XButton1) == System.Windows.Forms.MouseButtons.Left) buttons |= Input.Mouse.MouseButtons.XButton1;
			if ((button & System.Windows.Forms.MouseButtons.XButton2) == System.Windows.Forms.MouseButtons.Left) buttons |= Input.Mouse.MouseButtons.XButton2;
			return buttons;
		}

		private System.Windows.Forms.Keys ShortcutToShortcutKeys(Shortcut shortcut)
		{
			System.Windows.Forms.Keys keys = System.Windows.Forms.Keys.None;
			if (shortcut == null)
				return keys;

			if ((shortcut.ModifierKeys & KeyboardModifierKey.Alt) == KeyboardModifierKey.Alt)
				keys |= System.Windows.Forms.Keys.Alt;
			if ((shortcut.ModifierKeys & KeyboardModifierKey.Control) == KeyboardModifierKey.Control)
				keys |= System.Windows.Forms.Keys.Control;
			if ((shortcut.ModifierKeys & KeyboardModifierKey.Shift) == KeyboardModifierKey.Shift)
				keys |= System.Windows.Forms.Keys.Shift;
			if ((shortcut.ModifierKeys & KeyboardModifierKey.Meta) == KeyboardModifierKey.Meta)
				keys |= System.Windows.Forms.Keys.Alt;
			if ((shortcut.ModifierKeys & KeyboardModifierKey.Control) == KeyboardModifierKey.Super)
				keys |= System.Windows.Forms.Keys.LWin;

			switch (shortcut.Key)
			{
				case KeyboardKey.A: keys |= System.Windows.Forms.Keys.A; break;
				case KeyboardKey.B: keys |= System.Windows.Forms.Keys.B; break;
				case KeyboardKey.C: keys |= System.Windows.Forms.Keys.C; break;
				case KeyboardKey.D: keys |= System.Windows.Forms.Keys.D; break;
				case KeyboardKey.E: keys |= System.Windows.Forms.Keys.E; break;
				case KeyboardKey.F: keys |= System.Windows.Forms.Keys.F; break;
				case KeyboardKey.G: keys |= System.Windows.Forms.Keys.G; break;
				case KeyboardKey.H: keys |= System.Windows.Forms.Keys.H; break;
				case KeyboardKey.I: keys |= System.Windows.Forms.Keys.I; break;
				case KeyboardKey.J: keys |= System.Windows.Forms.Keys.J; break;
				case KeyboardKey.K: keys |= System.Windows.Forms.Keys.K; break;
				case KeyboardKey.L: keys |= System.Windows.Forms.Keys.L; break;
				case KeyboardKey.M: keys |= System.Windows.Forms.Keys.M; break;
				case KeyboardKey.N: keys |= System.Windows.Forms.Keys.N; break;
				case KeyboardKey.O: keys |= System.Windows.Forms.Keys.O; break;
				case KeyboardKey.P: keys |= System.Windows.Forms.Keys.P; break;
				case KeyboardKey.Q: keys |= System.Windows.Forms.Keys.Q; break;
				case KeyboardKey.R: keys |= System.Windows.Forms.Keys.R; break;
				case KeyboardKey.S: keys |= System.Windows.Forms.Keys.S; break;
				case KeyboardKey.T: keys |= System.Windows.Forms.Keys.T; break;
				case KeyboardKey.U: keys |= System.Windows.Forms.Keys.U; break;
				case KeyboardKey.V: keys |= System.Windows.Forms.Keys.V; break;
				case KeyboardKey.W: keys |= System.Windows.Forms.Keys.W; break;
				case KeyboardKey.X: keys |= System.Windows.Forms.Keys.X; break;
				case KeyboardKey.Y: keys |= System.Windows.Forms.Keys.Y; break;
				case KeyboardKey.Z: keys |= System.Windows.Forms.Keys.Z; break;
				case KeyboardKey.D0: keys |= System.Windows.Forms.Keys.D0; break;
				case KeyboardKey.D1: keys |= System.Windows.Forms.Keys.D1; break;
				case KeyboardKey.D2: keys |= System.Windows.Forms.Keys.D2; break;
				case KeyboardKey.D3: keys |= System.Windows.Forms.Keys.D3; break;
				case KeyboardKey.D4: keys |= System.Windows.Forms.Keys.D4; break;
				case KeyboardKey.D5: keys |= System.Windows.Forms.Keys.D5; break;
				case KeyboardKey.D6: keys |= System.Windows.Forms.Keys.D6; break;
				case KeyboardKey.D7: keys |= System.Windows.Forms.Keys.D7; break;
				case KeyboardKey.D8: keys |= System.Windows.Forms.Keys.D8; break;
				case KeyboardKey.D9: keys |= System.Windows.Forms.Keys.D9; break;
				case KeyboardKey.NumPad0: keys |= System.Windows.Forms.Keys.NumPad0; break;
				case KeyboardKey.NumPad1: keys |= System.Windows.Forms.Keys.NumPad1; break;
				case KeyboardKey.NumPad2: keys |= System.Windows.Forms.Keys.NumPad2; break;
				case KeyboardKey.NumPad3: keys |= System.Windows.Forms.Keys.NumPad3; break;
				case KeyboardKey.NumPad4: keys |= System.Windows.Forms.Keys.NumPad4; break;
				case KeyboardKey.NumPad5: keys |= System.Windows.Forms.Keys.NumPad5; break;
				case KeyboardKey.NumPad6: keys |= System.Windows.Forms.Keys.NumPad6; break;
				case KeyboardKey.NumPad7: keys |= System.Windows.Forms.Keys.NumPad7; break;
				case KeyboardKey.NumPad8: keys |= System.Windows.Forms.Keys.NumPad8; break;
				case KeyboardKey.NumPad9: keys |= System.Windows.Forms.Keys.NumPad9; break;
			}
			return keys;
		}

		internal ToolStripItem InitMenuItem(MenuItem menuItem, string accelPath = null)
		{
			if (menuItem is CommandMenuItem)
			{
				CommandMenuItem cmi = (menuItem as CommandMenuItem);
				if (accelPath != null)
				{

					string cmiName = cmi.Name;
					if (String.IsNullOrEmpty(cmiName))
					{
						cmiName = cmi.Text;
					}

					// clear out the possible mnemonic definitions
					cmiName = cmiName.Replace("_", String.Empty);

					accelPath += "/" + cmiName;
					if (cmi.Shortcut != null)
					{

					}
				}

				System.Windows.Forms.ToolStripMenuItem hMenuFile = new ToolStripMenuItem();
				hMenuFile.Tag = cmi;
				hMenuFile.Text = cmi.Text.Replace('_', '&');
				hMenuFile.Enabled = cmi.Enabled;
				hMenuFile.Click += HMenuFile_Click;

				try
				{
					hMenuFile.ShortcutKeys = ShortcutToShortcutKeys(cmi.Shortcut);
				}
				catch (Exception ex)
				{
					Console.WriteLine("could not set shortcut keys value from uwt {0} to winforms {1}", cmi.Shortcut, ShortcutToShortcutKeys(cmi.Shortcut));
				}

				string imageFileName = null;
				if (cmi.StockType != StockType.None)
				{
					imageFileName = ((UIApplication)Application.Instance).ExpandRelativePath(String.Format("~/Themes/{0}/Images/StockIcons/{1}.png", Theming.Theme.CurrentTheme.Name, StockTypeToString(cmi.StockType)));
				}
				else if (cmi.IconName != null)
				{
					imageFileName = ((UIApplication)Application.Instance).ExpandRelativePath(String.Format("~/Themes/{0}/Images/StockIcons/{1}.png", Theming.Theme.CurrentTheme.Name, cmi.IconName));
				}

				if (imageFileName != null)
					hMenuFile.Image = System.Drawing.Image.FromFile(imageFileName);

				/*
				if (menuItem.HorizontalAlignment == MenuItemHorizontalAlignment.Right)
				{
					hMenuFile.Alignment = ToolStripItemAlignment.Right;
				}
				*/

				if (cmi.Items.Count > 0)
				{
					for (int i = 0; i < cmi.Items.Count; i++)
					{
						if (cmi.Items[i] == null)
						{
							Console.WriteLine("uwt: wf: ERROR: MenuItem is null in {0} ({1})", cmi.Text, cmi.Name);
							continue;
						}

						System.Windows.Forms.ToolStripItem tsiChild = InitMenuItem(cmi.Items[i]);
						if (tsiChild == null) continue;

						hMenuFile.DropDownItems.Add(tsiChild);
					}
				}

				RegisterMenuItemHandle(cmi, new WindowsFormsNativeMenuItem(hMenuFile));
				return hMenuFile;
			}
			else if (menuItem is SeparatorMenuItem)
			{
				System.Windows.Forms.ToolStripSeparator hMenuFile = new ToolStripSeparator();
				RegisterMenuItemHandle(menuItem, new WindowsFormsNativeMenuItem(hMenuFile));
				return hMenuFile;
			}

			Console.WriteLine("uwt: wf: ERROR: could not create ToolStripMenuItem {0}", menuItem.GetType().FullName);
			return null;
		}

		void HMenuFile_Click(object sender, EventArgs e)
		{
			((sender as ToolStripMenuItem).Tag as CommandMenuItem).OnClick(e);
		}


		public ContextMenuStrip BuildContextMenuStrip(Menu menu, string accelPath = null)
		{
			System.Windows.Forms.ContextMenuStrip hMenuFileMenu = new ContextMenuStrip();
			if (menu.EnableTearoff)
			{
				// TODO: Implement this for Windows Forms
			}

			if (accelPath != null)
			{
			}

			foreach (MenuItem menuItem1 in menu.Items)
			{
				System.Windows.Forms.ToolStripItem hMenuItem = InitMenuItem(menuItem1, accelPath);
				hMenuFileMenu.Items.Add(hMenuItem);
			}
			return hMenuFileMenu;
		}

		public Control GetControlByHandle(System.Windows.Forms.Control handle)
		{
			foreach (KeyValuePair<NativeControl, Control> kvp in controlsByHandle)
			{
				if (kvp.Key is WindowsFormsNativeControl)
				{
					if ((kvp.Key as WindowsFormsNativeControl).ContainsHandle(handle))
					{
						return kvp.Value;
					}
				}
			}
			return null;
		}

		public ContextMenuStrip BuildContextMenuStrip(CommandMenuItem cmi, IntPtr hMenuFile, string accelPath = null)
		{
			ContextMenuStrip hMenuFileMenu = new ContextMenuStrip();
			if (cmi.EnableTearoff)
			{
				// TODO: implement this for Windows Forms
			}

			foreach (MenuItem menuItem1 in cmi.Items)
			{
				ToolStripItem hMenuItem = InitMenuItem(menuItem1, accelPath);
				hMenuFileMenu.Items.Add(hMenuItem);
			}
			return hMenuFileMenu;
		}

		public void UpdateSystemColor(SystemColor color, System.Drawing.Color value)
		{
			UpdateSystemColor(color, Color.FromRGBAByte(value.R, value.G, value.B, value.A));
		}

		protected override void UpdateControlLayoutInternal(Control control)
		{
		}
		protected override void UpdateControlPropertiesInternal(Control control, NativeControl handle)
		{
		}
		protected override void UpdateNotificationIconInternal(NotificationIcon nid, bool updateContextMenu)
		{
			throw new NotImplementedException();
		}

		protected override Graphics CreateGraphicsInternal(Image image)
		{
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(((WindowsFormsNativeImage)image).Handle);
			return new WindowsFormsNativeGraphics(g);
		}

		protected override Image CreateImage(int width, int height)
		{
			return new WindowsFormsNativeImage(new System.Drawing.Bitmap(width, height));
		}
		protected override Image LoadImageByName(string name, int size)
		{
			return null;
		}
		protected override Image LoadImage(StockType stockType, int size)
		{
			// TODO: figure out which images to use for stock images on WinForms
			// because WinForms does not have any concept of "stock images"
			return null;
		}
		protected override Image LoadImage(byte[] filedata, string type)
		{
			System.IO.MemoryStream ms = new System.IO.MemoryStream(filedata);
			System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
			return new WindowsFormsNativeImage(image);
		}
		protected override Image LoadImage(byte[] filedata, int width, int height, int rowstride)
		{
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(new System.IO.MemoryStream(filedata));
			return new WindowsFormsNativeImage(bitmap);
		}

		internal static Input.Keyboard.KeyEventArgs SWFKeyEventArgsToKeyEventArgs(System.Windows.Forms.KeyEventArgs e)
		{
			return new Input.Keyboard.KeyEventArgs(SWFKeysToKeyboardKey(e.KeyCode), SWFKeysToKeyboardModifierKey(e.KeyCode), e.KeyValue, e.KeyValue);
		}

		protected override Image LoadImage(string filename, string type = null)
		{
			System.Drawing.Image image = System.Drawing.Image.FromFile(filename);
			return new WindowsFormsNativeImage(image);
		}

		protected override bool WindowHasFocusInternal(Window window)
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
				{
					IntPtr hWndActive = Internal.Windows.Methods.GetActiveWindow();
					if (window.ControlImplementation != null)
					{
						if (window.ControlImplementation.Handle is Win32NativeControl)
						{
							return (window.ControlImplementation.Handle as Win32NativeControl).Handle.Equals(hWndActive);
						}
						else if (window.ControlImplementation.Handle is WindowsFormsNativeControl)
						{
							return (window.ControlImplementation.Handle as WindowsFormsNativeControl).Handle.Handle.Equals(hWndActive);
						}
					}
					else
					{
						NativeControl nc = GetHandleForControl(window);
						IntPtr hWnd = IntPtr.Zero;
						if (nc is Win32NativeControl)
						{
							hWnd = (nc as Win32NativeControl).Handle;
						}
						else if (nc is WindowsFormsNativeControl)
						{
							hWnd = (nc as WindowsFormsNativeControl).Handle.Handle;
						}
						return hWnd.Equals(hWndActive);
					}
					break;
				}
			}
			return false;
		}

		protected override DialogResult ShowDialogInternal(Dialog dialog, Window parent)
		{
			Console.WriteLine("dialog is {0}", dialog);
			System.Windows.Forms.IWin32Window parentHandle = null;
			parent = null;
			if (parent == null)
			{
				Console.WriteLine("uwt: wf: warning: ShowDialogInternal called with NULL parent, please fix!");

				if (dialog.Parent != null)
				{
					parentHandle = (GetHandleForControl(dialog.Parent) as WindowsFormsNativeControl)?.Handle;
				}
				else
				{
					Window focusedTopLevelWindow = GetFocusedToplevelWindow();
					Console.WriteLine("focusedTopLevelWindow is {0}", focusedTopLevelWindow);

					parentHandle = new Win32Window((GetHandleForControl(focusedTopLevelWindow) as Win32NativeControl).Handle);
				}
			}
			else
			{
				parentHandle = (GetHandleForControl(parent) as WindowsFormsNativeControl)?.Handle;
			}
			Type[] types = Reflection.GetAvailableTypes(new Type[] { typeof(WindowsFormsDialogImplementation) });
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsAbstract) continue;

				object[] atts = (types[i].GetCustomAttributes(typeof(ControlImplementationAttribute), false));
				if (atts.Length > 0)
				{
					ControlImplementationAttribute cia = (atts[0] as ControlImplementationAttribute);
					if (cia != null)
					{
						// yeah... that's a hack right ---------------------------------->there
						// it can be fixed, but we'd have to figure out the best way to implement CustomDialog vs. CommonDialog without
						// having the GenericDialogImplementation hijack the CommonDialog stuff if it comes up first in the list
						if (dialog.GetType().IsSubclassOf(cia.ControlType) || dialog.GetType() == cia.ControlType || (dialog.GetType().BaseType == typeof(Dialog) && cia.ControlType.BaseType == typeof(Dialog)))
						{
							WindowsFormsDialogImplementation di = (types[i].Assembly.CreateInstance(types[i].FullName, false, System.Reflection.BindingFlags.Default, null, new object[] { this, dialog }, System.Globalization.CultureInfo.CurrentCulture, null) as WindowsFormsDialogImplementation);
							WindowsFormsNativeDialog nc = (di.CreateControl(dialog) as WindowsFormsNativeDialog);
							DialogResult result1 = di.Run(parentHandle);
							return result1;
						}
					}
				}
			}
			return DialogResult.None;

			/*


			if (dialog is FileDialog)
			{
				FileDialog fd = (dialog as FileDialog);
				switch (fd.Mode)
				{
					case FileDialogMode.CreateFolder:
					case FileDialogMode.SelectFolder:
					{
						System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
						System.Windows.Forms.DialogResult result = fbd.ShowDialog();
						return SWFDialogResultToDialogResult(result);
					}
					case FileDialogMode.Open:
					{
						System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
						ofd.Title = fd.Text;
						ofd.FileName = fd.SelectedFileNames[fd.SelectedFileNames.Count - 1];
						System.Windows.Forms.DialogResult result = ofd.ShowDialog();
						foreach (string fn in ofd.FileNames)
						{
							fd.SelectedFileNames.Add(fn);
						}
						return SWFDialogResultToDialogResult(result);
					}
					case FileDialogMode.Save:
					{
						System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
						sfd.Title = fd.Text;
						sfd.FileName = fd.SelectedFileNames[fd.SelectedFileNames.Count - 1];
						System.Windows.Forms.DialogResult result = sfd.ShowDialog();
						foreach (string fn in sfd.FileNames)
						{
							fd.SelectedFileNames.Add(fn);
						}
						return SWFDialogResultToDialogResult(result);
					}
				}
			}
			else
			{
				if (!dialog.IsCreated)
					CreateControl(dialog);

				NativeControl nc = GetHandleForControl(dialog);
				if (nc == null)
				{
					Console.WriteLine("uwt: wf: error: dialog container did not get created");
					return DialogResult.None;
				}

				WindowsFormsNativeControl wfnc = (nc as WindowsFormsNativeControl);
				if (wfnc == null)
				{
					Console.WriteLine("uwt: wf: error: dialog container handle is not a WindowsFormsNativeControl");
					return DialogResult.None;
				}

				System.Windows.Forms.Form pnl = wfnc.Handle as System.Windows.Forms.Form;
				if (pnl == null)
				{
					Console.WriteLine("uwt: wf: error: wfnc.Handle is NULL or not a System.Windows.Forms.Form ({0})", wfnc.Handle?.GetType()?.FullName);
					return DialogResult.None;
				}

				System.Windows.Forms.Panel pnlButtons = new System.Windows.Forms.Panel();
				pnlButtons.Text = "<ButtonBox>";
				pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
				for (int i = 0; i < dialog.Buttons.Count; i++)
				{
					CreateControl(dialog.Buttons[i]);

					WindowsFormsNativeControl ncButton = (GetHandleForControl(dialog.Buttons[i]) as WindowsFormsNativeControl);
					if (ncButton == null)
					{
						Console.WriteLine("uwt: wf: error: button not registered '{0}'", dialog.Buttons[i].Text);
						continue;
					}
					pnlButtons.Controls.Add(ncButton.Handle);
				}
				pnl.Controls.Add(pnlButtons);
				pnl.Font = System.Drawing.SystemFonts.MenuFont;

				System.Windows.Forms.DialogResult result = pnl.ShowDialog();
				return SWFDialogResultToDialogResult(result);
			}
			throw new NotImplementedException();
			*/
		}

		internal static KeyboardKey SWFKeysToKeyboardKey(System.Windows.Forms.Keys keys)
		{
			// we should be able to do this...
			return (KeyboardKey)((int)keys);
		}
		internal static KeyboardModifierKey SWFKeysToKeyboardModifierKey(System.Windows.Forms.Keys keys)
		{
			KeyboardModifierKey modifiers = KeyboardModifierKey.None;
			if ((keys & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift) modifiers |= KeyboardModifierKey.Shift;
			if ((keys & System.Windows.Forms.Keys.Menu) == System.Windows.Forms.Keys.Menu) modifiers |= KeyboardModifierKey.Alt;
			if ((keys & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control) modifiers |= KeyboardModifierKey.Control;
			return modifiers;
		}

		private List<Window> _GetToplevelWindowsRetval = null;

		protected override Window[] GetToplevelWindowsInternal()
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT: return W32_GetToplevelWindowsInternal();
				default: throw new PlatformNotSupportedException();
			}
		}

		private Internal.Windows.Delegates.EnumWindowsProc W32_GetToplevelWindowsCallback;
		public WindowsFormsEngine()
		{
			W32_GetToplevelWindowsCallback = new Internal.Windows.Delegates.EnumWindowsProc(W32_GetToplevelWindowsCallbackFunc);

			// the stock types IDs here are the IDs used by Glade Interface Designer, have nothing to do with copypasta from GTKEngine ;)
			RegisterStockType(StockType.About, "gtk-about", "_About");
			RegisterStockType(StockType.Add, "gtk-add", "A_dd");
			RegisterStockType(StockType.Apply, "gtk-apply", "_Apply");
			RegisterStockType(StockType.Bold, "gtk-bold", "_Bold");
			RegisterStockType(StockType.Cancel, "gtk-cancel", "Cancel");
			RegisterStockType(StockType.CapsLockWarning, "gtk-caps-lock-warning", "Caps Lock");
			RegisterStockType(StockType.CDROM, "gtk-cdrom", "CD-ROM");
			RegisterStockType(StockType.Clear, "gtk-clear", "Clear");
			RegisterStockType(StockType.Close, "gtk-close", "Close");
			RegisterStockType(StockType.ColorPicker, "gtk-color-picker", "Color Picker");
			RegisterStockType(StockType.Connect, "gtk-connect", "Connect");
			RegisterStockType(StockType.Convert, "gtk-convert", "Convert");
			RegisterStockType(StockType.Copy, "gtk-copy", "_Copy");
			RegisterStockType(StockType.Cut, "gtk-cut", "Cu_t");
			RegisterStockType(StockType.Delete, "gtk-delete", "_Delete");
			RegisterStockType(StockType.DialogAuthentication, "gtk-dialog-authentication", "Authentication");
			RegisterStockType(StockType.DialogInfo, "gtk-dialog-info", "Information");
			RegisterStockType(StockType.DialogWarning, "gtk-dialog-warning", "Warning");
			RegisterStockType(StockType.DialogError, "gtk-dialog-error", "Error");
			RegisterStockType(StockType.DialogQuestion, "gtk-dialog-question", "Question");
			RegisterStockType(StockType.Directory, "gtk-directory", "Directory");
			RegisterStockType(StockType.Discard, "gtk-discard", "Discard");
			RegisterStockType(StockType.Disconnect, "gtk-disconnect", "Disconnect");
			RegisterStockType(StockType.DragAndDrop, "gtk-dnd", "Drag-and-Drop");
			RegisterStockType(StockType.DragAndDropMultiple, "gtk-dnd-multiple", "Drag-and-Drop Multiple");
			RegisterStockType(StockType.Edit, "gtk-edit", "Edit");
			RegisterStockType(StockType.Execute, "gtk-execute", "E_xecute");
			RegisterStockType(StockType.File, "gtk-file", "_File");
			RegisterStockType(StockType.Find, "gtk-find", "_Find");
			RegisterStockType(StockType.FindAndReplace, "gtk-find-and-replace", "Find and _Replace");
			RegisterStockType(StockType.Floppy, "gtk-floppy", "Floppy");
			RegisterStockType(StockType.Fullscreen, "gtk-fullscreen", "F_ullscreen");
			RegisterStockType(StockType.GotoBottom, "gtk-goto-bottom", "Go to _Bottom");
			RegisterStockType(StockType.GotoFirst, "gtk-goto-first", "Go to _First");
			RegisterStockType(StockType.GotoLast, "gtk-goto-last", "Go to _Last");
			RegisterStockType(StockType.GotoTop, "gtk-goto-top", "Go to _Top");
			RegisterStockType(StockType.GoBack, "gtk-go-back", "Go _Back");
			RegisterStockType(StockType.GoDown, "gtk-go-down", "Go _Down");
			RegisterStockType(StockType.GoForward, "gtk-go-forward", "Go _Forward");
			RegisterStockType(StockType.GoUp, "gtk-go-up", "Go _Up");
			RegisterStockType(StockType.HardDisk, "gtk-harddisk", "Hard Disk");
			RegisterStockType(StockType.Help, "gtk-help", "_Help");
			RegisterStockType(StockType.Home, "gtk-home", "Home");
			RegisterStockType(StockType.Index, "gtk-index", "Index");
			RegisterStockType(StockType.Indent, "gtk-indent", "Indent");
			RegisterStockType(StockType.Info, "gtk-info", "Information");
			RegisterStockType(StockType.Italic, "gtk-italic", "_Italic");
			RegisterStockType(StockType.JumpTo, "gtk-jump-to", "_Go To");
			RegisterStockType(StockType.JustifyCenter, "gtk-justify-center", "_Center");
			RegisterStockType(StockType.JustifyFill, "gtk-justify-fill", "_Justify");
			RegisterStockType(StockType.JustifyLeft, "gtk-justify-left", "Align _Left");
			RegisterStockType(StockType.JustifyRight, "gtk-justify-right", "Align _Right");
			RegisterStockType(StockType.LeaveFullscreen, "gtk-leave-fullscreen", "Leave F_ullscreen");
			RegisterStockType(StockType.MissingImage, "gtk-missing-image", "Missing Image");
			RegisterStockType(StockType.MediaForward, "gtk-media-forward", "_Forward");
			RegisterStockType(StockType.MediaNext, "gtk-media-next", "_Next");
			RegisterStockType(StockType.MediaPause, "gtk-media-pause", "_Pause");
			RegisterStockType(StockType.MediaPlay, "gtk-media-play", "_Play");
			RegisterStockType(StockType.MediaPrevious, "gtk-media-previous", "Pre_vious");
			RegisterStockType(StockType.MediaRecord, "gtk-media-record", "_Record");
			RegisterStockType(StockType.MediaRewind, "gtk-media-rewind", "Re_wind");
			RegisterStockType(StockType.MediaStop, "gtk-media-stop", "_Stop");
			RegisterStockType(StockType.Network, "gtk-network", "Network");
			RegisterStockType(StockType.New, "gtk-new", "_New");
			RegisterStockType(StockType.No, "gtk-no", "_No");
			RegisterStockType(StockType.OK, "gtk-ok", "_OK");
			RegisterStockType(StockType.Open, "gtk-open", "_Open");
			RegisterStockType(StockType.OrientationPortrait, "gtk-orientation-portrait", "Portrait");
			RegisterStockType(StockType.OrientationLandscape, "gtk-orientation-landscape", "Landscape");
			RegisterStockType(StockType.OrientationReverseLandscape, "gtk-orientation-reverse-landscape", "Reverse Landscape");
			RegisterStockType(StockType.OrientationReversePortrait, "gtk-orientation-reverse-portrait", "Reverse Portrait");
			RegisterStockType(StockType.PageSetup, "gtk-page-setup", "Page Set_up");
			RegisterStockType(StockType.Paste, "gtk-paste", "_Paste");
			RegisterStockType(StockType.Preferences, "gtk-preferences", "P_references");
			RegisterStockType(StockType.Print, "gtk-print", "_Print");
			RegisterStockType(StockType.PrintError, "gtk-print-error", "Print Error");
			RegisterStockType(StockType.PrintPaused, "gtk-print-paused", "Print Paused");
			RegisterStockType(StockType.PrintPreview, "gtk-print-preview", "Print Pre_view");
			RegisterStockType(StockType.PrintReport, "gtk-print-report", "Print Report");
			RegisterStockType(StockType.PrintWarning, "gtk-print-warning", "Print Warning");
			RegisterStockType(StockType.Properties, "gtk-properties", "P_roperties");
			RegisterStockType(StockType.Quit, "gtk-quit", "_Quit");
			RegisterStockType(StockType.Redo, "gtk-redo", "_Redo");
			RegisterStockType(StockType.Refresh, "gtk-refresh", "R_efresh");
			RegisterStockType(StockType.Remove, "gtk-remove", "Re_move");
			RegisterStockType(StockType.RevertToSaved, "gtk-revert-to-saved", "Revert to Saved");
			RegisterStockType(StockType.Save, "gtk-save", "_Save");
			RegisterStockType(StockType.SaveAs, "gtk-save-as", "Save _As");
			RegisterStockType(StockType.SelectAll, "gtk-select-all", "Select _All");
			RegisterStockType(StockType.SelectColor, "gtk-select-color", "Select _Color");
			RegisterStockType(StockType.SelectFont, "gtk-select-font", "Select _Font");
			RegisterStockType(StockType.SortAscending, "gtk-sort-ascending", "Sort _Ascending");
			RegisterStockType(StockType.SortDescending, "gtk-sort-descending", "Sort _Descending");
			RegisterStockType(StockType.SpellCheck, "gtk-spell-check", "Check Spelling");
			RegisterStockType(StockType.Stop, "gtk-stop", "_Stop");
			RegisterStockType(StockType.Strikethrough, "gtk-strikethrough", "Strikethrough");
			RegisterStockType(StockType.Undelete, "gtk-undelete", "Undelete");
			RegisterStockType(StockType.Underline, "gtk-underline", "_Underline");
			RegisterStockType(StockType.Undo, "gtk-undo", "_Undo");
			RegisterStockType(StockType.Unindent, "gtk-unindent", "Unindent");
			RegisterStockType(StockType.Yes, "gtk-yes", "_Yes");
			RegisterStockType(StockType.Zoom100, "gtk-zoom-100", "Zoom 100%");
			RegisterStockType(StockType.ZoomFit, "gtk-zoom-fit", "_Fit to Window");
			RegisterStockType(StockType.ZoomIn, "gtk-zoom-in", "Zoom _In");
			RegisterStockType(StockType.ZoomOut, "gtk-zoom-out", "Zoom _Out");
		}

		private bool W32_GetToplevelWindowsCallbackFunc(IntPtr hWnd, IntPtr lParam)
		{
			if (_GetToplevelWindowsRetval != null)
			{
				Window window = new Window();
				RegisterControlHandle(window, new Win32NativeControl(hWnd), true);

				_GetToplevelWindowsRetval.Add(window);
			}
			return true;
		}

		private Window[] W32_GetToplevelWindowsInternal()
		{
			if (_GetToplevelWindowsRetval != null)
				return new Window[0];

			_GetToplevelWindowsRetval = new List<Window>();
			bool success = Internal.Windows.Methods.EnumWindows(W32_GetToplevelWindowsCallback, IntPtr.Zero);
			Window[] list = _GetToplevelWindowsRetval.ToArray();
			_GetToplevelWindowsRetval = null;
			return list;
		}

		private void SingleInstanceManager_Callback(object sender, SingleInstanceManager.InstanceCallbackEventArgs e)
		{
			if (!e.IsFirstInstance)
			{
				ApplicationActivatedEventArgs ee = new ApplicationActivatedEventArgs(false);
				ee.CommandLine = new WindowsFormsCommandLine(e.CommandLineArgs);
				InvokeMethod(Application.Instance, "OnActivated", new object[] { ee });

				/*
				if (LastWindow != null)
				{
					Document[] documents = new Document[e.CommandLineArgs.Length - 1];
					for (int i = 1; i < e.CommandLineArgs.Length; i++)
					{
						documents[i - 1] = new Document(null, null, new FileAccessor(e.CommandLineArgs[i]));
					}

					LastWindow.OpenFile(documents);
					LastWindow.ActivateWindow();
				}
				*/
			}
		}

		protected override int StartInternal(Window waitForClose = null)
		{
			InvokeMethod(Application.Instance, "OnStartup", new object[] { EventArgs.Empty });

			string INSTANCEID = GetType().FullName + "$2d429aa3371c421fb63b42525e51a50c$92751853175891031214292357218181357901238$";
			/*
			if (ConfigurationManager.GetValue<bool>("SingleInstanceUniquePerDirectory", true))
			{
				// The single instance should be unique per directory
				INSTANCEID += System.Reflection.Assembly.GetEntryAssembly().Location;
			}
			*/

			if (!SingleInstanceManager.CreateSingleInstance(INSTANCEID, new EventHandler<SingleInstanceManager.InstanceCallbackEventArgs>(SingleInstanceManager_Callback))) return -1;

			if (waitForClose != null)
			{
				WindowsFormsNativeControl ncWaitForClose = (GetHandleForControl(waitForClose) as WindowsFormsNativeControl);
				System.Windows.Forms.Application.Run(ncWaitForClose.Handle as System.Windows.Forms.Form);
			}
			else
			{
				ApplicationActivatedEventArgs e = new ApplicationActivatedEventArgs();
				e.CommandLine = new WindowsFormsCommandLine(Environment.GetCommandLineArgs());
				InvokeMethod(Application.Instance, "OnActivated", new object[] { e });
				System.Windows.Forms.Application.Run();
			}
			return 0;
		}
		protected override void StopInternal(int exitCode)
		{
			System.Windows.Forms.Application.Exit();
		}

		protected override void ShowNotificationPopupInternal(NotificationPopup popup)
		{
			// Internal.Windows.UWP.Methods.RoGetActivationFactory(Internal.Windows.UWP.Constants.RuntimeClass_Windows_UI_Notifications_ToastNotificationManager, 0, ptr);
		}

		protected override void SetControlEnabledInternal(Control control, bool value)
		{
			if (handlesByControl[control] is Win32NativeControl)
			{
				// this is exactly why this should be handled by the ControlImplementation and not the Engine >.>
				// Internal.Windows.Methods.EnableWindow((handlesByControl[control] as Win32NativeControl).Handle, value ? 1 : 0);
			}
			else
			{
				(handlesByControl[control] as WindowsFormsNativeControl).Handle.Enabled = value;
			}
		}

		protected override Vector2D ClientToScreenCoordinatesInternal(Control control, Vector2D point)
		{
			if (!control.IsCreated)
				return point;
			return SystemDrawingPointToVector2D((((control.ControlImplementation as WindowsFormsNativeImplementation).Handle as WindowsFormsNativeControl)?.Handle.PointToScreen(Vector2DToSystemDrawingPoint(point))).GetValueOrDefault());
		}

		public static Vector2D SystemDrawingPointToVector2D(System.Drawing.Point v)
		{
			return new Vector2D(v.X, v.Y);
		}
		public static Vector2D SystemDrawingPointFToVector2D(System.Drawing.PointF v)
		{
			return new Vector2D(v.X, v.Y);
		}
		public static System.Drawing.PointF Vector2DToSystemDrawingPointF(Vector2D point)
		{
			return new System.Drawing.PointF((float)point.X, (float)point.Y);
		}
		public static System.Drawing.Point Vector2DToSystemDrawingPoint(Vector2D point)
		{
			return new System.Drawing.Point((int)point.X, (int)point.Y);
		}

		public static System.Drawing.SizeF Dimension2DToSystemDrawingSizeF(Dimension2D size)
		{
			return new System.Drawing.SizeF((float)size.Width, (float)size.Height);
		}
		public static System.Drawing.Size Dimension2DToSystemDrawingSize(Dimension2D size)
		{
			return new System.Drawing.Size((int)size.Width, (int)size.Height);
		}

		protected override void DoEventsInternal()
		{
			System.Windows.Forms.Application.DoEvents();
		}

		protected override Monitor[] GetMonitorsInternal()
		{
			List<Monitor> list = new List<Monitor>();
			foreach (System.Windows.Forms.Screen scr in System.Windows.Forms.Screen.AllScreens)
			{
				list.Add(new WindowsFormsMonitor(scr));
			}
			return list.ToArray();
		}

		private Rectangle SDRectangleToUWTRectangle(System.Drawing.Rectangle rect)
		{
			return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		}

		protected override void RepaintCustomControl(CustomControl control, int x, int y, int width, int height)
		{
			throw new NotImplementedException();
		}

		protected override bool IsControlDisposedInternal(Control ctl)
		{
			if (!IsControlCreated(ctl)) return true;
			return (GetHandleForControl(ctl) as WindowsFormsNativeControl).Handle.IsDisposed;
		}
		protected override bool IsControlEnabledInternal(Control control)
		{
			return (GetHandleForControl(control) as WindowsFormsNativeControl).Handle.Enabled;
		}

		protected override bool InitializeInternal()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
			{
				try
				{
					switch (((UIApplication)Application.Instance).DpiAwareness)
					{
						case DpiAwareness.Unaware:
						{
							Internal.Windows.Methods.SetProcessDpiAwarenessContext(Internal.Windows.Constants.DpiAwarenessContext.Unaware);
							break;
						}
						case DpiAwareness.SystemAware:
						{
							Internal.Windows.Methods.SetProcessDpiAwarenessContext(Internal.Windows.Constants.DpiAwarenessContext.SystemAware);
							break;
						}
						case DpiAwareness.PerMonitorAware:
						{
							Internal.Windows.Methods.SetProcessDpiAwarenessContext(Internal.Windows.Constants.DpiAwarenessContext.PerMonitorAware);
							break;
						}
						case DpiAwareness.PerWindowAware:
						{
							Internal.Windows.Methods.SetProcessDpiAwarenessContext(Internal.Windows.Constants.DpiAwarenessContext.PerWindowAware);
							break;
						}
						case DpiAwareness.UnawareGDIScaled:
						{
							Internal.Windows.Methods.SetProcessDpiAwarenessContext(Internal.Windows.Constants.DpiAwarenessContext.UnawareGDIScaled);
							break;
						}
					}
				}
				catch (EntryPointNotFoundException ex)
				{
					Console.WriteLine("SetProcessDpiAwarenessContext failed - are you using a supported version of Windows?");
				}
			}

			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

			if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 1))
			{
				Internal.Windows.Methods.SetCurrentProcessExplicitAppUserModelID(String.Format("net.alcetech.framework.apps.{0}", Application.Instance.ID.ToString("N")));
			}

			System.Windows.Forms.ToolStripManager.Renderer = new CBRenderer();
			return true;
		}

		protected override Printer[] GetPrintersInternal()
		{
			List<Printer> list = new List<Printer>();
			foreach (string p in PrinterSettings.InstalledPrinters)
			{
				list.Add(new WindowsFormsPrinter(p));
			}
			return list.ToArray();
		}

		public static System.Windows.Forms.ColumnHeaderStyle HeaderStyleToSWFHeaderStyle(ColumnHeaderStyle headerStyle)
		{
			switch (headerStyle)
			{
				case ColumnHeaderStyle.Clickable: return System.Windows.Forms.ColumnHeaderStyle.Clickable;
				case ColumnHeaderStyle.Nonclickable: return System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
				case ColumnHeaderStyle.None: return System.Windows.Forms.ColumnHeaderStyle.None;
			}
			throw new NotSupportedException();
		}

		protected override void PrintInternal(PrintJob job)
		{
			PrintDocument doc = new PrintDocument();
			doc.BeginPrint += Doc_BeginPrint;
			doc.PrintPage += Doc_PrintPage;
			doc.Print();
		}

		void Doc_PrintPage(object sender, PrintPageEventArgs e)
		{
		}


		void Doc_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
		}

		public override TreeModelManager TreeModelManager => new WindowsFormsTreeModelManager();

		private static WindowsFormsClipboard _Clipboard = null;
		protected override Clipboard GetDefaultClipboardInternal()
		{
			if (_Clipboard == null)
			{
				_Clipboard = new WindowsFormsClipboard();
			}
			return _Clipboard;
		}

		internal static Dimension2D SystemDrawingSizeToDimension2D(System.Drawing.Size size)
		{
			return new Dimension2D(size.Width, size.Height);
		}

		protected override void SetMenuItemVisibilityInternal(MenuItem item, bool visible)
		{
			(GetHandleForMenuItem(item) as WindowsFormsNativeMenuItem).Handle.Visible = visible;
		}

		protected override bool ShowHelpInternal(HelpTopic topic)
		{
			return false;
		}

		private Dictionary<Timer, TimerImplementation> _Timer_Implementations = new Dictionary<Timer, TimerImplementation>();
		protected override void Timer_StopInternal(Timer timer)
		{
			if (!_Timer_Implementations.ContainsKey(timer))
			{
				_Timer_Implementations[timer] = new WindowsFormsTimerImplementation(timer);
			}

			if (_Timer_Implementations[timer].Timer.Enabled)
				_Timer_Implementations[timer].Stop();
		}
		protected override void Timer_StartInternal(Timer timer)
		{
			if (!_Timer_Implementations.ContainsKey(timer))
			{
				_Timer_Implementations[timer] = new WindowsFormsTimerImplementation(timer);
			}

			if (_Timer_Implementations[timer].Timer.Enabled)
				_Timer_Implementations[timer].Stop();

			_Timer_Implementations[timer].Start();
		}

		private Screen _DefaultScreen = null;
		protected override Screen GetDefaultScreenInternal()
		{
			if (_DefaultScreen == null)
				_DefaultScreen = new WindowsFormsScreen();
			return _DefaultScreen;
		}

		private SystemSettings _SystemSettings = new WindowsFormsSystemSettings();
		public override SystemSettings SystemSettings => _SystemSettings;

		protected override void PresentWindowInternal(Window window, DateTime timestamp)
		{
			System.Windows.Forms.Form handle = (GetHandleForControl(window) as WindowsFormsNativeControl).Handle as System.Windows.Forms.Form;
			handle.Show();
			handle.BringToFront();
		}

		protected override void PlaySystemSoundInternal(SystemSound sound)
		{
			switch (sound)
			{
				case SystemSound.Asterisk: System.Media.SystemSounds.Asterisk.Play(); break;
				case SystemSound.Beep: System.Media.SystemSounds.Beep.Play(); break;
				case SystemSound.Exclamation: System.Media.SystemSounds.Exclamation.Play(); break;
				case SystemSound.Hand: System.Media.SystemSounds.Hand.Play(); break;
				case SystemSound.Question: System.Media.SystemSounds.Question.Play(); break;
			}
		}
	}
}
