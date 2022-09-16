using System;
using System.Collections.Generic;
using MBS.Framework.UserInterface.Layouts;

namespace MBS.Framework.UserInterface.Engines.GTK3.Controls
{
	[ControlImplementation(typeof(Container))]
	public class ContainerImplementation : GTKNativeImplementation, IControlContainerImplementation
	{
		public ContainerImplementation(Engine engine, Container control)
			: base(engine, control)
		{

		}

		static ContainerImplementation()
		{
			_hListBox_row_activated_d = new Action<IntPtr, IntPtr>(_hListBox_row_activated);
		}

		private static Action<IntPtr, IntPtr> _hListBox_row_activated_d;
		private static void _hListBox_row_activated(IntPtr listbox, IntPtr row)
		{
			Control ctlChild = (((UIApplication)Application.Instance).Engine as GTK3Engine).GetControlByHandle(row);
			if (ctlChild != null)
			{
				Console.WriteLine("activating row {0} for child {1}", row, (ctlChild as Container).Controls[0].Text);

				// FIXME: for some reason the SettingsDialog gets confused, perhaps by GetControlByHandle function
				// FIXME: also there is some really weird voodoo going on, second time SettingsDialog EVERYTHING is screwed up
				// ............ resulting in crash if we try to derefeerence ^^^ Text
				// InvokeMethod(ctlChild, "OnClick", new object[] { EventArgs.Empty });
			}
			else
			{
				Console.Error.WriteLine("uwt error: row {0} has no associated child control", row);
			}
		}

		private Dictionary<Layout, IntPtr> handlesByLayout = new Dictionary<Layout, IntPtr>();

		private IntPtr mvarContainerHandle = IntPtr.Zero;

