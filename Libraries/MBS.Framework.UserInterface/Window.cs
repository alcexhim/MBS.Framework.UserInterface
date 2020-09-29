using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MBS.Framework.UserInterface.Drawing;
using MBS.Framework.UserInterface.Controls.Ribbon;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Controls;

namespace MBS.Framework.UserInterface
{
	namespace Native
	{
		public interface IWindowNativeImplementation
		{
			Window[] GetToplevelWindows();

			void SetIconName(string value);
			string GetIconName();

			bool GetStatusBarVisible();
			void SetStatusBarVisible(bool value);

			void InsertMenuItem(int index, MenuItem item);
			void ClearMenuItems();
			void RemoveMenuItem(MenuItem item);

			string GetDocumentFileName();
			void SetDocumentFileName(string value);
		}
	}
	public class Window : Container
	{
		private RibbonControl mvarRibbon = new RibbonControl ();
		public RibbonControl Ribbon { get { return mvarRibbon; } }

		public bool Modal { get; set; } = false;
	
		private void tsbCommand_Click(object sender, EventArgs e)
		{
			ToolbarItemButton tsb = (sender as ToolbarItemButton);
			CommandReferenceCommandItem crci = tsb.GetExtraData<CommandReferenceCommandItem>("crci");
			Command cmd = Application.Commands[crci.CommandID];
			cmd.Execute();
		}

		public ToolbarItem[] LoadCommandBarItem(CommandItem ci)
		{
			System.Diagnostics.Contracts.Contract.Assert(ci != null);

			if (ci is SeparatorCommandItem)
			{
				return new ToolbarItem[] { new ToolbarItemSeparator() };
			}
			else if (ci is CommandReferenceCommandItem)
			{
				CommandReferenceCommandItem crci = (ci as CommandReferenceCommandItem);
				Command cmd = Application.Commands[crci.CommandID];
				if (cmd == null) return null;

				ToolbarItemButton tsb = new ToolbarItemButton(cmd.ID, (StockType)cmd.StockType);
				tsb.SetExtraData<CommandReferenceCommandItem>("crci", crci);
				tsb.Click += tsbCommand_Click;
				tsb.Title = cmd.Title;
				return new ToolbarItem[] { tsb };
			}
			else if (ci is GroupCommandItem)
			{
				GroupCommandItem gci = (ci as GroupCommandItem);
				List<ToolbarItem> list = new List<ToolbarItem>();
				for (int i = 0; i < gci.Items.Count; i++)
				{
					ToolbarItem[] items = LoadCommandBarItem(gci.Items[i]);
					list.AddRange(items);
				}
				return list.ToArray();
			}
			throw new NotImplementedException(String.Format("type of CommandItem '{0}' not implemented", ci.GetType()));
		}

		public Toolbar LoadCommandBar(CommandBar cb)
		{
			Toolbar tb = new Toolbar();
			for (int i = 0; i < cb.Items.Count; i++)
			{
				ToolbarItem[] items = LoadCommandBarItem(cb.Items[i]);
				if (items != null && items.Length > 0)
				{
					for (int j = 0; j < items.Length; j++)
						tb.Items.Add(items[j]);
				}
			}
			return tb;
		}

		internal protected override void OnCreating (EventArgs e)
		{
			switch (CommandDisplayMode) {
				case CommandDisplayMode.Ribbon:
				case CommandDisplayMode.Both:
				{
					this.Controls.Insert(0, mvarRibbon);
					break;
				}
			}

			base.OnCreating (e);
		}

		public Menu MenuBar { get; private set; } = null;
		public StatusBar StatusBar { get; private set; } = null;

		public Control ActiveControl
		{
			get
			{
				Control[] ctls = this.GetAllControls();
				foreach (Control ctl in ctls)
				{
					if (ctl.Focused)
						return ctl;
				}
				return null;
			}
		}

		public Button.ButtonCollection TitleBarButtons { get; private set; } = null;

		public CommandDisplayMode CommandDisplayMode { get; set; } = CommandDisplayMode.CommandBar;

		public WindowStartPosition StartPosition { get; set; } = WindowStartPosition.Default;

		public Window()
		{
			StatusBar = new StatusBar(this);
			MenuBar = new Menu(this);
			TitleBarButtons = new Button.ButtonCollection(this);

			Application.AddWindow(this);
		}

		private string mvarIconName = null;
		public string IconName
		{
			get
			{
				Native.IWindowNativeImplementation native = (ControlImplementation as Native.IWindowNativeImplementation);
				if (native != null)
				{
					return native.GetIconName();
				}
				return mvarIconName;
			}
			set
			{
				Native.IWindowNativeImplementation native = (ControlImplementation as Native.IWindowNativeImplementation);
				if (native != null)
				{
					native.SetIconName(value);
				}
				mvarIconName = value;
			}
		}
		
		/// <summary>
		/// Determines if this <see cref="Window" /> should be decorated (i.e., have a title bar and border) by the window manager.
		/// </summary>
		/// <value><c>true</c> if decorated; otherwise, <c>false</c>.</value>
		public bool Decorated { get; set; } = true;

		public bool Resizable { get; set; } = true;

		public Rectangle Bounds { get; set; } = Rectangle.Empty;

		public bool HasFocus => Application.Engine.WindowHasFocus(this);
		
		public event EventHandler Activate;
		protected virtual void OnActivate(EventArgs e)
		{
			if (Activate != null) Activate (this, e);
		}

		public event EventHandler Deactivate;
		protected virtual void OnDeactivate(EventArgs e)
		{
			if (Deactivate != null) Deactivate (this, e);
		}

		public event WindowClosingEventHandler Closing;
		protected virtual void OnClosing(WindowClosingEventArgs e)
		{
			if (Closing != null) Closing(this, e);
		}

		public event EventHandler Closed;
		protected virtual void OnClosed(EventArgs e)
		{
			if (Closed != null) Closed(this, e);
		}

		public static Window[] GetToplevelWindows()
		{
			return Application.Engine.GetToplevelWindows(); 
		}

		private string _DocumentFileName = null;
		public string DocumentFileName
		{
			get
			{
				if (IsCreated)
				{
					Native.IWindowNativeImplementation impl = (ControlImplementation as Native.IWindowNativeImplementation);
					if (impl != null) return impl.GetDocumentFileName();
				}
				return _DocumentFileName;
			}
			set
			{
				(ControlImplementation as Native.IWindowNativeImplementation)?.SetDocumentFileName(value); _DocumentFileName = value;
			}
		}

		public override string ToString()
		{
			return Text;
		}

		public bool Close()
		{
			// more than just a convenience method - Destroy() does not fire WindowClosing event
			WindowClosingEventArgs ee = new WindowClosingEventArgs(Application.Stopping ? WindowCloseReason.ApplicationStop : WindowCloseReason.None);
			OnClosing(ee);
			if (ee.Cancel)
				return false;

			this.Destroy();
			return true;
		}

		/// <summary>
		/// Presents a window to the user. This may mean raising the window in the stacking order, deiconifying it, moving it to the current desktop, and/or giving it the
		/// keyboard focus, possibly dependent on the user’s platform, window manager, and preferences.
		/// </summary>
		public void Present(DateTime timestamp)
		{
			Application.Engine.PresentWindow(this, timestamp);
		}
	}
}
