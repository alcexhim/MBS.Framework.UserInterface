using System;
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

		protected abstract int StartInternal();
		protected abstract void StopInternal();

		public int Start()
		{
			return StartInternal();
		}
		public void Stop()
		{
			StopInternal();
		}

		protected abstract void CreateControlInternal(Control control);
		public void CreateControl(Control control)
		{
			CreateControlInternal(control);
		}
	}
}
