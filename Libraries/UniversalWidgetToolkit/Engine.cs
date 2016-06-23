using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalWidgetToolkit.Controls;

namespace UniversalWidgetToolkit
{
	public abstract class Engine
	{
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

		public void Initialize()
		{
			InitializeInternal ();
		}
		protected abstract bool InitializeInternal();

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

		protected abstract bool CreateControlInternal(Control control);
		protected internal bool CreateControl(Control control)
		{
			bool result = CreateControlInternal(control);
			if (!result)
				return false;

			control.OnCreated(EventArgs.Empty);
			return true;
		}

		protected abstract void SetControlVisibilityInternal(Control control, bool visible);
		internal void SetControlVisibility(Control control, bool visible)
		{
			SetControlVisibilityInternal(control, visible);
		}

		protected abstract CommonDialogResult ShowDialogInternal(CommonDialog dialog);
		public CommonDialogResult ShowDialog(CommonDialog dialog)
		{
			return ShowDialogInternal(dialog);
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

		protected abstract bool IsControlCreatedInternal(Control control);
		public bool IsControlCreated(Control control)
		{
			return IsControlCreatedInternal(control);
		}

		protected abstract void InvalidateControlInternal(Control control);
		public void InvalidateControl(Control control)
		{
			InvalidateControlInternal(control);
		}

		protected abstract string GetControlTextInternal(Control control);
		public string GetControlText(Control control)
		{
			return GetControlTextInternal(control);
		}
		protected abstract void SetControlTextInternal(Control control, string text);
		public void SetControlText(Control control, string text)
		{
			SetControlTextInternal (control, text);
		}

		protected abstract void UpdateControlPropertiesInternal(Control control);
		public void UpdateControlProperties(Control control)
		{
			UpdateControlPropertiesInternal (control);
		}
		
		protected abstract void TabContainer_ClearTabPagesInternal (TabContainer parent);
		internal void TabContainer_ClearTabPages(TabContainer parent) {
			TabContainer_ClearTabPagesInternal (parent);
		}
		protected abstract void TabContainer_InsertTabPageInternal (TabContainer parent, int index, TabPage tabPage);
		internal void TabContainer_InsertTabPage(TabContainer parent, int index, TabPage tabPage) {
			TabContainer_InsertTabPageInternal (parent, index, tabPage);
		}
		protected abstract void TabContainer_RemoveTabPageInternal (TabContainer parent, TabPage tabPage);
		internal void TabContainer_RemoveTabPage(TabContainer parent, TabPage tabPage) {
			TabContainer_RemoveTabPageInternal (parent, tabPage);
		}

		protected abstract void UpdateNotificationIconInternal(NotificationIcon nid, bool updateContextMenu);
		public void UpdateNotificationIcon(NotificationIcon nid, bool updateContextMenu = false) {
			UpdateNotificationIconInternal (nid, updateContextMenu);
		}

		protected abstract bool IsControlDisposedInternal(Control ctl);
		public bool IsControlDisposed(Control ctl)
		{
			return IsControlDisposedInternal(ctl);
		}
	}
}
