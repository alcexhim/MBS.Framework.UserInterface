using System;
using System.Collections.Generic;

using UniversalWidgetToolkit.DragDrop;
using UniversalWidgetToolkit.Input.Keyboard;
using UniversalWidgetToolkit.Input.Mouse;

namespace UniversalWidgetToolkit
{
	/// <summary>
	/// Native implementation for the specified Control.
	/// </summary>
	public abstract class NativeImplementation
	{
		protected void InvokeMethod(object obj, string meth, params object[] parms)
		{
			if (obj == null)
			{
				Console.WriteLine("NativeImplementation::InvokeMethod: obj is null");
				return;
			}
			
			Type t = obj.GetType();
			System.Reflection.MethodInfo mi = t.GetMethod(meth, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			if (mi != null)
			{
				mi.Invoke(obj, parms);
			}
			else
			{
				Console.WriteLine("NativeImplementation::InvokeMethod: not found '" + meth + "' on '" + t.FullName + "'");
			}
		}
		
		private Control mvarControl = null;
		public Control Control {  get { return mvarControl; } }

		private Engine mvarEngine = null;
		public Engine Engine {  get { return mvarEngine; } }

		public NativeImplementation(Engine engine, Control control)
		{
			mvarEngine = engine;
			mvarControl = control;
		}

		private Dictionary<Control, string> _controlText = new Dictionary<Control, string>();
		protected virtual string GetControlTextInternal(Control control)
		{
			if (_controlText.ContainsKey(control))
				return _controlText[control];
			return String.Empty;
		}
		public string GetControlText(Control control)
		{
			return GetControlTextInternal(control);
		}
		protected virtual void SetControlTextInternal(Control control, string text)
		{
			_controlText[control] = text;
		}
		public void SetControlText(Control control, string text)
		{
			SetControlTextInternal(control, text);
		}

		protected abstract NativeControl CreateControlInternal(Control control);

		private NativeControl mvarHandle = null;
		public NativeControl Handle {  get { return mvarHandle; } }

		public NativeControl CreateControl(Control control)
		{
			NativeControl handle = CreateControlInternal(control);
			if (handle == null) throw new InvalidOperationException();

			mvarHandle = handle;
			control.NativeImplementation = this;
			AfterCreateControl();
			return handle;
		}

		protected virtual void AfterCreateControl()
		{

		}

		protected abstract void RegisterDragSourceInternal(Control control, DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys);
		public void RegisterDragSource(Control control, DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			RegisterDragSourceInternal(control, targets, actions, buttons, modifierKeys);
		}

		protected abstract void RegisterDropTargetInternal(Control control, DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys);
		public void RegisterDropTarget(Control control, DragDrop.DragDropTarget[] targets, DragDropEffect actions, MouseButtons buttons, KeyboardModifierKey modifierKeys)
		{
			RegisterDropTargetInternal(control, targets, actions, buttons, modifierKeys);
		}

		protected internal virtual void OnDragDropDataRequest(DragDropDataRequestEventArgs e)
		{
			Control.OnDragDropDataRequest(e);
		}
		protected internal virtual void OnDragBegin(DragEventArgs e)
		{
			Control.OnDragBegin(e);
		}
		protected internal virtual void OnDragDataDelete(EventArgs e)
		{
			Control.OnDragDataDelete(e);
		}
		
		protected internal virtual void OnKeyDown(KeyEventArgs e)
		{
			Control.OnKeyDown(e);
		}
		protected internal virtual void OnKeyUp(KeyEventArgs e)
		{
			Control.OnKeyUp(e);
		}

		protected internal virtual void OnClick(EventArgs e)
		{
			Control.OnClick(e);
		}
		protected internal virtual void OnMouseDown(MouseEventArgs e)
		{
			Control.OnMouseDown(e);
		}
		protected internal virtual void OnMouseMove(MouseEventArgs e)
		{
			Control.OnMouseMove(e);
		}
		protected internal virtual void OnMouseUp(MouseEventArgs e)
		{
			Control.OnMouseUp(e);
		}
		protected internal virtual void OnMouseDoubleClick(MouseEventArgs e)
		{
			Control.OnMouseDoubleClick(e);
		}

		protected internal virtual void OnRealize(EventArgs e)
		{
			Control.OnRealize(e);
		}
		protected internal virtual void OnUnrealize(EventArgs e)
		{
			Control.OnUnrealize(e);
		}
		
		protected internal virtual void OnMapping(EventArgs e)
		{
			Control.OnMapping(e);
		}
		protected internal virtual void OnMapped(EventArgs e)
		{
			Control.OnMapped(e);
		}
		protected internal virtual void OnShown(EventArgs e)
		{
			Control.OnShown(e);
		}
	}

	public class NativeImplementationAttribute : Attribute
	{
		public Type ControlType { get; private set; } = null;

		/// <summary>
		/// Determines if an exact match of <see cref="ControlType" /> must be made for this <see cref="NativeImplementationAttribute" /> to apply to a particular <see cref="NativeImplementation" />.
		/// </summary>
		/// <value><c>true</c> if exact match required; otherwise, <c>false</c>.</value>
		public bool Exact { get; set; } = false;

		public NativeImplementationAttribute(Type controlType, bool exact = false)
		{
			if (!controlType.IsSubclassOf(typeof(Control)))
				throw new InvalidOperationException();

			ControlType = controlType;
			Exact = exact;
		}
	}
}