		internal static void ApplyLayout(IntPtr hLayout, Control ctl, Layout layout)
		{
			GTKNativeControl hnc = (((UIApplication)Application.Instance).Engine.GetHandleForControl(ctl) as GTKNativeControl);
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
				Internal.GTK.Methods.GtkBox.gtk_box_set_spacing(hLayout, box.Spacing);
				Internal.GTK.Methods.GtkBox.gtk_box_set_homogeneous(hLayout, box.Homogeneous);

				BoxLayout.Constraints c = (box.GetControlConstraints(ctl) as BoxLayout.Constraints);
				if (c == null) c = new BoxLayout.Constraints();

				int padding = c.Padding == 0 ? ctl.Padding.All : c.Padding;

				switch (c.PackType)
				{
					case BoxLayout.PackType.Start:
					{
						Internal.GTK.Methods.GtkBox.gtk_box_pack_start(hLayout, ctlHandle, c.Expand, c.Fill, padding);
						break;
					}
					case BoxLayout.PackType.End:
					{
						Internal.GTK.Methods.GtkBox.gtk_box_pack_end(hLayout, ctlHandle, c.Expand, c.Fill, padding);
						break;
					}
				}
			}
			else if (layout is AbsoluteLayout)
			{
				AbsoluteLayout.Constraints constraints = (layout.GetControlConstraints(ctl) as Layouts.AbsoluteLayout.Constraints);
				if (constraints == null) constraints = new Layouts.AbsoluteLayout.Constraints(0, 0, 0, 0);
				Internal.GTK.Methods.GtkFixed.gtk_fixed_put(hLayout, ctlHandle, constraints.X, constraints.Y);
			}
			else if (layout is GridLayout)
			{
				GridLayout.Constraints constraints = (layout.GetControlConstraints(ctl) as Layouts.GridLayout.Constraints);
				if (constraints != null)
				{
					// GtkTable has been deprecated. Use GtkGrid instead. It provides the same capabilities as GtkTable for arranging widgets in a rectangular grid, but does support height-for-width geometry management.
					if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
					{
						Internal.GTK.Methods.GtkTable.gtk_table_attach(hLayout, ctlHandle, (uint)constraints.Column, (uint)(constraints.Column + constraints.ColumnSpan), (uint)constraints.Row, (uint)(constraints.Row + constraints.RowSpan), Internal.GTK.Constants.GtkAttachOptions.Expand, Internal.GTK.Constants.GtkAttachOptions.Fill, 0, 0);
					}
					else
					{
						Internal.GTK.Methods.GtkGrid.gtk_grid_attach(hLayout, ctlHandle, constraints.Column, constraints.Row, constraints.ColumnSpan, constraints.RowSpan);
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
				// Internal.GTK.Methods.GtkListBox.gtk_list_box_insert(hContainer, ctlHandle);
				Internal.GTK.Methods.GtkListBox.gtk_list_box_insert(hLayout, hListBoxRow);
			}
			else if (layout is StackLayout)
			{
				StackLayout.Constraints constraints = (layout.GetControlConstraints(ctl) as StackLayout.Constraints);
				Internal.GTK.Methods.GtkStack.gtk_stack_add_titled(hLayout, ctlHandle, constraints.Name, constraints.Title);
			}
			else if (layout is FlowLayout)
			{
				FlowLayout.Constraints constraints = (layout.GetControlConstraints(ctl) as FlowLayout.Constraints);
				Internal.GTK.Methods.GtkFlowBox.gtk_flow_box_insert(hLayout, ctlHandle);
			}
			else
			{
				throw new NotImplementedException(); // Internal.GTK.Methods.GtkContainer.gtk_container_add(hContainer, ctlHandle);
			}
		}


		protected override NativeControl CreateControlInternal(Control control)
		{
			IntPtr hContainer = IntPtr.Zero;
			Container container = (control as Container);

			Layout layout = container.Layout;
			if (container.Layout == null) layout = new Layouts.AbsoluteLayout();

			hContainer = CreateLayout(layout, container);

			if (hContainer != IntPtr.Zero)
			{
				mvarContainerHandle = hContainer;
				handlesByLayout[layout] = hContainer;

				Internal.GTK.Methods.GtkWidget.gtk_widget_show(hContainer);

				foreach (Control ctl in container.Controls)
				{
					bool ret = ctl.IsCreated;
					if (!ret) ret = Engine.CreateControl(ctl);
					if (!ret) continue;

					ApplyLayout(hContainer, ctl, layout);
				}
			}

			if (layout.Scrollable)
			{
				IntPtr hContainerWrapper = Internal.GTK.Methods.GtkScrolledWindow.gtk_scrolled_window_new(IntPtr.Zero, IntPtr.Zero);
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
			else
			{
				return new GTKNativeControl(hContainer, new KeyValuePair<string, IntPtr>[]
				{
					new KeyValuePair<string, IntPtr>("Container", hContainer),
					new KeyValuePair<string, IntPtr>("ScrolledWindow", IntPtr.Zero)
				});
			}
		}

		internal static IntPtr CreateLayout(Layout layout, Control control)
		{
			IntPtr hContainer = IntPtr.Zero;
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
				switch (((FlowLayout)layout).SelectionMode)
				{
					case SelectionMode.Browse:
					{
						Internal.GTK.Methods.GtkFlowBox.gtk_flow_box_set_selection_mode(hContainer, Internal.GTK.Constants.GtkSelectionMode.Browse);
						break;
					}
					case SelectionMode.Multiple:
					{
						Internal.GTK.Methods.GtkFlowBox.gtk_flow_box_set_selection_mode(hContainer, Internal.GTK.Constants.GtkSelectionMode.Multiple);
						break;
					}
					case SelectionMode.Single:
					{
						Internal.GTK.Methods.GtkFlowBox.gtk_flow_box_set_selection_mode(hContainer, Internal.GTK.Constants.GtkSelectionMode.Single);
						break;
					}
					default:
					{
						Internal.GTK.Methods.GtkFlowBox.gtk_flow_box_set_selection_mode(hContainer, Internal.GTK.Constants.GtkSelectionMode.None);
						break;
					}
				}
			}
			else if (layout is Layouts.ListLayout)
			{
				hContainer = Internal.GTK.Methods.GtkListBox.gtk_list_box_new();
				Internal.GObject.Methods.g_signal_connect(hContainer, "row_activated", _hListBox_row_activated_d);

				Internal.GTK.Methods.GtkListBox.gtk_list_box_set_selection_mode(hContainer, GTK3Engine.SelectionModeToGtkSelectionMode((layout as ListLayout).SelectionMode));

				if (control.BorderStyle == ControlBorderStyle.Fixed3D || control.BorderStyle == ControlBorderStyle.FixedSingle)
				{
					Internal.GTK.Methods.GtkStyleContext.gtk_style_context_add_class(Internal.GTK.Methods.GtkWidget.gtk_widget_get_style_context(hContainer), "frame");
				}
			}
			else if (layout is StackLayout)
			{
				hContainer = Internal.GTK.Methods.GtkStack.gtk_stack_new();
			}
			return hContainer;
		}

		public void InsertChildControl(Control.ControlCollection collection, Control child)
		{
			if (Control is Window && collection == ((Window)Control).StatusBar.Controls)
			{
				// HACK HACK HACK
				((WindowImplementation)this).InsertStatusBarControl(child);
			}
			else
			{
				ApplyLayout((Handle as GTKNativeControl).GetNamedHandle("Container"), child, (Control as Container).Layout);
			}
		}
		public void ClearChildControls(Control.ControlCollection collection)
		{
			Control[] ctls = (Control as IControlContainer).GetAllControls();

			IntPtr hContainer = (Handle as GTKNativeControl).GetNamedHandle("Container");
			List<IntPtr> _list = new List<IntPtr>();
			Internal.GTK.Methods.GtkContainer.gtk_container_forall(hContainer, delegate (IntPtr /*GtkWidget*/ widget, IntPtr data)
			{
				_list.Add(widget);
				Internal.GTK.Methods.GtkContainer.gtk_container_remove(hContainer, widget);
			}, IntPtr.Zero);

			// FIXME: this seems like it should be here but will cause crash later on when looking up control handle
			for (int i = 0; i < ctls.Length; i++)
			{
				Engine.UnregisterControlHandle(ctls[i]);
			}
		}
		public void RemoveChildControl(Control.ControlCollection collection, Control child)
		{
			IntPtr hContainer = ((GTKNativeControl)Handle).Handle;
			Layout layout = (Control as Container).Layout;
			if (layout is FlowLayout)
			{
				IntPtr hControl = ((GTKNativeControl)Engine.GetHandleForControl(child)).Handle;
				IntPtr hControlParent = Internal.GTK.Methods.GtkWidget.gtk_widget_get_parent(hControl);

				Internal.GTK.Methods.GtkContainer.gtk_container_remove(hContainer, hControlParent);
			}
			else
			{
				Internal.GTK.Methods.GtkContainer.gtk_container_remove(hContainer, ((GTKNativeControl)Engine.GetHandleForControl(child)).Handle);
			}
		}
		public void SetControlConstraints(Control.ControlCollection collection, Control control, Constraints cstr)
		{
			if (collection == (Control.ParentWindow)?.StatusBar?.Controls)
			{
				((WindowImplementation)Control.ParentWindow.ControlImplementation).SetStatusBarControlConstraints(control, cstr);
			}
			else
			{
				GTKNativeControl hnc = (Engine.GetHandleForControl(control) as GTKNativeControl);
				IntPtr ctlHandle = hnc.Handle;

				IntPtr hContainer = (Engine.GetHandleForControl((Control)control.Parent) as GTKNativeControl).Handle;
				Layout layout = ((Container)control.Parent).Layout;

				if (cstr != null)
				{
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_hexpand(ctlHandle, cstr.HorizontalExpand);
					Internal.GTK.Methods.GtkWidget.gtk_widget_set_vexpand(ctlHandle, cstr.VerticalExpand);

					if (layout is BoxLayout)
					{
						BoxLayout.Constraints constraints = (cstr as BoxLayout.Constraints);
						if (constraints == null) constraints = new BoxLayout.Constraints();

						int padding = constraints.Padding == 0 ? control.Padding.All : constraints.Padding;

						switch (constraints.PackType)
						{
							case BoxLayout.PackType.Start:
							{
								Internal.GTK.Methods.GtkBox.gtk_box_set_child_packing(hContainer, ctlHandle, constraints.Expand, constraints.Fill, padding, Internal.GTK.Constants.GtkPackType.Start);
								break;
							}
							case BoxLayout.PackType.End:
							{
								Internal.GTK.Methods.GtkBox.gtk_box_set_child_packing(hContainer, ctlHandle, constraints.Expand, constraints.Fill, padding, Internal.GTK.Constants.GtkPackType.End);
								break;
							}
						}
					}
					else if (layout is AbsoluteLayout)
					{
						AbsoluteLayout.Constraints constraints = (cstr as Layouts.AbsoluteLayout.Constraints);
						if (constraints == null) constraints = new Layouts.AbsoluteLayout.Constraints(0, 0, 0, 0);
						Internal.GTK.Methods.GtkFixed.gtk_fixed_move(hContainer, ctlHandle, constraints.X, constraints.Y);
					}
					else if (layout is GridLayout)
					{
						GridLayout.Constraints constraints = (cstr as Layouts.GridLayout.Constraints);
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
						// nothing to do
					}
					else if (layout is StackLayout)
					{
						StackLayout.Constraints constraints = (cstr as StackLayout.Constraints);
						// intentionally left blank as there are no StackLayout constraints
					}
					else if (layout is FlowLayout)
					{
						FlowLayout.Constraints constraints = (cstr as FlowLayout.Constraints);
						// intentionally left blank as there are no FlowLayout constraints
					}
					else
					{
						// intentionally left blank as there is nothing to do
					}
				}
			}
		}
	}
}
