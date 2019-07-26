using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Controls.Ribbon;

namespace UniversalWidgetToolkit
{
	namespace Native
	{
		public interface IWindowNativeImplementation
		{
			Window[] GetToplevelWindows();

			void SetIconName(string value);
			string GetIconName();
		}
	}
	public class Window : Container
	{
		private RibbonControl mvarRibbon = new RibbonControl ();
		public RibbonControl Ribbon { get { return mvarRibbon; } }

		internal protected override void OnCreating (EventArgs e)
		{
			switch (CommandDisplayMode) {
				case CommandDisplayMode.Ribbon:
				case CommandDisplayMode.Both:
				{
					this.Controls.Add (mvarRibbon);
					break;
				}
			}

			base.OnCreating (e);
		}

		private Menu mvarMenuBar = new Menu();
		public Menu MenuBar { get { return mvarMenuBar; } }

		public CommandDisplayMode CommandDisplayMode { get; set; } = CommandDisplayMode.CommandBar;

		public WindowStartPosition StartPosition { get; set; } = WindowStartPosition.Default;

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

		private Rectangle mvarBounds = Rectangle.Empty;
		public Rectangle Bounds { get { return mvarBounds; } set { mvarBounds = value; } }

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

		public event CancelEventHandler Closing;
		protected virtual void OnClosing(CancelEventArgs e)
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

		public override string ToString()
		{
			return Text;
		}

		public void Close()
		{
			// convenience method
			this.Destroy();
		}
	}
}
