//
//  ComboBoxImplementation.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Native;

namespace MBS.Framework.UserInterface.Engines.GTK.Controls
{
	[ControlImplementation(typeof(ComboBox))]
	public class ComboBoxImplementation : GTKNativeImplementation, IComboBoxNativeImplementation
	{
		public ComboBoxImplementation (Engine engine, Control control)
			: base(engine, control)
		{
			gc_Changed_Handler = new Action<IntPtr>(gc_Changed);
		}

		protected override string GetControlTextInternal(Control control)
		{
			ComboBox cbo = (control as ComboBox);
			if (cbo.SelectedItem != null)
			{
				if (cbo.SelectedItem.RowColumns.Count > 0)
				{
					return cbo.SelectedItem.RowColumns[0 /*cbo.DisplayIndex*/].Value?.ToString();
				}
			}
			else if (!cbo.ReadOnly)
			{
				IntPtr handle = (Handle as GTKNativeControl).Handle;
				IntPtr hEntry = Internal.GTK.Methods.GtkBin.gtk_bin_get_child(handle);
				return Internal.GTK.Methods.GtkEntry.gtk_entry_get_text(hEntry);
			}
			return base.GetControlTextInternal(control);
		}
		protected override void SetControlTextInternal(Control control, string text)
		{
			ComboBox cbo = (control as ComboBox);
			/*
			for (int i = 0; i < (cbo.Model as DefaultTreeModel).Rows.Count; i++)
			{

			}
			if (cbo.SelectedItem != null)
			{
				if (cbo.SelectedItem.RowColumns.Count > 0)
				{
					// cbo.DisplayIndex
					return cbo.SelectedItem.RowColumns[0].Value?.ToString();
				}
			}
			else
			*/
			if (!cbo.ReadOnly)
			{
				IntPtr handle = (Handle as GTKNativeControl).Handle;
				IntPtr hEntry = Internal.GTK.Methods.GtkBin.gtk_bin_get_child(handle);
				Internal.GTK.Methods.GtkEntry.gtk_entry_set_text(hEntry, text);
				return;
			}
			base.SetControlTextInternal(control, text);
		}

		public TreeModel GetModel()
		{
			IntPtr handle = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;
			IntPtr hTreeModel = Internal.GTK.Methods.GtkComboBox.gtk_combo_box_get_model(handle);

			TreeModel tm = Engine.TreeModelFromHandle(new GTKNativeTreeModel(hTreeModel));
			return tm;
		}

		public void SetModel(TreeModel value)
		{
			IntPtr handle = (Engine.GetHandleForControl(Control) as GTKNativeControl).Handle;

			GTKNativeTreeModel ncTreeModel = (Engine.CreateTreeModel(value) as GTKNativeTreeModel);
			IntPtr hTreeModel = ncTreeModel.Handle;
			Internal.GTK.Methods.GtkComboBox.gtk_combo_box_set_model(handle, hTreeModel);
			Internal.GTK.Methods.GtkComboBox.gtk_combo_box_set_entry_text_column(handle, 0);
		}

		private bool mvarReadOnly = false;
		public bool GetReadOnly()
		{
			return mvarReadOnly;
		}

		public void SetReadOnly(bool value)
		{
			mvarReadOnly = value;
		}

		public TreeModelRow GetSelectedItem()
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Structures.GtkTreeIter hIter = new Internal.GTK.Structures.GtkTreeIter();
			bool ret = Internal.GTK.Methods.GtkComboBox.gtk_combo_box_get_active_iter(handle, ref hIter);
			if (ret)
			{
				TreeModelRow row = (Engine as GTKEngine).GetTreeModelRowForGtkTreeIter(hIter);
				return row;
			}
			return null;
		}

		public void SetSelectedItem(TreeModelRow value)
		{
			IntPtr handle = (Handle as GTKNativeControl).Handle;
			Internal.GTK.Structures.GtkTreeIter hIter = (Engine as GTKEngine).GetGtkTreeIterForTreeModelRow(value);
			Internal.GTK.Methods.GtkComboBox.gtk_combo_box_set_active_iter(handle, ref hIter);
		}

		private Action<IntPtr> gc_Changed_Handler = null;
		private void gc_Changed(IntPtr combo_box)
		{
			OnChanged(EventArgs.Empty);
		}

		protected virtual void OnChanged(EventArgs e)
		{
			InvokeMethod((Control as ComboBox), "OnChanged", e);
		}

		protected override NativeControl CreateControlInternal (Control control)
		{
			IntPtr handle = IntPtr.Zero;

			ComboBox ctl = (control as ComboBox);
			if (ctl.ReadOnly)
			{
				if (Internal.GTK.Methods.Gtk.LIBRARY_FILENAME == Internal.GTK.Methods.Gtk.LIBRARY_FILENAME_V2)
				{
					handle = Internal.GTK.Methods.GtkComboBox.gtk_combo_box_new();
				}
				else
				{
					IntPtr area = Internal.GTK.Methods.GtkCellAreaBox.gtk_cell_area_box_new();

					IntPtr[] renderers = ListViewImplementation.CreateCellRenderers(area, ctl);

					for (int i = 0; i < renderers.Length; i++)
					{
						Internal.GTK.Methods.GtkCellArea.gtk_cell_area_attribute_connect(area, renderers[i], "text", 0);
						Internal.GTK.Methods.GtkCellAreaBox.gtk_cell_area_box_pack_start(area, renderers[i], true, true, false);
					}

					handle = Internal.GTK.Methods.GtkComboBox.gtk_combo_box_new_with_area(area);
				}
			}
			else
			{
				handle = Internal.GTK.Methods.GtkComboBox.gtk_combo_box_new_with_entry();
			}

			IntPtr hTreeModel = IntPtr.Zero;
			if (ctl.Model != null)
			{
				GTKNativeTreeModel ncTreeModel = (Engine.CreateTreeModel(ctl.Model) as GTKNativeTreeModel);
				hTreeModel = ncTreeModel.Handle;
				Internal.GTK.Methods.GtkComboBox.gtk_combo_box_set_model(handle, hTreeModel);
				Internal.GTK.Methods.GtkComboBox.gtk_combo_box_set_entry_text_column(handle, 0);
			}

			Internal.GObject.Methods.g_signal_connect(handle, "changed", gc_Changed_Handler);

			return new GTKNativeControl (handle, new KeyValuePair<string, IntPtr>[]
			{
				new KeyValuePair<string, IntPtr>("TreeModel", hTreeModel)
			});
		}
	}
}
