using System;
using System.Diagnostics.Contracts;
using UniversalWidgetToolkit.Engines.GTK.Drawing;

namespace UniversalWidgetToolkit.Engines.GTK.Controls
{
	[NativeImplementation(typeof(CustomControl))]
	public class CustomControlImplementation : GTKNativeImplementation
	{
		public CustomControlImplementation(Engine engine, Control control) : base(engine, control)
		{
			DrawHandler_Handler = new Internal.GObject.Delegates.DrawFunc(DrawHandler);
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			IntPtr handle = Internal.GTK.Methods.gtk_drawing_area_new();

			Internal.GObject.Methods.g_signal_connect(handle, "draw", DrawHandler_Handler);

			return new GTKNativeControl(handle);
		}

		private Internal.GObject.Delegates.DrawFunc DrawHandler_Handler;
		/// <summary>
		/// Handler for draw signal
		/// </summary>
		/// <param name="widget">the object which received the signal</param>
		/// <param name="cr">the cairo context to draw to</param>
		/// <param name="user_data">user data set when the signal handler was connected.</param>
		/// <returns>TRUE to stop other handlers from being invoked for the event. FALSE to propagate the event further.</returns>
		private bool DrawHandler(IntPtr /*GtkWidget*/ widget, IntPtr /*CairoContext*/ cr, IntPtr user_data)
		{
			CustomControl ctl = Engine.GetControlByHandle(widget) as CustomControl;
			Contract.Assert(ctl != null);

			GTKGraphics graphics = new GTKGraphics(cr);

			PaintEventArgs e = new PaintEventArgs(graphics);
			InvokeMethod(ctl, "OnPaint", e);
			if (e.Handled) return true;

			return false;
		}
	}
}
