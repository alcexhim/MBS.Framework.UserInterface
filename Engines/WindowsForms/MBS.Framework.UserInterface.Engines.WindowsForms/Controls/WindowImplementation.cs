using System;
using System.Drawing;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;
using MBS.Framework.UserInterface.Native;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls
{
	[ControlImplementation(typeof(Window))]
	public class WindowImplementation : ContainerImplementation, IWindowNativeImplementation
	{
		public WindowImplementation (Engine engine, Window control) : base(engine, control)
		{
		}

		private string _DocumentFileName = null;
		public string GetDocumentFileName()
		{
			return _DocumentFileName;
		}
		public void SetDocumentFileName(string value)
		{
			_DocumentFileName = value;
		}

		protected override void DestroyInternal()
		{
			((Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.Form).Close();
		}

		public bool GetStatusBarVisible()
		{
			if (sb != null)
				return sb.Visible;
			return true;
		}
		public void SetStatusBarVisible(bool value)
		{
			if (sb != null)
				sb.Visible = value;
		}

		public System.Windows.Forms.FormStartPosition WindowStartPositionToFormStartPosition(WindowStartPosition value)
		{
			switch (value)
			{
				case WindowStartPosition.CenterParent: return System.Windows.Forms.FormStartPosition.CenterParent;
				case WindowStartPosition.Center: return System.Windows.Forms.FormStartPosition.CenterScreen;
				case WindowStartPosition.Manual: return System.Windows.Forms.FormStartPosition.Manual;
				case WindowStartPosition.DefaultBounds: return System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
				case WindowStartPosition.Default: return System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
			}
			throw new NotSupportedException();
		}

		private System.Windows.Forms.StatusStrip sb = null;
		private System.Windows.Forms.MenuStrip mb = null;

		protected override NativeControl CreateControlInternal (Control control)
		{
			Window window = (control as Window);

			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();

			if (window.Decorated)
			{
				if (true) // window.Resizable)
				{
					form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
				}
				else
				{
					form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
				}
			}
			else
			{
				form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			}
			form.Location = new Point((int)window.Location.X, (int)window.Location.Y);
			form.Size = new System.Drawing.Size((int)window.Size.Width, (int)window.Size.Height);
			form.StartPosition = WindowStartPositionToFormStartPosition(window.StartPosition);
			form.FormClosing += Form_FormClosing;
			form.FormClosed += Form_FormClosed;
			form.Shown += form_Shown;

			Internal.CommandBars.ToolBarManager tbm = new Internal.CommandBars.ToolBarManager(form, form);

			// System.Windows.Forms.ToolStripContainer tsc = new System.Windows.Forms.ToolStripContainer();
			// tsc.Dock = System.Windows.Forms.DockStyle.Fill;

			mb = new Internal.CommandBars.HiDpi.MenuStrip();
			// mb.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
			mb.Stretch = true;

			// tsc.TopToolStripPanel.Controls.Add(mb);

			foreach (MenuItem m in window.MenuBar.Items)
			{
				System.Windows.Forms.ToolStripItem tsmi = (((UIApplication)Application.Instance).Engine as WindowsFormsEngine).InitMenuItem(m);
				if (tsmi != null)
					mb.Items.Add(tsmi);
			}

			if (mb.Items.Count > 0 && window.MenuBar.Visible && (window.CommandDisplayMode == CommandDisplayMode.CommandBar || window.CommandDisplayMode == CommandDisplayMode.Both))
				tbm.AddControl(mb);

			if (window.CommandDisplayMode == CommandDisplayMode.CommandBar || window.CommandDisplayMode == CommandDisplayMode.Both)
			{
				foreach (CommandBar cb in ((UIApplication)Application.Instance).CommandBars)
				{
					Toolbar tbCommandBar = window.LoadCommandBar(cb);
					if (!tbCommandBar.IsCreated)
					{
						Engine.CreateControl(tbCommandBar);
					}
					System.Windows.Forms.ToolStrip ts = ((tbCommandBar.ControlImplementation.Handle as WindowsFormsNativeControl).Handle as System.Windows.Forms.ToolStrip);
					ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
					ts.Text = cb.Title;

					// tsc.TopToolStripPanel.Controls.Add(ts);
					tbm.AddControl(ts);
				}
			}

			mb.Text = "Menu Bar";
			mb.Visible = window.MenuBar.Visible && (mb.Items.Count > 0);

			sb = new System.Windows.Forms.StatusStrip();
			sb.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
			// tsc.BottomToolStripPanel.Controls.Add(sb);

			sb.Text = "Status Bar";
			sb.Visible = window.StatusBar.Visible;

			Container container = new Container();
			container.Layout = window.Layout;
			for (int i = 0; i < window.Controls.Count; i++)
			{
				container.Controls.Add(window.Controls[i]);
			}
			Engine.CreateControl(container);

			WindowsFormsNativeControl ncContainer = (Engine.GetHandleForControl(container) as WindowsFormsNativeControl);
			// WindowsFormsNativeControl ncContainer = (base.CreateControlInternal(control) as WindowsFormsNativeControl);

			ncContainer.Handle.Dock = System.Windows.Forms.DockStyle.Fill;

			// tsc.TopToolStripPanel.Text = "MSOCommandBarTop";
			// tsc.LeftToolStripPanel.Text = "MSOCommandBarLeft";
			// tsc.BottomToolStripPanel.Text = "MSOCommandBarBottom";
			// tsc.RightToolStripPanel.Text = "MSOCommandBarRight";
			// tsc.ContentPanel.Controls.Add(ncContainer.Handle);
			form.Controls.Add(ncContainer.Handle);
			ncContainer.Handle.BringToFront();

			// tsc.Dock = System.Windows.Forms.DockStyle.Fill;
			// form.Controls.Add(tsc);
			form.Controls.Add(sb);
			form.Text = window.Text;
			form.MinimumSize = WindowsFormsEngine.Dimension2DToSystemDrawingSize(window.MinimumSize);
			form.MaximumSize = WindowsFormsEngine.Dimension2DToSystemDrawingSize(window.MaximumSize);
			form.Size = WindowsFormsEngine.Dimension2DToSystemDrawingSize(window.Size);
			if (window.Size == Dimension2D.Empty)
				form.AutoSize = true;

			form.Tag = window;
			form.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
			return new WindowsFormsNativeControl (form);
		}

		void Form_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			InvokeMethod((((System.Windows.Forms.Form)sender).Tag as Window), "OnClosed", e);
		}

		void Form_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			WindowClosingEventArgs ce = new WindowClosingEventArgs(CloseReasonToWindowCloseReason(e.CloseReason));
			InvokeMethod((((System.Windows.Forms.Form)sender).Tag as Window), "OnClosing", ce);
			e.Cancel = ce.Cancel;
		}

		private static WindowCloseReason CloseReasonToWindowCloseReason(System.Windows.Forms.CloseReason closeReason)
		{
			switch (closeReason)
			{
				case System.Windows.Forms.CloseReason.ApplicationExitCall: return WindowCloseReason.ApplicationStop;
				case System.Windows.Forms.CloseReason.FormOwnerClosing: return WindowCloseReason.OwnerClosing;
				case System.Windows.Forms.CloseReason.MdiFormClosing: return WindowCloseReason.MdiParentClosing;
				case System.Windows.Forms.CloseReason.None: return WindowCloseReason.None;
				case System.Windows.Forms.CloseReason.TaskManagerClosing: return WindowCloseReason.ForcedClosing;
				case System.Windows.Forms.CloseReason.UserClosing: return WindowCloseReason.UserClosing;
				case System.Windows.Forms.CloseReason.WindowsShutDown: return WindowCloseReason.SystemShutdown;
			}
			return WindowCloseReason.Unknown;
		}

		private void form_Shown(object sender, EventArgs e)
		{
			OnShown(e);
			OnMapped(e);
		}


		protected override void RegisterDragSourceInternal (Control control, DragDropTarget [] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKey)
		{
			Console.Error.WriteLine ("uwt: wf: error: registration of drag source / drop target not implemented yet");
		}

		protected override void RegisterDropTargetInternal (Control control, DragDropTarget [] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			Console.Error.WriteLine ("uwt: wf: error: registration of drag source / drop target not implemented yet");
		}

		protected override void SetControlVisibilityInternal (bool visible)
		{
			if (visible)
			{
				(Handle as WindowsFormsNativeControl).Handle.Show();
			}
			else
			{
				// this doesn't work on linux, but it is how we're supposed to do on WinForms
				if ((Handle as WindowsFormsNativeControl).Handle.InvokeRequired)
				{

					(Handle as WindowsFormsNativeControl).Handle.Invoke(new Action<System.Windows.Forms.Form>(delegate (System.Windows.Forms.Form parm)
					{
						parm.Hide();
					}), new object[] { (Handle as WindowsFormsNativeControl).Handle });
				}
				else
				{
					(Handle as WindowsFormsNativeControl).Handle.Hide();
				}
			}
		}

		protected override void SetFocusInternal ()
		{
			(Handle as WindowsFormsNativeControl).Handle.Focus ();
		}

		protected override Dimension2D GetControlSizeInternal()
		{
			return WindowsFormsEngine.SystemDrawingSizeToDimension2D((Handle as WindowsFormsNativeControl).Handle.Size);
		}

		protected override string GetTooltipTextInternal()
		{
			throw new NotSupportedException();
		}
		protected override void SetTooltipTextInternal(string value)
		{
			throw new NotSupportedException();
		}

		protected override void SetCursorInternal(Cursor value)
		{
			throw new NotImplementedException();
		}
		protected override Cursor GetCursorInternal()
		{
			throw new NotImplementedException();
		}

		public Window[] GetToplevelWindows()
		{
			throw new NotImplementedException();
		}

		public void SetIconName(string value)
		{
			throw new NotImplementedException();
		}

		public string GetIconName()
		{
			throw new NotImplementedException();
		}

		public void InsertMenuItem(int index, MenuItem item)
		{
			System.Windows.Forms.ToolStripItem tsi = (Engine as WindowsFormsEngine).InitMenuItem(item);
			mb.Items.Insert(index, tsi);
		}

		public void ClearMenuItems()
		{
			mb.Items.Clear();
		}

		public void RemoveMenuItem(MenuItem item)
		{
			System.Windows.Forms.ToolStripItem tsi = ((Engine as WindowsFormsEngine).GetHandleForMenuItem(item) as WindowsFormsNativeMenuItem).Handle;
			mb.Items.Remove(tsi);
		}

		public bool IsFullScreen()
		{
			return false;
		}
		public void SetFullScreen(bool value)
		{

		}
	}
}
