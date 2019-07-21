using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MBS.Framework.Drawing;
using UniversalWidgetToolkit.DragDrop;
using UniversalWidgetToolkit.Drawing;
using UniversalWidgetToolkit.Input.Keyboard;
using UniversalWidgetToolkit.Input.Mouse;

namespace UniversalWidgetToolkit
{
	public abstract class Control : IDisposable, ISupportsExtraData
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
				Add(item);
				if (constraints != null)
				{
					_parent.Layout?.SetControlConstraints(item, constraints);
				}
			}
		}

		public string Name { get; set; } = String.Empty;

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
		

		private Vector2D mvarLocation = new Vector2D(0, 0);
		public Vector2D Location { get { return mvarLocation; } set { mvarLocation = value; } }

		/// <summary>
		/// Translates the given <see cref="Vector2D" /> from client coordinates into screen coordinates.
		/// </summary>
		/// <returns>The to screen coordinates.</returns>
		/// <param name="point">Point.</param>
		public Vector2D ClientToScreenCoordinates(Vector2D point)
		{
			return Application.Engine.ClientToScreenCoordinates(point);
		}

		private Dimension2D mvarSize = new Dimension2D(0, 0);
		public Dimension2D Size { get { return mvarSize; } set { mvarSize = value; } }

		private NativeImplementation mvarNativeImplementation = null;
		public NativeImplementation NativeImplementation { get { return mvarNativeImplementation; } internal set { mvarNativeImplementation = value; } }

		public bool IsCreated { get { return Application.Engine.IsControlCreated(this); } }

		private Padding mvarMargin = new Padding();
		public Padding Margin { get { return mvarMargin; } set { mvarMargin = value; } }

		private Padding mvarPadding = new Padding();
		public Padding Padding { get { return mvarPadding; } set { mvarPadding = value; } }

		private Brush mvarBackgroundBrush = new SolidBrush(Colors.White);
		public Brush BackgroundBrush { get; set; }

		private string mvarClassName = null;
		public string ClassName { get { return mvarClassName; } set { mvarClassName = value; } }

		private bool mvarEnabled = true;
		public bool Enabled
		{
			get
			{
				if (Application.Engine == null) return mvarEnabled;
				if (!Application.Engine.IsControlCreated(this)) return mvarEnabled;
				return Application.Engine.IsControlEnabled(this);
			}
			set
			{
				if (Application.Engine != null && Application.Engine.IsControlCreated(this))
				{
					Application.Engine.SetControlEnabled(this, value);
				}
				mvarEnabled = value;
			}
		}

		private Font mvarFont = null;
		public Font Font { get { return mvarFont; } set { mvarFont = value; } }


		/// <summary>
		/// Gets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		public System.Collections.Generic.Dictionary<string, object> Attributes { get; } = new Dictionary<string, object>();

		private Container mvarParent = null;
		public Container Parent { get { return mvarParent; } }

		private string mvarText = null;
		public string Text
		{
			get
			{
				string text = Application.Engine.GetControlText(this);
				if (text == null) return mvarText;
				return text;
			}
			set
			{
				mvarText = value;
				Application.Engine.SetControlText(this, value);
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
			Application.Engine.DestroyControl(this);
		}



		#region Drag-n-Drop
		public event DragEventHandler DragBegin;
		protected virtual void OnDragBegin(DragEventArgs e)
		{
			DragBegin?.Invoke(this, e);
		}
		public event DragEventHandler DragEnter;
		protected virtual void OnDragEnter(DragEventArgs e)
		{
			DragEnter?.Invoke(this, e);
		}
		public event DragEventHandler DragDrop;
		protected virtual void OnDragDrop(DragEventArgs e)
		{
			DragDrop?.Invoke(this, e);
		}
		public event DragDropDataRequestEventHandler DragDropDataRequest;
		protected virtual void OnDragDropDataRequest(DragDropDataRequestEventArgs e)
		{
			DragDropDataRequest?.Invoke(this, e);
		}
		
		public void RegisterDragSource(DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons = MouseButtons.Primary | MouseButtons.Secondary, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None)
		{
			NativeImplementation.RegisterDragSource(this, targets, actions, buttons, modifierKeys);
		}
		public void RegisterDropTarget(DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons = MouseButtons.Primary | MouseButtons.Secondary, KeyboardModifierKey modifierKeys = KeyboardModifierKey.None)
		{
			NativeImplementation.RegisterDropTarget(this, targets, actions, buttons, modifierKeys);
		}


		#endregion
		#region Mouse Events
		public event MouseEventHandler MouseDown;
		protected virtual void OnMouseDown(MouseEventArgs e)
		{
			MouseDown?.Invoke(this, e);
		}
		public event MouseEventHandler MouseMove;
		protected virtual void OnMouseMove(MouseEventArgs e)
		{
			MouseMove?.Invoke(this, e);
		}
		public event MouseEventHandler MouseUp;
		protected virtual void OnMouseUp(MouseEventArgs e)
		{
			MouseUp?.Invoke(this, e);
		}
		#endregion
		#region Keyboard Events
		public event KeyEventHandler KeyDown;
		protected virtual void OnKeyDown(KeyEventArgs e)
		{
			KeyDown?.Invoke(this, e);
		}
		public event KeyEventHandler KeyPress;
		protected virtual void OnKeyPress(KeyEventArgs e)
		{
			KeyPress?.Invoke(this, e);
		}
		public event KeyEventHandler KeyUp;
		protected virtual void OnKeyUp(KeyEventArgs e)
		{
			KeyUp?.Invoke(this, e);
		}
		#endregion
		public event PaintEventHandler Paint;
		protected virtual void OnPaint(PaintEventArgs e)
		{
			Paint?.Invoke(this, e);
		}

		public event EventHandler Creating;
		protected virtual void OnCreating(EventArgs e)
		{
			Creating?.Invoke(this, e);
		}
		public event EventHandler Created;
		protected virtual void OnCreated(EventArgs e)
		{
			Created?.Invoke(this, e);
		}

		public event EventHandler Click;
		protected virtual void OnClick(EventArgs e)
		{
			Click?.Invoke(this, e);
		}

		public event EventHandler Realize;
		protected virtual void OnRealize(EventArgs e)
		{
			Realize?.Invoke(this, e);
		}
		public event EventHandler Unrealize;
		protected virtual void OnUnrealize(EventArgs e)
		{
			Unrealize?.Invoke(this, e);
		}

		public event ResizeEventHandler Resizing;
		protected virtual void OnResizing(ResizeEventArgs e)
		{
			Resizing?.Invoke(this, e);
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

		public Dimension2D MinimumSize { get; set; } = Dimension2D.Empty;

		public void Invalidate()
		{
			// TODO: actually get dimensions of this Control
			Invalidate(0, 0, 4096, 4096);
		}
		public void Invalidate(int x, int y, int width, int height)
		{
			Application.Engine.InvalidateControl(this, x, y, width, height);
		}
		public void Refresh()
		{
			// convenience method
			Invalidate();
		}
		
		private bool mvarIsDisposed = false;
		public bool IsDisposed
		{
			get
			{
				if (Application.Engine != null)
					return Application.Engine.IsControlDisposed(this);
				return mvarIsDisposed;
			}
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void DisposeManagedInternal()
		{
		}
		protected virtual void DisposeUnmanagedInternal()
		{
		}
		
		protected void Dispose(bool disposing)
		{
			if (mvarIsDisposed)
				return;
			
			if (disposing) {
				// free any managed objects here
				DisposeManagedInternal();
			}

			// free any unmanaged objects here
			DisposeUnmanagedInternal();
			
			mvarIsDisposed = true;
		}
		
		~Control()
		{
			Dispose(false);
		}

		private Dictionary<string, object> _ExtraData = new Dictionary<string, object>();
		public T GetExtraData<T>(string key, T defaultValue = default(T))
		{
			if (_ExtraData.ContainsKey(key)) return (T)_ExtraData[key];
			return defaultValue;
		}
		public object GetExtraData(string key, object defaultValue = null)
		{
			return GetExtraData<object>(key, defaultValue);
		}
		public void SetExtraData<T>(string key, T value)
		{
			_ExtraData[key] = value;
		}
		public void SetExtraData(string key, object value)
		{
			SetExtraData<object>(key, value);
		}
	}
}
