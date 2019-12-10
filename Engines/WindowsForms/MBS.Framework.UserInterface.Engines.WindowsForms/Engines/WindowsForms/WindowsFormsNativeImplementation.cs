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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
	}
}
