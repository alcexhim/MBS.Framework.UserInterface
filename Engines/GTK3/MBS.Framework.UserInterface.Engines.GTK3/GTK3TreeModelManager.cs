//
//  GTK3TreeModelManager.cs
//
//  Author:
//       beckermj <>
//
//  Copyright (c) 2022 ${CopyrightHolder}
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
namespace MBS.Framework.UserInterface.Engines.GTK3
{
	public class GTK3TreeModelManager : TreeModelManager
	{
		protected override NativeTreeModel CreateTreeModelInternal(TreeModel model)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));

			/*
			IntPtr[] types = new IntPtr[model.Columns.Count];
			for (int i = 0; i < model.Columns.Count; i++)
			{
				types[i] = Internal.GLib.Constants.GType.FromType(model.Columns[i].DataType);
			}
			IntPtr hModel = Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_newv(types.Length, types);
			return new GTKNativeTreeModel(hModel);
			*/

			IntPtr[] types = new IntPtr[model.Columns.Count];
			for (int i = 0; i < model.Columns.Count; i++)
			{
				IntPtr ptr = Internal.GLib.Constants.GType.FromType(model.Columns[i].DataType);
				types[i] = ptr;
			}

			if (types.Length <= 0)
			{
				Console.WriteLine("uwt ERROR: you did not specify any columns for the ListView!!!");
				types = new IntPtr[] { Internal.GLib.Constants.GType.FromType(typeof(string)) };
			}

			IntPtr hTreeStore = Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_newv(types.Length, types);

			HandleGtkTreeIterCompareFunc_d = new Internal.GTK.Delegates.GtkTreeIterCompareFunc(HandleGtkTreeIterCompareFunc);
			HandleGClosureNotify_d = new Internal.GObject.Delegates.GClosureNotify(HandleGClosureNotify);
			for (int i = 0; i < types.Length; i++)
			{
				Internal.GTK.Methods.GtkTreeSortable.gtk_tree_sortable_set_sort_func(hTreeStore, i, HandleGtkTreeIterCompareFunc_d, new IntPtr(i), HandleGClosureNotify_d);
			}

			return new GTKNativeTreeModel(hTreeStore);
		}

		void HandleGClosureNotify(IntPtr data, IntPtr closure)
		{
		}

		Internal.GObject.Delegates.GClosureNotify HandleGClosureNotify_d = null;
		Internal.GTK.Delegates.GtkTreeIterCompareFunc HandleGtkTreeIterCompareFunc_d = null;

		int HandleGtkTreeIterCompareFunc(IntPtr model, ref Internal.GTK.Structures.GtkTreeIter a, ref Internal.GTK.Structures.GtkTreeIter b, IntPtr user_data)
		{
			// user_data isn't actually a pointer, it's just an int wrapped in a ptr (bad? :P )
			int columnIndex = user_data.ToInt32();

			TreeModel tm = GetTreeModelForHandle(new GTKNativeTreeModel(model));
			if (tm == null)
				return -1;

			TreeModelRow rowA = GetTreeModelRowForHandle(a); // a.user_data
			TreeModelRow rowB = GetTreeModelRowForHandle(b); // b.user_data
			if (rowA == null || rowB == null)
			{
				return -1;
			}

			TreeModelRowCompareEventArgs ee = new TreeModelRowCompareEventArgs(rowA, rowB, columnIndex);
			Reflection.InvokeMethod(tm, "OnRowCompare", ee);
			if (ee.Handled)
				return ee.Value;

			if (columnIndex >= 0 && columnIndex < rowA.RowColumns.Count && columnIndex < rowB.RowColumns.Count)
			{
				if (rowA.RowColumns[columnIndex].RawValue is IComparable)
				{
					return (rowA.RowColumns[columnIndex].RawValue as IComparable).CompareTo(rowB.RowColumns[columnIndex].RawValue);
				}
				else if (rowB.RowColumns[columnIndex].RawValue is IComparable)
				{
					return (rowB.RowColumns[columnIndex].RawValue as IComparable).CompareTo(rowA.RowColumns[columnIndex].RawValue);
				}
			}
			return -1;
		}

		protected override void UpdateTreeModelColumnInternal(TreeModelRowColumn rc)
		{
			TreeModel tm = rc.Parent.ParentModel;
			IntPtr hTreeStore = (GetHandleForTreeModel(tm) as GTKNativeTreeModel).Handle;
			Internal.GTK.Structures.GtkTreeIter hIter = GetHandleForTreeModelRow<Internal.GTK.Structures.GtkTreeIter>(rc.Parent);

			Internal.GLib.Structures.Value val = Internal.GLib.Structures.Value.FromObject(rc.Value);
			Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_set_value(hTreeStore, ref hIter, tm.Columns.IndexOf(rc.Column), ref val);
		}

		protected override void InsertTreeModelRowInternal(TreeModel tm, TreeModelRow row, out NativeHandle rowHandle, int position, bool append)
		{
			IntPtr hTreeStore = ((GTKNativeTreeModel) GetHandleForTreeModel(tm)).Handle;
			Internal.GTK.Structures.GtkTreeIter hIter = new Internal.GTK.Structures.GtkTreeIter();
			Internal.GTK.Structures.GtkTreeIter hIterParent = new Internal.GTK.Structures.GtkTreeIter();

			if (row.ParentRow == null)
			{
				if (append)
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_append(hTreeStore, out hIter, IntPtr.Zero);
				}
				else
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_insert(hTreeStore, out hIter, IntPtr.Zero, position);
				}
			}
			else
			{
				hIterParent = GetHandleForTreeModelRow<Internal.GTK.Structures.GtkTreeIter>(row.ParentRow);
				if (append)
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_append(hTreeStore, out hIter, ref hIterParent);
				}
				else
				{
					Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_insert(hTreeStore, out hIter, ref hIterParent, position);
				}
			}

			rowHandle = new NativeHandle<Internal.GTK.Structures.GtkTreeIter>(hIter);

			foreach (TreeModelRowColumn rc in row.RowColumns)
			{
				// since "Marshalling of type object is not implemented"
				// (mono/metadata/marshal.c:6507) we have to do it ourselves


				Internal.GLib.Structures.Value val = Internal.GLib.Structures.Value.FromObject(rc.Value);

				// Internal.GTK.Methods.Methods.gtk_tree_store_insert(hTreeStore, out hIter, IntPtr.Zero, 0);
				Internal.GTK.Methods.GtkTreeStore.gtk_tree_store_set_value(hTreeStore, ref hIter, tm.Columns.IndexOf(rc.Column), ref val);

				// this can only be good, right...?
				// val.Dispose();

				// I thought this caused "malloc() : smallbin doubly linked list corrupted" error, but apparently it doesn't...?
				// back to square one...
			}
		}

		protected override bool UnregisterTreeModelInternal(TreeModel model)
		{
			IntPtr handle = ((GTKNativeTreeModel)GetHandleForTreeModel(model)).Handle;
			// Internal.GLib.Methods.g_free(handle);
			return true;
		}
	}
}
