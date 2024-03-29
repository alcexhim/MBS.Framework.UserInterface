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

			bool IsFullScreen();
			void SetFullScreen(bool value);

			void InsertCommandBar(int index, CommandBar toolbar);
			void ClearCommandBars();
			void RemoveCommandBar(CommandBar toolbar);

			void PresentWindow(DateTime timestamp);
			void BeginMoveDrag(Vector2D origin);
		}
	}
	public class Window : Container
	{
		private RibbonControl mvarRibbon = new RibbonControl();
		public RibbonControl Ribbon { get { return mvarRibbon; } }

		public CommandBar.CommandBarCollection CommandBars { get; private set; } = null;

		public bool Modal { get; set; } = false;

		public void BeginMove(Vector2D origin)
		{
			Native.IWindowNativeImplementation impl = (ControlImplementation as Native.IWindowNativeImplementation);
			if (impl != null)
			{
				impl.BeginMoveDrag(origin);
			}
		}

		internal protected override void OnCreating(EventArgs e)
		{
			switch (CommandDisplayMode)
			{
				case CommandDisplayMode.Ribbon:
				case CommandDisplayMode.Both:
				{
					this.Controls.Insert(0, mvarRibbon);
					break;
				}
			}

			base.OnCreating(e);
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
		public Control.ControlCollection TitleBarControls { get; private set; } = null;

		public CommandDisplayMode CommandDisplayMode { get; set; } = CommandDisplayMode.CommandBar;

		public WindowStartPosition StartPosition { get; set; } = WindowStartPosition.Default;

		public Window()
		{
			StatusBar = new StatusBar(this);
			MenuBar = new Menu(this);
			TitleBarButtons = new Button.ButtonCollection(this);
			TitleBarControls = new ControlCollection(this);
			CommandBars = new CommandBar.CommandBarCollection(this);

			((UIApplication)Application.Instance).AddWindow(this);
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

		public bool HasFocus => ((UIApplication)Application.Instance).Engine.WindowHasFocus(this);

		public event EventHandler Activate;
		protected virtual void OnActivate(EventArgs e)
		{
			if (Activate != null) Activate(this, e);
		}

		public event EventHandler Deactivate;
		protected virtual void OnDeactivate(EventArgs e)
		{
			if (Deactivate != null) Deactivate(this, e);
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
			return ((UIApplication)Application.Instance).Engine.GetToplevelWindows();
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
			WindowClosingEventArgs ee = new WindowClosingEventArgs(((UIApplication)Application.Instance).Stopping ? WindowCloseReason.ApplicationStop : WindowCloseReason.None);
			OnClosing(ee);
			if (ee.Cancel)
				return false;

			this.Destroy();
			return true;
		}

		public void Present()
		{
			Present(DateTime.Now);
		}
		/// <summary>
		/// Presents a window to the user. This may mean raising the window in the stacking order, deiconifying it, moving it to the current desktop, and/or giving it the
		/// keyboard focus, possibly dependent on the user’s platform, window manager, and preferences.
		/// </summary>
		public void Present(DateTime timestamp)
		{
			(ControlImplementation as Native.IWindowNativeImplementation)?.PresentWindow(timestamp);
		}

		private bool _FullScreen = false;
		public bool FullScreen
		{
			get
			{
				Native.IWindowNativeImplementation impl = (ControlImplementation as Native.IWindowNativeImplementation);
				if (impl == null)
					return _FullScreen;

				return impl.IsFullScreen();
			}
			set
			{
				_FullScreen = value;
				(ControlImplementation as Native.IWindowNativeImplementation)?.SetFullScreen(value);
			}
		}
	}
}
