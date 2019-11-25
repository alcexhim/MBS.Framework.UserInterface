//
//  Plugin.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;

namespace MBS.Framework.UserInterface
{
	public class Plugin
	{
		public bool Initialized { get; private set; } = false;
		public void Initialize()
		{
			if (Initialized)
				return;

			InitializeInternal();
			Initialized = true;
		}

		public Context Context { get; protected set; }

		protected virtual void InitializeInternal()
		{
			// this method intentionally left blank
		}

		private static Plugin[] _plugins = null;
		public static Plugin[] Get()
		{
			if (_plugins == null)
			{
				Type[] types = UniversalEditor.Common.Reflection.GetAvailableTypes(new Type[] { typeof(Plugin) });
				List<Plugin> plugins = new List<Plugin>();
				for (int i = 0; i < types.Length; i++)
				{
					try
					{
						Plugin plg = (Plugin)types[i].Assembly.CreateInstance(types[i].FullName);
						plugins.Add(plg);
					}
					catch (Exception ex)
					{
					}
				}
				_plugins = plugins.ToArray();
			}
			return _plugins;
		}
	}
}
