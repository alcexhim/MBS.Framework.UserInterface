using System;
using System.Windows.Forms;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Engines.WindowsForms
{
	public abstract class WindowsFormsNativeImplementation : NativeImplementation
	{
		public WindowsFormsNativeImplementation (Engine engine, Control control) : base(engine, control)
		{
		}

		protected override bool HasFocusInternal()
		{
			return ((Handle as WindowsFormsNativeControl).Handle).Focused;
		}

		protected override Dimension2D GetControlSizeInternal()
		{
			return new Framework.Drawing.Dimension2D((Handle as WindowsFormsNativeControl).Handle.Size.Width, ((Handle as WindowsFormsNativeControl).Handle.Size.Height));
		}

		protected override Cursor GetCursorInternal()
		{
			throw new NotImplementedException();
		}

		protected override string GetTooltipTextInternal()
		{
			throw new NotImplementedException();
		}

		protected override void RegisterDragSourceInternal(Control control, DragDropTarget[] targets, DragDropEffect actions, Input.Mouse.MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			throw new NotImplementedException();
		}

		protected override void RegisterDropTargetInternal(Control control, DragDropTarget[] targets, DragDropEffect actions, Input.Mouse.MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			throw new NotImplementedException();
		}

		protected override void SetControlVisibilityInternal(bool visible)
		{
			if (Handle is Win32NativeControl)
			{
				Internal.Windows.Methods.ShowWindow((Handle as Win32NativeControl).Handle, visible ? Internal.Windows.Constants.ShowWindowCommand.Show : Internal.Windows.Constants.ShowWindowCommand.Hide);
				return;
			}
			else if (Handle is WindowsFormsNativeControl)
			{
				(Handle as WindowsFormsNativeControl).Handle.Visible = visible;
				return;
			}
			throw new NotSupportedException();
		}
		protected override bool IsControlVisibleInternal()
		{
			if (Handle is Win32NativeControl)
			{
				return Internal.Windows.Methods.IsWindowVisible((Handle as Win32NativeControl).Handle);
			}
			else if (Handle is WindowsFormsNativeControl)
			{
				return (Handle as WindowsFormsNativeControl).Handle.Visible;
			}
			throw new NotSupportedException();
		}

		protected override void SetCursorInternal(Cursor value)
		{
			throw new NotImplementedException();
		}

		protected override void SetFocusInternal()
		{
			throw new NotImplementedException();
		}

		protected override void SetTooltipTextInternal(string value)
		{
			throw new NotImplementedException();
		}

		protected override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			SetupCommonEvents();
		}

		private void SetupCommonEvents()
		{
			System.Windows.Forms.Control ctl = ((Handle as WindowsFormsNativeControl)?.Handle as System.Windows.Forms.Control);
			if (ctl == null)
				return;

			ctl.Click += ctl_Click;
			ctl.MouseDown += ctl_MouseDown;
			ctl.MouseMove += ctl_MouseMove;
			ctl.MouseUp += ctl_MouseUp;
			ctl.MouseEnter += ctl_MouseEnter;
			ctl.MouseLeave += ctl_MouseLeave;
			ctl.KeyUp += ctl_KeyUp;
			ctl.KeyDown += ctl_KeyDown;
		}

		void ctl_Click(object sender, EventArgs e)
		{
			InvokeMethod(this, "OnClick", new object[] { e });
		}
		void ctl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(e.X, e.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(e.Button), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseDown", new object[] { ee });
		}
		void ctl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(e.X, e.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(e.Button), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseMove", new object[] { ee });
		}
		void ctl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(e.X, e.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(e.Button), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseUp", new object[] { ee });
		}
		void ctl_MouseEnter(object sender, EventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(System.Windows.Forms.Control.MouseButtons), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseEnter", new object[] { ee });
		}
		void ctl_MouseLeave(object sender, EventArgs e)
		{
			Input.Mouse.MouseEventArgs ee = new Input.Mouse.MouseEventArgs(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y, WindowsFormsEngine.SWFMouseButtonsToMouseButtons(System.Windows.Forms.Control.MouseButtons), WindowsFormsEngine.SWFKeysToKeyboardModifierKey(System.Windows.Forms.Control.ModifierKeys));
			InvokeMethod(this, "OnMouseLeave", new object[] { ee });
		}
		void ctl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			Input.Keyboard.KeyEventArgs ee = WindowsFormsEngine.SWFKeyEventArgsToKeyEventArgs(e);
			InvokeMethod(this, "OnKeyDown", new object[] { ee });

			if (ee.Cancel)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}
		void ctl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			Input.Keyboard.KeyEventArgs ee = WindowsFormsEngine.SWFKeyEventArgsToKeyEventArgs(e);
			InvokeMethod(this, "OnKeyUp", new object[] { ee });

			if (ee.Cancel)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

	}
}
