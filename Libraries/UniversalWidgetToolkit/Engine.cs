using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Drawing;

using MBS.Framework.Collections.Generic;
using UniversalWidgetToolkit.Printing;
using System.Diagnostics.Contracts;

namespace UniversalWidgetToolkit
{
	public abstract class Engine
	{
		protected static Dictionary<NativeControl, Control> controlsByHandle = new Dictionary<NativeControl, Control>();
		protected static Dictionary<Control, NativeControl> handlesByControl = new Dictionary<Control, NativeControl>();

		public Control GetControlByHandle(NativeControl handle)
		{
			if (controlsByHandle.ContainsKey(handle))
				return controlsByHandle[handle];
			return null;
		}
		[DebuggerNonUserCode()]
		public NativeControl GetHandleForControl(Control control)
		{
			if (control == null)
				return null;
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

		protected void RegisterControlHandle(Control control, NativeControl handle)
		{
			controlsByHandle[handle] = control;
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
			List<Type> possibleHandlers = new List<Type> ();
			foreach (Type t in ts)
			{
				if (t.IsSubclassOf(typeof(ControlImplementation)))
				{
					object[] atts = t.GetCustomAttributes(typeof(ControlImplementationAttribute), false);
					if (atts.Length == 1)
					{
						ControlImplementationAttribute att = (atts[0] as ControlImplementationAttribute);
						if (att == null) continue;

						if (control.GetType() == att.ControlType || control.GetType().IsSubclassOf(att.ControlType))
						{
							possibleHandlers.Add(t);
						}
					}
					else
					{
						continue;
					}
				}
 			}

			if (possibleHandlers.Count > 0) {
				if (possibleHandlers.Count > 1) {
					// holy shit this actually works
					possibleHandlers.Sort (runtimeTypeComparer);
				}
				Type t = possibleHandlers [0];
				return (t.Assembly.CreateInstance(t.FullName, false, System.Reflection.BindingFlags.Default, null, new object[] { this, control }, null, null) as ControlImplementation);
			}
			return null;
		}

		private _runtimeTypeComparer runtimeTypeComparer = new _runtimeTypeComparer();
		private class _runtimeTypeComparer : IComparer<Type>
		{
			public int Compare(Type left, Type right)
			{
				if (left.IsSubclassOf (right))
					return -1;
				if (right.IsSubclassOf (left))
					return 1;
				return 0;
			}
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
				// controlCreator.OnCreating (EventArgs.Empty);
				handle = controlCreator.CreateControl(control);
				// controlCreator.OnCreated (EventArgs.Empty);
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

		private Dictionary<Control, bool> _control_creating = new Dictionary<Control, bool>();
		public bool IsControlCreating(Control control) {
			if (_control_creating.ContainsKey (control)) {
				return _control_creating [control];
			}
			return false;
		}

		public bool CreateControl(Control control)
		{
			InvokeMethod(control, "OnCreating", EventArgs.Empty);

			_control_creating [control] = true;
			NativeControl result = CreateControlInternal(control);
			_control_creating [control] = false;

			if (result == null)
				return false;

			// set the control text if it has not been set already
			control.ControlImplementation?.SetControlText(control, control.Text);
			
			InvokeMethod(control, "OnCreated", EventArgs.Empty);
			return true;
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
			/*
			if (!IsControlCreated(dialog))
				CreateControl(dialog);
			*/
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

		protected abstract void UpdateControlLayoutInternal (Control control);
		public void UpdateControlLayout (Control control)
		{
			if (!IsControlCreated (control))
				return;

			UpdateControlLayoutInternal (control);
		}

		private bool inUpdateControlProperties = false;

		protected abstract void UpdateControlPropertiesInternal(Control control, NativeControl handle);
		public void UpdateControlProperties(Control control, NativeControl handle)
		{
			if (inUpdateControlProperties) return;
			inUpdateControlProperties = true;
			UpdateControlPropertiesInternal(control, handle);
			inUpdateControlProperties = false;
		}
		public void UpdateControlProperties(Control control)
		{
			if (!IsControlCreated(control)) return;
			NativeControl handle = GetHandleForControl(control);
			
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

		#region Printing
		protected abstract Printer[] GetPrintersInternal();
		public Printer[] GetPrinters()
		{
			return GetPrintersInternal();
		}

		protected abstract void PrintInternal(PrintJob job);
		public void Print(PrintJob job)
		{
			Contract.Requires(job != null);

			PrintInternal(job);
		}
		#endregion


		private Dictionary<NativeTreeModel, TreeModel> _TreeModelForHandle = new Dictionary<NativeTreeModel, TreeModel>();
		private Dictionary<TreeModel, NativeTreeModel> _HandleForTreeModel = new Dictionary<TreeModel, NativeTreeModel>();
		public TreeModel TreeModelFromHandle(NativeTreeModel handle)
		{
			if (_TreeModelForHandle.ContainsKey(handle)) return _TreeModelForHandle[handle];
			return null;
		}
		public NativeTreeModel GetHandleForTreeModel(TreeModel tm)
		{
			if (_HandleForTreeModel.ContainsKey(tm)) return _HandleForTreeModel[tm];
			return null;
		}

		private void RegisterTreeModel(TreeModel tm, NativeTreeModel handle)
		{
			_TreeModelForHandle[handle] = tm;
			_HandleForTreeModel[tm] = handle;
		}

		protected abstract NativeTreeModel CreateTreeModelInternal(TreeModel model);
		public NativeTreeModel CreateTreeModel(TreeModel model)
		{
			if (IsTreeModelCreated(model))
				return _HandleForTreeModel[model];

			NativeTreeModel handle = CreateTreeModelInternal(model);
			RegisterTreeModel(model, handle);
			return handle;
		}

		public bool IsTreeModelCreated(TreeModel model)
		{
			return _HandleForTreeModel.ContainsKey(model);
		}
	}
}
