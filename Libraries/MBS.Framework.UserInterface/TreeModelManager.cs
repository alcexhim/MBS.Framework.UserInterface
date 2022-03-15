using System;
using System.Diagnostics.Contracts;
using MBS.Framework.Collections.Generic;

namespace MBS.Framework.UserInterface
{
	public abstract class TreeModelManager
	{
		protected abstract NativeTreeModel CreateTreeModelInternal(TreeModel model);

		private HandleDictionary<TreeModel, NativeTreeModel> handles = new HandleDictionary<TreeModel, NativeTreeModel>();
		public bool IsTreeModelCreated(TreeModel model)
		{
			return handles.Contains(model);
		}

		public TreeModel GetTreeModelForHandle(NativeTreeModel handle)
		{
			return handles.GetObject(handle);
		}
		public NativeTreeModel GetHandleForTreeModel(TreeModel model)
		{
			return handles.GetHandle(model);
		}

		protected abstract bool UnregisterTreeModelInternal(TreeModel model);
		/// <summary>
		/// Unregisters the <see cref="TreeModel" /> referenced by
		/// <paramref name="model" />.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if the specified <see cref="TreeModel" /> was
		/// unregistered, <c>false</c> otherwise.
		/// </returns>
		/// <param name="model">The <see cref="TreeModel" /> to remove.</param>
		public bool UnregisterTreeModel(TreeModel model)
		{
			if (UnregisterTreeModelInternal(model))
			{
				handles.Remove(model);
				return true;
			}
			return false;
		}

		public NativeTreeModel CreateTreeModel(TreeModel model)
		{
			Contract.Requires(model != null);
			Contract.Ensures(Contract.Result<NativeTreeModel>() != null);

			if (IsTreeModelCreated(model))
			{
				return GetHandleForTreeModel(model);

				// TODO: unregister handle
				UnregisterTreeModel(model);
				// return _HandleForTreeModel[model]; // this fks up additional columns tweaking
			}

			NativeTreeModel handle = CreateTreeModelInternal(model);
			handles.Add(handle, model);

			// insert any rows specified by default in the treemodel
			DefaultTreeModel dtm = (model as DefaultTreeModel);
			if (dtm != null)
			{
				NativeHandle nh = null;
				foreach (TreeModelRow row in dtm.Rows)
				{
					InsertTreeModelRow(dtm, row, out nh, dtm.Rows.Count - 1);
				}
			}

			return handle;
		}

		internal void CreateTreeModelRow(TreeModelRow row, TreeModel model)
		{
			NativeHandle nh = null;
			InsertTreeModelRow(model, row, out nh, 0, true);
			// RecursiveTreeStoreInsertRow(model, row, hTreeModel, out hIter, null, (model as DefaultTreeModel).Rows.Count - 1);

			// CreateTreeModelRowInternal(row, model);
		}

		private HandleDictionary<TreeModelRow, NativeHandle> _TreeModelRowHandles = new HandleDictionary<TreeModelRow, NativeHandle>();

		public NativeHandle GetHandleForTreeModelRow(TreeModelRow value)
		{
			return _TreeModelRowHandles.GetHandle(value);
		}
		public T GetHandleForTreeModelRow<T>(TreeModelRow value)
		{
			NativeHandle h = GetHandleForTreeModelRow(value);
			if (h is NativeHandle<T>)
			{
				return ((NativeHandle<T>)h).Handle;
			}
			throw new InvalidCastException();
		}

		public TreeModelRow GetTreeModelRowForHandle(NativeHandle handle)
		{
			return _TreeModelRowHandles.GetObject(handle);
		}
		public TreeModelRow GetTreeModelRowForHandle<T>(T nativeHandle)
		{
			return GetTreeModelRowForHandle((NativeHandle)(new NativeHandle<T>(nativeHandle)));
		}

		protected abstract void UpdateTreeModelColumnInternal(TreeModelRowColumn rc);
		public void UpdateTreeModelColumn(TreeModelRowColumn rc)
		{
			UpdateTreeModelColumnInternal(rc);
		}

		public bool IsTreeModelRowRegistered(TreeModelRow row)
		{
			return _TreeModelRowHandles.Contains(row);
		}

		protected abstract void InsertTreeModelRowInternal(TreeModel tm, TreeModelRow row, out NativeHandle rowHandle, int position, bool append);
		public void InsertTreeModelRow(TreeModel tm, TreeModelRow row, out NativeHandle rowHandle, int position = 0, bool append = true)
		{
			row.ParentModel = tm;

			InsertTreeModelRowInternal(tm, row, out rowHandle, position, append);
			_TreeModelRowHandles.Add(rowHandle, row);

			foreach (TreeModelRow row2 in row.Rows)
			{
				NativeHandle hIter2 = null;
				InsertTreeModelRow(tm, row2, out hIter2, row.Rows.Count - 1);
			}
		}
	}
}
