using System;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.DragDrop;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.UserInterface.Input.Mouse;

namespace MBS.Framework.UserInterface.Engines.WindowsForms.Controls
{
	[ControlImplementation(typeof(Window))]
	public class WindowImplementation : WindowsFormsNativeImplementation
	{
		public WindowImplementation (Engine engine, Window control) : base(engine, control)
		{
		}

		protected override NativeControl CreateControlInternal (Control control)
		{
			System.Windows.Forms.Form form = new System.Windows.Forms.Form ();
			return new WindowsFormsNativeControl (form);
		}

		protected override void RegisterDragSourceInternal (Control control, DragDropTarget [] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			Console.Error.WriteLine ("uwt: wf: error: registration of drag source / drop target not implemented yet");
		}

		protected override void RegisterDropTargetInternal (Control control, DragDropTarget [] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			Console.Error.WriteLine ("uwt: wf: error: registration of drag source / drop target not implemented yet");
		}

		protected override void SetControlVisibilityInternal (bool visible)
		{
			(Handle as WindowsFormsNativeControl).Handle.Visible = visible;
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
	}
}
