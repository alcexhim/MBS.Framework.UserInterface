using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MBS.Framework.Drawing;
using MBS.Framework.UserInterface.Engines.GTK.Drawing;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(CustomControl))]
	public class CustomControlImplementation : GTKNativeImplementation
	{
		public CustomControlImplementation(Engine engine, Control control) : base(engine, control)
		{
			DrawHandler_Handler = new Internal.GObject.Delegates.DrawFunc(DrawHandler);
		}

		protected override NativeControl CreateControlInternal(Control control)
		{
			IntPtr handle = Internal.GTK.Methods.GtkLayout.gtk_layout_new(IntPtr.Zero, IntPtr.Zero);

			Internal.GObject.Methods.g_signal_connect(handle, "draw", DrawHandler_Handler);
			Internal.GTK.Methods.GtkWidget.gtk_widget_set_can_focus(handle, true);
			Internal.GTK.Methods.GtkWidget.gtk_widget_add_events(handle, Internal.GDK.Constants.GdkEventMask.ButtonPress | Internal.GDK.Constants.GdkEventMask.ButtonRelease | Internal.GDK.Constants.GdkEventMask.KeyPress | Internal.GDK.Constants.GdkEventMask.KeyRelease | Internal.GDK.Constants.GdkEventMask.PointerMotion | Internal.GDK.Constants.GdkEventMask.PointerMotionHint);

			IntPtr scrolledWindow = Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_new(IntPtr.Zero, IntPtr.Zero);

			// Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_set_policy(scrolledWindow, Internal.GTK.Constants.GtkPolicyType.Always, Internal.GTK.Constants.GtkPolicyType.Always);
			Internal.GTK.Methods.GtkContainer.gtk_container_add(scrolledWindow, handle);

			return new GTKNativeControl(scrolledWindow, new KeyValuePair<string, IntPtr>[]
			{
				new KeyValuePair<string, IntPtr>("Layout", handle)
			});
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
			_hadjDirty = true;
			_vadjDirty = true;

			// FIXME: UWT gets confused if we do this, probably because something's not quite right with the IntPtr comparison down the road
			// CustomControl ctl = (Engine as GTKEngine).GetControlByHandle(widget) as CustomControl;

			// doing it this way works though (probably because we don't compare any IntPtrs...)
			CustomControl ctl = Control as CustomControl;

			Contract.Assert(ctl != null);

			GTKGraphics graphics = new GTKGraphics(cr);

			IntPtr handle = (Handle as GTKNativeControl).Handle;
			IntPtr hLayout = (Handle as GTKNativeControl).GetNamedHandle("Layout");

			IntPtr adjH = Internal.GTK.Methods.GtkScrollable.gtk_scrollable_get_hadjustment(hLayout);
			IntPtr adjV = Internal.GTK.Methods.GtkScrollable.gtk_scrollable_get_vadjustment(hLayout);
			double offH = Internal.GTK.Methods.GtkAdjustment.gtk_adjustment_get_value(adjH);
			double offV = Internal.GTK.Methods.GtkAdjustment.gtk_adjustment_get_value(adjV);

			Internal.Cairo.Methods.cairo_translate(cr, -offH, -offV);

			PaintEventArgs e = new PaintEventArgs(graphics);
			InvokeMethod(ctl, "OnPaint", e);
			if (e.Handled) return true;

			return true;
		}

		private double _vadj, _hadj = 0;
		private bool _hadjDirty = true, _vadjDirty = true;

		protected override double GetAdjustmentValueInternal(Orientation orientation)
		{
			IntPtr hLayout = (Handle as GTKNativeControl).GetNamedHandle("Layout");
			switch (orientation)
			{
				case Orientation.Horizontal:
				{
					if (_hadjDirty)
					{
						IntPtr adj = Internal.GTK.Methods.GtkScrollable.gtk_scrollable_get_hadjustment(hLayout);
						_hadj = Internal.GTK.Methods.GtkAdjustment.gtk_adjustment_get_value(adj);
						_hadjDirty = false;
					}
					return _hadj;
				}
				case Orientation.Vertical:
				{
					if (_vadjDirty)
					{
						IntPtr adj = Internal.GTK.Methods.GtkScrollable.gtk_scrollable_get_vadjustment(hLayout);
						_vadj = Internal.GTK.Methods.GtkAdjustment.gtk_adjustment_get_value(adj);
						_vadjDirty = false;
					}
					return _vadj;
				}
			}
			throw new ArgumentOutOfRangeException(nameof(orientation));
		}

		protected override Dimension2D GetScrollBoundsInternal()
		{
			IntPtr hLayout = (Handle as GTKNativeControl).GetNamedHandle("Layout");
			uint w = 0, h = 0;
			Internal.GTK.Methods.GtkLayout.gtk_layout_get_size(hLayout, out w, out h);
			return new Dimension2D(w, h);
		}
		protected override void SetScrollBoundsInternal(Dimension2D value)
		{
			IntPtr hLayout = (Handle as GTKNativeControl).GetNamedHandle("Layout");
			Internal.GTK.Methods.GtkLayout.gtk_layout_set_size(hLayout, (uint)value.Width, (uint)value.Height);
		}
	}
}
