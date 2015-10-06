using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Drawing;

namespace UniversalWidgetToolkit
{
	public class Control
	{
		public class ControlCollection
			: System.Collections.ObjectModel.Collection<Control>
		{
			private Container _parent = null;
			public ControlCollection(Container parent = null)
			{
				_parent = parent;
			}

			protected override void ClearItems()
			{
				foreach (Control ctl in this)
				{
					ctl.mvarParent = null;
				}
				base.ClearItems();
				// if (_parent != null) Application.Engine.UpdateControlCollection(_parent);
			}
			protected override void InsertItem(int index, Control item)
			{
				base.InsertItem(index, item);
				item.mvarParent = _parent;
				// if (_parent != null) Application.Engine.UpdateControlCollection(_parent);
			}
			protected override void RemoveItem(int index)
			{
				this[index].mvarParent = null;
				base.RemoveItem(index);
				// if (_parent != null) Application.Engine.UpdateControlCollection(_parent);
			}
			protected override void SetItem(int index, Control item)
			{
				this[index].mvarParent = null;
				base.SetItem(index, item);
				item.mvarParent = _parent;
				// if (_parent != null) Application.Engine.UpdateControlCollection(_parent);
			}
		}

		private Brush mvarBackgroundBrush = new SolidBrush(Colors.White);
		public Brush BackgroundBrush { get; set; }

		private string mvarClassName = null;
		public string ClassName { get { return mvarClassName; } set { mvarClassName = value; } }

		private Font mvarFont = null;
		public Font Font { get { return mvarFont; } set { mvarFont = value; } }

		private Container mvarParent = null;
		public Container Parent { get { return mvarParent; } }

		private string mvarTitle = String.Empty;
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private bool mvarVisible = true;
		public bool Visible
		{
			get { return mvarVisible; }
			set
			{
				Application.Engine.SetControlVisibility(this, value);
			}
		}

		/// <summary>
		/// Shows this <see cref="Window" />.
		/// </summary>
		public void Show()
		{
			Visible = true;
		}
		/// <summary>
		/// Hides this <see cref="Window" />, keeping the native object around for later use.
		/// </summary>
		public void Hide()
		{
			Visible = false;
		}

		/// <summary>
		/// Destroys the handle associated with this <see cref="Control" />.
		/// </summary>
		public void Destroy()
		{

		}

		public event PaintEventHandler Paint;
		public virtual void OnPaint(PaintEventArgs e)
		{
			if (Paint != null) Paint(this, e);
		}

		public event EventHandler Created;
		public virtual void OnCreated(EventArgs e)
		{
			if (Created != null) Created(this, e);
		}

		public event EventHandler Click;
		public virtual void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}
	}
}
