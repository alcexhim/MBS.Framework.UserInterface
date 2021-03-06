using System;
using System.Collections.Generic;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(Container))]
	public class ContainerImplementation : GTKNativeImplementation, IControlContainerImplementation
	{
		public ContainerImplementation(Engine engine, Container control)
			: base(engine, control)
		{
			_hListBox_row_activated_d = new Action<IntPtr, IntPtr>(_hListBox_row_activated);
		}

		private Action<IntPtr, IntPtr> _hListBox_row_activated_d;
		private void _hListBox_row_activated(IntPtr listbox, IntPtr row)
		{
			Control ctlChild = (Engine as GTKEngine).GetControlByHandle(row);
			Console.WriteLine("activating row {0} for child {1}", row, (ctlChild as Container).Controls[0].Text);

			// FIXME: for some reason the SettingsDialog gets confused, perhaps by GetControlByHandle function
			// FIXME: also there is some really weird voodoo going on, second time SettingsDialog EVERYTHING is screwed up
			// ............ resulting in crash if we try to derefeerence ^^^ Text
			// InvokeMethod(ctlChild, "OnClick", new object[] { EventArgs.Empty });
		}

		private Dictionary<Layout, IntPtr> handlesByLayout = new Dictionary<Layout, IntPtr>();

		private IntPtr mvarContainerHandle = IntPtr.Zero;

		internal void ApplyLayout(IntPtr hContainer, Control ctl, Layout layout)
		{
			GTKNativeControl hnc = (Engine.GetHandleForControl(ctl) as GTKNativeControl);
			IntPtr ctlHandle = hnc.Handle;

			Constraints cstr = layout.GetControlConstraints(ctl);
			if (cstr != null)
			{
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_hexpand(ctlHandle, cstr.HorizontalExpand);
				Internal.GTK.Methods.GtkWidget.gtk_widget_set_vexpand(ctlHandle, cstr.VerticalExpand);
			}

			if (layout is BoxLayout)
			{
				BoxLayout box = (layout as BoxLayout);
				Internal.GTK.Methods.GtkBox.gtk_box_set_spacing(hContainer, box.Spacing);
				Internal.GTK.Methods.GtkBox.gtk_box_set_homogeneous(hContainer, box.Homogeneous);

				BoxLayout.Constraints c = (box.GetControlConstraints(ctl) as BoxLayout.Constraints);
				if (c == null) c = new BoxLayout.Constraints();

				int padding = c.Padding == 0 ? ctl.Padding.All : c.Padding;

				switch (c.PackType)
				{
					case BoxLayout.PackType.Start:
					{
						Internal.GTK.Methods.GtkBox.gtk_box_pack_start(hContainer, ctlHandle, c.Expand, c.Fill, padding);
						break;
					}
					case BoxLayout.PackType.End:
					{
						Internal.GTK.Methods.GtkBox.gtk_box_pack_end(hContainer, ctlHandle, c.Expand, c.Fill, padding);
						break;
					}
				}
			}
			else if (layout is Layouts.AbsoluteLayout)
			{
				Layouts.AbsoluteLayout.Constraints constraints = (layout.GetControlConstraints(ctl) as Layouts.AbsoluteLayout.Constraints);
				if (constraints == null) constraints = new Layouts.AbsoluteLayout.Constraints(0, 0, 0, 0);
				Internal.GTK.Methods.GtkFixed.gtk_fixed_put(hContainer, ctlHandle, constraints.X, constraints.Y);
			}
			else if (layout is Layouts.GridLayout)
			{
				Layouts.GridLayout.Constraints constraints = (layout.GetControlConstraints(ctl) as Layouts.GridLayout.Constraints);
				if (constraints != null)
				{
					// GtkTable has been deprecated. Use GtkGrid instead. It provides the same capabilities as GtkTable for arranging widgets in a rectangular grid, but does support height-for-width geometry management.
					if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
					{
						Internal.GTK.Methods.GtkTable.gtk_table_attach(hContainer, ctlHandle, (uint)constraints.Column, (uint)(constraints.Column + constraints.ColumnSpan), (uint)constraints.Row, (uint)(constraints.Row + constraints.RowSpan), Internal.GTK.Constants.GtkAttachOptions.Expand, Internal.GTK.Constants.GtkAttachOptions.Fill, 0, 0);
					}
					else
					{
						Internal.GTK.Methods.GtkGrid.gtk_grid_attach(hContainer, ctlHandle, constraints.Column, constraints.Row, constraints.ColumnSpan, constraints.RowSpan);
						// Internal.GTK.Methods.Methods.gtk_table_attach(hContainer, ctlHandle, (uint)constraints.Column, (uint)(constraints.Column + constraints.ColumnSpan), (uint)constraints.Row, (uint)(constraints.Row + constraints.RowSpan), Internal.GTK.Constants.GtkAttachOptions.Expand, Internal.GTK.Constants.GtkAttachOptions.Fill, 0, 0);

						if ((constraints.Expand & ExpandMode.Horizontal) == ExpandMode.Horizontal)
						{
							Internal.GTK.Methods.GtkWidget.gtk_widget_set_hexpand(ctlHandle, true);
						}
						else
						{
							Internal.GTK.Methods.GtkWidget.gtk_widget_set_hexpand(ctlHandle, false);
						}
						if ((constraints.Expand & ExpandMode.Vertical) == ExpandMode.Vertical)
						{
							Internal.GTK.Methods.GtkWidget.gtk_widget_set_vexpand(ctlHandle, true);
						}
						else
						{
							Internal.GTK.Methods.GtkWidget.gtk_widget_set_vexpand(ctlHandle, false);
						}
					}
				}
			}
			else if (layout is ListLayout)
			{
				IntPtr hListBoxRow = Internal.GTK.Methods.GtkListBox.gtk_list_box_row_new();
				hnc.SetNamedHandle("ListBoxRow", hListBoxRow);
				Internal.GTK.Methods.GtkContainer.gtk_container_add(hListBoxRow, ctlHandle);
				Internal.GTK.Methods.GtkContainer.gtk_container_add(hContainer, hListBoxRow);
			}
			else
			{
				Internal.GTK.Methods.GtkContainer.gtk_container_add(hContainer, ctlHandle);
			}
		}


		protected override NativeControl CreateControlInternal(Control control)
		{
			IntPtr hContainer = IntPtr.Zero;
			Container container = (control as Container);

			Layout layout = container.Layout;
			if (container.Layout == null) layout = new Layouts.AbsoluteLayout();

			if (layout is Layouts.BoxLayout)
			{
				Layouts.BoxLayout box = (layout as Layouts.BoxLayout);
				Internal.GTK.Constants.GtkOrientation orientation = Internal.GTK.Constants.GtkOrientation.Vertical;
				switch (box.Orientation)
				{
					case Orientation.Horizontal:
					{
						orientation = Internal.GTK.Constants.GtkOrientation.Horizontal;
						break;
					}
					case Orientation.Vertical:
					{
						orientation = Internal.GTK.Constants.GtkOrientation.Vertical;
						break;
					}
				}
				hContainer = Internal.GTK.Methods.GtkBox.gtk_box_new(orientation, ((Layouts.BoxLayout)layout).Homogeneous, ((Layouts.BoxLayout)layout).Spacing);
			}
			else if (layout is Layouts.AbsoluteLayout)
			{
				Layouts.AbsoluteLayout abs = (layout as Layouts.AbsoluteLayout);
				hContainer = Internal.GTK.Methods.GtkFixed.gtk_fixed_new();
			}
			else if (layout is Layouts.GridLayout)
			{
				if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
				{
					Layouts.GridLayout grid = (layout as Layouts.GridLayout);
					// GtkTable has been deprecated. Use GtkGrid instead. It provides the same capabilities as GtkTable for arranging widgets in a rectangular grid, but does support height-for-width geometry management.
					hContainer = Internal.GTK.Methods.GtkTable.gtk_table_new();
					// hContainer = Internal.GTK.Methods.Methods.gtk_table_new();
					Internal.GTK.Methods.GtkTable.gtk_table_set_row_spacings(hContainer, (uint)grid.RowSpacing);
					Internal.GTK.Methods.GtkTable.gtk_table_set_col_spacings(hContainer, (uint)grid.ColumnSpacing);
				}
				else
				{
					Layouts.GridLayout grid = (layout as Layouts.GridLayout);
					// GtkTable has been deprecated. Use GtkGrid instead. It provides the same capabilities as GtkTable for arranging widgets in a rectangular grid, but does support height-for-width geometry management.
					hContainer = Internal.GTK.Methods.GtkGrid.gtk_grid_new();
					// hContainer = Internal.GTK.Methods.Methods.gtk_table_new();
					Internal.GTK.Methods.GtkGrid.gtk_grid_set_row_spacing(hContainer, (uint)grid.RowSpacing);
					Internal.GTK.Methods.GtkGrid.gtk_grid_set_column_spacing(hContainer, (uint)grid.ColumnSpacing);
					Internal.GTK.Methods.GtkGrid.gtk_grid_set_row_homogeneous(hContainer, grid.RowHomogeneous);
					Internal.GTK.Methods.GtkGrid.gtk_grid_set_column_homogeneous(hContainer, grid.ColumnHomogeneous);
				}
			}
			else if (layout is Layouts.FlowLayout)
			{
				hContainer = Internal.GTK.Methods.GtkFlowBox.gtk_flow_box_new();
				Internal.GTK.Methods.GtkFlowBox.gtk_flow_box_set_selection_mode(hContainer, Internal.GTK.Constants.GtkSelectionMode.None);
			}
			else if (layout is Layouts.ListLayout)
			{
				hContainer = Internal.GTK.Methods.GtkListBox.gtk_list_box_new();
				Internal.GObject.Methods.g_signal_connect(hContainer, "row_activated", _hListBox_row_activated_d);

				Internal.GTK.Methods.GtkListBox.gtk_list_box_set_selection_mode(hContainer, GTKEngine.SelectionModeToGtkSelectionMode((layout as ListLayout).SelectionMode));

				if (control.BorderStyle == ControlBorderStyle.Fixed3D || control.BorderStyle == ControlBorderStyle.FixedSingle)
				{
					Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class(Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(hContainer), "frame");
				}
			}

			if (hContainer != IntPtr.Zero)
			{
				mvarContainerHandle = hContainer;
				handlesByLayout[layout] = hContainer;

				foreach (Control ctl in container.Controls)
				{
					bool ret = ctl.IsCreated;
					if (!ret) ret = Engine.CreateControl(ctl);
					if (!ret) continue;

					ApplyLayout(hContainer, ctl, layout);
				}
			}

			IntPtr hContainerWrapper = Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_new(IntPtr.Zero, IntPtr.Zero);
			Internal.GTK.Methods.GtkWidget.gtk_widget_show(hContainer);
			Internal.GTK.Methods.GtkContainer.gtk_container_add(hContainerWrapper, hContainer);

			Internal.GTK.Constants.GtkPolicyType policyH = Internal.GTK.Constants.GtkPolicyType.Never, policyV = Internal.GTK.Constants.GtkPolicyType.Never;
			switch (container.HorizontalAdjustment.ScrollType)
			{
				case AdjustmentScrollType.Always: policyH = Internal.GTK.Constants.GtkPolicyType.Always; break;
				case AdjustmentScrollType.Automatic: policyH = Internal.GTK.Constants.GtkPolicyType.Automatic; break;
				case AdjustmentScrollType.External: policyH = Internal.GTK.Constants.GtkPolicyType.External; break;
				case AdjustmentScrollType.Never: policyH = Internal.GTK.Constants.GtkPolicyType.Never; break;
			}
			switch (container.VerticalAdjustment.ScrollType)
			{
				case AdjustmentScrollType.Always: policyV = Internal.GTK.Constants.GtkPolicyType.Always; break;
				case AdjustmentScrollType.Automatic: policyV = Internal.GTK.Constants.GtkPolicyType.Automatic; break;
				case AdjustmentScrollType.External: policyV = Internal.GTK.Constants.GtkPolicyType.External; break;
				case AdjustmentScrollType.Never: policyV = Internal.GTK.Constants.GtkPolicyType.Never; break;
			}
			Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_set_policy(hContainerWrapper, policyH, policyV);

			return new GTKNativeControl(hContainerWrapper, new KeyValuePair<string, IntPtr>[]
			{
				new KeyValuePair<string, IntPtr>("Container", hContainer),
				new KeyValuePair<string, IntPtr>("ScrolledWindow", hContainerWrapper)
			});
		}

		public void InsertChildControl(Control child)
		{
			ApplyLayout((Handle as GTKNativeControl).GetNamedHandle("Container"), child, (Control as Container).Layout);
		}
		public void ClearChildControls()
		{
			Control[] ctls = (Control as IControlContainer).GetAllControls();

			IntPtr hContainer = (Handle as GTKNativeControl).GetNamedHandle("Container");
			List<IntPtr> _list = new List<IntPtr>();
			Internal.GTK.Methods.GtkContainer.gtk_container_forall(hContainer, delegate (IntPtr /*GtkWidget*/ widget, IntPtr data)
			{
				_list.Add(widget);
				Internal.GTK.Methods.GtkContainer.gtk_container_remove(hContainer, widget);
			}, IntPtr.Zero);

			for (int i = 0; i < ctls.Length; i++)
			{
				Engine.UnregisterControlHandle(ctls[i]);
			}
		}
		public void SetControlConstraints(Control control, Constraints constraints)
		{
			Console.WriteLine("Changing control constraints in a Container not supported YET");
		}
	}
}
