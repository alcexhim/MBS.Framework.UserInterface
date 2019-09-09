using System;
using MBS.Framework.Drawing;
using UniversalWidgetToolkit.DragDrop;
using UniversalWidgetToolkit.Input.Keyboard;
using UniversalWidgetToolkit.Input.Mouse;

namespace UniversalWidgetToolkit.Engines.WindowsForms.Controls
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
	}
}
