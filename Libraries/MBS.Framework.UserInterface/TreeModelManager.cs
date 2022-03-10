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
				// TODO: unregister handle
				UnregisterTreeModel(model);
				// return _HandleForTreeModel[model]; // this fks up additional columns tweaking
			}

			NativeTreeModel handle = CreateTreeModelInternal(model);
			handles.Add(handle, model);
			return handle;
		}

		protected abstract void CreateTreeModelRowInternal(TreeModelRow row, TreeModel model);
		internal void CreateTreeModelRow(TreeModelRow row, TreeModel model)
		{
			CreateTreeModelRowInternal(row, model);
		}
		
	}
}
