﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		protected abstract void CreateControlInternal(Control control);
		protected internal void CreateControl(Control control)
		{
			CreateControlInternal(control);
			control.OnCreated(EventArgs.Empty);
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
	}
}
