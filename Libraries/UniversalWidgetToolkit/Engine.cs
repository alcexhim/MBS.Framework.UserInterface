using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Drawing;

using MBS.Framework.Collections.Generic;

namespace UniversalWidgetToolkit
{
	public abstract class Engine
	{
		private static Dictionary<IntPtr, Control> controlsByHandle = new Dictionary<IntPtr, Control>();
		private static Dictionary<Control, IntPtr> handlesByControl = new Dictionary<Control, IntPtr>();

		public Control GetControlByHandle(IntPtr handle)
		{
			if (controlsByHandle.ContainsKey(handle))
				return controlsByHandle[handle];
			return null;
		}
		[DebuggerNonUserCode()]
		public IntPtr GetHandleForControl(Control control)
		{
			return handlesByControl[control];
		}
		public bool IsControlCreated(Control control)
		{
			return handlesByControl.ContainsKey(control);
		}

		protected void InvokeStaticMethod(Type typ, string meth, params object[] parms)
		{
			if (typ == null)
			{
				Console.WriteLine("Engine::InvokeStaticMethod: typ is null");
				return;
			}
			System.Reflection.MethodInfo mi = typ.GetMethod(meth, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
			if (mi != null)
			{
				mi.Invoke(null, parms);
			}
			else
			{
				Console.WriteLine("Engine::InvokeStaticMethod: not found '" + meth + "' on '" + typ.FullName + "'");
			}
		}
		protected void InvokeMethod(object obj, string meth, params object[] parms)
		{
			if (obj == null)
			{
				Console.WriteLine("Engine::InvokeMethod: obj is null");
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
				Console.WriteLine("Engine::InvokeMethod: not found '" + meth + "' on '" + t.FullName + "'");
			}
		}

		protected void RegisterControlHandle(Control control, IntPtr handle, params IntPtr[] additionalHandles)
		{
			controlsByHandle[handle] = control;
			
			// BEGIN: 2019-07-19 12:13 by beckermj - support for additional handles used by control
			// (e.g. for scrolled window container, etc.)
			foreach (IntPtr ptr in additionalHandles)
			{
				controlsByHandle[ptr] = control;
			}
			// we can only store the primary handle for the control in our dictionary, though...
			// END: 2019-07-19 12:13 by beckermj - support for additional handles used by control
			
			handlesByControl[control] = handle;
		}

		/// <summary>
		/// Determines whether the given window has toplevel focus.
		/// </summary>
		/// <returns><c>true</c> if window has focus, <c>false</c> otherwise.</returns>
		/// <param name="window">The window to query.</param>
		protected abstract bool WindowHasFocusInternal(Window window);
		public bool WindowHasFocus(Window window) => WindowHasFocusInternal(window);

		public static Engine[] Get()
		{
			List<Engine> list = new List<Engine>();
			Type[] engineTypes = UniversalWidgetToolkit.Common.Reflection.GetAvailableTypes(new Type[] { typeof(Engine) });
			foreach (Type type in engineTypes)
			{
				list.Add((Engine)type.Assembly.CreateInstance(type.FullName));
			}
			return list.ToArray();
		}


		private static BidirectionalDictionary<StockType, System.Collections.Specialized.StringCollection> mvarStockIDs = new BidirectionalDictionary<StockType, System.Collections.Specialized.StringCollection>();
		public void RegisterStockType(StockType stockType, string name)
		{
			if (!mvarStockIDs.ContainsValue1(stockType))
			{
				mvarStockIDs.Add(stockType, new System.Collections.Specialized.StringCollection());
			}
			mvarStockIDs.GetValue2(stockType).Add(name);
		}

		public string StockTypeToString(StockType stockType)
		{
			if (mvarStockIDs.ContainsValue1(stockType))
			{
				System.Collections.Specialized.StringCollection coll = mvarStockIDs.GetValue2(stockType);
				if (coll.Count > 0)
				{
					return coll[0];
				}
			}
			return null;
		}
		public StockType StockTypeFromString(string value)
		{
			foreach (KeyValuePair<StockType, System.Collections.Specialized.StringCollection> kvp in mvarStockIDs)
			{
				if (kvp.Value.Contains(value)) return kvp.Key;
			}
			return StockType.None;
		}


		public void Initialize()
		{
			InitializeInternal();
		}
		protected abstract bool InitializeInternal();

		public Window[] GetToplevelWindows()
		{
			return GetToplevelWindowsInternal();
		}
		protected abstract Window[] GetToplevelWindowsInternal();

		protected abstract int StartInternal(Window waitForClose = null);
		protected abstract void StopInternal(int exitCode);

		public int Start(Window waitForClose = null)
		{
			return StartInternal(waitForClose);
		}
		public void Stop(int exitCode = 0)
		{
			StopInternal(exitCode);
		}

		private ControlImplementation FindNativeImplementationForControl(Control control)
		{
			Type[] ts = Application.Engine.GetType().Assembly.GetTypes();
			foreach (Type t in ts)
			{
				if (t.IsSubclassOf(typeof(ControlImplementation)))
				{
					object[] atts = t.GetCustomAttributes(typeof(ControlImplementationAttribute), false);
					if (atts.Length == 1)
					{
						ControlImplementationAttribute att = (atts[0] as ControlImplementationAttribute);
						if (att == null) continue;

						if (control.GetType() == att.ControlType || (!att.Exact && control.GetType().IsSubclassOf(att.ControlType)))
						{
							return (t.Assembly.CreateInstance(t.FullName, false, System.Reflection.BindingFlags.Default, null, new object[] { this, control }, null, null) as ControlImplementation);
						}
					}
					else
					{
						continue;
					}
				}
			}
			return null;
		}

		protected abstract Vector2D ClientToScreenCoordinatesInternal(Vector2D point);
		public Vector2D ClientToScreenCoordinates(Vector2D point)
		{
			return ClientToScreenCoordinatesInternal(point);
		}

		/// <summary>
		/// Creates the specified <see cref="Control" />
		/// </summary>
		/// <returns>The control handle.</returns>
		/// <param name="control">Control.</param>
		protected virtual NativeControl CreateControlInternal(Control control)
		{
			NativeControl handle = null;

			ControlImplementation controlCreator = FindNativeImplementationForControl(control);

			if (controlCreator != null)
			{
				handle = controlCreator.CreateControl(control);
			}
			return handle;
		}

		protected abstract bool IsControlEnabledInternal(Control control);
		public bool IsControlEnabled(Control control)
		{
			return IsControlEnabledInternal(control);
		}
		protected abstract void SetControlEnabledInternal(Control control, bool value);
		public void SetControlEnabled(Control control, bool value)
		{
			SetControlEnabledInternal(control, value);
		}

		public bool CreateControl(Control control)
		{
			NativeControl result = CreateControlInternal(control);
			if (result == null)
				return false;

			// set the text we've previously set before
			if (_ControlTextForUncreatedControls.ContainsKey(control))
			{
				SetControlText(control, _ControlTextForUncreatedControls[control]);
				_ControlTextForUncreatedControls.Remove(control);
			}

			InvokeMethod(control, "OnCreated", EventArgs.Empty);
			return true;
		}

		protected abstract void SetControlVisibilityInternal(Control control, bool visible);
		internal void SetControlVisibility(Control control, bool visible)
		{
			SetControlVisibilityInternal(control, visible);
		}

		protected abstract void DestroyControlInternal(Control control);
		/// <summary>
		/// Destroys the handle associated with the specified <see cref="Control" />.
		/// </summary>
		public void DestroyControl(Control control)
		{
			DestroyControlInternal(control);
		}

		public Window GetFocusedToplevelWindow()
		{
			// In GTK+, this lists all toplevel windows in the system
			// Windows might only give us the windows in our process?
			Window[] toplevels = GetToplevelWindows();

			// figure out which of these toplevels is the window that currently has focus
			foreach (Window toplevel in toplevels)
			{
				if (toplevel.HasFocus)
				{
					return toplevel;
				}
			}
			return null;
		}

		protected abstract DialogResult ShowDialogInternal(Dialog dialog, Window parent);

		[DebuggerNonUserCode()]
		public DialogResult ShowDialog(Dialog dialog, Window parent)
		{
			if (parent == null)
			{
				// find the appropriate parent window
				parent = GetFocusedToplevelWindow();
			}
			return ShowDialogInternal(dialog, parent);
		}

		protected abstract Monitor[] GetMonitorsInternal();
		public Monitor[] GetMonitors()
		{
			return GetMonitorsInternal();
		}

		private Dictionary<string, object> mvarProperties = new Dictionary<string, object>();
		public object GetProperty(string propertyName, object defaultValue = null)
		{
			if (mvarProperties.ContainsKey(propertyName)) return mvarProperties[propertyName];
			return defaultValue;
		}
		public T GetProperty<T>(string propertyName, T defaultValue = default(T))
		{
			object value = GetProperty(propertyName, (object)defaultValue);
			if (value.GetType() == typeof(T)) return (T)value;
			return defaultValue;
		}
		public bool SetProperty(string propertyName, object value)
		{
			bool retval = true;
			if (mvarProperties.ContainsKey(propertyName))
			{
				retval = false;
			}
			mvarProperties[propertyName] = value;
			return retval;
		}
		public bool SetProperty<T>(string propertyName, T value)
		{
			return SetProperty(propertyName, (object)value);
		}

		protected abstract void InvalidateControlInternal(Control control, int x, int y, int width, int height);
		public void InvalidateControl(Control control, int x, int y, int width, int height)
		{
			InvalidateControlInternal(control, x, y, width, height);
		}

		private Dictionary<Control, string> _ControlTextForUncreatedControls = new Dictionary<Control, string>();

		public string GetControlText(Control control)
		{
			if (!IsControlCreated(control))
			{
				if (_ControlTextForUncreatedControls.ContainsKey(control))
				{
					return _ControlTextForUncreatedControls[control];
				}
				return String.Empty;
			}

			string text = null;

			ControlImplementation controlCreator = FindNativeImplementationForControl(control);

			if (controlCreator != null)
			{
				text = controlCreator.GetControlText(control);
			}


			if (text != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append(text);
				return sb.ToString();
			}
			return text;
		}
		public void SetControlText(Control control, string text)
		{
			if (!IsControlCreated(control))
			{
				_ControlTextForUncreatedControls[control] = text;
				return;
			}

			ControlImplementation controlCreator = FindNativeImplementationForControl(control);

			if (controlCreator != null)
			{
				controlCreator.SetControlText(control, text);
			}
		}

		private bool inUpdateControlProperties = false;

		protected abstract void UpdateControlPropertiesInternal(Control control, IntPtr handle);
		public void UpdateControlProperties(Control control, IntPtr handle)
		{
			if (inUpdateControlProperties) return;
			inUpdateControlProperties = true;
			UpdateControlPropertiesInternal(control, handle);
			inUpdateControlProperties = false;
		}
		public void UpdateControlProperties(Control control)
		{
			if (!IsControlCreated(control)) return;
			IntPtr handle = GetHandleForControl(control);
			
			UpdateControlProperties(control, handle);
		}

		protected abstract void TabContainer_ClearTabPagesInternal(TabContainer parent);
		internal void TabContainer_ClearTabPages(TabContainer parent)
		{
			TabContainer_ClearTabPagesInternal(parent);
		}
		protected abstract void TabContainer_InsertTabPageInternal(TabContainer parent, int index, TabPage tabPage);
		internal void TabContainer_InsertTabPage(TabContainer parent, int index, TabPage tabPage)
		{
			TabContainer_InsertTabPageInternal(parent, index, tabPage);
		}
		protected abstract void TabContainer_RemoveTabPageInternal(TabContainer parent, TabPage tabPage);
		internal void TabContainer_RemoveTabPage(TabContainer parent, TabPage tabPage)
		{
			TabContainer_RemoveTabPageInternal(parent, tabPage);
		}

		protected abstract void UpdateNotificationIconInternal(NotificationIcon nid, bool updateContextMenu);
		public void UpdateNotificationIcon(NotificationIcon nid, bool updateContextMenu = false)
		{
			UpdateNotificationIconInternal(nid, updateContextMenu);
		}

		protected abstract bool IsControlDisposedInternal(Control ctl);
		public bool IsControlDisposed(Control ctl)
		{
			return IsControlDisposedInternal(ctl);
		}

		protected abstract void ShowNotificationPopupInternal(NotificationPopup popup);
		public void ShowNotificationPopup(NotificationPopup popup)
		{
			ShowNotificationPopupInternal(popup);
		}

		protected internal abstract void RepaintCustomControl(CustomControl control, int x, int y, int width, int height);

		protected abstract void DoEventsInternal();
		public void DoEvents()
		{
			DoEventsInternal();
		}
	}
}
