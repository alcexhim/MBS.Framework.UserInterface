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

			public void Add(Control item, Constraints constraints)
			{
				Add (item);
				if (constraints != null) {
					_parent.Layout.SetControlConstraints (item, constraints);
				}
			}
		}

		public Rectangle ClientRectangle
		{
			get
			{
				if (this is Window)
				{
					return (this as Window).Bounds;
				}
				else if (mvarParent == null)
				{
					return Rectangle.Empty;
				}
				return mvarParent.Layout.GetControlBounds(this);
			}
		}

		public bool IsCreated { get { return Application.Engine.IsControlCreated(this); } }

		private Padding mvarMargin = new Padding();
		public Padding Margin { get { return mvarMargin; } set { mvarMargin = value; } }

		private Padding mvarPadding = new Padding();
		public Padding Padding { get { return mvarPadding; } set { mvarPadding = value; } }

		private Brush mvarBackgroundBrush = new SolidBrush(Colors.White);
		public Brush BackgroundBrush { get; set; }

		private string mvarClassName = null;
		public string ClassName { get { return mvarClassName; } set { mvarClassName = value; } }

		private Font mvarFont = null;
		public Font Font { get { return mvarFont; } set { mvarFont = value; } }

		private Container mvarParent = null;
		public Container Parent { get { return mvarParent; } }

		private string mvarText = null;
		public string Text
		{
			get
			{
				return mvarText;
			}
			set
			{
				mvarText = value;
				Application.Engine.UpdateControlProperties (this);
			}
		}

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

		public event EventHandler Resizing;
		public virtual void OnResizing(EventArgs e)
		{
			if (Resizing != null) Resizing(this, e);
		}

		public Window ParentWindow
		{
			get
			{
				Control ctl = mvarParent;
				while (ctl.Parent != null)
				{
					ctl = ctl.Parent;
				}
				if (ctl is Window) return (ctl as Window);
				return null;
			}
		}

		public void Invalidate()
		{
			Application.Engine.InvalidateControl(this);
		}
		

		public bool IsDisposed()
		{
			return Application.Engine.IsControlDisposed(this);
		}
	}
}
