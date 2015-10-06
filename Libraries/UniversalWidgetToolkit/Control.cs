using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalWidgetToolkit
{
	public class Control
	{
		public class ControlCollection
			: System.Collections.ObjectModel.Collection<Control>
		{
			private Control _parent = null;
			public ControlCollection(Control parent = null)
			{
				_parent = parent;
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				// if (_parent != null) Application.Engine.UpdateControlCollection(_parent);
			}
			protected override void InsertItem(int index, Control item)
			{
				base.InsertItem(index, item);
				// if (_parent != null) Application.Engine.UpdateControlCollection(_parent);
			}
			protected override void RemoveItem(int index)
			{
				base.RemoveItem(index);
				// if (_parent != null) Application.Engine.UpdateControlCollection(_parent);
			}
			protected override void SetItem(int index, Control item)
			{
				base.SetItem(index, item);
				// if (_parent != null) Application.Engine.UpdateControlCollection(_parent);
			}
		}

		private Control.ControlCollection mvarControls = null;
		public Control.ControlCollection Controls { get { return mvarControls; } }

		private string mvarClassName = String.Empty;
		public string ClassName { get { return mvarClassName; } set { mvarClassName = value; } }

		private IntPtr mvarHandle = IntPtr.Zero;
		public IntPtr Handle { get { return mvarHandle; } }
		public bool IsHandleCreated { get { return mvarHandle != IntPtr.Zero; } }
		public void CreateControl()
		{
			Application.Engine.CreateControl(this);
		}
		/// <summary>
		/// Shows this <see cref="Window" />.
		/// </summary>
		public void Show()
		{
			if (!IsHandleCreated) CreateControl();
		}
		/// <summary>
		/// Hides this <see cref="Window" />, keeping the native object around for later use.
		/// </summary>
		public void Hide()
		{

		}

		/// <summary>
		/// Destroys the handle associated with this <see cref="Control" />.
		/// </summary>
		public void Destroy()
		{

		}
	}
}
